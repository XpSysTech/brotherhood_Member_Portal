using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.API.Controllers
{
    public class AdminController(UserManager<AppUser> userManager) : BaseApiController
    {
        /*
            - This controller manages Admin-specific functionalities.
            - It includes endpoints for retrieving users with their roles, editing user roles,
              adding roles to users, and accessing photo moderation features.
            - TODO:
                - Implement a common for response structure for all endpoints.
                - Implement logging for all actions performed in this controller.
         */

        #region Get Users With Roles
        /*
            - Summary:
                - This endpoint allows an admin to retrieve a list of all users along with their assigned roles.
                - Only users with the "Admin" role can access this endpoint.
         */
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

            //return Ok(userList);

            return Ok(new
            {
                message = "Users Returned Successfully",
                userList
            });
        }
        #endregion

        #region Edit Roles
        /*
            - Summary:
                - This endpoint allows an admin to edit the roles of a specific user.
                - It takes the userId as a route parameter and a comma-separated list of roles as a query parameter.
                - Only users with the "Admin" role can access this endpoint.
         */
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{userId}")]
        public async Task<ActionResult<IList<string>>> EditRoles(string userId, [FromQuery] string roles)
        {
            if (string.IsNullOrEmpty(roles)) return BadRequest("You must have atleast one role");

            var selectedRoles = roles.Split(',').ToArray();

            var user = await userManager.FindByIdAsync(userId);

            if (user == null) return BadRequest("Could not retrieve user");

            var userRoles = await userManager.GetRolesAsync(user);

            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if(!result.Succeeded) return BadRequest("Failed to add to roles");

            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");

            //return Ok(await userManager.GetRolesAsync(user));

            return Ok(new
            {
                message = "User roles updated successfully",
                roles = await userManager.GetRolesAsync(user)
            });

        }
        #endregion

        #region Add Role To User
        /*
            - Summary:
                - This endpoint allows an admin to add a specific role to a user.
                - It takes the userId as a route parameter and the role to be added as a query parameter.
                - Only users with the "Admin" role can access this endpoint.
         */
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("add-role/{userId}")]
        public async Task<ActionResult> AddRoleToUser(string userId, [FromQuery] string role)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest("Could not retrieve user");
            
            var result = await userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded) return BadRequest("Failed to add role to user");
            return Ok(new
            {
                message = "Role added to user successfully",
                roles = await userManager.GetRolesAsync(user)
            });
        }
        #endregion

        #region Get Photos For Moderation
        [Authorize(Policy = "RequirePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult<string> GetPhotosForModeration()
        {
            return "Only users with the photo Admin or Moderator roles can see this";
        }
        #endregion
    }
}
