using Brotherhood_Portal.Application.Extensions;
using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.DTOs;
using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Brotherhood_Portal.API.Controllers
{
    public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) : BaseApiController
    {
        // Register User
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
                    //ImageUrl = registerDTO.ImageUrl,
                    //MemberBiography = registerDTO.MemberBiography,
                    //Occupation = registerDTO.Occupation,
                    //Business = registerDTO.Business,
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

            await userManager.AddToRoleAsync(user, "Member");

            return await user.ToDto(tokenService);
        }

        // Log User in
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await userManager.Users
                .Include(u => u.Member)
                .FirstOrDefaultAsync(u => u.Email == loginDTO.Email);

            if (user == null)
            {
                //await Task.Delay(2000); // Introduce a delay to mitigate timing attacks
                return Unauthorized("Invalid email or password");
            }

            var result = await userManager.CheckPasswordAsync(user, loginDTO.Password);
           
            if (!result)
            {
                //await Task.Delay(2000); // Introduce a delay to mitigate timing attacks
                return Unauthorized("Invalid email or password");
            }

            return await user.ToDto(tokenService);
        }
    }
}
