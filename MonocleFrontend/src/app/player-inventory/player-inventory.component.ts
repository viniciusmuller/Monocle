import { Component, Input, OnInit } from '@angular/core';
import { Item } from '../types/models';

@Component({
  selector: 'app-player-inventory',
  templateUrl: './player-inventory.component.html',
  styleUrls: ['./player-inventory.component.scss']
})
export class PlayerInventoryComponent implements OnInit {
  @Input() items!: Item[];

  trackItem(_index: number, item: Item) {
    return item.id;
  }

  constructor() { }
  ngOnInit(): void { }
}
