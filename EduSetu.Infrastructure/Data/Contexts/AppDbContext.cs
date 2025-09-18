using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using EduSetu.Application.Common.Interfaces;
using EduSetu.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EduSetu.Infrastructure.Data.Contexts
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CoachingDetails> CoachingDetails { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        public DatabaseFacade Database => base.Database;
        public ChangeTracker ChangeTracker => base.ChangeTracker;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        public EntityEntry Entry(object entity) => base.Entry(entity);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure PasswordResetToken entity
            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ResetToken).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.HasIndex(e => e.ResetToken).IsUnique();
            });
        }
    }
}
