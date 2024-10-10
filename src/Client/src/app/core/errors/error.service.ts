import { Injectable } from '@angular/core';
import { ValidationProblemDetails } from '../../shared/models/validation-problem-details.model';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ErrorService {
  messages: string[] = [];

  addMessage(message: string): void {
    this.messages.push(message);
  }

  clear(): void {
    this.messages = [];
  }

  addProblem(validationProblemDetails: ValidationProblemDetails): void {
    Object.keys(validationProblemDetails.errors).forEach((errorKey) => {
      const errors = validationProblemDetails.errors[errorKey];
      for (const error of errors) {
        const errorMessage = `${errorKey}: ${error}`;
        this.messages.push(errorMessage);
      }
    });
  }

  handleError<T>(result?: T) {
    return (error: HttpErrorResponse): Observable<T> => {
      console.error(error);

      switch (error.status) {
        case 0:
          this.addMessage('Something went wrong, please try again later.');
          break;
        case 400:
          this.addProblem(error.error as ValidationProblemDetails);
          break;
        case 500:
          this.addMessage(
            'An unexpected error occured, please try again later.',
          );
          break;
        default:
          this.addMessage('An unknown error occured.');
      }

      return of(result as T);
    };
  }
}
