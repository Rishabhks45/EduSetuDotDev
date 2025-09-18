using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using EduSetu.Domain.Entities;

namespace EduSetu.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<CoachingDetails> CoachingDetails { get; }
        DbSet<PasswordResetToken> PasswordResetTokens { get; }

        DatabaseFacade Database { get; }
        ChangeTracker ChangeTracker { get; }
        EntityEntry Entry(object entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
