export interface ProblemDetails {
  extensions: Record<string, object>;
  instance?: string;
  detail?: string;
  status?: number;
  title?: string;
  type?: string;
}
