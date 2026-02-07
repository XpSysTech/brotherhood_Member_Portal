import { NgIf } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AccountService } from '../../../../core/services/account-service';
import { ToastService } from '../../../../core/services/toast-service';

@Component({
  selector: 'app-member-sidebar',
  imports: [NgIf, RouterLink, RouterLinkActive],
  templateUrl: './member-sidebar.component.html',
  styleUrl: './member-sidebar.component.css'
})
export class MemberSidebarComponent {
  accountService = inject(AccountService);
  private toast = inject(ToastService);

  @Input() open = false;              // mobile drawer open/close
  @Output() close = new EventEmitter<void>();

  closeSidebar() {
    this.close.emit();
  }

  notAvailable() {
    this.toast.warning('This page is not available yet.');
  }
}
