using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Brotherhood_Portal.Domain.DTOs.Identity.Command
{
    public class UpdateMemberDto
    {
        [Required]
        public string? FirstName { get; set; }
        
        [Required]
        public string? LastName { get; set; }

        // DisplayName = FirstName + LastName
        [NotMapped]
        public string DisplayName => $"{FirstName} {LastName}".Trim();

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        public string? MemberBiography { get; set; }
        public string? Occupation { get; set; }
        public string? Business { get; set; }

        public required string? ContactNumber { get; set; }
        public required string? HomeAddress { get; set; }
        public required string HomeCity { get; set; }

        [Required] 
        public DateOnly DateOfBirth { get; set; }
    }
}
