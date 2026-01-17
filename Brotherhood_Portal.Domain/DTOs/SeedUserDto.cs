namespace Brotherhood_Portal.Domain.DTOs
{
    public class SeedUserDto
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string DisplayName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ImageUrl { get; set; }
        public string? MemberBiography { get; set; }
        public string? Occupation { get; set; }
        public string? Business { get; set; }
        public required string ContactNumber { get; set; }
        public required string HomeAddress { get; set; }
        public required string HomeCity { get; set; }
        public DateTime LastActive { get; set; }
        public DateTime Created { get; set; } 
        public bool IsActive { get; set; }
    }
}
