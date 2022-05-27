import { Component, Input, OnInit } from '@angular/core';
import { Equipment } from '../types/models';

@Component({
  selector: 'app-player-equipment',
  templateUrl: './player-equipment.component.html',
  styleUrls: ['./player-equipment.component.scss']
})
export class PlayerEquipmentComponent implements OnInit {
  @Input() equipment!: Equipment;

  constructor() { }
  ngOnInit(): void { }
}
