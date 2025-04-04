using Microsoft.EntityFrameworkCore;
using MalaysiaBusinessDirectory.Api.Models;

namespace MalaysiaBusinessDirectory.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Business> Businesses { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<BusinessTag> BusinessTags { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship
            modelBuilder.Entity<BusinessTag>()
                .HasKey(bt => bt.Id);

            modelBuilder.Entity<BusinessTag>()
                .HasOne(bt => bt.Business)
                .WithMany(b => b.BusinessTags)
                .HasForeignKey(bt => bt.BusinessId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BusinessTag>()
                .HasOne(bt => bt.Tag)
                .WithMany(t => t.BusinessTags)
                .HasForeignKey(bt => bt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // Setup spatial index for Business location
            modelBuilder.Entity<Business>()
                .HasIndex(b => b.Location)
                .HasMethod("GIST");
                
            // Add any other necessary configurations
        }
    }
}
