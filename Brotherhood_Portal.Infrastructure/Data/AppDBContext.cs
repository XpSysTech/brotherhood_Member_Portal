using Brotherhood_Portal.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Brotherhood_Portal.Infrastructure.Data
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : IdentityDbContext<AppUser>(options)
    {
        /*DbSets*/
        //public DbSet<AppUser> Users { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Finance> Finances { get; set; }
        public DbSet<MemberInvoiceSequence> MemberInvoiceSequences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AppUser <-> Member (1:1, shared PK)
            modelBuilder.Entity<Member>()
                .HasOne(m => m.User)
                .WithOne(u => u.Member)
                .HasForeignKey<Member>(m => m.Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Member <-> Photos (1:many)
            modelBuilder.Entity<Member>()
                .HasMany(m => m.Photos)
                .WithOne(p => p.Member)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            // Member <-> Finance (1:many)
            modelBuilder.Entity<Member>()
                .HasMany(m => m.Finances)
                .WithOne(f => f.Member)
                .HasForeignKey(f => f.MemberId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete to protect financial records

            // Explicit timezone config
            modelBuilder.Entity<Photo>()
                .Property(p => p.UploadedDate)
                .HasColumnType("timestamp with time zone");

            // MemberInvoiceSequence Composite Key 
            modelBuilder.Entity<MemberInvoiceSequence>()
                .HasKey(x => new { x.MemberId, x.Year });

            //Roles
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
