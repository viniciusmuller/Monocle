import { Component, Input, OnInit } from '@angular/core';
import { PlayerMessage } from '../types/models';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit {
  @Input() messages!: PlayerMessage[]

  constructor() { }
  ngOnInit(): void { }
}
