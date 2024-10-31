import {
  HttpContextToken,
  HttpErrorResponse,
  HttpInterceptorFn,
  HttpStatusCode,
} from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { ValidationProblemDetails } from '../../shared/models/validation-problem-details.model';
import { inject } from '@angular/core';
import { ErrorService } from '../errors/error.service';
import { Router } from '@angular/router';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const errorService = inject(ErrorService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      switch (error.status) {
        case 0:
          console.error('An error occured:', error.error.message);
          errorService.addMessage(`An error occured: ${error.error.message}`);
          break;
        case HttpStatusCode.BadRequest:
          console.error(`Bad Request: ${error.message}`);
          errorService.addProblem(error.error as ValidationProblemDetails);
          break;
        case HttpStatusCode.Unauthorized:
          console.error(`Unauthorized: ${error.message}`);
          if (!req.context.get(SKIP_UNAUTHORIZED_REDIRECT)) {
            void router.navigate(['/login']);
          }
          break;
        case HttpStatusCode.Forbidden:
          console.error(`Forbidden: ${error.message}`);
          break;
        case HttpStatusCode.NotFound:
          console.error(`Not Found: ${error.message}`);
          break;
        case HttpStatusCode.InternalServerError:
          console.error(`Internal Server Error: ${error.message}`);
          errorService.addMessage(
            'An unexpected error occured, please try again later.',
          );
          break;
        default:
          console.error(`HTTP error: ${error.status} - ${error.statusText}`);
          errorService.addMessage(
            `HTTP error: ${error.status} - ${error.statusText}`,
          );
      }

      return throwError(() => error);
    }),
  );
};

export const SKIP_UNAUTHORIZED_REDIRECT = new HttpContextToken<boolean>(
  () => false,
);
