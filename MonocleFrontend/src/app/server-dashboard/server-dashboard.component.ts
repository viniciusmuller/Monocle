import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Player } from '../types/models';
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

  connectAndLogin(payload: LoginPayload) {
    this.websocketService.connect(payload.host, payload.port);
    setTimeout(() => {
      this.websocketService.login(payload.username, payload.password);
    }, 1000);
  }

  constructor(private websocketService: WebsocketService,
    private cdr: ChangeDetectorRef) {
    this.players = [];
  }

  ngOnInit(): void {
    this.websocketService.onLoginSuccessful.subscribe(_ => {
      console.log('successfully logged in!');
      this.loggedIn = true;
    })

    this.websocketService.onGetPlayers.subscribe(players => {
      // TODO: This is not being rendered when we get players
      this.players = players;
      this.cdr.detectChanges();
    })
  }

  trackPlayer(index: number, player: Player) {
    return player.id;
  }

  getPlayers() {
    this.websocketService.getPlayers();
  }
}
