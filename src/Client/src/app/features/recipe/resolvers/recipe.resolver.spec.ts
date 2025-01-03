import { TestBed } from '@angular/core/testing';
import { ResolveFn } from '@angular/router';

import { recipeResolver } from './recipe.resolver';
import { GetRecipeByIdDto } from '../models/get/get-recipe-by-id-dto.model';

describe('recipeResolver', () => {
  const executeResolver: ResolveFn<GetRecipeByIdDto> = (
    ...resolverParameters
  ) =>
    TestBed.runInInjectionContext(() => recipeResolver(...resolverParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeResolver).toBeTruthy();
  });
});
