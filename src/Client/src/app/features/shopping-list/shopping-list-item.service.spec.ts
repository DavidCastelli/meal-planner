import { TestBed } from '@angular/core/testing';

import { ShoppingListItemService } from './shopping-list-item.service';

describe('ShoppingListItemService', () => {
  let service: ShoppingListItemService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ShoppingListItemService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
