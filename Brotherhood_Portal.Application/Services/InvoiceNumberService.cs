using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.Application.Services
{
    /// <summary>
    /// DOMAIN SERVICE
    ///
    /// [1] PURPOSE
    /// Generates sequential, member-specific invoice numbers.
    ///
    /// [2] RESPONSIBILITIES
    /// - Maintains yearly invoice sequences per member
    /// - Ensures sequential numbering within a given year
    /// - Produces formatted invoice identifiers
    ///
    /// [3] BUSINESS RULES
    /// - Each member has an independent invoice sequence per year.
    /// - Sequence resets automatically at the start of a new year.
    /// - Invoice numbers are formatted for readability and traceability.
    ///
    /// [4] FORMAT
    /// Example:
    ///     MEM001-2026-0001
    ///
    /// Structure:
    ///     {MEMBER_PREFIX}-{YEAR}-{SEQUENCE_PADDED}
    ///
    /// [5] ARCHITECTURE
    /// - Depends on IInvoiceSequenceRepository (write-side persistence)
    /// - Stateless service; state is persisted via MemberInvoiceSequence entity
    ///
    /// [6] IMPORTANT NOTE
    /// - Concurrency protection must be handled at database level
    ///   (unique constraint or transaction isolation recommended).
    /// </summary>
    public class InvoiceNumberService
    {
        private readonly IInvoiceSequenceRepository _invoiceSequence;

        public InvoiceNumberService(IInvoiceSequenceRepository invoiceSequence)
        {
            _invoiceSequence = invoiceSequence;
        }

        #region Generate Next Invoice Number

        /// <summary>
        /// [1] PURPOSE
        /// Generates the next invoice number for a specific member.
        ///
        /// [2] BUSINESS FLOW
        /// - Determine current UTC year.
        /// - Retrieve existing MemberInvoiceSequence record.
        /// - If none exists (first invoice this year):
        ///     - Create new sequence starting at 1.
        /// - Otherwise:
        ///     - Increment CurrentNumber.
        /// - Persist changes.
        /// - Return formatted invoice string.
        ///
        /// [3] STATE CHANGES
        /// - Inserts new sequence record if year changed.
        /// - Updates CurrentNumber when incremented.
        ///
        /// [4] RETURN VALUE
        /// Returns formatted invoice string.
        ///
        /// Example:
        ///     ABC123-2026-0007
        ///
        /// [5] EDGE CASES
        /// - MemberId must be at least 6 characters
        ///   (consider validation or safe substring handling).
        /// </summary>
        public async Task<string> GenerateAsync(string memberId)
        {
            var year = DateTime.UtcNow.Year;

            // Retrieve existing yearly sequence for member
            var sequence = await _invoiceSequence.GetAsync(memberId, year);

            if (sequence == null)
            {
                // First invoice for this member in this year
                sequence = new MemberInvoiceSequence
                {
                    MemberId = memberId,
                    Year = year,
                    CurrentNumber = 1
                };

                await _invoiceSequence.AddAsync(sequence);
            }
            else
            {
                // Increment existing yearly sequence
                sequence.CurrentNumber++;
            }

            await _invoiceSequence.SaveChangesAsync();

            // Format: MEMBER-YYYY-XXXX
            // Example: ABC123-2026-0001
            return $"{memberId[..6].ToUpper()}-{year}-{sequence.CurrentNumber:D4}";
        }

        #endregion
    }
}
