import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../auth.service';
import { map } from 'rxjs';

export const loginGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.isAuthenticated().pipe(
    map((isLoggedIn) => {
      if (isLoggedIn) {
        void router.navigate(['/home']);
        return false;
      }

      return true;
    }),
  );
};
