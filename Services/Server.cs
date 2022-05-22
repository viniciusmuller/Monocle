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
using static Monocle.Extensions;

namespace Monocle.Services
{
    internal class Server
    {
        WebSocketServer SocketServer { get; set; }
        Dictionary<Guid, AuthorizedUser> LoggedInUsers { get; set; }
        MonocleConfiguration Config { get; set; }

        public Server(MonocleConfiguration config)
        {
            Config = config;
            LoggedInUsers = new Dictionary<Guid, AuthorizedUser>();
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

            if (LoggedInUsers.ContainsKey(socket.ConnectionInfo.Id))
            {
                var user = LoggedInUsers[socket.ConnectionInfo.Id];
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
                SendError(socket, ErrorType.InvalidRequestType, "The request type was not provided or invalid");
            }
            else if (isAuthenticated)
            {
                ServeRequest(socket, type, payload);
            }
            else
            {
                var user = Authenticate(payload);
                if (user != null)
                {
                    var response = new SuccesfulLoginResponse { Message = "Authentication succeeded" };
                    SendResponse(socket, ResponseType.SuccessfulLogin, response);
                    Logger.LogWarning($"Host {socket.ConnectionInfo.Host} logged in as {user.Username}");
                    LoggedInUsers[socket.ConnectionInfo.Id] = user;
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
                    var playerModels = players.ConvertAll(p => new Player(p));
                    SendResponse(socket, ResponseType.Players, playerModels);
                    break;
                case RequestType.GetPlayerInfo:
                    var requestData = JsonConvert.DeserializeObject<GetUserInfoRequest>(payload);
                    var client = SDG.Unturned.Provider.clients.Where(p => p.playerID.steamID.ToString() == requestData?.UserId).FirstOrDefault();
                    if (client == null)
                    {
                        SendError(socket, ErrorType.UserNotFound, $"The user of ID {requestData?.UserId} was not found in the server.");
                        return;
                    }

                    var playerInventory = FetchInventoryItems(client);
                    var player = new Player(client, playerInventory);
                    SendResponse(socket, ResponseType.PlayerInfo, player);
                    break;
                case RequestType.GetStructures:
                    // Structs are floors, walls, roofs, stairs, etc
                    var structures = SDG.Unturned.StructureManager.regions.Cast<SDG.Unturned.StructureRegion>()
                                                                          .SelectMany(x => x.drops);

                    var structureModels = structures.Select(s => new Structure(s)).ToList();
                    SendResponse(socket, ResponseType.Structures, structureModels);
                    break;
                case RequestType.GetBarricades:
                    // Barricades are everything that can be stick into cars: lockers, wardrobes, metal plates, etc
                    var barricades = SDG.Unturned.BarricadeManager.regions.Cast<SDG.Unturned.BarricadeRegion>()
                                                                          .SelectMany(x => x.drops);

                    var barricadeModels = barricades.Select(s => new Barricade(s)).ToList();
                    SendResponse(socket, ResponseType.Barricades, barricadeModels);
                    break;
                case RequestType.GetVehicles:
                    var vehicles = SDG.Unturned.VehicleManager.vehicles;
                    var vehicleModels = vehicles.Select(v =>
                    {
                        var name = SDG.Unturned.Assets.find(SDG.Unturned.EAssetType.VEHICLE, v.id).FriendlyName;
                        return new Vehicle(v, name);
                    }).ToList();
                    SendResponse(socket, ResponseType.Vehicles, vehicleModels);
                    break;
                case RequestType.GetWorldSize:
                    // TODO: Fix this
                    //var worldSize = SDG.Unturned.Regions.WORLD_SIZE * SDG.Unturned.Regions.REGION_SIZE;
                    var model = new WorldSizeResponse() { Size = 2048 };
                    SendResponse(socket, ResponseType.WorldSize, model);
                    break;
            }
        }

        List<Item> FetchInventoryItems(SDG.Unturned.SteamPlayer player)
        {
            var playerInventory = new List<Item>();
            foreach (var itemPack in player.player.inventory.items)
            {
                if (itemPack == null)
                {
                    continue;
                }

                foreach (var item in itemPack.items)
                {
                    var itemName = SDG.Unturned.Assets.find(SDG.Unturned.EAssetType.ITEM, item.item.id).FriendlyName;
                    playerInventory.Add(new Item(item, itemName));
                }
            }
            return playerInventory;
        }

        AuthorizedUser? Authenticate(string message)
        {
            var payload = JsonConvert.DeserializeObject<LoginRequest>(message);
            if (payload == null)
            {
                return null;
            }

            return TryToLogin(payload.Username, payload.Password);
        }

        AuthorizedUser? TryToLogin(string username, string password) =>
           Config.AuthorizedUsers.Where(au => au.Username == username && au.Password == password).FirstOrDefault();

        RequestType? GetRequestType(string message)
        {
            // All requests sent by clients must have a "type" field.
            var baseRequest = JsonConvert.DeserializeObject<BaseRequest>(message);
            return baseRequest?.Type;
        }

        void SendResponse<T>(IWebSocketConnection socket, ResponseType type, T data)
        {
            var response = new BaseResponse<T>
            {
                Type = type,
                Data = data,
                Status = "Success"
            };
            var serialized = JsonConvert.SerializeObject(response);
            socket.Send(serialized);
        }

        void SendError(IWebSocketConnection socket, ErrorType type, string message)
        {
            var errorModel = new ErrorModel { Type = type, Message = message, Status = "Error" };
            var payload = JsonConvert.SerializeObject(errorModel);
            socket.Send(payload);
        }
    }

