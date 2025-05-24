import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { Schedule } from '../../shared/enums/schedule.enum';
import { GetMealsDto } from './models/get-meals-dto.model';

@Injectable({
  providedIn: 'root',
})
export class ScheduleService {
  private readonly http = inject(HttpClient);

  getMeals(scheduled = false): Observable<GetMealsDto[]> {
    return this.http
      .get<GetMealsDto[]>(`/manage/meals?scheduled=${scheduled}`, {
        withCredentials: true,
        responseType: 'json',
      })
      .pipe(catchError(() => of([] as GetMealsDto[])));
  }

  addScheduledMeal(id: number, day: Schedule) {
    const document = [
      {
        op: 'add',
        path: '/schedule',
        value: day,
      },
    ];

    const headers = new HttpHeaders({
      'Content-Type': 'application/json-patch+json; charset=UTF-8',
    });

    return this.http
      .patch(`/manage/meals/${id}/schedule`, document, {
        withCredentials: true,
        observe: 'response',
        responseType: 'json',
        headers: headers,
      })
      .pipe(
        map((res: HttpResponse<object>) => res.ok),
        catchError(() => of(false)),
      );
  }

  removeScheduledMeal(id: number): Observable<boolean> {
    const document = [
      {
        op: 'remove',
        path: '/schedule',
      },
    ];

    const headers = new HttpHeaders({
      'Content-Type': 'application/json-patch+json; charset=UTF-8',
    });

    return this.http
      .patch(`/manage/meals/${id}/schedule`, document, {
        withCredentials: true,
        observe: 'response',
        responseType: 'json',
        headers: headers,
      })
      .pipe(
        map((res: HttpResponse<object>) => res.ok),
        catchError(() => of(false)),
      );
  }

  clearScheduledMeals(): Observable<boolean> {
    return this.http
      .patch('/manage/meals/schedule', null, {
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
