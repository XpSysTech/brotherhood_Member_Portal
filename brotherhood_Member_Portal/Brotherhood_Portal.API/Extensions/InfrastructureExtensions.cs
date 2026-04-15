using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Infrastructure.Data;
using Brotherhood_Portal.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDBContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
        
        //NOTE: Use SQLite for local development to avoid PostgreSQL setup overhead
        //services.AddDbContext<AppDBContext>(options =>
        //    options.UseSqlite(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IInvoiceSequenceRepository, InvoiceSequenceRepository>();
        services.AddScoped<IFinanceRepository, FinanceRepository>();
        services.AddScoped<IFinanceQueryRepository, FinanceQueryRepository>();

        return services;
    }
}