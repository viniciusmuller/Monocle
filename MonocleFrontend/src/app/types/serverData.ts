import { ErrorType, EventType, ServerResponseType } from "./enums";

export interface LoginPayload {
    host: string;
    port: number;
    username: string;
    password: string;
}

export interface ServerMessage {
    kind: 'Response'         | 'Event'   | 'Error'
    type: ServerResponseType | EventType | ErrorType
    data: any
}