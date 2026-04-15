import { Component, inject, signal } from '@angular/core';
import { NgIf } from "@angular/common";
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AccountService } from '../../../../core/services/account-service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, NgIf],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  private account = inject(AccountService);
  private router = inject(Router);

  // mobile menu
  mobileOpen = false;

  // user dropdown
  userMenuOpen = signal(false);

  // expose user signal to template
  currentUser = this.account.currentUser;

  defaultAvatar =
    'https://ui-avatars.com/api/?background=0f172a&color=fff&name=Member';

  toggleMobile() {
    this.mobileOpen = !this.mobileOpen;
  }

  closeMobile() {
    this.mobileOpen = false;
  }

  toggleUserMenu() {
    this.userMenuOpen.set(!this.userMenuOpen());
  }

  logout() {
    this.account.logout();
    this.userMenuOpen.set(false);
    this.router.navigateByUrl('/');
  }
}
