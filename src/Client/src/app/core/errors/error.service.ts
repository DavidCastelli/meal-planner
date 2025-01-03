import { Injectable } from '@angular/core';
import { ValidationProblemDetails } from './models/validation-problem-details.model';

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
}
