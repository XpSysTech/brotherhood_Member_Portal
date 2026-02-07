import { Component, inject } from '@angular/core';
import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { AccountService } from '../../../../core/services/account-service';
import { mockFundFinanceSummary } from './mock-data/fund-finance-summary.mock';
import { mockMemberFinanceSummary } from './mock-data/member-finance-summary.mock';

@Component({
  selector: 'app-dashboard',
  imports: [CurrencyPipe, DecimalPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  private account = inject(AccountService);
  protected currentUser = this.account.currentUser;
  
  // Mock Data - To be replaced
  fund = mockFundFinanceSummary;
  member = mockMemberFinanceSummary;
  
  fundGoal = 500_000;

  get remainingToGoal(): number {
    return Math.max(this.fundGoal - this.fund.totalSavings, 0);
  }

  get goalProgressPercent(): number {
    return (this.fund.totalSavings / this.fundGoal) * 100;
  }
}
