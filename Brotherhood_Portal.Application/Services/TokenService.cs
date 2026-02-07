using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Brotherhood_Portal.Application.Services
{
    public class TokenService(IConfiguration config, UserManager<AppUser> userManager) : ITokenService
    {
        public async Task<string> CreateToken(AppUser user)
        {
            // Get the token key from configuration
            var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot get token key");

            // Ensure the token key is at least 64 characters long
            if (tokenKey.Length < 64) 
            {
                throw new Exception("Your token key needs to be >= 64 characters");
            }

            // Create the symmetric security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

            // Create claims for the token
            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.NameIdentifier, user.Id)
            };

            // Add user roles to claims
            var roles = await userManager.GetRolesAsync(user);

            // Add each role as a separate claim
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Create signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create the token descriptor - Used as a blueprint for the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                //Expires = DateTime.Now.AddDays(1),
                Expires = DateTime.Now.AddMinutes(15), // Shorter expiration time for better security
                SigningCredentials = creds
            };

            // Create the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return the serialized token
            return tokenHandler.WriteToken(token);
        }
    }
}
