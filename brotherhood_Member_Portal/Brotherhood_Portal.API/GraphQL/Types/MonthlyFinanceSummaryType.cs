using Brotherhood_Portal.Domain.DTOs.Finance.Query;

namespace Brotherhood_Portal.API.GraphQL.Types
{
    public class MonthlyFinanceSummaryType : ObjectType<MonthlyFinanceSummaryDTO>
    {
        protected override void Configure(IObjectTypeDescriptor<MonthlyFinanceSummaryDTO> dto)
        {
            dto.Name("MonthlyFinanceSummary");

            dto.Field(x => x.Year).Type<NonNullType<IntType>>();
            dto.Field(x => x.Month).Type<NonNullType<IntType>>();

            dto.Field(x => x.TotalSavings).Type<NonNullType<DecimalType>>();
            dto.Field(x => x.TotalOpsContribution).Type<NonNullType<DecimalType>>();

            dto.Field(x => x.ContributingMemberCount).Type<NonNullType<IntType>>();
        }
    }
}
