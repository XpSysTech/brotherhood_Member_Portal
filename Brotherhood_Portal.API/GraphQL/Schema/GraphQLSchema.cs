using Brotherhood_Portal.API.GraphQL.Queries;
using Brotherhood_Portal.API.GraphQL.Types;
using HotChocolate.Execution.Configuration;

namespace Brotherhood_Portal.API.GraphQL.Schema
{
    /// <summary>
    /// GRAPHQL SCHEMA CONFIGURATION
    ///
    /// [1] PURPOSE
    /// Centralizes GraphQL server configuration and schema registration.
    ///
    /// This is the composition root for all GraphQL types, queries,
    /// and middleware related to the Brotherhood Portal API.
    ///
    /// [2] RESPONSIBILITIES
    /// - Registers root query types
    /// - Registers explicit DTO object mappings
    /// - Enables authorization middleware
    /// - Configures GraphQL execution pipeline
    ///
    /// [3] ARCHITECTURE ROLE
    /// - Infrastructure Layer
    /// - No business logic
    /// - No data access
    /// - Only schema wiring and middleware configuration
    ///
    /// [4] DESIGN PRINCIPLES
    /// - Explicit type registration (recommended for clarity & control)
    /// - Thin API layer (business logic lives in Application layer)
    /// - Authorization integrated at GraphQL middleware level
    ///
    /// [5] SECURITY
    /// - .AddAuthorization() enables [Authorize] attributes in resolvers
    /// - Authorization policies are configured in Program.cs
    /// </summary>
    public static class GraphQLSchema
    {
        /// <summary>
        /// Registers and configures the GraphQL server.
        ///
        /// [1] Adds GraphQL execution engine.
        /// [2] Registers root query type(s).
        /// [3] Registers DTO object mappings explicitly.
        /// [4] Enables authorization middleware.
        ///
        /// Returns:
        /// IRequestExecutorBuilder for further chaining.
        ///
        /// NOTE:
        /// - Mutation types can be added later via .AddMutationType<T>()
        /// - Subscription types can be added via .AddSubscriptionType<T>()
        /// </summary>
        public static IRequestExecutorBuilder AddGraphQLSchema(this IServiceCollection services)
        {
            return services
                .AddGraphQLServer()

                // Root Query
                // Entry point for all read operations
                .AddQueryType<FinanceQuery>()

                // Explicit DTO Mappings
                // Prevents accidental exposure of unwanted properties
                // Provides strong schema control
                .AddType<MemberFinanceSummaryType>()
                .AddType<MonthlyFinanceSummaryType>()
                .AddType<FundFinanceSummaryType>()

                // Enables [Authorize] attribute support
                .AddAuthorization();
        }
    }
}
