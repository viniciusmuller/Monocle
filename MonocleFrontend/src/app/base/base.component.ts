import { Component, Input, OnInit } from '@angular/core';
import { Barricade, Base, Structure } from '../types/models';

@Component({
  selector: 'app-base',
  templateUrl: './base.component.html',
  styleUrls: ['./base.component.scss']
})
export class BaseComponent implements OnInit {
  @Input() base?: Base;
  @Input() structures?: Structure[];
  @Input() barricades?: Barricade[];

  trackBarricade(_index: number, barricade: Barricade) {
    return barricade.instanceId;
  }

  trackStructure(_index: number, structure: Structure) {
    return structure.instanceId;
  }

  round(number: number): number {
    return Math.round(number)
  }

  constructor() { }
  ngOnInit(): void { }
}
