import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { interval, Observable, of } from 'rxjs';
import { Barricade, Player } from '../types/models';
import { LoginPayload } from '../types/serverData';
import { WebsocketService } from './services/websocket.service';

@Component({
  selector: 'app-server-dashboard',
  templateUrl: './server-dashboard.component.html',
  styleUrls: ['./server-dashboard.component.scss'],
  providers: [WebsocketService],
  changeDetection:  ChangeDetectionStrategy.OnPush
})
export class ServerDashboardComponent implements OnInit {
  loggedIn: boolean = false;
  players: Player[];
  barricades: Barricade[];

  connectAndLogin(payload: LoginPayload) {
    this.websocketService.connect(payload.host, payload.port);
    setTimeout(() => {
      this.websocketService.login(payload.username, payload.password);
    }, 1000);
  }

  constructor(private websocketService: WebsocketService,
    private cdr: ChangeDetectorRef) {
    this.players = [];
    this.barricades = [];
  }

  ngOnInit(): void {
    // TODO: These should probably be inside the constructor

    this.websocketService.onLoginSuccessful.subscribe(_ => {
      console.log('successfully logged in!');
      this.loggedIn = true;
    })

    this.websocketService.onGetPlayers.subscribe(players => {
      this.players = players;
      this.cdr.detectChanges();
    })

    this.websocketService.onGetBarricades.subscribe(barricades => {
      this.barricades = barricades;
      this.cdr.detectChanges();
    })

    // TODO: Find more elegant approach to these
    this.getPlayers();
    const playerFetchInterval = interval(1000);
    playerFetchInterval.subscribe(() => this.getPlayers())

    this.getBarricades();
    const barricadeFetchInterval = interval(10000);
    barricadeFetchInterval.subscribe(() => this.getBarricades())
  }

  trackPlayer(_index: number, player: Player) {
    return player.id;
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
}
