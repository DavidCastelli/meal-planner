import { InjectionToken } from '@angular/core';
import { ValidationErrors } from '@angular/forms';

const defaultErrors: Record<string, (params: ValidationErrors) => string> = {
  required: () => 'This field is required.',
  email: () => 'Invalid email address.',
  maxlength: ({ requiredLength }) =>
    `Field must be at most ${requiredLength} characters long.`,
  minlength: ({ requiredLength }) =>
    `Field must be at least ${requiredLength} characters long.`,
  min: ({ min }) => `Field must be greater than or equal to ${min}.`,
  max: ({ max }) => `Field must be less than or equal to ${max}.`,
  pattern: ({ requiredPattern }) =>
    `Field must match the following pattern ${requiredPattern}.`,
};

export const FORM_ERRORS = new InjectionToken('FORM_ERRORS', {
  providedIn: 'root',
  factory: () => defaultErrors,
});
