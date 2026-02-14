using Brotherhood_Portal.Domain.DTOs.Finance.Query;

namespace Brotherhood_Portal.API.GraphQL.Types
{
    public class FundFinanceSummaryType : ObjectType<FundFinanceSummaryDTO>
    {
        protected override void Configure(IObjectTypeDescriptor<FundFinanceSummaryDTO> dto)
        {
            dto.Name("FundFinanceSummary");

            dto.Field(x => x.TotalSavings).Type<NonNullType<DecimalType>>();
            dto.Field(x => x.TotalOpsContribution).Type<NonNullType<DecimalType>>();

            dto.Field(x => x.TotalMembers).Type<NonNullType<IntType>>();
            dto.Field(x => x.ActiveMembers).Type<NonNullType<IntType>>();

            dto.Field(x => x.CurrentMonth)
                .Type<NonNullType<MonthlyFinanceSummaryType>>();

            dto.Field(x => x.MonthlyHistory)
                .Type<NonNullType<ListType<NonNullType<MonthlyFinanceSummaryType>>>>();
        }
    }
}
