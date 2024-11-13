import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { CreateRecipeRequest } from './models/create/create-recipe-request.model';
import { UpdateRecipeRequest } from './models/update/update-recipe-request.model';
import { catchError, EMPTY, Observable, of } from 'rxjs';
import { CreateRecipeDto } from './models/create/create-recipe-dto.model';
import { GetByIdRecipeDto } from './models/get/get-by-id-recipe-dto.model';
import { GetRecipesDto } from './models/get/get-recipes-dto.model';
import { UpdateRecipeDto } from './models/update/update-recipe-dto.model';

@Injectable({
  providedIn: 'root',
})
export class RecipeService {
  private readonly http = inject(HttpClient);

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

  getRecipe(id: number): Observable<GetByIdRecipeDto> {
    return this.http
      .get<GetByIdRecipeDto>(`/manage/recipes/${id}`, {
        withCredentials: true,
        responseType: 'json',
      })
      .pipe(
        catchError(() => {
          return of({} as GetByIdRecipeDto);
        }),
      );
  }

  createRecipe(
    request: CreateRecipeRequest,
    image: File | null,
  ): Observable<CreateRecipeDto> {
    const formData = new FormData();

    const data = new Blob([JSON.stringify(request)], {
      type: 'application/json',
    });
    formData.append('data', data, 'data.json');

    if (image) {
      formData.append('image', image);
    }

    return this.http
      .post<CreateRecipeDto>('/manage/recipes', formData, {
        withCredentials: true,
        responseType: 'json',
      })
      .pipe(
        catchError(() => {
          return of({} as CreateRecipeDto);
        }),
      );
  }

  deleteRecipe(id: number): Observable<HttpResponse<string>> {
    return this.http
      .delete(`manage/recipes/${id}`, {
        withCredentials: true,
        observe: 'response',
        responseType: 'text',
      })
      .pipe(
        catchError(() => {
          return EMPTY;
        }),
      );
  }

  updateRecipe(
    id: number,
    request: UpdateRecipeRequest,
  ): Observable<UpdateRecipeDto> {
    return this.http
      .put<UpdateRecipeDto>(`/manage/recipes/${id}`, request, {
        withCredentials: true,
        responseType: 'json',
      })
      .pipe(
        catchError(() => {
          return of({} as UpdateRecipeDto);
        }),
      );
  }
}
