using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.Application.Interfaces
{
    public interface IFinanceRepository
    {
        Task AddAsync(Finance finance); // Stages a new finance record for addition
        Task<Finance?> GetByIdAsync(int id);  // Retrieves a finance record by its unique identifier
        Task ApplyToMemberBalanceAsync(Finance finance); // Applies an approved finance record to the member's balance
        Task SaveChangesAsync(); // Commits all staged changes to the database
    }
}
