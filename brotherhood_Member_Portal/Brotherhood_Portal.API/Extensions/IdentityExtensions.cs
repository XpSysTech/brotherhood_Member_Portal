using Brotherhood_Portal.Domain.Entities;
using Brotherhood_Portal.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

public static class IdentityExtensions
{
    public static IServiceCollection AddAppIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<AppUser>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.User.RequireUniqueEmail = true;
            opt.Password.RequireDigit = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AppDBContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        return services;
    }
}