import { Component, Input, OnInit } from '@angular/core';
import { ItemRarity, ItemType } from '../types/enums';
import { Item } from '../types/models';

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.scss']
})
export class ItemComponent implements OnInit {
  @Input() item!: Item;
  @Input() description?: string;

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

  constructor() { }
  ngOnInit(): void { }
}
