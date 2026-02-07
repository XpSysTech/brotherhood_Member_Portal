using Brotherhood_Portal.Application.Aggregates;
using Brotherhood_Portal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brotherhood_Portal.Application.Interfaces
{
    public interface IFinanceQueryRepository
    {
        Task<IReadOnlyList<Finance>> GetApprovedFinancesAsync();
        Task<IReadOnlyList<Finance>> GetApprovedFinancesByMemberAsync(string memberId);
        Task<IReadOnlyList<Finance>> GetApprovedFinancesByMonthAsync(int year, int month);
        Task<int> GetTotalMemberCountAsync();
        Task<int> GetActiveMemberCountAsync();
        Task<IReadOnlyCollection<MonthlyFinanceAggregate>> GetMonthlyAggregatesAsync();
    }
}
