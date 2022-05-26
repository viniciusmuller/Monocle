using Fleck;
using Monocle.Api;
using Monocle.Config;
using Monocle.Exceptions;
using Monocle.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Rocket.Core.Logging;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static Monocle.Extensions;

namespace Monocle.Services
{
    internal class ServerService
    {
        WebSocketServer SocketServer { get; set; }
        Dictionary<Guid, (IWebSocketConnection, AuthorizedUser)> LoggedInUsers { get; set; }
        MonocleConfiguration Config { get; set; }
        UnturnedService UnturnedService { get; set; }

        public ServerService(MonocleConfiguration config, UnturnedService unturnedService)
        {
            Config = config;
            LoggedInUsers = new();
            UnturnedService = unturnedService;
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
                BroadcastEvent(EventType.PlayerDeath, @event);
            };

            UnturnedPlayerEvents.OnPlayerChatted += (UnturnedPlayer player, ref UnityEngine.Color color, string message, EChatMode chatMode, ref bool _cancel) =>
            {
                // TODO: If message starts with /, then it's a command, but can we check if the command failed or succeeded?

                var @event = EventHandlers.PlayerMessage(player, color, chatMode, message);
                BroadcastEvent(EventType.PlayerMessage, @event);
            };

            U.Events.OnPlayerConnected += (player) =>
            {
                var @event = EventHandlers.PlayerJoinedOrLeft(player);
                BroadcastEvent(EventType.PlayerJoined, @event);
            };

            U.Events.OnPlayerDisconnected += (player) =>
            {
                var @event = EventHandlers.PlayerJoinedOrLeft(player);
                BroadcastEvent(EventType.PlayerLeft, @event);
            };
        }

        void BroadcastEvent(EventType type, Event @event)
        {
            var baseEvent = new BaseEvent(type, @event);
            foreach (var (socket, _) in LoggedInUsers.Values)
            {
                var payload = JsonConvert.SerializeObject(baseEvent);
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

            var type = GetRequestType(payload);
            if (type == null)
            {
                var error = new ErrorModel(ErrorType.InvalidRequestType, "The request type was not provided or invalid");
                SendResponse(socket, error);
            }
            else if (isAuthenticated)
            {
                try
                {
                    var response = ServeRequest(type, payload);
                    SendResponse(socket, response);
                } catch (ApiException ex)
                {
                    SendResponse(socket, ex.ErrorModel);
                }
            }
            else
            {
                var loginPayload = JsonConvert.DeserializeObject<LoginRequest>(payload);
                var user = Authenticate(loginPayload);
                if (user != null)
                {
                    // TODO: Refactor how responses are handled as data in the code
                    var response = new BaseResponse<string>(ResponseType.SuccessfulLogin, "Authentication succeeded");
                    SendResponse(socket, response);
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

        Response ServeRequest(RequestType? type, string payload)
        {
            switch (type)
            {
                case RequestType.GetPlayers:
                    var playerModels = UnturnedService.GetPlayers();
                    return new BaseResponse<List<PlayerModel>>(ResponseType.Players, playerModels);
                case RequestType.GetPlayerDetails:
                    var requestData = JsonConvert.DeserializeObject<GetUserInfoRequest>(payload);
                    var player = UnturnedService.GetPlayerDetails(requestData?.UserId);
                    return new BaseResponse<PlayerModel>(ResponseType.PlayerInfo, player);
                case RequestType.GetStructures:
                    // Structs are floors, walls, roofs, stairs, etc
                    var structures = UnturnedService.GetStructures();
                    return new BaseResponse<List<StructureModel>>(ResponseType.Structures, structures);
                case RequestType.GetBarricades:
                    // Barricades are everything that can be stick into cars: lockers, wardrobes, metal plates, etc
                    var barricades = UnturnedService.GetBarricades();
                    return new BaseResponse<List<BarricadeModel>>(ResponseType.Barricades, barricades);
                case RequestType.GetVehicles:
                    var vehicles = UnturnedService.GetVehicles();
                    return new BaseResponse<List<VehicleModel>>(ResponseType.Vehicles, vehicles);
                case RequestType.ServerInfo:
                    // TODO: Get this data and move this to UnturnedService
                    var serverInfo = new ServerInfoModel
                    {
                        WorldSize = 2048,
                    };
                    return new BaseResponse<ServerInfoModel>(ResponseType.ServerInfo, serverInfo);
                default:
                    throw new ArgumentException("We should never get an invalid request type here");
            }
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
            // All requests sent by clients must have a "type" field.
            var baseRequest = JsonConvert.DeserializeObject<BaseRequest>(message);
            return baseRequest?.Type;
        }

        void SendResponse(IWebSocketConnection socket, Response response)
        {
            var serialized = JsonConvert.SerializeObject(response);
            socket.Send(serialized);
        }
    }
}
