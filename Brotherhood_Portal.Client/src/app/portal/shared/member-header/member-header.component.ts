import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { NgIf } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AccountService } from '../../../../core/services/account-service';

@Component({
  selector: 'app-member-header',
  standalone: true,
  imports: [NgIf, RouterLink],
  templateUrl: './member-header.component.html',
  styleUrl: './member-header.component.css'
})
export class MemberHeaderComponent {
  private account = inject(AccountService);
  private router = inject(Router);

  // Expose user signal to template
  protected currentUser = this.account.currentUser;
  
  // Sidebar toggle (parent handles open/close)
  // @Output() sidebarToggle = new EventEmitter<void>();

  @Input() open = false;
  @Output() sidebarToggle = new EventEmitter<void>();


  // Menus
  desktopMenuOpen = signal(false);
  mobileMemberMenuOpen = signal(false);

  protected defaultAvatar =
    'https://ui-avatars.com/api/?background=0f172a&color=fff&name=Member';

  toggleSidebar() {
    this.sidebarToggle.emit();
  }

  toggleDesktopMenu() {
    this.desktopMenuOpen.set(!this.desktopMenuOpen());
  }

  toggleMobileMemberMenu() {
    this.mobileMemberMenuOpen.set(!this.mobileMemberMenuOpen());
  }

  closeMenus() {
    this.desktopMenuOpen.set(false);
    this.mobileMemberMenuOpen.set(false);
  }

  logout() {
    this.account.logout();
    this.closeMenus();
    this.router.navigateByUrl('/');
  }
}