    class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    class GetUserInfoRequest
    {
        public string UserId { get; set; }
    }

    class BaseResponse<T>
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ResponseType Type { get; set; }

        public T Data { get; set; }
        public string Status { get; set; }
    }

    class InformativeResponse
    {
        public string Message { get; set; }
    }

    class WorldSizeResponse
    {
        // Apparently all of unturned maps are square
        public int Size { get; set; }
    }

    class SuccesfulLoginResponse : InformativeResponse { }

    class Vehicle
    {
        public bool IsLocked { get; set; }
        public string Name { get; set; }
        public Position Position { get; set; }
        public ushort Id { get; set; }

        public Vehicle(SDG.Unturned.InteractableVehicle vehicle, string vehicleName)
        {
            IsLocked = vehicle.isLocked;
            Id = vehicle.id;
            Name = vehicleName;
            Position = vehicle.transform.position.ToPosition();
        }
    }

    abstract class PlayerBuilding
    {
        public string Name { get; set; }
        public Position Position { get; set; }
        public float Health { get; set; }
    }

    class Structure : PlayerBuilding
    {
        public Structure(SDG.Unturned.StructureDrop drop)
        {
            Name = drop.asset.name;
            Position = drop.model.position.ToPosition();
            Health = drop.asset.health;
        }
    }

    class Barricade : PlayerBuilding
    {
        public Barricade(SDG.Unturned.BarricadeDrop drop)
        {
            Name = drop.asset.name;
            Position = drop.model.position.ToPosition();
            Health = drop.asset.health;
        }
    }

    class Position
    {
        public float x;
        public float y;
        public float z;
    }

    class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public int Ping { get; set; }
        public List<Item> Items { get; set; }
        public Position? Position { get; set; }
        public byte? Health { get; set; }

        public Player(SDG.Unturned.SteamPlayer player)
        {
            Id = player.playerID.steamID.ToString();
            IsAdmin = player.isAdmin;
            Name = player.player.name;
            Items = new List<Item>();
            Ping = (int)Math.Ceiling(player.ping);
            Position = player.player.transform.position.ToPosition();
            // Health = client.player.life.health // TODO: Get correct player life
        }

        public Player(SDG.Unturned.SteamPlayer player, List<Item> items) : this(player)
        {
            Items = items;
        }
    }

    class Item
    {
        public string Name;
        public int Amount;
        public ushort Id;
        public byte Durability;

        public Item(SDG.Unturned.ItemJar item, string name)
        {
            Amount = item.item.amount;
            Name = name;
            Durability = item.item.durability;
            Id = item.item.id;
        }
    }

    class ErrorModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorType Type { get; set; }

        public string Status;
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
        UserNotFound,
    }

    enum RequestType
    {
        Authenticate,
        GetPlayers,
        GetPlayerInfo,
        GetStructures,
        GetBarricades,
        GetVehicles,
        GetWorldSize,
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
        WorldSize,
    }

    enum EventType
    {
        PlayerDied,
    }
}
