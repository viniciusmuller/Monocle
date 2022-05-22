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
    }

    internal enum RequestType
    {
        Authenticate,
        GetPlayers,
        GetPlayerInfo,
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
        PlayerDied,
    }
}
