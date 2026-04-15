using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Brotherhood_Portal.Domain.Entities
{
    public class FinanceApproval
    {
        [Key]
        public int Id { get; set; }

        // Relationship to Finance
        [Required]
        public int FinanceId { get; set; }

        [ForeignKey(nameof(FinanceId))]
        public Finance Finance { get; set; } = null!;

        // Relationship to User
        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; } = null!;

        // Metadata
        [Required]
        public string Role { get; set; } = null!;

        [Required]
        public DateTime ApprovedAt { get; set; } = DateTime.UtcNow;
    }
}
