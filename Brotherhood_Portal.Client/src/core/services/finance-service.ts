// core/services/finance.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { Observable } from 'rxjs';
import { AddDepositDto, DepositApprovalResponse, FinanceRecord, FundFinanceSummaryResponse, GraphQLResponse, MemberDepositHistoryDto, PendingDeposit } from '../interfaces/FinanceDto';
import { MemberByName } from '../interfaces/Member';


@Injectable({ providedIn: 'root' })
export class FinanceService {
  private baseUrl = `${environment.apiBaseUrl}`;
  private baseFinanceUrl = `${environment.apiBaseUrl}finance`;
  private graphqlUrl = `${environment.graphQLBaseUrl}`;

  constructor(private http: HttpClient) { }

  addDeposit(dto: AddDepositDto): Observable<any> {
    return this.http.post(`${this.baseFinanceUrl}/add-deposit`, dto);
  }

  getMemberHistory(memberId: string): Observable<MemberDepositHistoryDto[]> {
    return this.http.get<MemberDepositHistoryDto[]>(
      `${this.baseFinanceUrl}/member/${memberId}/deposit-history`
    );
  }

  approveDeposit(financeId: number) {
    return this.http.post<DepositApprovalResponse>(
      `${this.baseFinanceUrl}/approve-deposit/${financeId}`,
      {}
    );
  }

  getPendingDeposits() {
    return this.http.get<PendingDeposit[]>(
      `${this.baseFinanceUrl}/pending-deposits`
    );
  }

  getMembersForDropdown() {
    return this.http.get<MemberByName[]>(
      `${this.baseUrl}members/dropdown`
    );
  }

  // ===============================
  // GRAPHQL METHODS
  // ===============================

  getMemberFinanceSummary(year: number, month: number) {
    const query = `
      query GetMemberFinanceSummary($year: Int!, $month: Int!) {
        memberFinanceSummary(year: $year, month: $month) {
          totalSavings
          totalOpsContribution
          monthlySavings
          monthlyOpsContribution
          ownershipPercentageOfFund
        }
      }
    `;

    return this.http.post<any>(this.graphqlUrl, {
      query,
      variables: { year, month }
    });
  }

  getFundFinanceSummary(year: number, month: number) {
    const query = `
    query GetFundFinanceSummary($year: Int!, $month: Int!) {
      fundFinanceSummary(year: $year, month: $month) {
        totalSavings
        totalOpsContribution
        totalMembers
        activeMembers

        currentMonth {
          year
          month
          totalSavings
          totalOpsContribution
          contributingMemberCount
        }

        monthlyHistory {
          year
          month
          totalSavings
          totalOpsContribution
          contributingMemberCount
        }
      }
    }
  `;

    return this.http.post<GraphQLResponse<FundFinanceSummaryResponse>>(
      this.graphqlUrl,
      {
        query,
        variables: { year, month }
      }
    );
  }

}
