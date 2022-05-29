import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Player, PlayerId } from '../types/models';

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.scss']
})
export class PlayerComponent implements OnInit {
  @Input() player?: Player;
  @Output() onWatchRequest = new EventEmitter<PlayerId>();

  constructor() { }
  ngOnInit(): void { }

  round(number: number): number {
    return Math.round(number)
  }

  emitWatchPlayer() {
    if (this.player) {
      this.onWatchRequest.emit(this.player.id);
    }
  }
}
