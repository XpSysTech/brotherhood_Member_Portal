using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.API.Controllers
{
    /// <summary>
    /// Administrative controller responsible for:
    /// - Role management
    /// - User-role inspection
    /// - Moderation-level endpoints
    ///
    /// SECURITY:
    /// - All endpoints require elevated authorization policies.
    /// - Intended strictly for system administrators.
    ///
    /// FUTURE IMPROVEMENTS:
    /// - Standardized API response envelope.
    /// - Structured audit logging for all administrative actions.
    /// </summary>
    public class AdminController(UserManager<AppUser> userManager) : BaseApiController
    {

        #region Get Users With Roles

        /// <summary>
        /// [1] PURPOSE
        /// Retrieves all registered users along with their assigned roles.
        ///
        /// [2] BUSINESS RULES
        /// - Returns all users in the system.
        /// - Each user includes a list of assigned roles.
        ///
        /// [3] RESPONSE
        /// - User Id
        /// - Username
        /// - List of Roles
        ///
        /// [4] SECURITY
        /// - Requires Admin authorization policy.
        /// - Only accessible to users with administrative privileges.
        /// </summary>
        
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await userManager.Users.ToListAsync();
            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);

                userList.Add(new
                {
                    user.Id,
                    Username = user.UserName,
                    Roles = roles.ToList()
                });
            }

            return Ok(new
            {
                message = "Users returned successfully.",
                userList
            });
        }

        #endregion


        #region Edit User Roles

        /// <summary>
        /// [1] PURPOSE
        /// Updates the full role set of a specific user.
        ///
        /// [2] BUSINESS RULES
        /// - Accepts comma-separated roles.
        /// - Adds missing roles.
        /// - Removes roles not included in the request.
        /// - User must exist.
        ///
        /// [3] RESPONSE
        /// - Updated list of roles assigned to the user.
        ///
        /// [4] SECURITY
        /// - Requires Admin authorization policy.
        /// - Role modification is a high-privilege action.
        /// </summary>
        
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{userId}")]
        public async Task<ActionResult> EditRoles(string userId, [FromQuery] string roles)
        {
            if (string.IsNullOrEmpty(roles))
                return BadRequest("At least one role must be specified.");

            var selectedRoles = roles.Split(',').ToArray();
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return BadRequest("User not found.");

            var userRoles = await userManager.GetRolesAsync(user);

            var addResult = await userManager.AddToRolesAsync(
                user,
                selectedRoles.Except(userRoles));

            if (!addResult.Succeeded)
                return BadRequest("Failed to add roles.");

            var removeResult = await userManager.RemoveFromRolesAsync(
                user,
                userRoles.Except(selectedRoles));

            if (!removeResult.Succeeded)
                return BadRequest("Failed to remove roles.");

            return Ok(new
            {
                message = "User roles updated successfully.",
                roles = await userManager.GetRolesAsync(user)
            });
        }

        #endregion


        #region Add Role To User

        /// <summary>
        /// [1] PURPOSE
        /// Adds a single role to a user without removing existing roles.
        ///
        /// [2] BUSINESS RULES
        /// - User must exist.
        /// - Role must be valid.
        ///
        /// [3] RESPONSE
        /// - Updated list of roles assigned to the user.
        ///
        /// [4] SECURITY
        /// - Requires Admin authorization policy.
        /// </summary>
        
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("add-role/{userId}")]
        public async Task<ActionResult> AddRoleToUser(string userId, [FromQuery] string role)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return BadRequest("User not found.");

            var result = await userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
                return BadRequest("Failed to assign role.");

            return Ok(new
            {
                message = "Role added successfully.",
                roles = await userManager.GetRolesAsync(user)
            });
        }

        #endregion


        #region Get Photos For Moderation

        /// <summary>
        /// [1] PURPOSE
        /// Placeholder endpoint for retrieving photos requiring moderation.
        ///
        /// [2] BUSINESS RULES
        /// - Intended for content moderation workflow.
        ///
        /// [3] RESPONSE
        /// - Returns informational string (temporary implementation).
        ///
        /// [4] SECURITY
        /// - Requires Photo moderation authorization policy.
        /// </summary>
        
        [Authorize(Policy = "RequirePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult<string> GetPhotosForModeration()
        {
            return "Only users with Photo Admin or Moderator roles can access moderation features.";
        }

        #endregion
    }
}
