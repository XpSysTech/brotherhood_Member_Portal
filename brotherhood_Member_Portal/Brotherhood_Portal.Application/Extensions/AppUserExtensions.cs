using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.DTOs.Identity.Query;
using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.Application.Extensions
{
    /// <summary>
    /// EXTENSION METHODS FOR APPUSER
    ///
    /// [1] PURPOSE
    /// Provides mapping logic from AppUser domain entity
    /// to UserDTO used by the API layer.
    ///
    /// [2] RESPONSIBILITIES
    /// - Transforms internal identity model into a safe API response model
    /// - Generates a JWT token for authenticated sessions
    /// - Ensures no sensitive internal properties are exposed
    ///
    /// [3] SECURITY BOUNDARY
    /// - Password hash is NEVER exposed
    /// - Security stamps are NEVER exposed
    /// - Only safe identity fields are returned
    /// - JWT token is generated at transformation time
    ///
    /// [4] ARCHITECTURAL ROLE
    /// - Keeps mapping logic out of controllers
    /// - Promotes clean controller code
    /// - Centralizes identity-to-DTO conversion
    ///
    /// This follows the principle:
    /// "Entities should not leak directly to the API layer."
    /// </summary>
    public static class AppUserExtensions
    {
        /// <summary>
        /// [1] PURPOSE
        /// Converts an AppUser entity into a UserDTO
        /// and attaches a newly generated JWT token.
        ///
        /// [2] FLOW
        /// - Copies safe identity fields
        /// - Generates token using ITokenService
        ///
        /// [3] RETURNS
        /// A fully populated UserDTO including:
        /// - Id
        /// - DisplayName
        /// - Email
        /// - ImageUrl
        /// - Token (JWT)
        ///
        /// [4] NOTE
        /// Token generation is asynchronous because
        /// role retrieval occurs inside TokenService.
        /// </summary>
        public static async Task<UserDTO> ToDto(
            this AppUser user,
            ITokenService tokenService)
        {
            return new UserDTO
            {
                Id = user.Id,

                // DisplayName is intentionally taken from stored value.
                // Avoid computing names dynamically here unless required.
                DisplayName = user.DisplayName!,

                Email = user.Email!,
                ImageUrl = user.ImageUrl,

                // Generates signed JWT including roles
                Token = await tokenService.CreateToken(user)
            };
        }
    }
}
