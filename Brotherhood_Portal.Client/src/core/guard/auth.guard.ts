import { inject } from '@angular/core';
import { CanActivateFn, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AccountService } from '../services/account-service';
import { ToastService } from '../services/toast-service';

export const authGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const accountService = inject(AccountService);
  const toastService = inject(ToastService);
  const router = inject(Router);

  /* ------------------------------
     1. Not Logged In
  ------------------------------ */
  if (!accountService.isLoggedIn()) {
    toastService.error('You must be logged in to access this page.');
    router.navigate(['/login']);
    return false;
  }

  /* ------------------------------
     2. Role-Based Access
  ------------------------------ */
  const requiredRoles = route.data['roles'] as string[] | undefined;

  if (requiredRoles && !accountService.hasAnyRole(requiredRoles)) {
    toastService.warning('You do not have permission to access this section.');
    // router.navigate(['/unauthorized']);
    return false;
  }

  /* ------------------------------
     3. Access Granted
  ------------------------------ */
  return true;
};
