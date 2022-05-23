import { Component, OnInit } from '@angular/core';
import { WebsocketService } from './services/websocket.service';

@Component({
  selector: 'app-server-dashboard',
  templateUrl: './server-dashboard.component.html',
  styleUrls: ['./server-dashboard.component.scss'],
  providers: []
})
export class ServerDashboardComponent implements OnInit {

  constructor() { }

  ngOnInit(): void { }
}
