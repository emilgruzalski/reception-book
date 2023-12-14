import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from '../services/auth.service';

/** Allows the route only for users with one of the roles listed in route data. */
export const roleGuard: CanActivateFn = route => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const roles: string[] = route.data['roles'] ?? [];

  return authService.hasAnyRole(roles) ? true : router.createUrlTree(['/dashboard']);
};
