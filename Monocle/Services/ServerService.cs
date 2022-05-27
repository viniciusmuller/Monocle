using Fleck;
using Monocle.Api;
using Monocle.Config;
using Monocle.Exceptions;
using Monocle.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rocket.Core.Logging;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Monocle.Services
{
    internal class ServerService
    {
        WebSocketServer SocketServer { get; set; }
        Dictionary<Guid, (IWebSocketConnection, AuthorizedUser)> LoggedInUsers { get; set; }
        MonocleConfiguration Config { get; set; }
        UnturnedService UnturnedService { get; set; }

        JsonSerializerSettings SerializationSettings;

        public ServerService(MonocleConfiguration config, UnturnedService unturnedService)
        {
            Config = config;
            LoggedInUsers = new();
            UnturnedService = unturnedService;

            SerializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public void Start(string ip, int port)
        {
            BindListeners();
            var host = $"ws://{IPAddress.Parse(ip)}:{port}";
            SocketServer = new WebSocketServer(host);
            Logger.Log($"Starting WebSocket server at {host}");
            SocketServer.Start(socket =>
            {
                socket.OnOpen = () => HandleOpen(socket);
                socket.OnClose = () => HandleClose(socket);
                socket.OnMessage = message => HandleMessage(socket, message);
            });
        }

        public void Stop()
        {
            Logger.Log($"Stopping server...");
            SocketServer.Dispose();
        }

        void BindListeners()
        {
            UnturnedPlayerEvents.OnPlayerDeath += (deadPlayer, cause, limb, murdererId) =>
            {
                var @event = EventHandlers.PlayerDeath(deadPlayer, murdererId, cause);
                var message = CreateEvent(EventType.PlayerDeath, @event);
                BroadcastEvent(message);
            };

            UnturnedPlayerEvents.OnPlayerChatted += (UnturnedPlayer player, ref UnityEngine.Color color, string message, EChatMode chatMode, ref bool _cancel) =>
            {
                // TODO: If message starts with /, then it's a command, but can we check if the command failed or succeeded?

                var @event = EventHandlers.PlayerMessage(player, color, chatMode, message);
                var serverMessage = CreateEvent(EventType.PlayerMessage, @event);
                BroadcastEvent(serverMessage);
            };

            U.Events.OnPlayerConnected += (player) =>
            {
                var @event = EventHandlers.PlayerJoinedOrLeft(player);
                var message = CreateEvent(EventType.PlayerJoined, @event);
                BroadcastEvent(message);
            };

            U.Events.OnPlayerDisconnected += (player) =>
            {
                var @event = EventHandlers.PlayerJoinedOrLeft(player);
                var message = CreateEvent(EventType.PlayerLeft, @event);
                BroadcastEvent(message);
            };
        }

        void BroadcastEvent(ServerMessage<EventType, Event> message)
        {
            foreach (var (socket, _) in LoggedInUsers.Values)
            {
                var payload = JsonConvert.SerializeObject(message, SerializationSettings);
                socket.Send(payload);
            }
        }

        private ServerMessage<EventType, Event> CreateEvent(EventType type, Event data) =>
            new(MessageKind.Event, type, data);

        void HandleOpen(IWebSocketConnection socket)
        {
            Logger.Log($"New connection from {socket.ConnectionInfo.Host}");
        }

        void HandleClose(IWebSocketConnection socket)
        {
            Logger.Log($"Closed connection from {socket.ConnectionInfo.Host}");

            if (LoggedInUsers.ContainsKey(socket.ConnectionInfo.Id))
            {
                var (_, user) = LoggedInUsers[socket.ConnectionInfo.Id];
                Logger.Log($"User {user.Username} logged off");
                LoggedInUsers.Remove(socket.ConnectionInfo.Id);
            }
        }

        void HandleMessage(IWebSocketConnection socket, string payload)
        {
            var isAuthenticated = LoggedInUsers.ContainsKey(socket.ConnectionInfo.Id);

            var type = GetRequestType(payload);
            if (type == null)
            {
                var message = new ServerMessage<ErrorType, string>(MessageKind.Error, ErrorType.InvalidRequestType, "The request type was not provided or invalid");
                SendMessage(socket, message);
            }
            else if (isAuthenticated)
            {
                try
                {
                    var response = ServeRequest(type, payload);
                    SendMessage(socket, response);
                } catch (ApiException ex)
                {
                    var message = new ServerMessage<ErrorType, ErrorModel>(MessageKind.Error, ErrorType.InvalidRequestData, ex.ErrorModel);
                    SendMessage(socket, message);
                }
            }
            else
            {
                var loginPayload = JsonConvert.DeserializeObject<LoginRequest>(payload);
                var user = Authenticate(loginPayload);
                if (user != null)
                {
                    // TODO: Refactor how responses are handled as data in the code
                    var response = new ServerMessage<ResponseType, string>(MessageKind.Response, ResponseType.SuccessfulLogin, "Authentication succeeded");
                    SendMessage(socket, response);
                    Logger.LogWarning($"Host {socket.ConnectionInfo.Host} logged in as {user.Username}");
                    LoggedInUsers[socket.ConnectionInfo.Id] = (socket, user);
                }
                else
                {
                    // We don't send an error message because the socket closes before it receives the message
                    socket.Close();
                }
            }
        }

        ServerMessage<ResponseType, dynamic> ServeRequest(RequestType? type, string payload)
        {
            switch (type)
            {
                case RequestType.Players:
                    var playerModels = UnturnedService.GetPlayers();
                    return BuildResponse(ResponseType.Players, playerModels);
                case RequestType.PlayerDetails:
                    var requestData = JsonConvert.DeserializeObject<GetUserInfoRequest>(payload);
                    var player = UnturnedService.GetPlayerDetails(requestData?.UserId);
                    return BuildResponse(ResponseType.PlayerInfo, player);
                case RequestType.Structures:
                    // Structs are floors, walls, roofs, stairs, etc
                    var structures = UnturnedService.GetStructures();
                    return BuildResponse(ResponseType.Structures, structures);
                case RequestType.Barricades:
                    // Barricades are everything that can be stick into cars: lockers, wardrobes, metal plates, etc
                    var barricades = UnturnedService.GetBarricades();
                    return BuildResponse(ResponseType.Barricades, barricades);
                case RequestType.Vehicles:
                    var vehicles = UnturnedService.GetVehicles();
                    return BuildResponse(ResponseType.Vehicles, vehicles);
                case RequestType.ServerInfo:
                    var serverInfo = UnturnedService.GetServerInfo();
                    return BuildResponse(ResponseType.ServerInfo, serverInfo);
                default:
                    throw new ArgumentException("We should never get an invalid request type here");
            }
        }
        
        private ServerMessage<ResponseType, dynamic> BuildResponse<T>(ResponseType type, T data) => 
            new(MessageKind.Response, type, data!);

        AuthorizedUser? Authenticate(LoginRequest? payload)
        {
            if (payload == null)
            {
                return null;
            }

            return TryToLogin(payload.Username, payload.Password);
        }

        AuthorizedUser? TryToLogin(string? username, string? password) =>
           Config.AuthorizedUsers.Where(au => au.Username == username && au.Password == password).FirstOrDefault();

        RequestType? GetRequestType(string message)
        {
            // All requests sent by clients must have a "type" field.
            var baseRequest = JsonConvert.DeserializeObject<BaseRequest>(message, SerializationSettings);
            return baseRequest?.Type;
        }

        void SendMessage<T, D>(IWebSocketConnection socket, ServerMessage<T, D> response)
        {
            var serialized = JsonConvert.SerializeObject(response, SerializationSettings);
            socket.Send(serialized);
        }
    }
}
