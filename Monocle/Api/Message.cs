using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Api
{
    internal enum ServerMessageType
    {
        // Responses
        Players,
        SuccessfulLogin,
        Vehicles,
        Barricades,
        Structures,
        ServerInfo,
        PlayerScreenshot,

        // Events
        OnPlayerDeath,
        OnPlayerMessage,
        OnPlayerJoined,
        OnPlayerLeft,

        // Errors
        InvalidRequestType,
        UserNotFound,
        InvalidRequestData,
        InternalServerError,
    }

    internal class ServerMessage<D>
    {
        public ServerMessageType Type { get; set; }

        public D Data { get; set; }

        public ServerMessage(ServerMessageType type, D data)
        {
            Type = type;
            Data = data;
        }
    }
}
