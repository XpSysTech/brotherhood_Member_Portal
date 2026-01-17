using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.Infrastructure.Context
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : IdentityDbContext<AppUser>(options)
    {
        /*DbSets*/
        //public DbSet<AppUser> Users { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Photo> Photos { get; set; }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure one-to-many relationship between Member and Photo
            modelBuilder.Entity<Member>()
                .HasMany(m => m.Photos)
                .WithOne(p => p.Member)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IdentityRole>()
                .HasData(
                    new IdentityRole { Id = "member-id", Name = "Member", NormalizedName = "MEMBER" }, //General Member
                    new IdentityRole { Id = "moderator-id", Name = "Moderator", NormalizedName = "MODERATOR" }, //Council & President
                    new IdentityRole { Id = "admin-id", Name = "Admin", NormalizedName = "ADMIN" } //Shakeel
                );

            //var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            //    v => v.ToUniversalTime(),
            //    v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            //);

            //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    foreach (var property in entityType.GetProperties())
            //    {
            //        if (property.ClrType == typeof(DateTime))
            //        {
            //            property.SetValueConverter(dateTimeConverter);
            //        }
            //    }
            //}
        }
    }
}
