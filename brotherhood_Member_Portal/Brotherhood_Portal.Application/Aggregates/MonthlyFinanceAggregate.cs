namespace Brotherhood_Portal.Application.Aggregates
{
    public class MonthlyFinanceAggregate
    {
        /*
            MonthlyFinanceAggregate determines the total savings and operations contributions
            for a given month and year, along with the count of members who contributed
            to the operations fund during that period.
                    
            Used by:
            - FinanceService to generate monthly financial reports

            MonthlyFinanceAggregate 
                -> FinanceQueryRepository.GetApprovedFinancesByDateRangeAsync
                    -> FinanceQueryService as part of reporting operations
                        -> FinanceReportController API endpoint
         */

        public int Year { get; init; }
        public int Month { get; init; }
        public decimal TotalSavings { get; init; }
        public decimal TotalOpsContribution { get; init; }
        public int ContributingMemberCount { get; init; }
    }
}
