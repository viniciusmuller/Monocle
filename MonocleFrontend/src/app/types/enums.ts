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
    PlayerScreenshot = "PlayerScreenshot",
}

export enum ServerResponseType
{
    Players = "Players",
    CurrentBuildings = "CurrentBuildings",
    SuccessfulLogin = "SuccessfulLogin",
    Vehicles = "Vehicles",
    Barricades = "Barricades",
    Structures = "Structures",
    ServerInfo = "ServerInfo",
    PlayerScreenshot = "PlayerScreenshot",
}

export enum EventType
{
    PlayerDeath = "PlayerDeath",
    PlayerMessage = "PlayerMessage",
    PlayerJoined = "PlayerJoined",
    PlayerLeft = "PlayerLeft",
}

export enum ChatMode {
    Local = 1,
    Global,
    Group,
    Say,
    Welcome
}

export enum VehicleType {
    Car = 1,
    Plane,
    Helicopter,
    Blimp,
    Boat,
    Train,
}

export enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythical
}