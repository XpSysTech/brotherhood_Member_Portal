import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ToastService } from '../services/toast-service';
import { Router } from '@angular/router';
import { AccountService } from '../services/account-service';
import { catchError, throwError } from 'rxjs';

/**
 * AuthInterceptor - Functional HTTP interceptor that automatically attaches JWT authentication tokens
 * to outgoing HTTP requests. Uses the modern functional interceptor approach (Angular 14+).
 * 
 * @param req - The outgoing HTTP request
 * @param next - Function to pass the request to the next interceptor in the chain
 * @returns Observable of the HTTP event after processing through the interceptor chain
 */
export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
  const accountService = inject(AccountService);
  const router = inject(Router);
  const toast = inject(ToastService);

  // Retrieve the JWT token from browser localStorage
  // const token = localStorage.getItem('token');
  const token = accountService.currentUser()?.token;

  // If a token exists, clone the request and add the Authorization header with Bearer token
  // if (token) {
  //   req = req.clone({
  //     setHeaders: {
  //       Authorization: `Bearer ${token}`
  //     }
  //   });
  // }
  const authReq = token
    ? req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        toast.warning('Your session has expired. Please log in again.');
        accountService.logout();
        router.navigate(['/login']);
      }

      return throwError(() => error);
    })
  );

  // Forward the request (with or without the token) to the next interceptor in the chain
  // return next(req);
};


/**
 * ============ USAGE GUIDE ============
 * WHEN IT TRIGGERS:
 * - Every time your application makes an HTTP request (GET, POST, PUT, DELETE, etc.)
 * - Automatically runs before the request is sent to the backend server
 * 
 * WHAT IT DOES:
 * - Checks if a 'token' exists in browser localStorage
 * - If token exists: Adds an "Authorization: Bearer {token}" header to the request
 * - If no token: Passes the request through unchanged
 * - Works with protected API endpoints that require authentication
 * 
 * EXAMPLE USE CASES:
 * - Making authenticated API calls from any service (MemberService, FinanceService, etc.)
 * - Protecting routes that require user login
 * - Ensuring all API requests to the backend include the user's JWT token
 * - No need to manually add Authorization header in each HTTP request
 * 
 * NOTE:
 * - Only one interceptor registration needed - it applies globally to all HTTP requests
 * - The token must be stored in localStorage with the key 'token' for this to work
 * =====================================
 */