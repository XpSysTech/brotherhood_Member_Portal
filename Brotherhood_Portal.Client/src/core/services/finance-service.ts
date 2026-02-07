// core/services/finance.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { AddDepositDto, DepositApprovalResponse, FinanceRecord, PendingDeposit } from '../interfaces/FinanceDto';


@Injectable({ providedIn: 'root' })
export class FinanceService {
  private baseUrl = `${environment.apiBaseUrl}/finance`;

  constructor(private http: HttpClient) {}

  addDeposit(dto: AddDepositDto): Observable<any> {
    return this.http.post(`${this.baseUrl}/add-deposit`, dto);
  }

  getMemberHistory(memberId: string): Observable<FinanceRecord[]> {
    return this.http.get<FinanceRecord[]>(
      `${environment.apiBaseUrl}/members/${memberId}/finances`
    );
  }

  approveDeposit(financeId: number) {
    return this.http.post<DepositApprovalResponse>(
      `${this.baseUrl}/approve-deposit/${financeId}`,
      {}
    );
  }

  getPendingDeposits() {
    return this.http.get<PendingDeposit[]>(
      `${this.baseUrl}/pending-deposits`
    );
  }


  
}
