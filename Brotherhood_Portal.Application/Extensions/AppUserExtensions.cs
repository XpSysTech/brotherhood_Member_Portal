using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.DTOs;
using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.Application.Extensions
{
    public static class AppUserExtensions
    {
        public static async Task<UserDTO> ToDto(this AppUser user, ITokenService tokenService)
        {
            return new UserDTO
            {
                Id = user.Id,
                //DisplayName = user == null
                //    ? user.DisplayName ?? user.Email!
                //    : $"{user.FirstName} {user.LastName}".Trim(),
                DisplayName = user.DisplayName!,
                Email = user.Email!,
                ImageUrl = user.ImageUrl,
                Token = await tokenService.CreateToken(user)
            };
        }
    }
}
