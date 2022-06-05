export enum ServerMessageType {
  // Responses
  Players,
  SuccessfulLogin,
  Vehicles,
  Barricades,
  Structures,
  ServerInfo,
  PlayerScreenshot,
  GameMap,

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

export interface ServerMessage<T> {
    type: ServerMessageType,
    data: T
}