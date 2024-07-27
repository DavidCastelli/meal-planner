import {
  HttpContextToken,
  HttpErrorResponse,
  HttpInterceptorFn,
} from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401 && !req.context.get(SKIP_AUTH_INTERCEPTOR)) {
        void router.navigate(['/login']);
      }
      return throwError(() => err);
    }),
  );
};

export const SKIP_AUTH_INTERCEPTOR = new HttpContextToken<boolean>(() => false);
