import { TestBed } from '@angular/core/testing';

import { LoanEligibilityServicesService } from './loan-eligibility-services.service';

describe('LoanEligibilityServicesService', () => {
  let service: LoanEligibilityServicesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LoanEligibilityServicesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
