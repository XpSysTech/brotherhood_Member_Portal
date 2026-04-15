namespace Brotherhood_Portal.Application.Enums
{
    public enum DepositApprovalOutcome
    {
        Approved, // Fully Approved
        ApprovalRecorded, // Approval Recorded, Pending Further Approvals
        NotFound,
        AlreadyApproved,
        AlreadyApplied,
        Rejected
    }
}
