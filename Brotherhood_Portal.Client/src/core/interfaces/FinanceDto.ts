export interface AddDepositDto {
  memberId: string;
  savingsAmount: number;
  opsContribution: number;
  description?: string;
}

export interface FinanceRecord {
  id: number;
  depositDate: Date;
  savingsAmount: number;
  opsContributionAmount: number;
  description?: string;
  status: 'Pending' | 'Approved' | 'Rejected';
}

export interface PendingDeposit {
  id: number;
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