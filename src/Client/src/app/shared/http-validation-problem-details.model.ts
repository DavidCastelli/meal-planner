import { ProblemDetails } from './problem-details.model';

export interface HttpValidationProblemDetails extends ProblemDetails {
  errors: Record<string, string[]>;
}
