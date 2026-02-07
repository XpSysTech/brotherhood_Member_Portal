using Brotherhood_Portal.Application.Common;
using Brotherhood_Portal.Application.Enums;
using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.Application.Services
{
    public class FinanceService
    {
        /*
            - Coordinates database access for finance-related operations
            - Applies business rules for deposits and approvals
            - Utilizes repositories for data persistence
            - Provides methods to add deposits and approve them
            - This is for Mutation Logic
         */

        private readonly IFinanceRepository _financeRepository;
        private readonly InvoiceNumberService _invoiceNumberService;

        public FinanceService(IFinanceRepository financeRepository, InvoiceNumberService invoiceNumberService)
        {
            _financeRepository = financeRepository;
            _invoiceNumberService = invoiceNumberService;
        }

        #region Create Deposit
        public async Task<int> AddDepositAsync(string memberId, decimal savings, decimal ops, string createdByUserId, string createdByRole)
        {
            // Generate invoice number
            var invoiceNo = await _invoiceNumberService.GenerateAsync(memberId);

            var finance = new Finance
            {
                MemberId = memberId,
                SavingsAmount = savings,
                OpsContributionAmount = ops,
                InvoiceNo = invoiceNo,
                CreatedByUserId = createdByUserId,
                Status = FinanceStatus.Pending,
                ApprovalCount = 0
            };

            await _financeRepository.AddAsync(finance);
            await _financeRepository.SaveChangesAsync();

            // Finance.Id is now guaranteed to exist
            return finance.Id!.Value;
        }

        #endregion

        #region Approve Deposit
        public async Task<OperationResult<DepositApprovalOutcome>> ApproveDepositAsync(int financeId, string approverUserId, string approverRole)
        {
            var finance = await _financeRepository.GetByIdAsync(financeId); // Retrieve the finance record

            // Validation checks
            if (finance == null)
            {
                return OperationResult<DepositApprovalOutcome>.Fail(
                    DepositApprovalOutcome.NotFound,
                    "Deposit not found."
                );
            }

            if (finance.Status == FinanceStatus.Approved)
            { 
                return OperationResult<DepositApprovalOutcome>.Fail(
                    DepositApprovalOutcome.AlreadyApproved,
                    "Deposit has already been approved."
                );
            }

            if (finance.AppliedAt != null)
            { 
                return OperationResult<DepositApprovalOutcome>.Fail(
                    DepositApprovalOutcome.AlreadyApplied,
                    "Deposit has already been applied to member balance."
                );
            }

            // ---- ADMIN FLOW (single approval) ----
            if (approverRole == "Admin")
            {
                finance.Status = FinanceStatus.Approved;
                finance.ApprovedByUserId = approverUserId;
                finance.ApprovedAt = DateTime.UtcNow;

                // Apply deposit to member balance and save changes
                await _financeRepository.ApplyToMemberBalanceAsync(finance);
                await _financeRepository.SaveChangesAsync();

                return OperationResult<DepositApprovalOutcome>.Ok(
                    DepositApprovalOutcome.Approved,
                    "Deposit approved and applied by Admin."
                );
            }

            // ---- MODERATOR FLOW (multi-approval) ----
            finance.ApprovalCount++;

            if (finance.ApprovalCount >= 2)
            {
                finance.Status = FinanceStatus.Approved;
                finance.ApprovedAt = DateTime.UtcNow;

                await _financeRepository.ApplyToMemberBalanceAsync(finance);

                await _financeRepository.SaveChangesAsync();

                return OperationResult<DepositApprovalOutcome>.Ok(
                    DepositApprovalOutcome.Approved,
                    "Deposit approved after required approvals."
                );
            }

            await _financeRepository.SaveChangesAsync();

            return OperationResult<DepositApprovalOutcome>.Ok(
                DepositApprovalOutcome.ApprovalRecorded,
                "Approval recorded. Awaiting further approvals."
            );
        }

        #endregion
    }
}
