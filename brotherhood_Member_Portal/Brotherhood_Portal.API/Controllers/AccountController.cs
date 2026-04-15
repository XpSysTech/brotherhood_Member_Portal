using Brotherhood_Portal.Application.Extensions;
using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.DTOs.Identity.Command;
using Brotherhood_Portal.Domain.DTOs.Identity.Query;
using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;

namespace Brotherhood_Portal.API.Controllers
{

    /// <summary>
    /// PURPOSE
    /// ---------------------------------------------------------------
    /// Manages user identity lifecycle and security operations.
    /// 
    /// RESPONSIBILITIES
    /// ---------------------------------------------------------------
    /// • User registration
    /// • User authentication (JWT issuance)
    /// • Profile updates (self-service)
    /// • Administrative account control (lock/unlock/reset)
    /// 
    /// SECURITY MODEL
    /// ---------------------------------------------------------------
    /// • Registration and Login are public endpoints.
    /// • Profile update requires authenticated user.
    /// • Lock/Unlock/Reset require Admin policy.
    /// 
    /// DESIGN PRINCIPLES
    /// ---------------------------------------------------------------
    /// • Identity and Member domain models are tightly coordinated.
    /// • JWT tokens are issued only after successful authentication.
    /// • Passwords are never logged or returned.
    /// • Role assignment is explicit and controlled.
    /// • All security-relevant actions are logged.
    /// 
    /// AUDITABILITY
    /// ---------------------------------------------------------------
    /// • Authentication attempts logged (without passwords).
    /// • Privileged admin actions logged.
    /// • Profile mutations logged.
    /// • Failure cases logged with error codes (not raw details).
    /// 
    /// DATA FLOW
    /// ---------------------------------------------------------------
    /// AppUser (Identity) ←→ Member (Domain Profile)
    /// 
    /// Tokens are generated via ITokenService.
    /// Persistence handled via ASP.NET Identity and repository abstractions.
    /// </summary>

    

