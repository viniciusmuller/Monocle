import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Item } from '../types/models';

@Component({
  selector: 'app-player-inventory',
  templateUrl: './player-inventory.component.html',
  styleUrls: ['./player-inventory.component.scss']
})
// TODO: This should be called something like item-list
export class PlayerInventoryComponent implements OnInit {
  @Input() items!: Item[];
  @Input() userCanModerate?: boolean;
  @Input() canAddItems?: boolean;
  @Output() onItemDestroy = new EventEmitter<Item>();

  trackItem(_index: number, item: Item) {
    // TODO: Track by instance id
    if (item.weaponAttachments != null) {
      const wa = item.weaponAttachments;
      // Track changes in attachments, so the component gets re-rendered accordingly
      // TODO: Try to maintain the foldable open when re-rendering this
      return item.id 
             + wa.currentAmmo
             + (wa.sight?.id ?? 0)
             + (wa.tactical?.id ?? 0)
             + (wa.grip?.id ?? 0)
             + (wa.barrel?.id ?? 0)
             + (wa.ammo?.id ?? 0)
    } else {
      return item.id;
    }
  }

  trackNullableNumber(n?: number) {
    return n ?? 0;
  }

  emitItemDestroy(item: Item) {
    this.onItemDestroy.emit(item);
  }

  constructor() { }
  ngOnInit(): void { }
}
