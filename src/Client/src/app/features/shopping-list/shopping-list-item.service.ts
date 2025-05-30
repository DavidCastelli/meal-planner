import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ShoppingListItemService {
  private readonly selectedSource = new Subject<number>();
  public readonly selected$ = this.selectedSource.asObservable();

  setSelected(id: number) {
    this.selectedSource.next(id);
  }
}
