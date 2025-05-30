import { ResolveFn } from '@angular/router';
import { GetShoppingItemsDto } from '../models/get-shopping-items-dto.model';
import { inject } from '@angular/core';
import { ShoppingListService } from '../shopping-list.service';
import { map } from 'rxjs';

export const shoppingListResolver: ResolveFn<GetShoppingItemsDto[]> = () => {
  return inject(ShoppingListService)
    .getShoppingItems()
    .pipe(
      map((shoppingItems) => {
        return shoppingItems.sort((shoppingItemA, shoppingItemB) => {
          return shoppingItemA.name.localeCompare(shoppingItemB.name);
        });
      }),
    );
};
