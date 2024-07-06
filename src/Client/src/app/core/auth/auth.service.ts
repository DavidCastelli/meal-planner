import { Inject, inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { API_CONFIG, ApiConfig } from '../../shared/api.config';
import { Observable, catchError, throwError } from 'rxjs';
import { ProblemDetails } from '../../shared/problem-details.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly ENDPOINT: string;

  constructor(@Inject(API_CONFIG) private readonly apiConfig: ApiConfig) {
    this.ENDPOINT = `${apiConfig.baseUrl}/${apiConfig.prefix}`;
  }

  register(credentials: { email: string; password: string }): Observable<void> {
    // Expects a type of void because the register endpoint of the API returns 200 OK with no response on success.
    return this.http
      .post<void>(`${this.ENDPOINT}/register`, credentials)
      .pipe(catchError(this.handleError));
  }

  private handleError(err: HttpErrorResponse): Observable<never> {
    let errorMessage = '';
    if (err.status === 0) {
      // A client or network error occured.
      console.error('An error occured: ', err.error);
      errorMessage = 'Something went wrong, please try again later.';
    } else {
      // A server error occured and returned an unsuccessful response code.
      console.error(
        `Server returned code: ${err.status}, error message is: `,
        err.message,
      );
      // Expects the API to return an error of type ProblemDetails.
      const problemDetails = err.error as ProblemDetails;
      const errors = Object.values(problemDetails.errors);

      // The error property of a ProblemDetails comes in the format of an array of string arrays.
      // Each inner array is expected to only have a single value.
      // Iterates over the outer array and builds a single message in the case of multiple errors.
      let result = '';
      for (const error of errors) {
        result += error[0] + '\n';
      }
      errorMessage = result;
    }
    // User facing message.
    return throwError(() => errorMessage);
  }
}
