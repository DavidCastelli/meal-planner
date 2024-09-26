import { Injectable } from '@angular/core';
import { ValidationProblemDetails } from '../../shared/validation-problem-details.model';
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

  addProblem(validationProblemDetails: ValidationProblemDetails): void {
    const message = Object.entries(validationProblemDetails.errors).reduce(
      (acc, cur) => {
        acc += `${cur[0]}:`;
        for (const error of cur[1]) {
          acc += ` ${error}`;
        }
        acc += '\n';

        return acc;
      },
      '',
    );

    this.addMessage(message);
  }

  clear(): void {
    this.messages = [];
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
