using Athos.ReviewAutomation.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Athos.ReviewAutomation.Infrastructure.Data
{
    public class ReviewDbContext : DbContext
    {
        public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options) { }

        public DbSet<DbReview> Reviews { get; set; } = null!;
        public DbSet<Business> Businesses { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<BusinessUser> BusinessUsers { get; set; } = null!;
        public DbSet<BusinessSettings> BusinessSettings { get; set; } = null!;
        public DbSet<BusinessOAuthToken> BusinessOAuthTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Business entity configuration
            modelBuilder.Entity<Business>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SubscriptionTier).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.GoogleBusinessProfileId).IsUnique();
            });

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.GoogleId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.GoogleId).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // BusinessUser entity configuration
            modelBuilder.Entity<BusinessUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Business)
                    .WithMany(b => b.BusinessUsers)
                    .HasForeignKey(e => e.BusinessId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.BusinessUsers)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.InvitedBy)
                    .WithMany()
                    .HasForeignKey(e => e.InvitedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasIndex(e => new { e.BusinessId, e.UserId }).IsUnique();
            });

            // DbReview entity configuration
            modelBuilder.Entity<DbReview>(entity =>
            {
                entity.HasKey(e => e.ReviewId);
                entity.HasOne(e => e.Business)
                    .WithMany(b => b.Reviews)
                    .HasForeignKey(e => e.BusinessId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.BusinessId, e.Timestamp });
                entity.HasIndex(e => new { e.BusinessId, e.IsApproved });
                entity.HasIndex(e => new { e.BusinessId, e.Sentiment });
            });

            // BusinessSettings entity configuration
            modelBuilder.Entity<BusinessSettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Business)
                    .WithOne(b => b.Settings)
                    .HasForeignKey<BusinessSettings>(e => e.BusinessId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // BusinessOAuthToken entity configuration
            modelBuilder.Entity<BusinessOAuthToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Business)
                    .WithMany(b => b.OAuthTokens)
                    .HasForeignKey(e => e.BusinessId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.BusinessId, e.Provider });
            });
        }
    }
}
