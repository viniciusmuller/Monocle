import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WsServerTestComponent } from './ws-server-test.component';

describe('WsServerTestComponent', () => {
  let component: WsServerTestComponent;
  let fixture: ComponentFixture<WsServerTestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WsServerTestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WsServerTestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
