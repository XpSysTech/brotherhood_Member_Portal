using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Brotherhood_Portal.Domain.Entities
{
    public class Member
    {
        /*Member Info*/
        public string Id { get; set; } = null!;
        //public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string? DisplayName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ImageUrl { get; set; }
        public string? MemberBiography { get; set; }
        public string? Occupation { get; set; }
        public string? Business { get; set; }

        /*Address*/
        public required string? ContactNumber { get; set; }
        public required string? HomeAddress { get; set; }
        public required string HomeCity { get; set; }

        /*Member System Info*/
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        //Navigation Properties
        public List<Photo> Photos { get; set; } = new List<Photo>();

        [JsonIgnore]
        [ForeignKey(nameof(Id))]
        public AppUser User { get; set; } = null!;

    }
}
