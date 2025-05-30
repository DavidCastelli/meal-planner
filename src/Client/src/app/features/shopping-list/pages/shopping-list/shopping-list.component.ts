import { Component, DestroyRef, inject, Input } from '@angular/core';
import { GetShoppingItemsDto } from '../../models/get-shopping-items-dto.model';
import { ShoppingListService } from '../../shopping-list.service';
import { SidebarService } from '../../../../core/layout/sidebar.service';
import { slideAnimation } from '../../../../shared/animations/slide.animation';
import { AsyncPipe } from '@angular/common';
import { ShoppingListItemComponent } from './components/shopping-list-item/shopping-list-item.component';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { EMPTY, exhaustMap } from 'rxjs';
import { UpdateShoppingItemRequest } from '../../models/update-shopping-item-request.model';
import { CreateShoppingListItemComponent } from './components/create-shopping-list-item/create-shopping-list-item.component';

@Component({
  selector: 'app-shopping-list',
  standalone: true,
  imports: [
    AsyncPipe,
    ShoppingListItemComponent,
    CreateShoppingListItemComponent,
  ],
  animations: [slideAnimation],
  templateUrl: './shopping-list.component.html',
  styleUrl: './shopping-list.component.css',
})
export class ShoppingListComponent {
  private readonly shoppingListService = inject(ShoppingListService);
  private readonly sidebarService = inject(SidebarService);
  private readonly destroyRef = inject(DestroyRef);

  public readonly isSidebarOpen$ = this.sidebarService.openClose$;
  public createMode = false;

  @Input() shoppingItems: GetShoppingItemsDto[] = [];

  total() {
    let total = 0;
    for (const item of this.shoppingItems) {
      const price = item.price ?? 0;
      const quantity = item.quantity ?? 1;

      total += price * quantity;
    }

    return total;
  }

  update(request: UpdateShoppingItemRequest) {
    const index = this.shoppingItems.findIndex(
      (item) => item.id === request.id,
    );
    const shoppingItem = this.shoppingItems[index];

    shoppingItem.name = request.name;
    shoppingItem.measurement = request.measurement;
    shoppingItem.price = request.price;
    shoppingItem.quantity = request.quantity;
    shoppingItem.isChecked = request.isChecked;
    shoppingItem.isLocked = request.isLocked;
  }

  delete(id: number) {
    this.shoppingListService
      .deleteShoppingItem(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((success) => {
        if (success) {
          const index = this.shoppingItems.findIndex((item) => item.id === id);
          this.shoppingItems.splice(index, 1);
        }
      });
  }

  generate() {
    this.shoppingListService
      .generateShoppingItems()
      .pipe(
        exhaustMap((success) => {
          if (success) {
            return this.shoppingListService.getShoppingItems();
          } else {
            return EMPTY;
          }
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((shoppingItems) => {
        this.shoppingItems = shoppingItems.sort(
          (shoppingItemA, shoppingItemB) => {
            return shoppingItemA.name.localeCompare(shoppingItemB.name);
          },
        );
      });
  }

  startCreate() {
    this.createMode = true;
  }

  cancelCreate() {
    this.createMode = false;
  }

  created() {
    this.createMode = false;
    this.shoppingListService
      .getShoppingItems()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((shoppingItems) => {
        this.shoppingItems = shoppingItems.sort(
          (shoppingItemA, shoppingItemB) => {
            return shoppingItemA.name.localeCompare(shoppingItemB.name);
          },
        );
      });
  }

  clear(clear: string) {
    this.shoppingListService
      .clearShoppingItems(clear)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((success) => {
        if (success) {
          if (clear === 'all') {
            this.shoppingItems = this.shoppingItems.filter(
              (item) => item.isLocked,
            );
          } else if (clear === 'checked') {
            this.shoppingItems = this.shoppingItems.filter(
              (item) => !item.isChecked || item.isLocked,
            );
          }
        }
      });
  }
}
