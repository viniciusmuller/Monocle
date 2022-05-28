import { Component, Input, OnInit } from '@angular/core';
import { Player } from '../types/models';

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.scss']
})
export class PlayerComponent implements OnInit {
  @Input() player?: Player;

  constructor() { }
  ngOnInit(): void { }

  round(number: number): number {
    return Math.round(number)
  }
}
