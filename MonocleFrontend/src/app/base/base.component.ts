import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Barricade, Base, Item, Structure } from '../types/models';

@Component({
  selector: 'app-base',
  templateUrl: './base.component.html',
  styleUrls: ['./base.component.scss']
})
export class BaseComponent implements OnInit {
  @Input() base?: Base;
  @Input() userCanModerate?: boolean;
  @Input() structures?: Structure[];
  @Input() barricades?: Barricade[];
  @Output() destroyItemRequested = new EventEmitter<Item>();

  trackBarricade(_index: number, barricade: Barricade) {
    return barricade.instanceId;
  }

  trackStructure(_index: number, structure: Structure) {
    return structure.instanceId;
  }

  emitDestroyItem(item: Item) {
    this.destroyItemRequested.emit(item);
  }

  round(number: number): number {
    return Math.round(number)
  }

  constructor() { }
  ngOnInit(): void { }
}
