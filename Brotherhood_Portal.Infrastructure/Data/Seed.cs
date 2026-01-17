using Brotherhood_Portal.Domain.DTOs;
using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Brotherhood_Portal.Infrastructure.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var memberData = await File.ReadAllTextAsync("Data/SeedData.json");
            var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);

            if (members == null)
            {
                Console.WriteLine("No members in seed data");
                return;
            }

            foreach (var member in members)
            {
                var user = new AppUser
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName,
                    Email = member.Email,
                    UserName = member.Email!,
                    ImageUrl = member.ImageUrl,
                    Member = new Member
                    {
                        DateOfBirth = member.DateOfBirth,
                        DisplayName = member.DisplayName,
                        FirstName = member.FirstName!,
                        LastName = member.LastName!,
                        ImageUrl = member.ImageUrl,
                        MemberBiography = member.MemberBiography,
                        Occupation = member.Occupation,
                        Business = member.Business,
                        ContactNumber = member.ContactNumber,
                        HomeAddress = member.HomeAddress,
                        HomeCity = member.HomeCity
                    }
                };

                user.Member.Photos.Add(new Photo
                {
                    Url = member.ImageUrl!,
                    MemberId = member.Id,
                    //IsApproved = true
                });

                var result = await userManager.CreateAsync(user, "Pa$$w0rd");
                if (!result.Succeeded)
                {
                    Console.WriteLine(result.Errors.First().Description);
                }
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                DisplayName = "Admin",
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
        }
    }
}
