    export enum ErrorType
    {
        InvalidRequestType = "InvalidRequestType",
        UserNotFound = "UserNotFound",
        InvalidRequestData = "InvalidRequestData",
    }

    export enum RequestType
    {
        Authenticate = "Authenticate",
        GetPlayers = "GetPlayers",
        GetPlayerDetails = "GetPlayerDetails",
        GetStructures = "GetStructures",
        GetBarricades = "GetBarricades",
        GetVehicles = "GetVehicles",
        GetWorldSize = "GetWorldSize",
    }

    export enum ServerResponseType
    {
        Players = "Players",
        PlayerInfo = "PlayerInfo",
        CurrentBuildings = "CurrentBuildings",
        SuccessfulLogin = "SuccessfulLogin",
        Vehicles = "Vehicles",
        Barricades = "Barricades",
        Structures = "Structures",
        WorldSize = "WorldSize", // TODO: Turn this into ServerDetails request
    }

    export enum EventType
    {
        PlayerDeath = "PlayerDeath",
        PlayerMessage = "PlayerMessage",
        PlayerJoined = "PlayerJoined",
        PlayerLeft = "PlayerLeft",
    }
