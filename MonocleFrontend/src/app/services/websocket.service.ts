import { Observable, Observer } from 'rxjs';
import { AnonymousSubject } from 'rxjs/internal/Subject';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { ServerMessage, ServerMessageType } from 'src/app/types/server';
import { Barricade, Item, Player, PlayerDeath, PlayerId, PlayerJoinOrLeave, PlayerMessage, ServerInfo, Structure, Vehicle } from 'src/app/types/models';
import { PlayerScreenshotResponse, SuccesfulAuthenticationResponse } from '../types/responses';
import { RequestType } from '../types/requests';
import { InternalServerError, UserNotFoundError } from '../types/errors';

export interface ServerResponse {
  type: string;
}

@Injectable()
export class WebsocketService {
  private subject: AnonymousSubject<MessageEvent> | null = null;
  private connection: Subject<any> | null = null;

  // Responses
  public onLoginSuccessful: Subject<SuccesfulAuthenticationResponse> = new Subject();
  public onGetPlayers: Subject<Player[]> = new Subject();
  public onGetBarricades: Subject<Barricade[]> = new Subject();
  public onGetStructures: Subject<Structure[]> = new Subject();
  public onGetVehicles: Subject<Vehicle[]> = new Subject();
  public onGetServerInfo: Subject<ServerInfo> = new Subject();
  public onGetPlayerScreenshot: Subject<PlayerScreenshotResponse> = new Subject();
  public onGetGameMap: Subject<string> = new Subject();

  // Events
  public onPlayerMessage: Subject<PlayerMessage> = new Subject();
  public onPlayerLeft: Subject<PlayerJoinOrLeave> = new Subject();
  public onPlayerJoin: Subject<PlayerJoinOrLeave> = new Subject();
  public onPlayerDeath: Subject<PlayerDeath> = new Subject();

  // Errors
  public internalServerError: Subject<InternalServerError> = new Subject();
  public userNotFound: Subject<UserNotFoundError> = new Subject();

  public connect(host: string, port: number, ssl: boolean) {
    let protocol = ssl ? "wss" : "ws"; 
    this.subject = this.create(`${protocol}://${host}:${port}`);
    this.connection = <Subject<any>>this.subject.pipe(map(this.deserialize));
    this.connection.subscribe(this.handleServerMessage.bind(this));
  }

  deserialize(response: MessageEvent): any {
    return JSON.parse(response.data);
  }

  handleServerMessage(message: ServerMessage<any>) {
    // Response
    switch (message.type) {
      case ServerMessageType.SuccessfulLogin:
        return this.onLoginSuccessful.next(message.data as SuccesfulAuthenticationResponse);

      case ServerMessageType.Players:
        return this.onGetPlayers.next(message.data as Player[]);

      case ServerMessageType.Barricades:
        return this.onGetBarricades.next(message.data as Barricade[]);

      case ServerMessageType.Structures:
        return this.onGetStructures.next(message.data as Structure[]);

      case ServerMessageType.Vehicles:
        return this.onGetVehicles.next(message.data as Vehicle[]);

      case ServerMessageType.ServerInfo:
        return this.onGetServerInfo.next(message.data as ServerInfo);

      case ServerMessageType.PlayerScreenshot:
        return this.onGetPlayerScreenshot.next(message.data as PlayerScreenshotResponse);

      case ServerMessageType.GameMap:
        return this.onGetGameMap.next(message.data as string);

      // Events
      case ServerMessageType.OnPlayerMessage:
        return this.onPlayerMessage.next(message.data as PlayerMessage);

      case ServerMessageType.OnPlayerDeath:
        return this.onPlayerDeath.next(message.data as PlayerDeath);

      case ServerMessageType.OnPlayerLeft:
        return this.onPlayerLeft.next(message.data as PlayerJoinOrLeave);

      case ServerMessageType.OnPlayerJoined:
        return this.onPlayerJoin.next(message.data as PlayerJoinOrLeave);

      // Errors
      case ServerMessageType.InternalServerError:
        return this.internalServerError.next(message.data as InternalServerError);

      case ServerMessageType.UserNotFound:
        return this.userNotFound.next(message.data as UserNotFoundError);
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

  public getGameMap() {
    this.sendRequestType(RequestType.GameMap, null); 
  }

  public getStructures() {
    this.sendRequestType(RequestType.Structures, null); 
  }

  public getServerDetails() {
    this.sendRequestType(RequestType.ServerInfo, null); 
  }

  public kickPlayer(player: Player) {
    this.sendRequestType(RequestType.KickPlayer, player.id); 
  }

  public banPlayer(player: Player) {
    this.sendRequestType(RequestType.BanPlayer, player.id); 
  }

  public destroyVehicle(vehicle: Vehicle) {
    this.sendRequestType(RequestType.DestroyVehicle, vehicle.instanceId); 
  }

  public destroyItem(item: Item) {
    this.sendRequestType(RequestType.DestroyItem, item.instanceId); 
  }
  
  public getPlayerScreenshot(id: PlayerId) {
    let request = { userId: id };
    this.sendRequestType(RequestType.PlayerScreenshot, request); 
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

  // TODO: Big refactor in websocket service
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
        if (ws.readyState === WebSocket.OPEN) {
          ws.send(JSON.stringify(data));
        }
      }
    };
    return new AnonymousSubject<MessageEvent>(observer, observable);
  }
}