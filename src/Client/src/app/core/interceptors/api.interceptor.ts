import { HttpInterceptorFn } from '@angular/common/http';

export const apiInterceptor: HttpInterceptorFn = (req, next) => {
  const reqWithBaseUrl = req.clone({
    url: `https://localhost:7256/api${req.url}`,
  });
  return next(reqWithBaseUrl);
};
