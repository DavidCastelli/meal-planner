import { TestBed } from '@angular/core/testing';
import { ResolveFn } from '@angular/router';

import { recipesResolver } from './recipes.resolver';
import { GetRecipesDto } from '../models/get/get-recipes-dto.model';

describe('recipesResolver', () => {
  const executeResolver: ResolveFn<GetRecipesDto[]> = (...resolverParameters) =>
    TestBed.runInInjectionContext(() => recipesResolver(...resolverParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeResolver).toBeTruthy();
  });
});
