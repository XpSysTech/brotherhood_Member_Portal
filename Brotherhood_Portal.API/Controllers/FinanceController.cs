using Brotherhood_Portal.Application.Enums;
using Brotherhood_Portal.Application.Services;
using Brotherhood_Portal.Domain.DTOs.Finance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Brotherhood_Portal.API.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
    public class FinanceController : BaseApiController
    {
        /*
            - API endpoints for Adding and Approving Deposits
                - AddDeposit (POST): Allows an Admin User or Moderator Users to add a deposit for a member.
                - ApproveDeposit (POST): Allows Admin/Moderator User to approve a pending deposit.
            - Restrict access to these endpoints to Admin and Moderator roles only.
            - is it possible to ensure that when a deposit is added, by a moderator it needs to 
                be approved by an admin or two other moderators before it reflects in the member's account balance.
         */

        private readonly FinanceService _financeService;
        public FinanceController(FinanceService financeService)
        {
            _financeService = financeService;
        }

        #region Add Member Deposit
        /// <summary>
        /// Adds a deposit for a member.
        /// Admin deposits are auto-approved.
        /// Moderator deposits require approval.
        /// </summary>
        [HttpPost("add-deposit")]
        public async Task<IActionResult> AddDeposit(AddDepositDto dto)
        {
            // Get the user ID and role from the claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            // Validate user context
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
                return Unauthorized("Invalid authentication context.");

            // Add the deposit via the finance service
            var financeId = await _financeService.AddDepositAsync(
                memberId: dto.MemberId,
                savings: dto.SavingsAmount,
                ops: dto.OpsContribution,
                createdByUserId: userId,
                createdByRole: role
            );

            return Ok(new
            {
                financeId,
                message = role == "Admin"
                ? "Deposit added and approved."
                : "Deposit added and awaiting approval."
            });
        }

        #endregion

        #region Approve Member Deposit

        /// <summary>
        /// Approves a pending deposit.
        /// Admin can approve immediately.
        /// Moderators contribute toward approval threshold.
        /// </summary>
        [HttpPost("approve-deposit/{financeId}")]
        public async Task<IActionResult> ApproveDeposit(int financeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
                return Unauthorized("Invalid authentication context.");

            var result = await _financeService.ApproveDepositAsync(
                financeId,
                approverUserId: userId,
                approverRole: role
            );

            return result.Outcome switch
            {
                DepositApprovalOutcome.Approved => Ok(result),
                DepositApprovalOutcome.ApprovalRecorded => Ok(result),
                DepositApprovalOutcome.AlreadyApproved => Conflict(result),
                DepositApprovalOutcome.AlreadyApplied => Conflict(result),
                DepositApprovalOutcome.NotFound => NotFound(result),
                _ => BadRequest(result)
            };
        }

        #endregion
    }
}
