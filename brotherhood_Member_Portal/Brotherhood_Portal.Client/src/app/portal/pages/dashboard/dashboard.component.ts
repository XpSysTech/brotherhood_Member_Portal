import { Component, inject, OnInit } from '@angular/core';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { AccountService } from '../../../../core/services/account-service';
import { FinanceService } from '../../../../core/services/finance-service';
import { FundFinanceSummary, MemberFinanceSummary } from '../../../../core/interfaces/FinanceDto';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CurrencyPipe, DecimalPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {

  private account = inject(AccountService);
  private financeService = inject(FinanceService);

  protected currentUser = this.account.currentUser;

  totalMembers = 0;
  fund: FundFinanceSummary | null = null;
  member: MemberFinanceSummary | null = null;
  loading = true;
  error: string | null = null;
  fundGoal = 150_000;

  ngOnInit(): void {
    const now = new Date();
    const year = now.getFullYear();
    const month = now.getMonth() + 1;

    this.loading = true;

    Promise.all([
      this.loadFundSummary(year, month),
      this.loadMemberSummary(year, month)
    ])
      .then(() => this.loading = false)
      .catch(err => {
        this.error = 'Failed to load financial data.';
        this.loading = false;
        // console.error(err);
      });

    this.financeService.getMembersForDropdown()
    .subscribe(members => {
      this.totalMembers = members.length;
    });
  }

  private loadFundSummary(year: number, month: number): Promise<void> {
    return new Promise((resolve, reject) => {
      this.financeService.getFundFinanceSummary(year, month)
        .subscribe({
          next: response => {
            this.fund = response.data.fundFinanceSummary;
            resolve();
          },
          error: reject
        });
    });
  }

  private loadMemberSummary(year: number, month: number): Promise<void> {
    return new Promise((resolve, reject) => {
      this.financeService.getMemberFinanceSummary(year, month)
        .subscribe({
          next: response => {
            this.member = response.data.memberFinanceSummary;
            resolve();
          },
          error: reject
        });
    });
  }
  get remainingToGoal(): number {
    if (!this.fund) return 0;
    return Math.max(this.fundGoal - this.fund.totalSavings, 0);
  }

  get goalProgressPercent(): number {
    if (!this.fund || this.fundGoal === 0) return 0;
    return (this.fund.totalSavings / this.fundGoal) * 100;
  }
}
