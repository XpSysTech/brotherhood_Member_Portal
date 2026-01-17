using Microsoft.AspNetCore.Identity;

namespace Brotherhood_Portal.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public required string? DisplayName { get; set; }
        public string? ImageUrl { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        //Navigation Property
        public Member? Member { get; set; } = null;
    }
}
