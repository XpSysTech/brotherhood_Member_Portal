import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinanceService } from '../../../../../core/services/finance-service';
import { FinanceRecord } from '../../../../../core/interfaces/FinanceDto';

@Component({
  selector: 'app-manage-member-finance',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './manage-member-finance.component.html'
})
export class ManageMemberFinanceComponent {

  selectedMemberId: string | null = null;

  depositForm = {
    savingsAmount: 0,
    opsContribution: 0,
    description: ''
  };

  history: FinanceRecord[] = [];
  loadingHistory = false;
  submitting = false;
  feedback: string | null = null;

  constructor(private financeService: FinanceService) {}

  // SUBMIT DEPOSIT METHOD
  submitDeposit(): void {
    if (!this.selectedMemberId) return;

    this.submitting = true;
    this.feedback = null;

    this.financeService.addDeposit({
      memberId: this.selectedMemberId,
      savingsAmount: this.depositForm.savingsAmount,
      opsContribution: this.depositForm.opsContribution,
      description: this.depositForm.description
    }).subscribe({
      next: res => {
        this.feedback = res.message;
        this.loadHistory();
        this.resetForm();
      },
      error: err => {
        this.feedback = err.error?.message ?? 'Failed to add deposit';
      },
      complete: () => this.submitting = false
    });
  }

  // LOAD MEMBER FINANCIAL HISTORY
  loadHistory(): void {
    if (!this.selectedMemberId) return;

    this.loadingHistory = true;

    this.financeService.getMemberHistory(this.selectedMemberId)
      .subscribe({
        next: data => this.history = data,
        complete: () => this.loadingHistory = false
      });
  }

  // RESET DEPOSIT FORM
  resetForm(): void {
    this.depositForm = {
      savingsAmount: 0,
      opsContribution: 0,
      description: ''
    };
  }
}
