import { TestBed } from '@angular/core/testing';

import { MealSelectService } from './meal-select.service';

describe('MealSelectService', () => {
  let service: MealSelectService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MealSelectService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
