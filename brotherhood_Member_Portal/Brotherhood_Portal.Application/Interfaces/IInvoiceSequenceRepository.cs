using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.Application.Interfaces
{
    // Business Logic Abstraction for MemberInvoiceSequence Repository
    public interface IInvoiceSequenceRepository
    {
        Task<MemberInvoiceSequence?> GetAsync(string memberId, int year);
        Task AddAsync(MemberInvoiceSequence sequence);
        Task SaveChangesAsync();
    }

}
