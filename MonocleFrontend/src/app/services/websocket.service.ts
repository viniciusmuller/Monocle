import { Observable, Observer } from 'rxjs';
import { AnonymousSubject } from 'rxjs/internal/Subject';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { EventType, RequestType, ServerResponseType } from "src/app/types/enums";
import { Injectable } from '@angular/core';
import { ServerMessage } from 'src/app/types/serverData';
import { Barricade, Player, ServerInfo, Structure } from 'src/app/types/models';

export interface ServerResponse {
  type: string;
}

@Injectable()
export class WebsocketService {
  private subject: AnonymousSubject<MessageEvent> | null = null;
  private connection: Subject<any> | null = null;

  public onLoginSuccessful: Subject<void>;
  public onGetPlayers: Subject<Player[]>;
  public onGetBarricades: Subject<Barricade[]>;
  public onGetStructures: Subject<Structure[]>;
  public onGetServerInfo: Subject<ServerInfo>;

  // Events
  public onPlayerMessage: Subject<any>;

  constructor() {
    this.onLoginSuccessful = new Subject();
    this.onPlayerMessage = new Subject();
    this.onGetPlayers = new Subject();
    this.onGetBarricades = new Subject();
    this.onGetStructures = new Subject();
    this.onGetServerInfo = new Subject();
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
    if (message.kind == 'Response') {
      const type = message.type as ServerResponseType;

      switch (type) {
        case ServerResponseType.SuccessfulLogin:
          this.onLoginSuccessful.next();
          return;

        case ServerResponseType.Players:
          this.onGetPlayers.next(message.data as Player[]);
          return;

        case ServerResponseType.Barricades:
          this.onGetBarricades.next(message.data as Barricade[]);
          return;

        case ServerResponseType.Structures:
          this.onGetStructures.next(message.data as Structure[]);
          return;

        case ServerResponseType.ServerInfo:
          this.onGetServerInfo.next(message.data as ServerInfo);
          return;
      }
    }
    
    // Events
    if (message.kind == 'Event') {
      const type = message.type as EventType;

      switch (type) {
        case EventType.PlayerMessage:
          this.onPlayerMessage.next({

          });
          break;
      }
    }
  }

  public getPlayers() {
    this.sendRequestType(RequestType.Players, null); 
  }

  public getBarricades() {
    this.sendRequestType(RequestType.Barricades, null); 
  }

  public getStructures() {
    this.sendRequestType(RequestType.Structures, null); 
  }

  public getServerDetails() {
    this.sendRequestType(RequestType.ServerInfo, null); 
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