import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ItemRarity, ItemType } from '../types/enums';
import { Item, WeaponAttachmentsModel } from '../types/models';

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.scss']
})
export class ItemComponent implements OnInit {
  @Input() item!: Item;
  @Input() description?: string;
  @Input() userCanModerate?: boolean;
  @Output() onDestroy = new EventEmitter<Item>();

  weaponAttachments?: Item[];

  getCssClassFromItemRarity(type: ItemRarity): string {
    switch (type) {
      case ItemRarity.Common: return "common";
      case ItemRarity.Uncommon: return "uncommon";
      case ItemRarity.Rare: return "rare";
      case ItemRarity.Epic: return "epic";
      case ItemRarity.Legendary: return "legendary";
      case ItemRarity.Mythical: return "mythical";
    }
  }

  destroyItem(item: Item) {
    this.onDestroy.emit(item);
  }

  listAttachments(weaponAttachments: WeaponAttachmentsModel): (Item | undefined)[] {
    return [
      weaponAttachments?.sight,
      weaponAttachments?.tactical,
      weaponAttachments?.grip,
      weaponAttachments?.barrel,
      weaponAttachments?.ammo,
    ];
  }

  constructor() { }
  ngOnInit(): void {
    if (this.item.weaponAttachments) {
      this.weaponAttachments = [];
      for (const item of this.listAttachments(this.item.weaponAttachments)) {
        if (item != undefined) {
          this.weaponAttachments.push(item);
        }
      }
    }
  }
}
