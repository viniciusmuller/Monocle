using Fleck;
using Monocle.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Monocle.Services
{
    internal class Server
    {
        WebSocketServer SocketServer { get; set; }
        HashSet<Guid> LoggedInUsers { get; set; }
        MonocleConfiguration Config { get; set; }

        public Server(MonocleConfiguration config)
        {
            Config = config;
            LoggedInUsers = new HashSet<Guid>();
        }

        public void Start(string ip, int port)
        {
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

        void HandleOpen(IWebSocketConnection socket)
        {
            Logger.Log($"New connection from {socket.ConnectionInfo.Host}");
        }

        void HandleClose(IWebSocketConnection socket)
        {
            Logger.Log($"Closed connection from {socket.ConnectionInfo.Host}");

            if (LoggedInUsers.Contains(socket.ConnectionInfo.Id))
            {
                LoggedInUsers.Remove(socket.ConnectionInfo.Id);
            }
        }

        void HandleMessage(IWebSocketConnection socket, string payload)
        {
            var isAuthenticated = LoggedInUsers.Contains(socket.ConnectionInfo.Id);

            var type = GetRequestType(payload);
            if (type == null)
            {
                SendError(socket, ErrorType.InvalidRequestType, "The request type was not provided or invalid");
            }
            else if (isAuthenticated)
            {
                ServeRequest(socket, type, payload);
            }
            else
            {
                if (Authenticate(payload))
                {
                    var response = new SuccesfulLoginResponse { Message = "Authentication succeeded" };
                    SendResponse(socket, ResponseType.SuccessfulLogin, response);
                    LoggedInUsers.Add(socket.ConnectionInfo.Id);
                }
                else
                {
                    // We don't send an error message because the socket closes before it receives the message
                    socket.Close();
                }
            }
        }

        // TODO: This should return a response or error object
        void ServeRequest(IWebSocketConnection socket, RequestType? type, string payload)
        {
            switch (type)
            {
                case RequestType.GetPlayers:
                    var players = SDG.Unturned.Provider.clients;
                    var playerModels = players.ConvertAll(p => new Player
                    {
                        // TODO: Get how much time the player is in the server (p.joined returns a float, find out what its format)
                        Name = p.player.name,
                        IsAdmin = p.isAdmin,
                        Ping = (int)Math.Ceiling(p.ping),
                    });
                    SendResponse(socket, ResponseType.Players, playerModels);
                    break;
                case RequestType.GetPlayerInfo:
                    throw new NotImplementedException();
                case RequestType.GetStructures:
                    // Structs are floors, walls, roofs, stairs, etc
                    var structures = SDG.Unturned.StructureManager.regions.Cast<SDG.Unturned.StructureRegion>()
                                                                          .SelectMany(x => x.drops);

                    var structureModels = structures.Select(s => new Structure
                    {
                        Name = s.asset.name,
                        Position = Vector3ToPosition(s.model.position),
                        Health = s.asset.health
                    }).ToList();
                    SendResponse(socket, ResponseType.Structures, structureModels);
                    break;
                case RequestType.GetBarricades:
                    // Barricades are everything that can be stick into cars: lockers, wardrobes, metal plates, etc
                    var barricades = SDG.Unturned.BarricadeManager.regions.Cast<SDG.Unturned.BarricadeRegion>()
                                                                          .SelectMany(x => x.drops);

                    var barricadeModels = barricades.Select(s => new Barricade
                    {
                        Name = s.asset.name,
                        Position = Vector3ToPosition(s.model.position),
                        Health = s.asset.health
                    }).ToList();
                    SendResponse(socket, ResponseType.Barricades, barricadeModels);
                    break;
                case RequestType.GetVehicles:
                    var vehicles = SDG.Unturned.VehicleManager.vehicles;
                    var vehicleModels = vehicles.Select(v => new Vehicle {
                        IsLocked = v.isLocked,
                        Name = v.name,
                        // TODO: Find who locked the vehicle (owner)
                        // Position = Vector3ToPosition() // TODO: Find exact vehicle position
                    }).ToList();
                    SendResponse(socket, ResponseType.Vehicles, vehicleModels);
                    break;
            }
        }

        Position Vector3ToPosition(UnityEngine.Vector3 v)
        {
            return new Position
            {
                x = v.x,
                y = v.y,
                z = v.z,
            };
        }

        bool Authenticate(string message)
        {
            var payload = JsonConvert.DeserializeObject<LoginPayload>(message);
            if (payload == null)
            {
                return false;
            }

            return AreCredentialsValid(payload.Username, payload.Password);
        }

        bool AreCredentialsValid(string username, string password) =>
            Config.AuthorizedUsers.Any(au => au.Username == username && au.Password == password);

        RequestType? GetRequestType(string message)
        {
            // All requests sent by clients must have a "type" field.
            var baseRequest = JsonConvert.DeserializeObject<BaseRequest>(message);
            return baseRequest.Type;
        }

        void SendResponse<T>(IWebSocketConnection socket, ResponseType type, T data)
        {
            var response = new BaseResponse<T>
            {
                Type = type,
                Data = data
            };
            var serialized = JsonConvert.SerializeObject(response);
            socket.Send(serialized);
        }

        void SendError(IWebSocketConnection socket, ErrorType type, string message)
        {
            var errorModel = new ErrorModel { Type = type, Message = message };
            var payload = JsonConvert.SerializeObject(errorModel);
            socket.Send(payload);
        }
    }

    class LoginPayload
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    class BaseResponse<T>
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseType Type { get; set; }

        public T Data { get; set; }
    }

    class InformativeResponse
    {
        public string Message { get; set; }
    }

    class SuccesfulLoginResponse : InformativeResponse { }

    class Vehicle
    {
        public bool IsLocked;
        public string Name { get; set; }
        public Position Position { get; set; }
    }

    abstract class PlayerBuilding
    {
        public string Name { get; set; }
        public Position Position { get; set; }
        public float Health { get; set; }
    }

    class Structure : PlayerBuilding { }
    class Barricade : PlayerBuilding { }

    class Position
    {
        public float x;
        public float y;
        public float z;
    }

    class Player
    {
        public string Name;
        public bool IsAdmin;
        public int Ping;
    }

    class ErrorModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorType Type { get; set; }

        public string Message { get; set; }
    }

    class BaseRequest
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public RequestType? Type { get; set; }
    }

    enum ErrorType
    {
        InvalidRequestType,
    }

    enum RequestType
    {
        Authenticate,
        GetPlayers,
        GetPlayerInfo,
        GetStructures,
        GetBarricades,
        GetVehicles,
    }

    enum ResponseType
    {
        Players,
        PlayerInfo,
        CurrentBuildings,
        SuccessfulLogin,
        Vehicles,
        Barricades,
        Structures,
    }

    enum EventType
    {
        PlayerDied,
    }
}
