import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FinanceService } from '../../../../../core/services/finance-service';
import { PendingDeposit } from '../../../../../core/interfaces/FinanceDto';

@Component({
  selector: 'app-approve-deposits',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './approve-deposits.component.html'
})
export class ApproveDepositsComponent implements OnInit {

  pendingDeposits: PendingDeposit[] = [];
  loading = false;
  feedback: string | null = null;

  constructor(private financeService: FinanceService) {}

  ngOnInit(): void {
    this.loadPendingDeposits();
  }

  // loadPendingDeposits(): void {
  //   this.loading = true;

  //   this.financeService.getPendingDeposits()
  //     .subscribe({
  //       next: data => this.pendingDeposits = data,
  //       complete: () => this.loading = false
  //     });
  // }

  loadPendingDeposits(): void {
  this.loading = true;

  this.financeService.getPendingDeposits()
    .subscribe({
      next: data => {
        console.log('Pending deposits:', data);
        this.pendingDeposits = data;
      },
      complete: () => this.loading = false
    });
  }

  approve(financeId: number): void {
    this.feedback = null;

    this.financeService.approveDeposit(financeId)
      .subscribe({
        next: res => {
          this.feedback = res.message;
          this.loadPendingDeposits();
        },
        error: err => {
          this.feedback = err.error?.message ?? 'Approval failed';
        }
      });
  }

  cancelDepositOld(financeId: number): void {
  if (!confirm('Are you sure you want to cancel this deposit?')) return;

  this.feedback = null;

  this.financeService.cancelDeposit(financeId)
    .subscribe({
      next: res => {
        this.feedback = res.message;
        this.loadPendingDeposits();
      },
      error: err => {
        this.feedback = err.error?.message ?? 'Cancel failed';
      }
    });
  }

  cancelDeposit(financeId: number) {
    return this.http.delete<any>(
      `${this.baseFinanceUrl}/cancel-deposit/${financeId}`
    );
  }

}
