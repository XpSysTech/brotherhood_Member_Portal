using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Application.Services;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<FinanceService>();
        services.AddScoped<FinanceQueryService>();
        services.AddScoped<InvoiceNumberService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}