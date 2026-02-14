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
    /// <summary>
    /// SECURITY SERVICE
    ///
    /// [1] PURPOSE
    /// Generates JSON Web Tokens (JWT) for authenticated users.
    ///
    /// [2] RESPONSIBILITIES
    /// - Create signed JWT tokens
    /// - Embed identity and role claims
    /// - Enforce token expiration
    /// - Ensure cryptographic signing integrity
    ///
    /// [3] SECURITY MODEL
    /// - Uses symmetric HMAC SHA-512 signing
    /// - Requires secret key length >= 64 characters
    /// - Includes user roles as claims
    /// - Token expiration set to 15 minutes
    ///
    /// [4] CLAIMS INCLUDED
    /// - Email
    /// - NameIdentifier (User ID)
    /// - Role (one claim per assigned role)
    ///
    /// [5] ARCHITECTURE
    /// - Depends on IConfiguration for TokenKey
    /// - Depends on UserManager<AppUser> for role retrieval
    /// - Implements ITokenService
    ///
    /// [6] IMPORTANT SECURITY NOTE
    /// - TokenKey must be stored securely (e.g., environment variable)
    /// - Never commit TokenKey to source control
    /// - Consider refresh tokens for production systems
    /// </summary>
    public class TokenService(IConfiguration config, UserManager<AppUser> userManager) : ITokenService
    {
        /// <summary>
        /// [1] PURPOSE
        /// Creates a signed JWT token for the specified authenticated user.
        ///
        /// [2] FLOW
        /// - Retrieve TokenKey from configuration
        /// - Validate key length (>= 64 characters)
        /// - Build claims list (Email, UserId, Roles)
        /// - Create signing credentials (HMAC SHA-512)
        /// - Build SecurityTokenDescriptor
        /// - Generate and serialize JWT
        ///
        /// [3] TOKEN LIFETIME
        /// - Expires after 15 minutes
        /// - Short-lived tokens reduce attack window
        ///
        /// [4] RETURN
        /// - Serialized JWT string
        ///
        /// [5] EXCEPTIONS
        /// - Throws if TokenKey missing
        /// - Throws if TokenKey < 64 characters
        /// </summary>
        public async Task<string> CreateToken(AppUser user)
        {
            // Retrieve token secret from configuration
            var tokenKey = config["TokenKey"]
                ?? throw new Exception("Cannot get token key");

            // Enforce minimum cryptographic strength
            if (tokenKey.Length < 64)
            {
                throw new Exception("Your token key needs to be >= 64 characters");
            }

            // Create symmetric signing key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(tokenKey)
            );

            // Build identity claims
            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, user.Email!),
                new(ClaimTypes.NameIdentifier, user.Id)
            };

            // Retrieve and attach role claims
            var roles = await userManager.GetRolesAsync(user);

            claims.AddRange(
                roles.Select(role => new Claim(ClaimTypes.Role, role))
            );

            // Configure signing credentials
            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha512Signature
            );

            // Define token blueprint
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),

                // Short-lived token for improved security
                Expires = DateTime.UtcNow.AddMinutes(15),

                SigningCredentials = creds
            };

            // Generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Return serialized JWT
            return tokenHandler.WriteToken(token);
        }
    }
}
