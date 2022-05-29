import { ErrorType, EventType, ServerResponseType } from "./enums";

export interface LoginPayload {
    host: string;
    port: number;
    username: string;
    password: string;
    ssl: boolean;
}

export interface PlayerScreenshotResponse {
    playerId: string;
    screenEncoded: string;
}

export interface ServerMessage {
    kind: 'Response'         | 'Event'   | 'Error'
    type: ServerResponseType | EventType | ErrorType
    data: any
}