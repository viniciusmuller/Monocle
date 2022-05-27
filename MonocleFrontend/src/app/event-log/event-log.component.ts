import { Component, Input, OnInit } from '@angular/core';
import { MonocleEvent } from '../types/models';

@Component({
  selector: 'app-event-log',
  templateUrl: './event-log.component.html',
  styleUrls: ['./event-log.component.scss']
})
export class EventLogComponent implements OnInit {
  @Input() events!: MonocleEvent[];

  constructor() { }
  ngOnInit(): void { }
}
