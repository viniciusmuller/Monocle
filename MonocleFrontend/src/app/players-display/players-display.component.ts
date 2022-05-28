import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Player, PlayerId } from '../types/models';

@Component({
  selector: 'app-players-display',
  templateUrl: './players-display.component.html',
  styleUrls: ['./players-display.component.scss']
})
export class PlayersDisplayComponent implements OnInit {
  @Input() players?: Player[];
  @Output() onPlayerSelected = new EventEmitter<PlayerId>();

  constructor() { }

  ngOnInit(): void { }

  playerSelected(player: Player) {
    this.onPlayerSelected.emit(player.id);
  }

  trackPlayer(_index: number, player: Player) {
    return player.id;
  }
}
