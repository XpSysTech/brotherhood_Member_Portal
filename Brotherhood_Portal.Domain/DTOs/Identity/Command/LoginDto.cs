namespace Brotherhood_Portal.Domain.DTOs.Identity.Command
{
    public class LoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
