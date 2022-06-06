import { PlayerId } from "./models";

export enum RequestType {
  Unknown,
  Authenticate,
  Players,
  Structures,
  Barricades,
  Vehicles,
  ServerInfo,
  PlayerScreenshot,
  GameMap,
  KickPlayer,
  BanPlayer,
  DestroyVehicle
}

// TODO: This is doing more than the necessary (should have only username and password)
export interface AuthenticationRequest {
    host: string;
    port: number;
    username: string;
    password: string;
    ssl: boolean;
}

export interface PlayerScreenshotRequest {
    userId: PlayerId;
}
