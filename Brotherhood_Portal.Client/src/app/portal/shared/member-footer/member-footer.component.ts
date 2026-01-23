import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AccountService } from '../../../../core/services/account-service';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-member-footer',
  imports: [NgIf, RouterLink],
  templateUrl: './member-footer.component.html',
  styleUrl: './member-footer.component.css'
})
export class MemberFooterComponent {
  private account = inject(AccountService);
  private router = inject(Router);

  protected currentUser = this.account.currentUser;
  protected currentYear = new Date().getFullYear();
}
