import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import * as _ from 'lodash';
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

  groupPlayersByGroup(players: Player[]) {
    return _.groupBy(players, p => p.groupId);
  }

  playerSelected(player: Player) {
    this.onPlayerSelected.emit(player.id);
  }
}
