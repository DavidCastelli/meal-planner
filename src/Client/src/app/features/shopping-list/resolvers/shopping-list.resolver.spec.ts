import { TestBed } from '@angular/core/testing';
import { ResolveFn } from '@angular/router';

import { shoppingListResolver } from './shopping-list.resolver';
import { GetShoppingItemsDto } from '../models/get-shopping-items-dto.model';

describe('shoppingListResolver', () => {
  const executeResolver: ResolveFn<GetShoppingItemsDto[]> = (
    ...resolverParameters
  ) =>
    TestBed.runInInjectionContext(() =>
      shoppingListResolver(...resolverParameters),
    );

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeResolver).toBeTruthy();
  });
});
