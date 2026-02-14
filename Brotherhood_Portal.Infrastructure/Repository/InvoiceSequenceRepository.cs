using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.Infrastructure.Repository
{
    /// <summary>
    /// Repository responsible for persistence of MemberInvoiceSequence entities.
    ///
    /// PURPOSE:
    /// - Tracks sequential invoice numbers per Member per Year.
    /// - Ensures invoice numbering remains consistent and incremental.
    ///
    /// RESPONSIBILITIES:
    /// - Retrieve existing invoice sequence for a member/year.
    /// - Create new invoice sequence records.
    /// - Persist changes to the database.
    ///
    /// RULES:
    /// - No invoice generation logic (handled in Application layer).
    /// - No formatting logic.
    /// - No business validation.
    ///
    /// ARCHITECTURE:
    /// - Storage-agnostic.
    /// - Only concerned with data persistence.
    /// </summary>
    public class InvoiceSequenceRepository : IInvoiceSequenceRepository
    {
        private readonly AppDBContext _context;

        public InvoiceSequenceRepository(AppDBContext context)
        {
            _context = context;
        }

        #region Get Invoice Sequence By Member And Year

        /// <summary>
        /// [1] PURPOSE
        /// Retrieves the invoice sequence record for a given member and calendar year.
        ///
        /// [2] BEHAVIOR
        /// - Returns the existing sequence if found.
        /// - Returns null if no sequence exists.
        ///
        /// [3] USAGE
        /// Called by InvoiceNumberService before generating the next invoice number.
        ///
        /// [4] GUARANTEE
        /// Only one record should exist per (MemberId, Year) combination.
        /// This uniqueness should be enforced via database constraint.
        /// </summary>
        public async Task<MemberInvoiceSequence?> GetAsync(string memberId, int year)
        {
            return await _context.MemberInvoiceSequences
                .SingleOrDefaultAsync(x => x.MemberId == memberId && x.Year == year);
        }

        #endregion


        #region Add New Invoice Sequence

        /// <summary>
        /// [1] PURPOSE
        /// Adds a new MemberInvoiceSequence entity to the DbContext.
        ///
        /// [2] WHEN USED
        /// - First invoice of a new calendar year.
        /// - First invoice ever created for a member.
        ///
        /// [3] IMPORTANT
        /// Does NOT automatically persist to the database.
        /// SaveChangesAsync() must be called explicitly.
        /// </summary>
        public async Task AddAsync(MemberInvoiceSequence sequence)
        {
            await _context.MemberInvoiceSequences.AddAsync(sequence);
        }

        #endregion


        #region Save Changes To Database

        /// <summary>
        /// [1] PURPOSE
        /// Commits all staged invoice sequence changes to the database.
        ///
        /// [2] BEHAVIOR
        /// Executes EF Core SaveChangesAsync.
        ///
        /// [3] RULE
        /// If a method returns Task, do not return values — only await.
        ///
        /// [4] TRANSACTION NOTE
        /// Should be coordinated at the Application layer if invoice
        /// generation must be atomic with deposit creation.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
