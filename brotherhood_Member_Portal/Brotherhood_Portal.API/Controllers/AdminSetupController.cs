using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.API.Controllers
{
    /// <summary>
    /// ONE-TIME USE ONLY
    /// Remove or disable this controller after initial admin creation
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AdminSetupController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminSetupController> _logger;
        public AdminSetupController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, 
            IConfiguration configuration, ILogger<AdminSetupController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Creates the initial admin account
        /// ⚠️ USE ONCE THEN REMOVE THIS ENDPOINT
        /// </summary>
        [HttpPost("initialize")]
        public async Task<IActionResult> Initialize([FromBody] InitialAdminDto dto)
        {
            _logger.LogWarning(
                "Admin setup attempt for email {Email} from IP {IP}",
                dto.Email,
                HttpContext.Connection.RemoteIpAddress?.ToString()
            );

            // Reads from configuration (environment variable or user secrets)
            var setupKey = _configuration["SetupKey"];

            if (string.IsNullOrEmpty(setupKey))
            {
                _logger.LogWarning("Admin setup endpoint disabled.");
                return Forbid("Setup endpoint disabled.");
            }

            if (setupKey != dto.SetupKey)
            {
                _logger.LogWarning("Invalid setup key used.");
                return Unauthorized("Invalid setup key.");
            }

            if (await _userManager.Users.AnyAsync())
            {
                _logger.LogWarning("Admin setup attempted but system already initialized.");
                return BadRequest("System already initialized.");
            }

            // Create roles
            string[] roles = { "Admin", "Moderator", "Member" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create admin user
            var admin = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                DisplayName = dto.DisplayName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(admin, dto.Password);
            
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator", "Member" });

            return Ok(new { message = "Admin created successfully. REMOVE this endpoint now!" });
        }
    }

    public class InitialAdminDto
    {
        public string SetupKey { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}

//POWERSHELL CURL
//curl - X POST http://your-api/api/adminsetup/initialize \
//  -H "Content-Type: application/json" \
//  -d '{
//    "setupKey": "YourOneTimeSetupKeyThatYouWillDeleteAfterUse123!",
//    "email": "admin@yourdomain.com",
//    "displayName": "System Administrator",
//    "password": "YourSecureP@ssw0rd123!"
//  }'

//JSON
//{
//  "setupKey": "MyOneTimeSetupKey123!",
//  "email": "admin@brotherhood.com",
//  "displayName": "System Administrator",
//  "password": "SecureP@ssw0rd123!"
//}