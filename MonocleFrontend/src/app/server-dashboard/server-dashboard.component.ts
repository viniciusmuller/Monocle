import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { interval, Observable, of } from 'rxjs';
import { Barricade, Player, PlayerMessage, ServerInfo, Structure } from '../types/models';
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

  connectAndLogin(payload: LoginPayload) {
    this.websocketService.connect(payload.host, payload.port);
    setTimeout(() => {
      this.websocketService.login(payload.username, payload.password);
    }, 1000);
  }

  constructor(private websocketService: WebsocketService) {
    this.chatLog = [];
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

    this.websocketService.onPlayerMessage.subscribe(playerMessage => {
      this.chatLog = [...this.chatLog, playerMessage];
    })

    this.websocketService.onGetStructures.subscribe(structures => {
      this.structures = structures;
    })

    this.websocketService.onGetServerInfo.subscribe(serverDetails => {
      this.serverInfo = serverDetails;
    })

    // TODO: Find more elegant approach to these
    this.getPlayers();
    const playerFetchInterval = interval(1000);
    playerFetchInterval.subscribe(() => this.getPlayers())

    this.getBarricades();
    const barricadeFetchInterval = interval(10000);
    barricadeFetchInterval.subscribe(() => this.getBarricades())

    // TODO: Find more elegant approach to these
    this.getServerDetails();
    const serverDetailsFetchInterval = interval(30000);
    playerFetchInterval.subscribe(() => this.getServerDetails())
  }

  getPlayers() {
    if (this.loggedIn) {
      this.websocketService.getPlayers();
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
