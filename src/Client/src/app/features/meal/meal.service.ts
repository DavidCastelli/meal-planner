import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { catchError, EMPTY, map, Observable, of } from 'rxjs';
import { GetMealsDto } from './models/get/get-meals-dto.model';
import { GetMealByIdDto } from './models/get/get-meal-by-id-dto.model';
import { CreateMealRequest } from './models/create/create-meal-request.model';
import { UpdateMealRequest } from './models/update/update-meal-request.model';

@Injectable({
  providedIn: 'root',
})
export class MealService {
  private readonly http = inject(HttpClient);

  getMeals(): Observable<GetMealsDto[]> {
    return this.http
      .get<GetMealsDto[]>('/manage/meals', {
        withCredentials: true,
        responseType: 'json',
      })
      .pipe(
        catchError(() => {
          return of([] as GetMealsDto[]);
        }),
      );
  }

  getMeal(id: number): Observable<GetMealByIdDto> {
    return this.http
      .get<GetMealByIdDto>(`/manage/meals/${id}`, {
        withCredentials: true,
        responseType: 'json',
      })
      .pipe(
        catchError(() => {
          return EMPTY;
        }),
      );
  }

  createMeal(
    request: CreateMealRequest,
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
      .post('/manage/meals', formData, {
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

  deleteMeal(id: number): Observable<boolean> {
    return this.http
      .delete(`/manage/meals/${id}`, {
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

  updateMeal(
    id: number,
    request: UpdateMealRequest,
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
      .put(`/manage/meals/${id}`, formData, {
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
