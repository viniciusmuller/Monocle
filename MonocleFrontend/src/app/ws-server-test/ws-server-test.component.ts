import { Component, OnInit } from '@angular/core';
import { WebsocketService } from "../services/websocket.service";

@Component({
  selector: 'app-ws-server-test',
  templateUrl: './ws-server-test.component.html',
  styleUrls: ['./ws-server-test.component.css']
})
export class WsServerTestComponent implements OnInit {
  received = [];
  sent = [];

  constructor(private websocketService: WebsocketService) { }

  getPlayers() {

  }

  ngOnInit(): void {
    this.websocketService.messages.subscribe(msg => {
      this.received.push(msg);
      console.log("Response from websocket: " + msg);
    });
  }
}
