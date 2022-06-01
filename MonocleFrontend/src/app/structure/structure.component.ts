import { Component, Input, OnInit } from '@angular/core';
import { Structure } from '../types/models';

@Component({
  selector: 'app-structure',
  templateUrl: './structure.component.html',
  styleUrls: ['./structure.component.scss']
})
export class StructureComponent implements OnInit {
  @Input() structure?: Structure;

  round(number: number): number {
    return Math.round(number)
  }

  constructor() { }
  ngOnInit(): void { }
}
