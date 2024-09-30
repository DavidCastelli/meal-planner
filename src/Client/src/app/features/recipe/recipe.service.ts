import { inject, Injectable } from '@angular/core';
import {
  HttpClient,
  HttpErrorResponse,
  HttpResponse,
} from '@angular/common/http';
import { CreateRecipeRequest } from './models/create/create-recipe-request.model';
import { UpdateRecipeRequest } from './models/update/update-recipe-request.model';
import { catchError, EMPTY, Observable } from 'rxjs';
import { CreateRecipeDto } from './models/create/create-recipe-dto.model';
import { GetByIdRecipeDto } from './models/get/get-by-id-recipe-dto.model';
import { GetRecipesDto } from './models/get/get-recipes-dto.model';
import { UpdateRecipeDto } from './models/update/update-recipe-dto.model';
import { ErrorService } from '../../core/errors/error.service';

@Injectable({
  providedIn: 'root',
})
export class RecipeService {
  private readonly http = inject(HttpClient);
  private readonly errorService = inject(ErrorService);

  GetRecipes(): Observable<GetRecipesDto[]> {
    return this.http
      .get<GetRecipesDto[]>('/manage/recipes', {
        withCredentials: true,
        responseType: 'json',
      })
      .pipe(catchError(this.handleError<GetRecipesDto[]>([])));
  }

  GetRecipe(id: number): Observable<GetByIdRecipeDto> {
    return this.http
      .get<GetByIdRecipeDto>(`/manage/recipes/${id}`, {
        withCredentials: true,
        responseType: 'json',
      })
      .pipe(catchError(this.handleError<GetByIdRecipeDto>()));
  }

  GetRecipeImage(id: number): Observable<Blob> {
    return this.http
      .get(`manage/recipes/${id}`, {
        withCredentials: true,
        responseType: 'blob',
      })
      .pipe(catchError(this.handleError<Blob>()));
  }

  CreateRecipe(
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
      .pipe(catchError(this.handleError<CreateRecipeDto>()));
  }

  DeleteRecipe(id: number): Observable<HttpResponse<string>> {
    return this.http
      .delete(`manage/recipes/${id}`, {
        withCredentials: true,
        observe: 'response',
        responseType: 'text',
      })
      .pipe(
        catchError((err: HttpErrorResponse) => {
          console.error(err);
          return EMPTY;
        }),
      );
  }

  UpdateRecipe(
    id: number,
    request: UpdateRecipeRequest,
  ): Observable<UpdateRecipeDto> {
    return this.http
      .put<UpdateRecipeDto>(`/manage/recipes/${id}`, request, {
        withCredentials: true,
        responseType: 'json',
      })
      .pipe(catchError(this.handleError<UpdateRecipeDto>()));
  }

  private handleError<T>(result?: T) {
    return this.errorService.handleError<T>(result);
  }
}
