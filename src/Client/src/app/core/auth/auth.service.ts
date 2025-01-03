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
  map,
  Observable,
  of,
  tap,
} from 'rxjs';
import { UserInfo } from './user-info.model';
import {
  SKIP_ERROR_INTERCEPTOR,
  SKIP_UNAUTHORIZED_REDIRECT,
} from '../errors/error.interceptor';
import { ErrorService } from '../errors/error.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly errorService = inject(ErrorService);

  private readonly curUserInfoSource = new BehaviorSubject<UserInfo | null>(
    null,
  );
  public readonly curUserInfo$ = this.curUserInfoSource
    .asObservable()
    .pipe(distinctUntilChanged());

  register(credentials: {
    email: string;
    password: string;
  }): Observable<boolean> {
    return this.http
      .post('/register', credentials, {
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

  login(credentials: { email: string; password: string }): Observable<boolean> {
    return this.http
      .post('/login', credentials, {
        params: { useCookies: true },
        withCredentials: true,
        observe: 'response',
        responseType: 'json',
        context: new HttpContext().set(SKIP_UNAUTHORIZED_REDIRECT, true),
      })
      .pipe(
        map((res: HttpResponse<object>) => {
          return res.ok;
        }),
        catchError((error: HttpErrorResponse) => {
          if (error.status === 401) {
            this.errorService.addMessage('The email or password is incorrect.');
          }
          return of(false);
        }),
      );
  }

  logout(): Observable<boolean> {
    return this.http
      .post(
        '/logout',
        {},
        {
          withCredentials: true,
          observe: 'response',
          responseType: 'json',
        },
      )
      .pipe(
        map((res: HttpResponse<object>) => {
          return res.ok;
        }),
        catchError(() => {
          return of(false);
        }),
        tap(() => this.curUserInfoSource.next(null)),
      );
  }

  getUserInfo(): Observable<UserInfo> {
    return this.http
      .get<UserInfo>('/manage/info', {
        withCredentials: true,
        responseType: 'json',
        context: new HttpContext().set(SKIP_ERROR_INTERCEPTOR, true),
      })
      .pipe(
        catchError(() => {
          return of({} as UserInfo);
        }),
        tap((userInfo) => {
          const currentUser =
            Object.keys(userInfo).length === 0 ? null : userInfo;
          this.curUserInfoSource.next(currentUser);
        }),
      );
  }

  isAuthenticated(): Observable<boolean> {
    return this.getUserInfo().pipe(
      map((userInfo) => {
        return userInfo && Object.keys(userInfo).length > 0;
      }),
      catchError(() => {
        return of(false);
      }),
    );
  }
}
