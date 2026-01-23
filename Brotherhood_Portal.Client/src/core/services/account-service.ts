import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { tap } from 'rxjs/internal/operators/tap';

export interface UserDto {
  id: string;
  displayName: string;
  email: string;
  imageUrl?: string | null;
  token: string;
}

@Injectable({
  providedIn: 'root'
})

export class AccountService {
  private http = inject(HttpClient);

  baseUrl: string = 'https://localhost:5001/api/';

  // ✅ global auth state
  currentUser = signal<UserDto | null>(this.loadUser());
  isLoggedIn = computed(() => !!this.currentUser());

  login(creds: { email: string; password: string }) {
    return this.http.post<UserDto>(this.baseUrl + 'account/login', creds).pipe(
      tap((user) => {
        this.currentUser.set(user);
        localStorage.setItem('user', JSON.stringify(user));
        localStorage.setItem('token', user.token);
      })
    );
  }

  logout() {
    this.currentUser.set(null);
    localStorage.removeItem('user');
    localStorage.removeItem('token');
  }

  private loadUser(): UserDto | null {
    try {
      const raw = localStorage.getItem('user');
      return raw ? (JSON.parse(raw) as UserDto) : null;
    } catch {
      return null;
    }
  }
}
