using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.API.Controllers
{
    public class AdminController(UserManager<AppUser> userManager) : BaseApiController
    {
        /*USE THIS TO TEST THEM REMOVE*/
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("test-admin-role")]
        public ActionResult<string> TestAdminEndpoint()
        {
            return "Only admins can see this";
        }

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


        [Authorize(Policy = "RequirePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult<string> GetPhotosForModeration()
        {
            return "Only users with the photo Admin or Moderator roles can see this";
        }
    }
}
