import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { interval, Observable, of } from 'rxjs';
import { Barricade, MonocleEvent, Player, PlayerId, PlayerMessage, ServerInfo, Structure, Vehicle } from '../types/models';
import { LoginPayload } from '../types/serverData';
import { WebsocketService } from '../services/websocket.service';

@Component({
  selector: 'app-server-dashboard',
  templateUrl: './server-dashboard.component.html',
  styleUrls: ['./server-dashboard.component.scss'],
  providers: [WebsocketService]
})
export class ServerDashboardComponent implements OnInit {
  loggedIn: boolean = false;
  players?: Player[];
  barricades?: Barricade[];
  structures?: Structure[];
  vehicles?: Vehicle[];
  serverInfo?: ServerInfo;
  chatLog: PlayerMessage[];
  eventLog: MonocleEvent[];
  selectedPlayerId?: PlayerId;
  imagePath?: string;

  connectAndLogin(payload: LoginPayload) {
    this.websocketService.connect(payload.host, payload.port, payload.ssl);
    setTimeout(() => {
      this.websocketService.login(payload.username, payload.password);
    }, 500);
  }

  constructor(private websocketService: WebsocketService) {
    this.chatLog = [];
    this.eventLog = [];
  }

  ngOnInit(): void {
    // TODO: These should probably be inside the constructor

    this.websocketService.onLoginSuccessful.subscribe(_ => {
      this.loggedIn = true;
      this.bindRequests();
    })

    this.websocketService.onGetPlayers.subscribe(players => {
      this.players = players;
    })

    this.websocketService.onGetBarricades.subscribe(barricades => {
      this.barricades = barricades;
    })

    this.websocketService.onGetStructures.subscribe(structures => {
      this.structures = structures;
    })

    this.websocketService.onGetVehicles.subscribe(vehicles => {
      this.vehicles = vehicles;
    })

    this.websocketService.onGetServerInfo.subscribe(serverDetails => {
      this.serverInfo = serverDetails;
    })

    this.websocketService.onPlayerMessage.subscribe(playerMessage => {
      this.chatLog = [playerMessage, ...this.chatLog];
    })

    this.websocketService.onGetPlayerScreenshot.subscribe(screenshotResponse => {
        this.imagePath = 'data:image/jpg;base64,' + screenshotResponse.screenEncoded;
    })

    this.websocketService.onPlayerLeft.subscribe(leftEvent => {
      let message = `${leftEvent.player.name} left`;
      let event = this.buildEvent(leftEvent.time, message);
      this.eventLog = [event, ...this.eventLog];
    })

    this.websocketService.onPlayerJoin.subscribe(joinEvent => {
      let message = `${joinEvent.player.name} joined`;
      let event = this.buildEvent(joinEvent.time, message);
      this.eventLog = [event, ...this.eventLog];
    })

    this.websocketService.onPlayerDeath.subscribe(deathEvent => {
      let message; 
      if (!deathEvent.killer) {
        message = `${deathEvent.dead.name} died! [${deathEvent.cause}]`
      } else {
        message = `${deathEvent.killer.name} killed ${deathEvent.dead.name} - [${deathEvent.cause}]`
      }
      
      let event = this.buildEvent(deathEvent.time, message);
      this.eventLog = [event, ...this.eventLog];
    })
  }

  watchPlayer(id: PlayerId) {
    this.getPlayerScreenshot(id);
  }

  clearImage() {
    this.imagePath = undefined;
  }

  bindRequests() {
    // TODO: Find more elegant approach to these
    this.getPlayers();
    const playerFetchInterval = interval(1000);
    playerFetchInterval.subscribe(() => this.getPlayers())

    this.getVehicles()
    const vehicleFetchInterval = interval(5_000);
    vehicleFetchInterval.subscribe(() => this.getVehicles())

    this.getBarricades();
    const barricadesFetchInterval = interval(60_0000);
    barricadesFetchInterval.subscribe(() => this.getBarricades())

    this.getStructures();
    const structuresFetchInterval = interval(60_0000);
    structuresFetchInterval.subscribe(() => this.getStructures())

    // TODO: Find more elegant approach to these
    this.getServerDetails();
    const serverDetailsFetchInterval = interval(60_000);
    serverDetailsFetchInterval.subscribe(() => this.getServerDetails())
  }

  buildEvent(time: Date, message: string): MonocleEvent {
    return { time, message }
  }

  playerSelected(id: PlayerId) {
    this.selectedPlayerId = id;
  }

  getSelectedPlayer(): Player | undefined {
    return this.players?.find(p => p.id == this.selectedPlayerId);
  }

  getPlayers() { this.websocketService.getPlayers(); }
  getPlayerScreenshot(id: PlayerId) { this.websocketService.getPlayerScreenshot(id); }
  getVehicles() { this.websocketService.getVehicles(); }
  getStructures() { this.websocketService.getStructures(); }
  getBarricades() { this.websocketService.getBarricades(); }
  getServerDetails() { this.websocketService.getServerDetails(); }
}
