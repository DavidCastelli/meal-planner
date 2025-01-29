import { inject, Injectable } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,
  HttpResponse,
} from '@angular/common/http';
import { CreateRecipeRequest } from './models/create/create-recipe-request.model';
import { UpdateRecipeRequest } from './models/update/update-recipe-request.model';
import { catchError, EMPTY, map, Observable, of } from 'rxjs';
import { GetRecipeByIdDto } from './models/get/get-recipe-by-id-dto.model';
import { GetRecipesDto } from './models/get/get-recipes-dto.model';
import { ErrorService } from '../../core/errors/error.service';
import { ModalService } from '../../core/services/modal.service';

@Injectable({
  providedIn: 'root',
})
export class RecipeService {
  private readonly http = inject(HttpClient);
  private readonly errorService = inject(ErrorService);
  private readonly modalService = inject(ModalService);

  getRecipes(): Observable<GetRecipesDto[]> {
    return this.http
      .get<GetRecipesDto[]>('/manage/recipes', {
        withCredentials: true,
        responseType: 'json',
      })
      .pipe(
        catchError(() => {
          return of([] as GetRecipesDto[]);
        }),
      );
  }

  getRecipe(id: number): Observable<GetRecipeByIdDto> {
    return this.http
      .get<GetRecipeByIdDto>(`/manage/recipes/${id}`, {
        withCredentials: true,
        responseType: 'json',
      })
      .pipe(
        catchError(() => {
          return EMPTY;
        }),
      );
  }

  createRecipe(
    request: CreateRecipeRequest,
    image: File | null,
  ): Observable<boolean> {
    const formData = new FormData();

    const data = new Blob([JSON.stringify(request)], {
      type: 'application/json',
    });
    formData.append('data', data, 'data.json');

    if (image) {
      formData.append('image', image);
    }

    return this.http
      .post('/manage/recipes', formData, {
        withCredentials: true,
        observe: 'response',
        responseType: 'json',
      })
      .pipe(
        map((res: HttpResponse<object>) => {
          return res.ok;
        }),
        catchError(() => {
          return of(false);
        }),
      );
  }

  deleteRecipe(id: number): Observable<boolean> {
    return this.http
      .delete(`/manage/recipes/${id}`, {
        withCredentials: true,
        observe: 'response',
        responseType: 'json',
      })
      .pipe(
        map((res: HttpResponse<object>) => {
          return res.ok;
        }),
        catchError((error: HttpErrorResponse) => {
          if (error.status === 409) {
            const title = 'Could Not Delete Recipe';
            const message = this.errorService.messages.pop();
            this.modalService.openNotificationModal(title, message);
          }
          return of(false);
        }),
      );
  }

  updateRecipe(
    id: number,
    request: UpdateRecipeRequest,
    image: File | null,
  ): Observable<boolean> {
    const formData = new FormData();

    const data = new Blob([JSON.stringify(request)], {
      type: 'application/json',
    });
    formData.append('data', data, 'data.json');

    if (image) {
      formData.append('image', image);
    }

    return this.http
      .put(`/manage/recipes/${id}`, formData, {
        withCredentials: true,
        observe: 'response',
        responseType: 'json',
      })
      .pipe(
        map((res: HttpResponse<object>) => {
          return res.ok;
        }),
        catchError(() => {
          return of(false);
        }),
      );
  }
}
