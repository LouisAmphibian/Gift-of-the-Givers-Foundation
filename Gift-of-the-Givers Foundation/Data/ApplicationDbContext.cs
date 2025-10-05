using Microsoft.EntityFrameworkCore;
using Gift_of_the_Givers_Foundation.Models;

namespace Gift_of_the_Givers_Foundation.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<IncidentReport> IncidentReports { get; set; }
        public DbSet<Donation> Donations { get; set; } // ADD THIS LINE

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Incident Report configuration
            modelBuilder.Entity<IncidentReport>(entity =>
            {
                entity.HasKey(e => e.IncidentID);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(150);
                entity.Property(e => e.IncidentType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Severity).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ReportedDate).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(ir => ir.ReportedByUser)
                      .WithMany()
                      .HasForeignKey(ir => ir.ReportedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ADD DONATION CONFIGURATION
            modelBuilder.Entity<Donation>(entity =>
            {
                entity.HasKey(e => e.DonationID);
                entity.Property(e => e.DonationType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Unit).HasMaxLength(50);
                entity.Property(e => e.Urgency).HasMaxLength(20);
                entity.Property(e => e.SpecialInstructions).HasMaxLength(500);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DonationDate).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.DonatedByUser)
                      .WithMany()
                      .HasForeignKey(d => d.DonatedByUserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}