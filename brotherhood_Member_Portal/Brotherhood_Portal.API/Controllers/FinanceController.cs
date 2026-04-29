using Asp.Versioning;
using Brotherhood_Portal.API.Controllers;
using Brotherhood_Portal.Application.Enums;
using Brotherhood_Portal.Application.Services;
using Brotherhood_Portal.Domain.DTOs.Finance.Command;
using Brotherhood_Portal.Domain.DTOs.Finance.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "Admin,Moderator")]
[ApiVersion("1.0")]
public class FinanceController : BaseApiController
{
    private readonly FinanceService _financeService;
    private readonly ILogger<FinanceController> _logger;

    public FinanceController(
        FinanceService financeService,
        ILogger<FinanceController> logger)
    {
        _financeService = financeService;
        _logger = logger;
    }

    #region Add Member Deposit

    /// <summary>
    /// [1] PURPOSE
    /// Creates a new financial contribution record for a member
    /// and initiates the approval workflow.
    /// 
    /// [2] WORKFLOW LOGIC
    /// - If created by Admin:
    ///     → Deposit is immediately Approved.
    ///     → ApprovalCount = 1.
    /// - If created by Moderator:
    ///     → Deposit is created as Pending.
    ///     → Requires two Moderator approvals OR one Admin approval.
    /// 
    /// [3] STATE TRANSITION
    /// Admin:
    ///     None → Approved
    /// Moderator:
    ///     None → Pending
    /// 
    /// [4] DATA INTEGRITY
    /// - Deposit creation is atomic.
    /// - CreatedByUserId and CreatedByRole are persisted.
    /// - Timestamp is UTC-based.
    /// 
    /// [5] RESPONSE CONTRACT
    /// Returns:
    /// - FinanceId
    /// - MemberId
    /// - Status
    /// - ApprovalCount
    /// - RequiresApproval flag
    /// - Human-readable status message
    /// - CreatedAt timestamp
    /// 
    /// [6] SECURITY MODEL
    /// - Restricted to Admin and Moderator roles.
    /// - Rejects invalid authentication contexts.
    /// - All attempts logged for traceability.
    /// 
    /// [7] AUDIT TRAIL
    /// - Logs actor identity
    /// - Logs target member
    /// - Logs created finance record ID
    /// - Does NOT log monetary values
    /// </summary>


