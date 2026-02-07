using Brotherhood_Portal.API.GraphQL.Queries;
using Brotherhood_Portal.API.GraphQL.Types;
using HotChocolate.Execution.Configuration;

namespace Brotherhood_Portal.API.GraphQL.Schema
{
    public static class GraphQLSchema
    {
        public static IRequestExecutorBuilder AddGraphQLSchema(this IServiceCollection services)
        {
            return services
                .AddGraphQLServer()

                // Root query
                .AddQueryType<FinanceQuery>()

                // Explicit DTO mappings (recommended)
                .AddType<MemberFinanceSummaryType>()
                .AddType<MonthlyFinanceSummaryType>()
                .AddType<FundFinanceSummaryType>();

                // Authorization support
                //.AddAuthorization();
        }
    }
}
