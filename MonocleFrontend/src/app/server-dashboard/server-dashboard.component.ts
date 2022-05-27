import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { interval, Observable, of } from 'rxjs';
import { Barricade, MonocleEvent, Player, PlayerMessage, ServerInfo, Structure } from '../types/models';
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
  serverInfo?: ServerInfo;
  chatLog: PlayerMessage[];
  eventLog: MonocleEvent[];

  connectAndLogin(payload: LoginPayload) {
    this.websocketService.connect(payload.host, payload.port);
    setTimeout(() => {
      this.websocketService.login(payload.username, payload.password);
    }, 1000);
  }

  constructor(private websocketService: WebsocketService) {
    this.chatLog = [];
    this.eventLog = [];
  }

  ngOnInit(): void {
    // TODO: These should probably be inside the constructor

    this.websocketService.onLoginSuccessful.subscribe(_ => {
      this.loggedIn = true;
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

    this.websocketService.onGetServerInfo.subscribe(serverDetails => {
      this.serverInfo = serverDetails;
    })

    this.websocketService.onPlayerMessage.subscribe(playerMessage => {
      this.chatLog = [...this.chatLog, playerMessage];
    })

    this.websocketService.onPlayerLeft.subscribe(leftEvent => {
      let message = `${leftEvent.player.name} left`;
      let event = this.buildEvent(message);
      this.eventLog = [...this.eventLog, event];
    })

    this.websocketService.onPlayerJoin.subscribe(joinEvent => {
      let message = `${joinEvent.player.name} joined`;
      let event = this.buildEvent(message);
      this.eventLog = [...this.eventLog, event];
    })

    this.websocketService.onPlayerDeath.subscribe(deathEvent => {
      let message = `${deathEvent.killer.name} killed ${deathEvent.dead.name} - [${deathEvent.cause}]`
      let event = this.buildEvent(message);
      this.eventLog = [...this.eventLog, event];
    })

    // TODO: Find more elegant approach to these
    this.getPlayers();
    const playerFetchInterval = interval(1000);
    playerFetchInterval.subscribe(() => this.getPlayers())

    this.getBarricades();
    const barricadesFetchInterval = interval(10000);
    barricadesFetchInterval.subscribe(() => this.getBarricades())

    this.getStructures();
    const structuresFetchInterval = interval(10000);
    structuresFetchInterval.subscribe(() => this.getStructures())

    // TODO: Find more elegant approach to these
    this.getServerDetails();
    const serverDetailsFetchInterval = interval(3000);
    serverDetailsFetchInterval.subscribe(() => this.getServerDetails())
  }

  buildEvent(message: string): MonocleEvent {
    return {
        time: new Date(),
        message
    }
  }

  getPlayers() {
    if (this.loggedIn) {
      this.websocketService.getPlayers();
    }
  }

  getStructures() {
    if (this.loggedIn) {
      this.websocketService.getStructures();
    }
  }

  getBarricades() {
    if (this.loggedIn) {
      this.websocketService.getBarricades();
    }
  }

  getServerDetails() {
    if (this.loggedIn) {
      this.websocketService.getServerDetails();
    }
  }
}
