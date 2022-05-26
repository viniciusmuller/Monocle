import { Observable, Observer } from 'rxjs';
import { AnonymousSubject } from 'rxjs/internal/Subject';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { EventType, RequestType, ServerResponseType } from "src/app/types/enums";
import { Injectable } from '@angular/core';
import { ServerMessage } from 'src/app/types/serverData';
import { Player } from 'src/app/types/models';

export interface ServerResponse {
  type: string;
}

@Injectable()
export class WebsocketService {
  private subject: AnonymousSubject<MessageEvent> | null = null;
  private connection: Subject<any> | null = null;
  public onLoginSuccessful: Subject<void>;
  public onPlayerMessage: Subject<any>;
  public onGetPlayers: Subject<Player[]>;

  constructor() {
    this.onLoginSuccessful = new Subject();
    this.onPlayerMessage = new Subject();
    this.onGetPlayers = new Subject();
  }

  public connect(host: string, port: number) {
    this.subject = this.create(`ws://${host}:${port}`);
    this.connection = <Subject<any>>this.subject.pipe(
      map(
        (response: MessageEvent): any => {
          return JSON.parse(response.data);
        }
      ));
    this.connection.subscribe(this.handleServerMessage.bind(this));
  }

  handleServerMessage(message: ServerMessage) {
    console.log(message);
    // Response
    if (message.Kind == 'Response') {
      const type = message.Type as ServerResponseType;

      switch (type) {
        case ServerResponseType.SuccessfulLogin:
          this.onLoginSuccessful.next();
          return;

        case ServerResponseType.Players:
          this.onGetPlayers.next(message.Data as Player[]);
          return;
      }
    }
    
    // Events
    if (message.Kind == 'Event') {
      const type = message.Type as EventType;

      switch (type) {
        case EventType.PlayerMessage:
          this.onPlayerMessage.next({

          });
          break;
      }
    }
  }

  public getPlayers() {
    this.sendRequestType(RequestType.GetPlayers, null); 
  }

  private sendRequestType<T>(type: RequestType, data: T) {
    if (this.connection) {
      this.connection.next({ type, data });
    }
  }

  public login(username: string, password: string) {
    if (this.connection) {
      this.connection.next({ username, password, type: RequestType.Authenticate });
    }
  }

  private create(url: string): AnonymousSubject<MessageEvent> {
    let ws = new WebSocket(url);
    let observable = new Observable((obs: Observer<MessageEvent>) => {
      ws.onmessage = obs.next.bind(obs);
      ws.onerror = obs.error.bind(obs);
      ws.onclose = obs.complete.bind(obs);
      return ws.close.bind(ws);
    });
    let observer = {
      error: () => { },
      complete: () => { },
      next: (data: Object) => {
        console.log('Message sent to websocket: ', data);
        if (ws.readyState === WebSocket.OPEN) {
          ws.send(JSON.stringify(data));
        }
      }
    };
    return new AnonymousSubject<MessageEvent>(observer, observable);
  }
}