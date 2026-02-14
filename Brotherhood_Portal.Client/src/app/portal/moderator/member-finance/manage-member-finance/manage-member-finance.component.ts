import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinanceService } from '../../../../../core/services/finance-service';
import { FinanceRecord } from '../../../../../core/interfaces/FinanceDto';
import { MemberByName } from '../../../../../core/interfaces/Member';

@Component({
  selector: 'app-manage-member-finance',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './manage-member-finance.component.html'
})
export class ManageMemberFinanceComponent implements OnInit {

  /**
   * List of members used for dropdown selection.
   * This is intentionally a lightweight DTO (MemberByName),
   * not a full Member object, because we only need id + displayName.
   */
  members: MemberByName[] = [];

  /**
   * Currently selected member id from dropdown.
   * Used when submitting deposits and loading history.
   */
  selectedMemberId: string | null = null;

  /**
   * Deposit form model.
   * This object is bound to the template using ngModel.
   * Keeping it grouped improves clarity and makes reset easier.
   */
  depositForm = {
    savingsAmount: 0,
    opsContribution: 0,
    description: ''
  };

  /**
   * Financial history records for selected member.
   */
  history: FinanceRecord[] = [];

  /**
   * UI state flags
   */
  loadingHistory = false;  // Controls history loading indicator
  submitting = false;      // Prevents double submission
  feedback: string | null = null; // User feedback messages

  constructor(private financeService: FinanceService) {}

  /**
   * Lifecycle hook.
   * On component load, fetch members for dropdown.
   */
  ngOnInit(): void {
    console.log('Component loaded');
    this.loadMembers();
  }

  /**
   * Loads members for dropdown selection.
   *
   * - Uses FinanceService to fetch minimal DTO.
   * - Creates a NEW sorted array instead of mutating the API response.
   * - Uses null-safe sorting to prevent runtime errors.
   */
  private loadMembers(): void {
    this.financeService.getMembersForDropdown()
      .subscribe({
        // next: data => {
        //   // Clone array before sorting to avoid mutating response reference
        //   // TODO: Add FirstName and lastName to sort
        //   this.members = [...data].sort((a, b) =>
        //     (a.displayName ?? '').localeCompare(b.displayName ?? ''),
        //   );
        // },
        next: data => {
          console.log('Members received:', data);
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

  /**
   * Submits a deposit for the selected member.
   *
   * Guards:
   * - Prevent submission if no member selected
   * - Prevent duplicate submission while request is in flight
   */
  submitDeposit(): void {
    if (!this.selectedMemberId || this.submitting) {
      return;
    }

    this.submitting = true;
    this.feedback = null;

    this.financeService.addDeposit({
      memberId: this.selectedMemberId,
      savingsAmount: this.depositForm.savingsAmount,
      opsContribution: this.depositForm.opsContribution,
      description: this.depositForm.description
    }).subscribe({
      next: res => {
        // Show backend message (Admin vs Moderator differs)
        this.feedback = res.message;

        // Clear form inputs after success
        this.resetForm();

        // Refresh history so new deposit appears immediately
        this.loadHistory();
      },
      error: err => {
        // Display meaningful backend error if available
        this.feedback = err?.error?.message ?? 'Failed to add deposit.';
      },
      complete: () => {
        // Reset submitting flag regardless of outcome
        this.submitting = false;
      }
    });
  }

  /**
   * Loads financial history for the selected member.
   *
   * Only executes if a member is selected.
   */
  loadHistory(): void {
    if (!this.selectedMemberId) return;

    this.loadingHistory = true;
    this.feedback = null;

    this.financeService.getMemberHistory(this.selectedMemberId)
      .subscribe({
        next: data => {
          this.history = data;
        },
        error: () => {
          this.feedback = 'Failed to load history.';
        },
        complete: () => {
          this.loadingHistory = false;
        }
      });
  }

  /**
   * Resets deposit form back to initial state.
   * Keeps state predictable and avoids stale data.
   */
  resetForm(): void {
    this.depositForm = {
      savingsAmount: 0,
      opsContribution: 0,
      description: ''
    };
  }
}
