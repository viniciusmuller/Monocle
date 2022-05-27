import { Component, Input, OnInit } from '@angular/core';
import { ServerInfo } from '../types/models';

@Component({
  selector: 'app-server-details',
  templateUrl: './server-details.component.html',
  styleUrls: ['./server-details.component.scss']
})
export class ServerDetailsComponent implements OnInit {
  @Input() totalPlayers: number = 0;
  @Input() serverInfo?: ServerInfo;
  
  constructor() { }
  ngOnInit(): void { }
}
