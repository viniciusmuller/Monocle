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

        void HandleMessage(IWebSocketConnection socket, string message)
        {
            var isAuthenticated = LoggedInUsers.Contains(socket.ConnectionInfo.Id);

            var type = GetRequestType(message);
            if (type == null)
            {
                SendError(socket, ErrorType.InvalidRequestType, "The request type was not provided or invalid");
            }
            else if (isAuthenticated)
            {
                ServeRequest(socket, type);
            }
            else
            {
                if (Authenticate(message))
                {
                    var response = new SuccesfulLoginResponse { Message = "Authentication succeeded" };
                    SendResponse(socket, ResponseType.SuccessfulLogin, response);
                    LoggedInUsers.Add(socket.ConnectionInfo.Id);
                }
                else
                {
                    // We don't send an error message because the socket closes before it receives the message
                    // socket.Close();
                    Logger.LogWarning("damn son");
                    Logger.Log(message);
                }
            }
        }

        // TODO: This should return a response or error object
        void ServeRequest(IWebSocketConnection socket, RequestType? type)
        {
            switch (type)
            {
                case RequestType.GetPlayers:
                    throw new NotImplementedException();
                case RequestType.GetPlayerInfo:
                    throw new NotImplementedException();
                case RequestType.GetBuildings:
                    throw new NotImplementedException();
            }
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
        GetBuildings,
    }

    enum ResponseType
    {
        TotalPlayers,
        PlayerInfo,
        CurrentBuildings,
        SuccessfulLogin,
        AuthenticationFailed,
    }

    enum EventType
    {
        PlayerDied,
    }
}
