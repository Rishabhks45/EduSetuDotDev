using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Common.Interfaces;
using EduSetu.Domain.Entities;
using EduSetu.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EduSetu.Application.Features.Profile.Infrastructure;

public class Repository
{
    private readonly IAppDbContext _ctx;

    public Repository(IAppDbContext context)
    {
        _ctx = context;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _ctx.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId && u.RowStatus == RowStatus.Active, cancellationToken);
    }

    public async Task<bool> ExistsUserByEmailAsync(string email, Guid excludeId, CancellationToken cancellationToken)
    {
        return await _ctx.Users.AsNoTracking().AnyAsync(
            u => u.Email == email &&
                 u.Id != excludeId &&
                 u.RowStatus != RowStatus.Deleted,
            cancellationToken);
    }

    public async Task<bool> UpdateUserAsync(User user, Session session, CancellationToken cancellationToken)
    {
        _ctx.ChangeTracker.Clear();

        user.LastModifiedBy = session.UserId;
        user.LastModifiedDate = DateTime.UtcNow;
        var entry = _ctx.Entry(user);

        entry.Property(nameof(User.FirstName)).IsModified = true;
        entry.Property(nameof(User.LastName)).IsModified = true;
        entry.Property(nameof(User.Email)).IsModified = true;
        entry.Property(nameof(User.PhoneNumber)).IsModified = true;
        entry.Property(nameof(User.Username)).IsModified = true;
        entry.Property(nameof(User.ProfilePictureUrl)).IsModified = true;
        var rowsAffected = await _ctx.SaveChangesAsync(cancellationToken);
        return rowsAffected > 0;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string newPassword, Session session, CancellationToken cancellationToken)
    {
        _ctx.ChangeTracker.Clear();

        var userToUpdate = new User
        {
            Id = userId,
            Password = newPassword,
            LastModifiedBy = session.UserId,
            LastModifiedDate = DateTime.UtcNow
        };

        _ctx.Users.Attach(userToUpdate);
        _ctx.Entry(userToUpdate).Property(nameof(User.Password)).IsModified = true;
        _ctx.Entry(userToUpdate).Property(nameof(User.LastModifiedBy)).IsModified = true;
        _ctx.Entry(userToUpdate).Property(nameof(User.LastModifiedDate)).IsModified = true;

        var rowsAffected = await _ctx.SaveChangesAsync(cancellationToken);
        return rowsAffected > 0;
    }
}
