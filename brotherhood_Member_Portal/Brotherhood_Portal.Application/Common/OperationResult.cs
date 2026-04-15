namespace Brotherhood_Portal.Application.Common
{
    /// <summary>
    /// Represents the standardized result of an application-level operation.
    ///
    /// Purpose:
    /// - Encapsulates the outcome of a use case (success or failure)
    /// - Carries a strongly-typed business outcome (enum) instead of raw booleans
    /// - Provides a human-readable message for UI, API, or logging layers
    ///
    /// Why this exists:
    /// - Keeps Application Services free from HTTP, UI, or transport concerns
    /// - Avoids throwing exceptions for expected business flow (e.g. "Already Approved")
    /// - Enables controllers, GraphQL resolvers, and background jobs to
    ///   map business outcomes to their own response formats
    ///
    /// What it is NOT:
    /// - Not a DTO (it does not represent data transfer)
    /// - Not a Domain Entity (it does not represent persisted state)
    ///
    /// Typical usage:
    /// - Returned from Application Services
    /// - Interpreted by Controllers / GraphQL resolvers
    ///
    /// Example:
    ///     return OperationResult<DepositApprovalOutcome>.Ok(
    ///         DepositApprovalOutcome.Approved,
    ///         "Deposit approved successfully."
    ///     );
    /// </summary>
    /// 
    public sealed class OperationResult<TOutcome> where TOutcome : Enum
    {
        public bool Success { get; init; } // Indicates if the operation was successful
        public required TOutcome Outcome { get; init; } // The specific outcome of the operation
        public string Message { get; init; } = string.Empty; // Additional message or information intended for presentation or logging

        // Factory methods for creating OperationResult instances
        public static OperationResult<TOutcome> Ok(
            TOutcome outcome,
            string message
        ) =>
            new() { Success = true, Outcome = outcome, Message = message };

        // Factory method for creating a failed OperationResult instance
        public static OperationResult<TOutcome> Fail(
            TOutcome outcome,
            string message
        ) =>
            new() { Success = false, Outcome = outcome, Message = message };
    }
}

