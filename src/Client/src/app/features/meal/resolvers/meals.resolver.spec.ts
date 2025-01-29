import { TestBed } from '@angular/core/testing';
import { ResolveFn } from '@angular/router';

import { mealsResolver } from './meals.resolver';

describe('mealsResolver', () => {
  const executeResolver: ResolveFn<boolean> = (...resolverParameters) =>
    TestBed.runInInjectionContext(() => mealsResolver(...resolverParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeResolver).toBeTruthy();
  });
});
