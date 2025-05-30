import { inject, Injectable } from '@angular/core';
import { CreateShoppingItemRequest } from './models/create-shopping-item-request.model';
import { UpdateShoppingItemRequest } from './models/update-shopping-item-request.model';
import {
  HttpClient,
  HttpErrorResponse,
  HttpResponse,
} from '@angular/common/http';
import { GetShoppingItemsDto } from './models/get-shopping-items-dto.model';
import { catchError, map, Observable, of } from 'rxjs';
import { ModalService } from '../../core/services/modal.service';

@Injectable({
  providedIn: 'root',
})
export class ShoppingListService {
  private readonly http = inject(HttpClient);
  private readonly modalService = inject(ModalService);

  getShoppingItems(): Observable<GetShoppingItemsDto[]> {
    return this.http
      .get<GetShoppingItemsDto[]>('/shopping-items', {
        withCredentials: true,
        responseType: 'json',
      })
      .pipe(catchError(() => of([] as GetShoppingItemsDto[])));
  }

  createShoppingItem(
    shoppingItem: CreateShoppingItemRequest,
  ): Observable<boolean> {
    return this.http
      .post(`/shopping-items`, shoppingItem, {
        withCredentials: true,
        observe: 'response',
        responseType: 'json',
      })
      .pipe(
        map((res: HttpResponse<object>) => res.ok),
        catchError(() => of(false)),
      );
  }

  updateShoppingItem(
    id: number,
    shoppingItem: UpdateShoppingItemRequest,
  ): Observable<boolean> {
    return this.http
      .put(`/shopping-items/${id}`, shoppingItem, {
        withCredentials: true,
        observe: 'response',
        responseType: 'json',
      })
      .pipe(
        map((res: HttpResponse<object>) => res.ok),
        catchError(() => of(false)),
      );
  }

  deleteShoppingItem(id: number): Observable<boolean> {
    return this.http
      .delete(`/shopping-items/${id}`, {
        withCredentials: true,
        observe: 'response',
        responseType: 'json',
      })
      .pipe(
        map((res: HttpResponse<object>) => res.ok),
        catchError(() => of(false)),
      );
  }

  generateShoppingItems(): Observable<boolean> {
    return this.http
      .post('/shopping-items/generate', null, {
        withCredentials: true,
        observe: 'response',
        responseType: 'json',
      })
      .pipe(
        map((res: HttpResponse<object>) => res.ok),
        catchError((error: HttpErrorResponse) => {
          if (error.status === 409) {
            const title = 'Could Not Generate Shopping Items';
            const message =
              'Previously generated shopping items must be cleared first.';
            this.modalService.openNotificationModal(title, message);
          }

          return of(false);
        }),
      );
  }

  clearShoppingItems(clear = 'all') {
    return this.http
      .delete(`/shopping-items?clear=${clear}`, {
        withCredentials: true,
        observe: 'response',
        responseType: 'json',
      })
      .pipe(
        map((res: HttpResponse<object>) => res.ok),
        catchError(() => of(false)),
      );
  }
}
