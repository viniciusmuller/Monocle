import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Vehicle } from '../types/models';

@Component({
  selector: 'app-vehicle',
  templateUrl: './vehicle.component.html',
  styleUrls: ['./vehicle.component.scss']
})
export class VehicleComponent implements OnInit {
  @Input() vehicle?: Vehicle;
  @Input() userCanModerate?: boolean;
  @Output() onEmitDestroy = new EventEmitter<Vehicle>();

  round(number: number): number {
    return Math.round(number)
  }

  emitDestroy() {
    this.onEmitDestroy.emit(this.vehicle);
  }

  constructor() { }
  ngOnInit(): void { }
}
