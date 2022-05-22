using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Api
{
    internal enum ErrorType
    {
        InvalidRequestType,
        UserNotFound,
        InvalidRequestData,
    }

    internal enum RequestType
    {
        Authenticate,
        GetPlayers,
        GetPlayerDetails,
        GetStructures,
        GetBarricades,
        GetVehicles,
        GetWorldSize,
    }

    internal enum ResponseType
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

    internal enum EventType
    {
        PlayerDeath,
        PlayerMessage,
        PlayerJoined,
        PlayerLeft,
    }
}
