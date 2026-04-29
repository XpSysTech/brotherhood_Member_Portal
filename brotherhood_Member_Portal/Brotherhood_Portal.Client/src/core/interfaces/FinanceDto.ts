export interface AddDepositDto {
  memberId: string;
  memberDisplayName: string;
  savingsAmount: number;
  opsContribution: number;
  description?: string;
}

export interface FinanceRecord {
  financeId: number;
  depositDate: Date;
  savingsAmount: number;
  opsContributionAmount: number;
  description?: string;
  status: 'Pending' | 'Approved' | 'Rejected';
}

export interface PendingDeposit {
  financeId: number;
  memberName: string;
  savingsAmount: number;
  opsContributionAmount: number;
  approvalCount: number;
  requiredApprovals: number;
  approvals: ApprovalRecord[];
  createdAt: string;
}

export interface ApprovalRecord {
  userId: string;
  displayName: string;
  role: 'Admin' | 'Moderator';
  approvedAt: string;
}

export interface DepositApprovalResponse {
  financeId: number;
  outcome: 'Approved' | 'ApprovalRecorded';
  message: string;
  approvedBy: {
    userId: string;
    displayName: string;
    role: 'Admin' | 'Moderator';
    approvedAt: string;
  }[];
}

export interface FinanceApprovalDto {
  userId: string;
  displayName: string;
  role: string;
  approvedAt: string; // ISO string
}

export interface MemberDepositHistoryDto {
  financeId: number;
  invoiceNo: string;
  savingsAmount: number;
  opsContributionAmount: number;
  status: string;
  createdAt: string;   // ISO string
  approvedAt?: string; // ISO string | null
  createdByUserId?: string;
  approvedByUserId?: string;
  approvalCount: number;
  approvals: FinanceApprovalDto[];
}

// ===============================
// GRAPHQL RESPONSE MODELS
// ===============================

export interface MonthlyFundSummary {
  month: number;
  savings: number;
  opsContribution: number;
}

export interface FundFinanceSummary {
  totalSavings: number;
  totalOpsContribution: number;
  activeMembers: number;
  totalMembers: number;
  currentMonth: any;
  monthlyHistory: any[];
}

export interface GraphQLResponse<T> {
  data: T;
}

export interface FundFinanceSummaryResponse {
  fundFinanceSummary: FundFinanceSummary;
}

export interface MemberFinanceSummary {
  totalSavings: number;
  totalOpsContribution: number;
  monthlySavings: number;
  monthlyOpsContribution: number;
  ownershipPercentageOfFund: number;
}

export interface ApiResponse<T> {
  data: T;
}