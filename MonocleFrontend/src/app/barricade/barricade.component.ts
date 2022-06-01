import { Component, Input, OnInit } from '@angular/core';
import { Barricade } from '../types/models';

@Component({
  selector: 'app-barricade',
  templateUrl: './barricade.component.html',
  styleUrls: ['./barricade.component.scss']
})
export class BarricadeComponent implements OnInit {
  @Input() barricade?: Barricade;

  round(number: number): number {
    return Math.round(number)
  }

  constructor() { }
  ngOnInit(): void { }
}
