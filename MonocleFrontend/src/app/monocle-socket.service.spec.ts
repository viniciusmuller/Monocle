import { TestBed } from '@angular/core/testing';

import { MonocleSocketService } from './monocle-socket.service';

describe('MonocleSocketService', () => {
  let service: MonocleSocketService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MonocleSocketService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
