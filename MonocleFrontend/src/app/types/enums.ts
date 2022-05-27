export enum ErrorType
{
    InvalidRequestType = "InvalidRequestType",
    UserNotFound = "UserNotFound",
    InvalidRequestData = "InvalidRequestData",
}

export enum RequestType
{
    Authenticate = "Authenticate",
    Players = "Players",
    PlayerDetails = "PlayerDetails",
    Structures = "Structures",
    Barricades = "Barricades",
    Vehicles = "Vehicles",
    ServerInfo = "ServerInfo",
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
    ServerInfo = "ServerInfo",
}

export enum EventType
{
    PlayerDeath = "PlayerDeath",
    PlayerMessage = "PlayerMessage",
    PlayerJoined = "PlayerJoined",
    PlayerLeft = "PlayerLeft",
}

export enum ChatMode {
    Local = "Local",
    Global = "Global",
    Group = "Group",
    Say = "Say",
    Welcome = "Welcome"
}