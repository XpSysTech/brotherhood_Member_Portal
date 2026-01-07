namespace Brotherhood_Portal.Domain.Entities
{
    public class AppUser
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string DisplayName { get; set; }
        public required string Email { get; set; }

        /*AppUser Details: Contact Number, Date Of Birth, Location/Address, Occupation/Business*/
    }
}
