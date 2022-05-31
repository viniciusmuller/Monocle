using Fleck;
using Monocle.Api;
using Monocle.Config;
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

namespace Monocle.Services
{
    internal class ServerService
    {
        WebSocketServer? SocketServer { get; set; }
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

        public void Start()
        {
            BindListeners();
            SocketServer = ConfigureServer();
            SocketServer.Start(socket =>
            {
                socket.OnOpen = () => HandleOpen(socket);
                socket.OnClose = () => HandleClose(socket);
                socket.OnMessage = message => HandleMessage(socket, message);
            });
        }

        private WebSocketServer ConfigureServer()
        {
            var url = $"ws://{Config.BindAddress}:{Config.ListenPort}";
            var server = new WebSocketServer(url);
            Logger.Log($"Starting WebSocket server at {url}");
            return server;
        }

        public void Stop()
        {
            if (SocketServer != null)
            {
                Logger.Log($"Stopping server...");
                SocketServer.Dispose();
            }
        }

        void BindListeners()
        {
            UnturnedPlayerEvents.OnPlayerDeath += (deadPlayer, cause, limb, murdererId) =>
            {
                var @event = EventHandlers.PlayerDeath(deadPlayer, murdererId, cause);
                var message = BuildMessage(ServerMessageType.OnPlayerDeath, @event);
                BroadcastEvent(message);
            };

            UnturnedPlayerEvents.OnPlayerChatted += (UnturnedPlayer player, ref UnityEngine.Color color, string message, EChatMode chatMode, ref bool _cancel) =>
            {
                // TODO: If message starts with /, then it's a command, but can we check if the command failed or succeeded?

                var @event = EventHandlers.PlayerMessage(player, color, chatMode, message);
                var serverMessage = BuildMessage(ServerMessageType.OnPlayerMessage, @event);
                BroadcastEvent(serverMessage);
            };

            U.Events.OnPlayerConnected += (player) =>
            {
                var @event = EventHandlers.PlayerJoinedOrLeft(player);
                var message = BuildMessage(ServerMessageType.OnPlayerJoined, @event);
                BroadcastEvent(message);
            };

            U.Events.OnPlayerDisconnected += (player) =>
            {
                var @event = EventHandlers.PlayerJoinedOrLeft(player);
                var message = BuildMessage(ServerMessageType.OnPlayerLeft, @event);
                BroadcastEvent(message);
            };
        }

        void BroadcastEvent<T>(ServerMessage<T> message)
        {
            foreach (var (socket, _) in LoggedInUsers.Values)
            {
                var payload = JsonConvert.SerializeObject(message, SerializationSettings);
                socket.Send(payload);
            }
        }

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
            var type = GetRequestType(payload) ?? RequestType.Unknown;
            if (isAuthenticated)
            {
                try
                {
                    ServeRequest(socket, type, payload);
                } catch (Exception ex)
                {
                    Logger.Log(ex.Message);
                    var message = BuildMessage(ServerMessageType.InternalServerError, new InternalError(ex));
                    SendMessage(socket, message);
                }
            }
            else
            {
                var loginPayload = JsonConvert.DeserializeObject<LoginRequest>(payload);
                var user = Authenticate(loginPayload);
                if (user != null)
                {
                    var model = new SuccesfulAuthResponse(user.Type);
                    var response = BuildMessage(ServerMessageType.SuccessfulLogin, model);
                    SendMessage(socket, response);
                    Logger.Log($"Host {socket.ConnectionInfo.Host} logged in as {user.Username}");
                    LoggedInUsers[socket.ConnectionInfo.Id] = (socket, user);
                }
                else
                {
                    // We don't send an error message because the socket closes before it receives the message
                    socket.Close();
                }
            }
        }

        private void GotScreenshot(string playerId, Guid socketId, byte[] jpg)
        {
            if (LoggedInUsers.TryGetValue(socketId, out var tuple))
            {
                var (socket, _) = tuple;
                var message = BuildMessage(ServerMessageType.PlayerScreenshot, new PlayerScreenshotResponse
                {
                    PlayerId = playerId,
                    ScreenEncoded = Convert.ToBase64String(jpg),
                });
                SendMessage(socket, message);
            }
        }

        void ServeRequest(IWebSocketConnection socket, RequestType? type, string payload)
        {
            switch (type)
            {
                case RequestType.Players:
                    var playerModels = UnturnedService.GetPlayers();
                    BuildMessageAndSend(socket, ServerMessageType.Players, playerModels);
                    return;
                case RequestType.Structures:
                    // Structs are floors, walls, roofs, stairs, etc
                    var structures = UnturnedService.GetStructures();
                    BuildMessageAndSend(socket, ServerMessageType.Structures, structures);
                    return;
                case RequestType.Barricades:
                    // Barricades are everything that can be stick into cars: lockers, wardrobes, metal plates, etc
                    var barricades = UnturnedService.GetBarricades();
                    BuildMessageAndSend(socket, ServerMessageType.Barricades, barricades);
                    return;
                case RequestType.Vehicles:
                    var vehicles = UnturnedService.GetVehicles();
                    BuildMessageAndSend(socket, ServerMessageType.Vehicles, vehicles);
                    return;
                case RequestType.ServerInfo:
                    var serverInfo = UnturnedService.GetServerInfo();
                    BuildMessageAndSend(socket, ServerMessageType.ServerInfo, serverInfo);
                    return;
                case RequestType.PlayerScreenshot:
                    // TODO: Validate user permissions

                    var request = DeserializeRequest<PlayerScreenshotRequest>(payload);
                    var userId = request?.Data?.UserId;

                    if (userId != null)
                    {
                        UnturnedService.ScreenshotPlayer(userId, socket.ConnectionInfo.Id, GotScreenshot);
                    } else
                    {
                        // TODO: Throw error if user id does not exist in the request
                        BuildMessageAndSend(socket, ServerMessageType.UserNotFound, new UserNotFoundError(userId!));
                    }
                    return;
                case RequestType.Unknown:
                    BuildMessageAndSend(socket, ServerMessageType.InvalidRequestType, "The request type was not provided or invalid");
                    return;
                default:
                    throw new ArgumentException("We should never get an invalid request type here");
            }
        }

        private BaseRequest<T>? DeserializeRequest<T>(string payload) 
            => JsonConvert.DeserializeObject<BaseRequest<T>>(payload);

        private void BuildMessageAndSend<T>(IWebSocketConnection socket, ServerMessageType type, T data)
        {
            var message = BuildMessage(type, data);
            SendMessage(socket, message);
        }

        private ServerMessage<T> BuildMessage<T>(ServerMessageType type, T data) {
            return new ServerMessage<T>(type, data);
        }

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
            var baseRequest = JsonConvert.DeserializeObject<BaseRequest<dynamic>>(message, SerializationSettings);
            return baseRequest?.Type;
        }

        void SendMessage<T>(IWebSocketConnection socket, ServerMessage<T> response)
        {
            var serialized = JsonConvert.SerializeObject(response, SerializationSettings);
            socket.Send(serialized);
        }
    }
}
