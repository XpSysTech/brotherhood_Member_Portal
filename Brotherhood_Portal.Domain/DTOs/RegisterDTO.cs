using Brotherhood_Portal.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Brotherhood_Portal.Domain.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string?  DisplayName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(4)]
        public string Password { get; set; } = "";

        [Required] public required string? ContactNumber { get; set; } = string.Empty;
        [Required] public required string? HomeAddress { get; set; } = string.Empty;
        [Required] public required string HomeCity { get; set; } = string.Empty;
        [Required] public DateTime DateOfBirth { get; set; }

        //[Required]
        //public Member? Member { get; set; } = null;
    }
}
