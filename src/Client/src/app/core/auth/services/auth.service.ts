import { Inject, inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { API_CONFIG, ApiConfig } from '../../shared/api.config';
import {Observable, catchError, throwError, of} from 'rxjs';
import { ValidationProblemDetails } from '../../shared/validation-problem-details.model';
import {User} from "./user.model";

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

  login(credentials: { email: string; password: string }): Observable<void> {
    // Expects a type of void because the login endpoint of the API returns 200 OK with no response on success.
    return this.http
      .post<void>(`${this.ENDPOINT}/login`, credentials, {
        params: { useCookies: true },
        withCredentials: true,
      })
      .pipe(catchError(this.handleError));
  }

  getUserInfo(): Observable<User> {
    // TODO handle errors
    return this.http
      .get<User>(`${this.ENDPOINT}/manage/info`, {
        withCredentials: true,
      });
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

      if (err.status === 400) {
        // Expects the API to return an error of type ValidationProblemDetails.
        const validationProblemDetails = err.error as ValidationProblemDetails;

        // Each inner array is expected to only have a single value.
        const errors = Object.values(validationProblemDetails.errors);

        // Iterates over the outer array and builds a single message in the case of multiple errors.
        let result = '';
        for (const error of errors) {
          result += error[0] + '\n';
        }
        errorMessage = result;
      } else if (err.status === 401) {
        errorMessage = 'The email or password is incorrect.';
      } else {
        errorMessage = 'An unexpected error occured.';
      }
    }
    // User facing message.
    return throwError(() => errorMessage);
  }
}
