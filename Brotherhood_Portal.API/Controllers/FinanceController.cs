using Brotherhood_Portal.Application.Enums;
using Brotherhood_Portal.Application.Services;
using Brotherhood_Portal.Domain.DTOs.Finance.Command;
using Brotherhood_Portal.Domain.DTOs.Finance.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using Brotherhood_Portal.API.Controllers;

[Authorize(Roles = "Admin,Moderator")]
public class FinanceController : BaseApiController
{
    private readonly FinanceService _financeService;

    public FinanceController(FinanceService financeService)
    {
        _financeService = financeService;
    }

    #region Add Member Deposit

    /// <summary>
    /// [1] PURPOSE
    /// Creates a new deposit record for a member and initiates the approval workflow.
    ///
    /// [2] BUSINESS RULES
    /// - Admin-created deposits are automatically approved and applied.
    /// - Moderator-created deposits are created with Pending status.
    /// - Pending deposits require a minimum of 2 moderator approvals
    ///   (or a single admin approval).
    ///
    /// [3] RESPONSE
    /// Returns:
    /// - FinanceId of the created deposit
    /// - Current Status (Approved or Pending)
    /// - Approval count at time of creation
    /// - Whether further approvals are required
    /// - User-facing status message
    ///
    /// [4] SECURITY
    /// - Accessible only by Admin or Moderator roles.
    /// </summary>

    [HttpPost("add-deposit")]
    public async Task<IActionResult> AddDeposit(AddDepositRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var role = User.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
            return Unauthorized("Invalid authentication context.");

        var financeId = await _financeService.AddDepositAsync(
            memberId: dto.MemberId,
            savings: dto.SavingsAmount,
            ops: dto.OpsContribution,
            createdByUserId: userId,
            createdByRole: role
        );

        var response = new AddDepositResponseDto
        {
            FinanceId = financeId,
            MemberId = dto.MemberId,
            Status = role == "Admin" ? "Approved" : "Pending",
            ApprovalCount = role == "Admin" ? 1 : 0,
            RequiresApproval = role != "Admin",
            Message = role == "Admin"
                ? "Deposit added and approved."
                : "Deposit added and awaiting approval.",
            CreatedAt = DateTime.UtcNow
        };

        return Ok(response);
    }

    #endregion


    #region Approve Deposit

    /// <summary>
    /// [1] PURPOSE
    /// Registers an approval action against a pending deposit.
    ///
    /// [2] BUSINESS RULES
    /// - Admin approval immediately finalizes the deposit.
    /// - Moderator approvals increment ApprovalCount.
    /// - When ApprovalCount >= 2, deposit becomes Approved.
    /// - Deposit cannot be approved if already approved or applied.
    ///
    /// [3] RESPONSE
    /// Returns an OperationResult describing:
    /// - ApprovalRecorded (still pending)
    /// - Approved (threshold met)
    /// - AlreadyApproved
    /// - AlreadyApplied
    /// - NotFound
    ///
    /// [4] SECURITY
    /// - Accessible only by Admin or Moderator roles.
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


    #region Get Pending Deposits

    /// <summary>
    /// [1] PURPOSE
    /// Retrieves all deposits currently awaiting approval.
    ///
    /// [2] BUSINESS RULES
    /// - Only deposits with Status == Pending are returned.
    /// - Includes full approval history for transparency.
    ///
    /// [3] RESPONSE
    /// Returns a list containing:
    /// - FinanceId
    /// - Member identity
    /// - Contribution amounts
    /// - Invoice number
    /// - Approval count
    /// - Required approvals
    /// - Created timestamp
    /// - Approval history (who approved and when)
    ///
    /// [4] SECURITY
    /// - Accessible only by Admin or Moderator roles.
    /// </summary>

    [HttpGet("pending-deposits")]
    public async Task<ActionResult<List<PendingDepositDto>>> GetPendingDeposits(
        [FromServices] FinanceQueryService queryService)
    {
        var result = await queryService.GetPendingDepositsAsync();
        return Ok(result);
    }

    #endregion


    #region Get Member Deposit History

    /// <summary>
    /// [1] PURPOSE
    /// Retrieves the full deposit ledger for a specific member.
    ///
    /// [2] BUSINESS RULES
    /// - Includes all deposit states (Pending, Approved, Rejected).
    /// - Includes creator and approval metadata.
    /// - Does not modify any system state.
    ///
    /// [3] RESPONSE
    /// Returns a chronological list containing:
    /// - FinanceId
    /// - Deposit amounts
    /// - Status
    /// - Approval count
    /// - Created/Approved timestamps
    /// - Approval history details
    ///
    /// [4] SECURITY
    /// - Accessible only by Admin or Moderator roles.
    /// </summary>

    [HttpGet("member/{memberId}/deposit-history")]
    public async Task<ActionResult<List<MemberDepositHistoryDto>>> GetMemberDepositHistoryAsync(
        string memberId,
        [FromServices] FinanceQueryService queryService)
    {
        var result = await queryService.GetMemberDepositHistoryAsync(memberId);
        return Ok(result);
    }

    #endregion

}

