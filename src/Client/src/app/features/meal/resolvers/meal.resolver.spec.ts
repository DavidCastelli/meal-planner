import { TestBed } from '@angular/core/testing';
import { ResolveFn } from '@angular/router';

import { mealResolver } from './meal.resolver';

describe('mealResolver', () => {
  const executeResolver: ResolveFn<boolean> = (...resolverParameters) =>
    TestBed.runInInjectionContext(() => mealResolver(...resolverParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeResolver).toBeTruthy();
  });
});