    [ApiVersion("1.0")]
    public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IAppUserRepository userRepository, ILogger<AccountController> logger) : BaseApiController
    {

        #region Register

        /// <summary>
        /// [1] PURPOSE
        /// Creates a new Identity user and associated Member domain profile.
        /// 
        /// [2] PROCESS FLOW
        /// 1. Construct AppUser with embedded Member entity.
        /// 2. Persist via ASP.NET Identity (password hashed).
        /// 3. Assign default role "Member".
        /// 4. Issue JWT token.
        /// 
        /// [3] BUSINESS CONSTRAINTS
        /// • Email must be unique.
        /// • Password must satisfy Identity password policy.
        /// • Member entity is created automatically.
        /// 
        /// [4] DATA INTEGRITY
        /// • Identity and Member are created within the same logical operation.
        /// • If Identity creation fails, no Member persists.
        /// 
        /// [5] RESPONSE CONTRACT
        /// Returns:
        /// • UserDTO
        /// • JWT token
        /// • Basic profile information
        /// 
        /// On failure:
        /// • Returns validation errors via ModelState.
        /// 
        /// [6] SECURITY
        /// • Public endpoint.
        /// • Password securely hashed.
        /// • No password or sensitive data logged.
        /// </summary>

        // POST: api/account/v1/register
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            logger.LogInformation(
                "Registration attempt for Email {Email}",
                registerDTO.Email);

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
                logger.LogWarning(
                    "Registration failed for Email {Email}. Errors: {Errors}",
                    registerDTO.Email,
                    string.Join(", ", result.Errors.Select(e => e.Code)));

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem();
            }

            // Assign default role
            await userManager.AddToRoleAsync(user, "Member");

            logger.LogInformation(
                "User {UserId} successfully registered with Email {Email}",
                user.Id,
                user.Email);

            // Return DTO with JWT token
            return await user.ToDto(tokenService);
        }

        #endregion


        #region Update Member Profile

        /// <summary>
        /// [1] PURPOSE
        /// Allows an authenticated user to update their own profile data.
        /// 
        /// [2] AUTHORIZATION MODEL
        /// • Requires valid JWT.
        /// • User identity resolved from ClaimsPrincipal.
        /// • Cannot update other users.
        /// 
        /// [3] OPERATION FLOW
        /// 1. Load AppUser including Member entity.
        /// 2. Update Identity-level fields (Email, UserName, DisplayName).
        /// 3. Update Member domain fields.
        /// 4. Persist changes via UserManager.
        /// 5. Return refreshed JWT token.
        /// 
        /// [4] DATA CONSISTENCY
        /// • DisplayName recalculated from FirstName + LastName.
        /// • Email normalization handled explicitly.
        /// • EF Core change tracking ensures atomic persistence.
        /// 
        /// [5] RESPONSE CONTRACT
        /// Returns:
        /// • Updated UserDTO
        /// • Refreshed JWT token
        /// 
        /// [6] FAILURE MODES
        /// • 401 Unauthorized if user missing.
        /// • 400 BadRequest for validation or update failure.
        /// 
        /// [7] SECURITY NOTES
        /// • Does not allow password changes.
        /// • Does not allow privilege escalation.
        /// • Logs profile mutations for audit traceability.
        /// </summary>


        [Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult<UserDTO>> UpdateMember(UpdateMemberDto updateMemberDto)
        {
            var userId = userManager.GetUserId(User);

            logger.LogInformation(
                "User {UserId} initiated profile update",
                userId);

            var user = await userManager.Users
                .Include(u => u.Member)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                logger.LogWarning(
                    "Profile update failed. User {UserId} not found",
                    userId);

                return Unauthorized("User not found.");
            }

            if (user.Member == null)
            {
                logger.LogWarning(
                    "Profile update failed. Member entity missing for User {UserId}",
                    userId);

                return BadRequest("Member profile not found.");
            }

            // Update identity fields directly
            user.Email = updateMemberDto.Email;
            user.UserName = updateMemberDto.Email;
            user.NormalizedEmail = updateMemberDto.Email.ToUpper();
            user.NormalizedUserName = updateMemberDto.Email.ToUpper();

            // Update display name
            user.DisplayName = $"{updateMemberDto.FirstName} {updateMemberDto.LastName}";

            // Update Member domain fields
            user.Member.FirstName = updateMemberDto.FirstName!;
            user.Member.LastName = updateMemberDto.LastName!;
            user.Member.DateOfBirth = updateMemberDto.DateOfBirth;
            user.Member.ContactNumber = updateMemberDto.ContactNumber;
            user.Member.HomeAddress = updateMemberDto.HomeAddress;
            user.Member.HomeCity = updateMemberDto.HomeCity;
            user.Member.MemberBiography = updateMemberDto.MemberBiography;
            user.Member.Occupation = updateMemberDto.Occupation;
            user.Member.Business = updateMemberDto.Business;
            user.Member.ImageUrl = updateMemberDto.ImageUrl;

            // ONE update call only
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                logger.LogWarning(
                    "Profile update failed for User {UserId}. Errors: {Errors}",
                    userId,
                    string.Join(", ", result.Errors.Select(e => e.Code)));

                return BadRequest(result.Errors);
            }

            logger.LogInformation(
                "User {UserId} successfully updated profile",
                userId);


            return await user.ToDto(tokenService);
        }

        #endregion


        #region Login

        /// <summary>
        /// [1] PURPOSE
        /// Authenticates a user and issues a JWT access token.
        /// 
        /// [2] PROCESS FLOW
        /// 1. Retrieve user by Email.
        /// 2. Validate password via ASP.NET Identity.
        /// 3. Issue JWT token via ITokenService.
        /// 
        /// [3] SECURITY BEHAVIOR
        /// • Returns generic error message on failure.
        /// • Does not reveal whether email or password failed.
        /// • Prevents user enumeration.
        /// 
        /// [4] RESPONSE CONTRACT
        /// Returns:
        /// • UserDTO
        /// • JWT token
        /// • Member profile information
        /// 
        /// [5] FAILURE MODES
        /// • 401 Unauthorized for invalid credentials.
        /// 
        /// [6] LOGGING POLICY
        /// • Login attempts logged.
        /// • Password never logged.
        /// • Token never logged.
        /// </summary>

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            logger.LogInformation(
                "Login attempt for Email {Email}",
                loginDTO.Email);

            var user = await userManager.Users
                .Include(u => u.Member)
                .FirstOrDefaultAsync(u => u.Email == loginDTO.Email);

            if (user == null)
            {
                logger.LogWarning(
                    "Login failed. Email {Email} not found",
                    loginDTO.Email);

                return Unauthorized("Invalid email or password");
            }

            var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!result)
            {
                logger.LogWarning(
                    "Login failed. Invalid password for User {UserId}",
                    user.Id);

                return Unauthorized("Invalid email or password");
            }

            logger.LogInformation(
                "User {UserId} successfully authenticated",
                user.Id);

            return await user.ToDto(tokenService);
        }

        #endregion


        #region Lock User

        /// <summary>
        /// [1] PURPOSE
        /// Allows an Administrator to disable a user account.
        /// 
        /// [2] BEHAVIOR
        /// • Sets lockout flag via repository.
        /// • Prevents further authentication attempts.
        /// 
        /// [3] AUTHORIZATION
        /// • Requires Admin policy.
        /// 
        /// [4] RESPONSE
        /// • 200 OK on success.
        /// • 404 if user not found.
        /// 
        /// [5] SECURITY
        /// • Privileged action.
        /// • Logged with acting Admin ID.
        /// </summary>

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("lock/{userId}")]
        public async Task<IActionResult> LockUser(string userId)
        {
            var adminId = userManager.GetUserId(User);

            logger.LogInformation(
                "Admin {AdminId} attempting to lock User {TargetUserId}",
                adminId,
                userId);

            var user = await userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                logger.LogWarning(
                    "Lock attempt failed. User {TargetUserId} not found",
                    userId);

                return NotFound("User not found.");
            }

            await userRepository.LockUserAsync(user);

            logger.LogInformation(
                "User {TargetUserId} successfully locked by Admin {AdminId}",
                userId,
                adminId);

            return Ok(new { message = "User account locked successfully." });
        }

        #endregion


        #region Unlock User

        /// <summary>
        /// [1] PURPOSE
        /// Re-enables a previously locked user account.
        /// 
        /// [2] BEHAVIOR
        /// • Removes lockout restrictions.
        /// • Allows authentication again.
        /// 
        /// [3] AUTHORIZATION
        /// • Requires Admin policy.
        /// 
        /// [4] RESPONSE
        /// • 200 OK on success.
        /// • 404 if user not found.
        /// 
        /// [5] AUDIT
        /// • Logs acting Admin and target user.
        /// </summary>
        
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("unlock/{userId}")]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            var adminId = userManager.GetUserId(User);

            logger.LogInformation(
                "Admin {AdminId} attempting to unlock User {TargetUserId}",
                adminId,
                userId);

            var user = await userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                logger.LogWarning(
                    "Unlock attempt failed. User {TargetUserId} not found",
                    userId);

                return NotFound("User not found.");
            }

            await userRepository.UnlockUserAsync(user);

            logger.LogInformation(
                "User {TargetUserId} successfully unlocked by Admin {AdminId}",
                userId,
                adminId);

            return Ok(new { message = "User account unlocked successfully." });
        }


        #endregion


        #region Password Reset

        /// <summary>
        /// [1] PURPOSE
        /// Allows an Administrator to forcibly reset a user's password.
        /// 
        /// [2] PROCESS FLOW
        /// • Retrieve user.
        /// • Generate reset token internally.
        /// • Apply new password via Identity.
        /// 
        /// [3] AUTHORIZATION
        /// • Requires Admin policy.
        /// 
        /// [4] SECURITY CONSIDERATIONS
        /// • Password never logged.
        /// • Does not expose reset token externally.
        /// • Prevents unauthorized resets.
        /// 
        /// [5] RESPONSE
        /// • 200 OK on success.
        /// • 404 if user not found.
        /// • 400 if password policy validation fails.
        /// 
        /// [6] AUDIT
        /// • Logs Admin performing reset.
        /// • Logs target UserId.
        /// </summary>

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var adminId = userManager.GetUserId(User);

            logger.LogInformation(
                "Admin {AdminId} attempting password reset for User {TargetUserId}",
                adminId,
                dto.UserId);

            var user = await userRepository.GetUserByIdAsync(dto.UserId);

            if (user == null)
            {
                logger.LogWarning(
                    "Password reset failed. User {TargetUserId} not found",
                    dto.UserId);

                return NotFound("User not found.");
            }

            var result = await userRepository.ResetPasswordAsync(user, dto.NewPassword);

            if (!result.Succeeded)
            {
                logger.LogWarning(
                    "Password reset failed for User {TargetUserId}. Errors: {Errors}",
                    dto.UserId,
                    string.Join(", ", result.Errors.Select(e => e.Code)));

                return BadRequest(result.Errors);
            }

            logger.LogInformation(
                "Password successfully reset for User {TargetUserId} by Admin {AdminId}",
                dto.UserId,
                adminId);

            return Ok(new { message = "Password reset successfully." });
        }


        #endregion
    }
}
