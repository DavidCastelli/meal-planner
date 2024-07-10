import { HttpValidationProblemDetails } from './http-validation-problem-details.model';

export interface ValidationProblemDetails extends HttpValidationProblemDetails {
  errors: Record<string, string[]>;
}