    [HttpPost("add-deposit")]
    public async Task<IActionResult> AddDeposit(AddDepositRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // var role = User.FindFirstValue(ClaimTypes.Role);
        var role = User.IsInRole("Admin") ? "Admin"
         : User.IsInRole("Moderator") ? "Moderator"
         : null;

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
        {
            _logger.LogWarning(
                "Unauthorized deposit attempt. UserId: {UserId}, Role: {Role}",
                userId,
                role);

            return Unauthorized("Invalid authentication context.");
        }

        _logger.LogInformation(
            "User {UserId} ({Role}) initiating deposit for Member {MemberId}",
            userId,
            role,
            dto.MemberId);

        var financeId = await _financeService.AddDepositAsync(
            memberId: dto.MemberId,
            savings: dto.SavingsAmount,
            ops: dto.OpsContribution,
            createdByUserId: userId,
            createdByRole: role
        );

        _logger.LogInformation(
            "Deposit {FinanceId} created for Member {MemberId} by {UserId} ({Role})",
            financeId,
            dto.MemberId,
            userId,
            role);

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
    /// Registers an approval action against an existing Pending deposit.
    /// 
    /// [2] APPROVAL RULES
    /// - Admin approval:
    ///     → Immediately transitions deposit to Approved.
    /// - Moderator approval:
    ///     → Increments ApprovalCount.
    ///     → When ApprovalCount >= 2 → Status becomes Approved.
    /// 
    /// [3] STATE VALIDATION
    /// Deposit cannot be:
    /// - Approved if already Approved.
    /// - Approved if already Applied.
    /// - Approved if not found.
    /// 
    /// [4] OUTCOME MODEL
    /// Returns OperationResult with one of:
    /// - Approved
    /// - ApprovalRecorded
    /// - AlreadyApproved
    /// - AlreadyApplied
    /// - NotFound
    /// - Invalid
    /// 
    /// [5] RESPONSE MAPPING
    /// - Approved / ApprovalRecorded → 200 OK
    /// - AlreadyApproved / AlreadyApplied → 409 Conflict
    /// - NotFound → 404 NotFound
    /// - Other → 400 BadRequest
    /// 
    /// [6] SECURITY MODEL
    /// - Restricted to Admin and Moderator roles.
    /// - Actor identity resolved from JWT claims.
    /// - All approval attempts logged.
    /// 
    /// [7] AUDIT TRACEABILITY
    /// - Logs actor
    /// - Logs target FinanceId
    /// - Logs final outcome state
    /// </summary>


    [HttpPost("approve-deposit/{financeId}")]
    public async Task<IActionResult> ApproveDeposit(int financeId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // var role = User.FindFirstValue(ClaimTypes.Role);
        var role = User.IsInRole("Admin") ? "Admin"
         : User.IsInRole("Moderator") ? "Moderator"
         : null;


        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(role))
        {
            _logger.LogWarning(
                "Unauthorized approval attempt for Deposit {FinanceId}. UserId: {UserId}",
                financeId,
                userId);

            return Unauthorized("Invalid authentication context.");
        }

        _logger.LogInformation(
            "User {UserId} ({Role}) attempting approval for Deposit {FinanceId}",
            userId,
            role,
            financeId);

        var result = await _financeService.ApproveDepositAsync(
            financeId,
            approverUserId: userId,
            approverRole: role
        );

        _logger.LogInformation(
            "Approval attempt outcome for Deposit {FinanceId} by {UserId}: {Outcome}",
            financeId,
            userId,
            result.Outcome);

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
    /// [2] FILTERING RULES
    /// - Only Status == Pending.
    /// - Includes approval metadata for transparency.
    /// 
    /// [3] DATA CHARACTERISTICS
    /// Returned data includes:
    /// - FinanceId
    /// - Member identity reference
    /// - Contribution totals
    /// - Invoice reference
    /// - Approval count
    /// - Required approvals
    /// - Created timestamp
    /// - Approval history (who + when)
    /// 
    /// [4] SYSTEM IMPACT
    /// - Read-only operation.
    /// - No state mutation.
    /// 
    /// [5] SECURITY
    /// - Restricted to Admin and Moderator roles.
    /// - Intended for review dashboard.
    /// 
    /// [6] OBSERVABILITY
    /// - Logs requesting actor.
    /// - Logs result count.
    /// </summary>


    [HttpGet("pending-deposits")]
    public async Task<ActionResult<List<PendingDepositDto>>> GetPendingDeposits(
        [FromServices] FinanceQueryService queryService)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        _logger.LogInformation(
            "User {UserId} requested pending deposits list",
            userId);

        var result = await queryService.GetPendingDepositsAsync();

        _logger.LogInformation(
            "Pending deposits query returned {Count} records",
            result.Count);

        return Ok(result);
    }

    #endregion


    #region Get Member Deposit History

    /// <summary>
    /// [1] PURPOSE
    /// Retrieves the full financial ledger for a specific member.
    /// 
    /// [2] SCOPE
    /// Includes:
    /// - Pending deposits
    /// - Approved deposits
    /// - Rejected deposits (if implemented)
    /// - Applied deposits
    /// 
    /// [3] DATA MODEL
    /// Returns chronological financial history containing:
    /// - FinanceId
    /// - Savings contribution
    /// - Operations contribution
    /// - Status
    /// - Approval count
    /// - Created timestamp
    /// - Approved timestamp (if applicable)
    /// - Approval history metadata
    /// 
    /// [4] SYSTEM IMPACT
    /// - Pure read operation.
    /// - No financial state mutation.
    /// 
    /// [5] SECURITY
    /// - Restricted to Admin and Moderator roles.
    /// - Intended for oversight and reconciliation workflows.
    /// 
    /// [6] AUDITABILITY
    /// - Logs actor requesting the ledger.
    /// - Logs member target.
    /// - Logs record count.
    /// </summary>


    [HttpGet("member/{memberId}/deposit-history")]
    public async Task<ActionResult<List<MemberDepositHistoryDto>>> GetMemberDepositHistoryAsync(
        string memberId,
        [FromServices] FinanceQueryService queryService)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        _logger.LogInformation(
            "User {UserId} requested deposit history for Member {MemberId}",
            userId,
            memberId);

        var result = await queryService.GetMemberDepositHistoryAsync(memberId);

        _logger.LogInformation(
            "Deposit history returned {Count} records for Member {MemberId}",
            result.Count,
            memberId);

        return Ok(result);
    }

    #endregion

    #region Cancel Deposit

    [HttpDelete("cancel-deposit/{financeId}")]
    public async Task<IActionResult> CancelDeposit(int financeId)
    {
        var deposit = await _context.Finances.FindAsync(financeId);
    
        if (deposit == null)
            return NotFound(new { message = "Deposit not found" });
    
        _context.Finances.Remove(deposit);
        await _context.SaveChangesAsync();
    
        return Ok(new { message = "Deposit canceled successfully" });
    }
    
    #endregion 

}

