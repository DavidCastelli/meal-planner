import { TestBed } from '@angular/core/testing';
import { ResolveFn } from '@angular/router';

import { scheduleResolver } from './schedule.resolver';
import { GetMealsDto } from '../models/get-meals-dto.model';

describe('scheduleResolver', () => {
  const executeResolver: ResolveFn<GetMealsDto[]> = (...resolverParameters) =>
    TestBed.runInInjectionContext(() =>
      scheduleResolver(...resolverParameters),
    );

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeResolver).toBeTruthy();
  });
});
