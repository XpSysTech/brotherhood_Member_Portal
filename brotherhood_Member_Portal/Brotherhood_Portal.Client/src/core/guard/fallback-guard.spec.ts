import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { fallbackGuard } from './fallback-guard';

describe('fallbackGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => fallbackGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
