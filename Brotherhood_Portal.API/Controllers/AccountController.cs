using Brotherhood_Portal.Application.Extensions;
using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.DTOs.Identity.Command;
using Brotherhood_Portal.Domain.DTOs.Identity.Query;
using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.API.Controllers
{
    public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) : BaseApiController
    {
        #region Register

        /// <summary>
        /// [1] PURPOSE
        /// Registers a new user account and creates an associated Member profile.
        ///
        /// [2] BUSINESS RULES
        /// - Email must be unique.
        /// - Password must satisfy Identity password policy.
        /// - A corresponding Member entity is created automatically.
        /// - User is assigned the default role: "Member".
        ///
        /// [3] RESPONSE
        /// Returns:
        /// - User identity information
        /// - JWT authentication token
        /// - Basic profile data
        ///
        /// On validation failure:
        /// - Returns detailed validation errors via ModelState.
        ///
        /// [4] SECURITY
        /// - Public endpoint.
        /// - Password is securely hashed via ASP.NET Identity.
        /// </summary>

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            var user = new AppUser
            {
                DisplayName = registerDTO.DisplayName,
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                Member = new Member
                {
                    DateOfBirth = registerDTO.DateOfBirth,
                    DisplayName = registerDTO.DisplayName,
                    ContactNumber = registerDTO.ContactNumber,
                    HomeAddress = registerDTO.HomeAddress,
                    HomeCity = registerDTO.HomeCity
                }
            };

            var result = await userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem();
            }

            // Assign default role
            await userManager.AddToRoleAsync(user, "Member");

            // Return DTO with JWT token
            return await user.ToDto(tokenService);
        }

        #endregion

        #region Login

        /// <summary>
        /// [1] PURPOSE
        /// Authenticates an existing user and issues a JWT access token.
        ///
        /// [2] BUSINESS RULES
        /// - Email must match an existing user.
        /// - Password must match stored hashed password.
        /// - Authentication is validated using ASP.NET Identity.
        ///
        /// [3] RESPONSE
        /// Returns:
        /// - User identity information
        /// - JWT authentication token
        /// - Member profile details
        ///
        /// On failure:
        /// - Returns 401 Unauthorized
        /// - Does not reveal whether email or password was incorrect
        ///
        /// [4] SECURITY
        /// - Public endpoint.
        /// - Prevents user enumeration by returning generic error message.
        /// - Token generation handled by ITokenService.
        /// </summary> 

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await userManager.Users
                .Include(u => u.Member)
                .FirstOrDefaultAsync(u => u.Email == loginDTO.Email);

            if (user == null)
                return Unauthorized("Invalid email or password");

            var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!result)
                return Unauthorized("Invalid email or password");

            return await user.ToDto(tokenService);
        }

        #endregion
    }
}
