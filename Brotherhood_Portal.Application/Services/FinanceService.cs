using Brotherhood_Portal.Application.Common;
using Brotherhood_Portal.Application.Enums;
using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.Application.Services
{
    /// <summary>
    /// MUTATION / COMMAND SERVICE
    ///
    /// [1] PURPOSE
    /// Handles all state-changing finance operations.
    /// This service applies business rules and coordinates repository writes.
    ///
    /// [2] RESPONSIBILITIES
    /// - Creating deposits
    /// - Managing approval workflow
    /// - Applying deposits to member balances
    ///
    /// [3] ARCHITECTURE
    /// - Depends on IFinanceRepository (write-side repository)
    /// - Depends on InvoiceNumberService for invoice sequencing
    /// - Follows CQRS (Command side)
    ///
    /// [4] RULES
    /// - All balance mutations must go through this service
    /// - Approval logic must be enforced here (never in controllers)
    /// - Idempotency must be respected when applying deposits
    /// </summary>
    public class FinanceService
    {
        private readonly IFinanceRepository _financeRepository;
        private readonly InvoiceNumberService _invoiceNumberService;

        public FinanceService(
            IFinanceRepository financeRepository,
            InvoiceNumberService invoiceNumberService)
        {
            _financeRepository = financeRepository;
            _invoiceNumberService = invoiceNumberService;
        }

        #region Create Deposit

        /// <summary>
        /// [1] PURPOSE
        /// Creates a new deposit record for a member.
        ///
        /// [2] BUSINESS RULES
        /// - A unique invoice number is generated per member.
        /// - Deposit is created in Pending state.
        /// - ApprovalCount starts at 0.
        /// - Deposit is NOT applied to member balance at creation.
        ///
        /// [3] RESPONSE
        /// Returns:
        /// - The generated FinanceId.
        ///
        /// [4] DESIGN NOTES
        /// - Finance.Id is guaranteed after SaveChangesAsync.
        /// - Approval and balance application handled separately.
        /// </summary>
        public async Task<int> AddDepositAsync(
            string memberId,
            decimal savings,
            decimal ops,
            string createdByUserId,
            string createdByRole)
        {
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

            return finance.Id!.Value;
        }

        #endregion


        #region Approve Deposit

        /// <summary>
        /// [1] PURPOSE
        /// Registers an approval action against a deposit.
        ///
        /// [2] BUSINESS RULES
        /// - Deposit must exist.
        /// - Deposit cannot be approved if already Approved.
        /// - Deposit cannot be applied more than once (idempotent protection).
        ///
        /// ADMIN FLOW:
        /// - Admin approval immediately:
        ///     - Marks deposit as Approved
        ///     - Applies to member balance
        ///     - Sets ApprovedByUserId and ApprovedAt
        ///
        /// MODERATOR FLOW:
        /// - Increments ApprovalCount
        /// - If ApprovalCount >= 2:
        ///     - Deposit becomes Approved
        ///     - Deposit applied to member balance
        ///
        /// [3] RESPONSE
        /// Returns OperationResult with:
        /// - Approved
        /// - ApprovalRecorded
        /// - AlreadyApproved
        /// - AlreadyApplied
        /// - NotFound
        ///
        /// [4] DESIGN NOTES
        /// - Idempotency enforced via AppliedAt check.
        /// - Member balance mutation delegated to repository.
        /// - All business logic centralized here.
        /// </summary>
        public async Task<OperationResult<DepositApprovalOutcome>> ApproveDepositAsync(
            int financeId,
            string approverUserId,
            string approverRole)
        {
            var finance = await _financeRepository.GetByIdAsync(financeId);

            // ---- VALIDATION ----

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
