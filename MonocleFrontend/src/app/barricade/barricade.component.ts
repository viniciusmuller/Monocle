import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Barricade, Item } from '../types/models';

@Component({
  selector: 'app-barricade',
  templateUrl: './barricade.component.html',
  styleUrls: ['./barricade.component.scss']
})
export class BarricadeComponent implements OnInit {
  @Input() barricade?: Barricade;
  @Input() userCanModerate?: boolean;
  @Output() destroyItem = new EventEmitter<Item>()

  round(number: number): number {
    return Math.round(number)
  }

  emitDestroyItem(item: Item) {
    this.destroyItem.emit(item);
  }

  constructor() { }
  ngOnInit(): void { }
}
