using Brotherhood_Portal.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.API.Controllers
{
    [Route("api/[controller]")] //api/members
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IAppUserRepository _appUserRepository;
        public MembersController(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        /*Get Member*/
        [HttpGet]
        public async Task<IActionResult> GetMembers()
        {
            var users = await _appUserRepository.GetAllAppUsersAsync();
            return Ok(users);
        }

        /*Get Members By ID*/
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMemberById(string id)
        {
            var user = await _appUserRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }

        /*Create Member*/
        [HttpPost]
        public async Task<IActionResult> CreateMember([FromBody] AppUser user)
        {
            var result = await _appUserRepository.CreateUserAsync(user);
            if (!result) return BadRequest("Failed to create user");
            return Ok("User created successfully");
        }

        /*Delete Member*/
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(string id)
        {
            var result = await _appUserRepository.DeleteUserAsync(id);
            if (!result) return NotFound("User not found or could not be deleted");
            return Ok("User deleted successfully");
        }

        /*Update Member*/
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(string id, [FromBody] AppUser user)
        {
            if (id != user.Id) return BadRequest("User ID mismatch");
            var result = await _appUserRepository.UpdateUserAsync(user);
            if (!result) return NotFound("User not found or could not be updated");
            return Ok("User updated successfully");
        }

    }
}
