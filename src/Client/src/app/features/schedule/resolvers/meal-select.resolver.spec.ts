import { TestBed } from '@angular/core/testing';
import { ResolveFn } from '@angular/router';

import { mealSelectResolver } from './meal-select.resolver';
import { GetMealsDto } from '../models/get-meals-dto.model';

describe('mealSelectResolver', () => {
  const executeResolver: ResolveFn<GetMealsDto[]> = (...resolverParameters) =>
    TestBed.runInInjectionContext(() =>
      mealSelectResolver(...resolverParameters),
    );

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeResolver).toBeTruthy();
  });
});
