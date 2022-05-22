import { Injectable } from '@angular/core';
import { Observable, Subject } from "rxjs";
import { WebsocketService } from "./websocket.service";

const SOCKET_URL = "ws://127.0.0.1:55554";

export interface Message {
  author: string;
  message: string;
}

@Injectable()
export class MonocleSocketService {
  public messages: Subject<Message>;

  constructor(wsService: WebsocketService) { }
}