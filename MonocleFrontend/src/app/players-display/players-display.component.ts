import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';
import { Player } from '../types/models';

@Component({
  selector: 'app-players-display',
  templateUrl: './players-display.component.html',
  styleUrls: ['./players-display.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PlayersDisplayComponent implements OnInit {
  @Input() players?: Player[];

  constructor() { }

  ngOnInit(): void {
  }

  trackPlayer(_index: number, player: Player) {
    return player.id;
  }
}
