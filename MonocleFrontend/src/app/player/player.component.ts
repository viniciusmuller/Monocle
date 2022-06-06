import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Player, PlayerId } from '../types/models';

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.scss']
})
export class PlayerComponent implements OnInit {
  @Input() player?: Player;
  @Input() userCanModerate?: boolean;
  @Output() onWatchRequest = new EventEmitter<PlayerId>();
  @Output() banRequested = new EventEmitter<Player>();
  @Output() kickRequested = new EventEmitter<Player>();

  constructor() { }
  ngOnInit(): void { }

  round(number: number): number {
    return Math.round(number)
  }

  getTotalEquippedItems(player: Player): number {
    return Object.values(player.equipment).reduce((acc, item) => acc + (item ? 1 : 0), 0)
  }

  emitBanRequested() {
    this.banRequested.emit(this.player!);
  }

  emitKickRequested() {
    this.kickRequested.emit(this.player!);
  }

  emitWatchPlayer() {
    this.onWatchRequest.emit(this.player!.id);
  }
}
