import { ErrorType, EventType, ServerResponseType } from "./enums";

export interface LoginPayload {
    host: string;
    port: number;
    username: string;
    password: string;
}

export interface ServerMessage {
    Kind: 'Response'         | 'Event'   | 'Error'
    Type: ServerResponseType | EventType | ErrorType
    Data: any
}