import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { tap } from 'rxjs/internal/operators/tap';
import { JwtPayload, ResetPasswordDto, UpdateProfileDto, UserDto } from '../interfaces/AccountDto';
import { jwtDecode } from 'jwt-decode';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})

export class AccountService {
  private http = inject(HttpClient);

  private baseUrl = `${environment.apiBaseUrl}`;
  currentUser = signal<UserDto | null>(this.loadUser());
  isLoggedIn = computed(() => !!this.currentUser());
  
  userRoles = computed(() => {
    const token = this.currentUser()?.token;
    if (!token) return [];

    const decoded = jwtDecode<JwtPayload>(token);

    const roles = decoded.role;
    return Array.isArray(roles) ? roles : roles ? [roles] : [];
  });

  roles = computed(() => this.userRoles());

  // Check if user has a specific role
  hasRole(role: string): boolean {
    return this.roles().includes(role);
  }

  // Check if user has any of the provided roles (OR logic)
  hasAnyRole(required: string[]): boolean {
    return required.some(r => this.hasRole(r));
  }

  // ============================================
  // ROLE HIERARCHY HELPERS
  // ============================================
  // These computed signals provide convenient access to common role checks
  // Admin is the highest privilege level
  isAdmin = computed(() => this.hasRole('Admin'));

  // Moderator has admin privileges or is explicitly a moderator
  isModerator = computed(() =>
    this.hasRole('Admin') || this.hasRole('Moderator')
  );

  // Member has admin, moderator, or member privileges
  isMember = computed(() =>
    this.hasRole('Admin') || this.hasRole('Moderator') || this.hasRole('Member')
  );

  // ============================================
  // AUTHENTICATION METHODS
  // ============================================
  // Login endpoint - sends credentials and stores user + token on success
  login(creds: { email: string; password: string }) {
    return this.http.post<UserDto>(this.baseUrl + 'account/login', creds).pipe(
      tap((user) => {
        this.currentUser.set(user);
        localStorage.setItem('user', JSON.stringify(user));
        localStorage.setItem('token', user.token);
      })
    );
  }

  // Logout - clears user state and removes stored data
  logout() {
    this.currentUser.set(null);
    localStorage.removeItem('user');
    localStorage.removeItem('token');
  }

  // THE AUTO-LOGIN Version for when the user register themselves
  // Register endpoint - sends registration credentials and stores user + token on success
  // register(creds: { displayName: string; email: string; password: string }) {
  //   return this.http.post<UserDto>(this.baseUrl + 'account/register', creds).pipe(
  //     tap((user) => {
  //       this.currentUser.set(user);
  //       localStorage.setItem('user', JSON.stringify(user));
  //       localStorage.setItem('token', user.token);
  //     })
  //   );
  // }

  // Register endpoint - sends registration credentials without auto-login
  register(creds: { displayName: string; email: string; password: string }) {
    return this.http.post<UserDto>(this.baseUrl + 'account/register', creds);
    // Removed tap operator - new users are NOT automatically logged in
  }

  getProfile() {
    return this.http.get<UpdateProfileDto>(
      this.baseUrl + 'members/profile'
    );
  }

  updateProfile(profileData: UpdateProfileDto) {
    return this.http.put<UserDto>(this.baseUrl + 'account/profile', profileData).pipe(
      tap((updatedUser) => {
        this.currentUser.set(updatedUser);
        localStorage.setItem('user', JSON.stringify(updatedUser));
        localStorage.setItem('token', updatedUser.token);
      })
    );
  }

  resetPassword(data: ResetPasswordDto) {
    return this.http.post<{ message: string }>(
      this.baseUrl + 'account/reset-password',
      data
    );
  }

  // ============================================
  // PRIVATE HELPER METHODS
  // ============================================
  // Attempts to restore user from localStorage on service initialization
  // Returns null if no stored user or if JSON parsing fails
  private loadUser(): UserDto | null {
    try {
      const raw = localStorage.getItem('user');
      return raw ? (JSON.parse(raw) as UserDto) : null;
    } 
    catch {
      return null;
    }
  }
}
