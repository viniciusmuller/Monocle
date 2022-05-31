export interface InternalServerError {
    stackTrace: string;
    code: string;
    message: string;
}

export interface UserNotFoundError {
    userId: string;
}
