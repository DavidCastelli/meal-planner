import { inject, Injectable } from '@angular/core';
import {
  HttpClient,
  HttpContext,
  HttpErrorResponse,
  HttpResponse,
} from '@angular/common/http';
import {
  BehaviorSubject,
  catchError,
  distinctUntilChanged,
  EMPTY,
  map,
  Observable,
  of,
  switchMap,
  tap,
  throwError,
} from 'rxjs';
import { ValidationProblemDetails } from '../../shared/models/validation-problem-details.model';
import { UserInfo } from './user-info.model';
import { SKIP_AUTH_INTERCEPTOR } from '../interceptors/auth.interceptor';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly curUserInfoSource = new BehaviorSubject<UserInfo | null>(
    JSON.parse(localStorage.getItem('currentUser') as string),
  );
  public curUserInfo$ = this.curUserInfoSource
    .asObservable()
    .pipe(distinctUntilChanged());

  register(credentials: {
    email: string;
    password: string;
  }): Observable<HttpResponse<string>> {
    return this.http
      .post('/register', credentials, {
        observe: 'response',
        responseType: 'text',
      })
      .pipe(
        catchError((err: HttpErrorResponse) => {
          this.logError(err);
          if (err.status === 0) {
            return throwError(
              () => 'Something went wrong, please try again later.',
            );
          } else if (err.status === 400) {
            // Expects the API to return an error of type ValidationProblemDetails.
            const validationProblemDetails =
              err.error as ValidationProblemDetails;

            // Each inner array is expected to only have a single value.
            const errors = Object.values(validationProblemDetails.errors);

            // Iterates over the outer array and builds a single message in the case of multiple errors.
            let result = '';
            for (const error of errors) {
              result += error[0] + '\n';
            }
            return throwError(() => result);
          } else {
            return throwError(() => 'An unknown error occured.');
          }
        }),
      );
  }

  login(credentials: {
    email: string;
    password: string;
  }): Observable<UserInfo> {
    return this.http
      .post('/login', credentials, {
        params: { useCookies: true },
        withCredentials: true,
        observe: 'response',
        responseType: 'text',
        context: new HttpContext().set(SKIP_AUTH_INTERCEPTOR, true),
      })
      .pipe(
        catchError((err: HttpErrorResponse) => {
          this.logError(err);
          if (err.status === 0) {
            return throwError(
              () => 'Something went wrong, please try again later.',
            );
          } else if (err.status === 401) {
            return throwError(() => 'The email or password is incorrect.');
          } else {
            return throwError(() => 'An unknown error occured.');
          }
        }),
        switchMap(() => this.getUserInfo()),
      );
  }

  logout(): Observable<HttpResponse<string>> {
    return this.http
      .post(
        '/logout',
        {},
        {
          withCredentials: true,
          observe: 'response',
          responseType: 'text',
        },
      )
      .pipe(
        catchError((err: HttpErrorResponse) => {
          this.logError(err);
          return EMPTY;
        }),
        tap(() => {
          localStorage.removeItem('currentUser');
          this.curUserInfoSource.next(null);
        }),
      );
  }

  getUserInfo(): Observable<UserInfo> {
    return this.http
      .get<UserInfo>('/manage/info', {
        withCredentials: true,
        context: new HttpContext().set(SKIP_AUTH_INTERCEPTOR, true),
      })
      .pipe(
        catchError((err: HttpErrorResponse) => {
          this.logError(err);
          return of({} as UserInfo);
        }),
        tap((userInfo) => {
          const currentUser =
            Object.keys(userInfo).length === 0 ? null : userInfo;
          localStorage.setItem('currentUser', JSON.stringify(currentUser));
          this.curUserInfoSource.next(currentUser);
        }),
      );
  }

  isAuthenticated(): Observable<boolean> {
    return this.getUserInfo().pipe(
      map((userInfo) => {
        return userInfo && Object.keys(userInfo).length > 0;
      }),
      catchError((err: HttpErrorResponse) => {
        this.logError(err);
        return of(false);
      }),
    );
  }

  private logError(err: HttpErrorResponse) {
    if (err.status === 0) {
      // A client or network error occured.
      console.error('An error occured: ', err.error);
    } else {
      // A server error occured and returned an unsuccessful response code.
      console.error(
        `Server returned code: ${err.status}, error message is: `,
        err.message,
      );
    }
  }
}
