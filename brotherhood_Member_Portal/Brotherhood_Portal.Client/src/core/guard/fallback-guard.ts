import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../services/account-service';

/**
 * Fallback Guard
 * 
 * A route guard that prevents direct navigation to undefined or invalid routes.
 * Redirects authenticated users to the member dashboard and unauthenticated users to the home page.
 * 
 * This guard is typically used as a wildcard route handler to catch any attempts to access
 * routes that don't exist or haven't been explicitly defined in the routing configuration.
 */
export const fallbackGuard: CanActivateFn = () => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  // Check if user is currently authenticated
  if (accountService.isLoggedIn()) {
    // Redirect logged-in users to their member dashboard
    router.navigate(['/member/dashboard']);
  } else {
    // Redirect unauthenticated users to the home page
    router.navigate(['/']);
  }

  // Return false to cancel the original navigation attempt
  // This prevents the user from staying on the invalid/undefined route
  return false;
};
