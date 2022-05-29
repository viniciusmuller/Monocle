import { Observable, Observer } from 'rxjs';
import { AnonymousSubject } from 'rxjs/internal/Subject';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { EventType, RequestType, ServerResponseType } from "src/app/types/enums";
import { Injectable } from '@angular/core';
import { PlayerScreenshotResponse, ServerMessage } from 'src/app/types/serverData';
import { Barricade, Player, PlayerDeath, PlayerId, PlayerJoinOrLeave, PlayerMessage, ServerInfo, Structure, Vehicle } from 'src/app/types/models';

export interface ServerResponse {
  type: string;
}

@Injectable()
export class WebsocketService {
  private subject: AnonymousSubject<MessageEvent> | null = null;
  private connection: Subject<any> | null = null;

  // Responses
  public onLoginSuccessful: Subject<void>;
  public onGetPlayers: Subject<Player[]>;
  public onGetBarricades: Subject<Barricade[]>;
  public onGetStructures: Subject<Structure[]>;
  public onGetVehicles: Subject<Vehicle[]>;
  public onGetServerInfo: Subject<ServerInfo>;
  public onGetPlayerScreenshot: Subject<PlayerScreenshotResponse>;

  // Events
  public onPlayerMessage: Subject<PlayerMessage>;
  public onPlayerLeft: Subject<PlayerJoinOrLeave>;
  public onPlayerJoin: Subject<PlayerJoinOrLeave>;
  public onPlayerDeath: Subject<PlayerDeath>;

  constructor() {
    // Responses
    this.onPlayerMessage = new Subject();
    this.onGetPlayers = new Subject();
    this.onGetBarricades = new Subject();
    this.onGetStructures = new Subject();
    this.onGetVehicles = new Subject();
    this.onGetServerInfo = new Subject();
    this.onGetPlayerScreenshot = new Subject();

    // Events
    this.onLoginSuccessful = new Subject();
    this.onPlayerDeath = new Subject();
    this.onPlayerLeft = new Subject();
    this.onPlayerJoin = new Subject();
  }

  public connect(host: string, port: number, ssl: boolean) {
    let protocol = ssl ? "wss" : "ws"; 
    this.subject = this.create(`${protocol}://${host}:${port}`);
    this.connection = <Subject<any>>this.subject.pipe(map(this.deserialize));
    this.connection.subscribe(this.handleServerMessage.bind(this));
  }

  deserialize(response: MessageEvent): any {
    return JSON.parse(response.data);
  }

  handleServerMessage(message: ServerMessage) {
    console.log(message);


    // Response
    if (message.kind == 'Response') {
      const type = message.type as ServerResponseType;

      switch (type) {
        case ServerResponseType.SuccessfulLogin:
          return this.onLoginSuccessful.next();

        case ServerResponseType.Players:
          return this.onGetPlayers.next(message.data as Player[]);

        case ServerResponseType.Barricades:
          return this.onGetBarricades.next(message.data as Barricade[]);

        case ServerResponseType.Structures:
          return this.onGetStructures.next(message.data as Structure[]);

        case ServerResponseType.Vehicles:
          return this.onGetVehicles.next(message.data as Vehicle[]);

        case ServerResponseType.ServerInfo:
          return this.onGetServerInfo.next(message.data as ServerInfo);

        case ServerResponseType.PlayerScreenshot:
          return this.onGetPlayerScreenshot.next(message.data as PlayerScreenshotResponse);
      }
    }
    
    // Events
    if (message.kind == 'Event') {
      const type = message.type as EventType;

      switch (type) {
        case EventType.PlayerMessage:
          return this.onPlayerMessage.next(message.data as PlayerMessage);

        case EventType.PlayerDeath:
          return this.onPlayerDeath.next(message.data as PlayerDeath);

        case EventType.PlayerLeft:
          return this.onPlayerLeft.next(message.data as PlayerJoinOrLeave);

        case EventType.PlayerJoined:
          return this.onPlayerJoin.next(message.data as PlayerJoinOrLeave);
      }
    }
  }

  public getPlayers() {
    this.sendRequestType(RequestType.Players, null); 
  }

  public getVehicles() {
    this.sendRequestType(RequestType.Vehicles, null); 
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
  
  public getPlayerScreenshot(id: PlayerId) {
    this.sendRequestType(RequestType.PlayerScreenshot, id); 
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