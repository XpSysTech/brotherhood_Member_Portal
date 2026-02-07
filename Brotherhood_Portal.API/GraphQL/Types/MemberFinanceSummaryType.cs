using Brotherhood_Portal.Domain.DTOs.Finance;

namespace Brotherhood_Portal.API.GraphQL.Types
{
    public class MemberFinanceSummaryType : ObjectType<MemberFinanceSummaryDTO>
    {
        protected override void Configure(IObjectTypeDescriptor<MemberFinanceSummaryDTO> dto)
        {
            dto.Name("MemberFinanceSummary");

            dto.Field(x => x.MemberId).Type<NonNullType<IdType>>();
            dto.Field(x => x.DisplayName).Type<StringType>();

            dto.Field(x => x.TotalSavings).Type<NonNullType<DecimalType>>();
            dto.Field(x => x.TotalOpsContribution).Type<NonNullType<DecimalType>>();

            dto.Field(x => x.Year).Type<NonNullType<IntType>>();
            dto.Field(x => x.Month).Type<NonNullType<IntType>>();

            dto.Field(x => x.MonthlySavings).Type<NonNullType<DecimalType>>();
            dto.Field(x => x.MonthlyOpsContribution).Type<NonNullType<DecimalType>>();

            dto.Field(x => x.OwnershipPercentageOfFund).Type<NonNullType<DecimalType>>();
            dto.Field(x => x.MonthlyOwnershipPercentage).Type<NonNullType<DecimalType>>();

            dto.Field(x => x.CalculatedAt).Type<NonNullType<DateTimeType>>();
        }
    }
}
