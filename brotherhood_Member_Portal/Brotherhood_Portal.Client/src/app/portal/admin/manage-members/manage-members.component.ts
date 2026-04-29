import { Component, inject } from '@angular/core';
import { AccountService } from '../../../../core/services/account-service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MemberByName } from '../../../../core/interfaces/Member';
import { FinanceService } from '../../../../core/services/finance-service';

@Component({
  selector: 'app-manage-members',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './manage-members.component.html',
  styleUrl: './manage-members.component.css'
})
export class ManageMembersComponent {
  private account = inject(AccountService);
  private financeService = inject(FinanceService);

  members: MemberByName[] = [];
  selectedMemberId: string | null = null;
  feedback: string | null = null; 

  ngOnInit(): void {
    // console.log('Component loaded');
    this.loadMembers();
  }

  get selectedMember() {
    return this.members.find(m => m.id === this.selectedMemberId);
  }

  get selectedDisplayName(): string {
    return this.selectedMember?.displayName ?? 'No member selected';
  }

  //Loads members for dropdown selection.
  private loadMembers(): void {
    this.financeService.getMembersForDropdown()
      .subscribe({
        next: data => {
          // console.log('Members received:', data);
          this.members = [...data].sort((a, b) =>
            (a.displayName ?? '').localeCompare(b.displayName ?? '')
          );
        },
        error: () => {
          // Provide user-visible feedback instead of console logging
          this.feedback = 'Failed to load members.';
        }
      });
  }

  // Lock Member

  // Update Member Info

  // Reset Password

  newPassword: string = '';
  loading = false;
  message = '';

  resetPassword() {
    if (!this.selectedMemberId || !this.newPassword) return;

    this.loading = true;

    this.account.resetPassword({
      userId: this.selectedMemberId,
      newPassword: this.newPassword
    }).subscribe({
      next: (res: any) => {
        this.message = res.message;
        this.newPassword = '';
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }

  // Audit Trail
}
