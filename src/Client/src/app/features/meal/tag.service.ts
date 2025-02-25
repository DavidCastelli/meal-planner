import { inject, Injectable } from '@angular/core';
import { catchError, Observable, of } from 'rxjs';
import { GetTagsDto } from './models/get/get-tags-dto.model';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class TagService {
  private readonly http = inject(HttpClient);

  getTags(): Observable<GetTagsDto[]> {
    return this.http
      .get<GetTagsDto[]>('/manage/tags', {
        withCredentials: true,
        responseType: 'json',
      })
      .pipe(
        catchError(() => {
          return of([] as GetTagsDto[]);
        }),
      );
  }
}
