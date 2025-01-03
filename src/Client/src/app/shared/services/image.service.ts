import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ImageService {
  private readonly http = inject(HttpClient);

  getImage(fileName: string): Observable<Blob> {
    return this.http
      .get(`/images/${fileName}`, {
        withCredentials: true,
        responseType: 'blob',
      })
      .pipe(
        catchError(() => {
          return of(new Blob());
        }),
      );
  }
}
