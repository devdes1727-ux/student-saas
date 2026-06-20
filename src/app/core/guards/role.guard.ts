import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { SessionService } from '../services/session.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const session = inject(SessionService);

  if (!session.isLoggedIn()) {
    return router.createUrlTree(['/login']);
  }

  const user = session.getUser();
  const userRole = user?.role;

  // Read expected roles from route data
  const expectedRoles = route.data['expectedRoles'] as string[];

  if (expectedRoles && expectedRoles.length > 0) {
    const hasRole = expectedRoles.some(role => 
      userRole?.toLowerCase() === role.toLowerCase()
    );

    if (!hasRole) {
      return router.createUrlTree(['/unauthorized']);
    }
  }

  return true;
};
