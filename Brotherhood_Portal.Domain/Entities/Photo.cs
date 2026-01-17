using System.Text.Json.Serialization;

namespace Brotherhood_Portal.Domain.Entities
{
    public class Photo
    {
        public string? Id { get; set; }
        public string Url { get; set; } = null!;
        public string? PublicId { get; set; } 
        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

        //Navigation Property
        [JsonIgnore]
        public Member Member { get; set; } = null!;
        public string MemberId { get; set; } = null!;
    }
}
