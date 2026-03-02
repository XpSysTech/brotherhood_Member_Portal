using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Ensure roles exist
        string[] roles = { "Admin", "Moderator", "Member" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Check if admin exists
        var adminEmail = "admin@boena.local";

        var adminUser = await userManager.Users
            .FirstOrDefaultAsync(x => x.Email == adminEmail);

        if (adminUser == null)
        {
            var user = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                DisplayName = "System Admin",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                throw new Exception("Admin user creation failed");
            }
        }
    }
}