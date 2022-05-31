import { AuthorizedUserType } from "./enums";

export interface PlayerScreenshotResponse {
    playerId: string;
    screenEncoded: string;
}

export interface SuccesfulAuthenticationResponse {
    userType: AuthorizedUserType
}
