using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.Infrastructure.Repository
{
    public class InvoiceSequenceRepository : IInvoiceSequenceRepository
    {
        /*
            - Data Access Implementation for MemberInvoiceSequence
            - Interacts with AppDBContext
            - Provides methods to get, add, and save invoice sequences
            - Storage Agnostic, your business logic does not care how or where data is stored.
         */

        private readonly AppDBContext _context;
        public InvoiceSequenceRepository(AppDBContext context)
        {
            _context = context;
        }

        #region Get Invoice Sequence by MemberId and Year
        public async Task<MemberInvoiceSequence?> GetAsync(string memberId, int year)
        {
            return await _context.MemberInvoiceSequences
                .SingleOrDefaultAsync(x => x.MemberId == memberId && x.Year == year);
        }
        #endregion

        #region Add New Invoice Sequence
        public async Task AddAsync(MemberInvoiceSequence sequence)
        {
            await _context.MemberInvoiceSequences.AddAsync(sequence);
        }
        #endregion

        #region Save Changes to Database
        // RULE: If a method returns Task, never return a value — only await.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
