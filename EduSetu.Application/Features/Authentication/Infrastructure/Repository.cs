using EduSetu.Application.Common.Interfaces;
using EduSetu.Domain.Entities;
using EduSetu.Domain.Enums;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EduSetu.Application.Features.Authentication.Infrastructure;

public class Repository
{
    public IAppDbContext Ctx { get; }
    public Repository(IAppDbContext _ctx)
    {
        Ctx = _ctx;
    }

    // Check User Exists
    public async Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken)
    {
        // Remove the RowStatus check to prevent reuse of soft-deleted email addresses
        return await Ctx.Users.AnyAsync(u => u.Email == email && u.RowStatus != RowStatus.Deleted, cancellationToken);
    }

    // Create User
    public async Task<bool> AddUserAsync(StudentDTOs UserData, CancellationToken cancellationToken)
    {
        Ctx.ChangeTracker.Clear();
        try
        {
            User newuser = UserData.Adapt<User>();
            await Ctx.Users.AddAsync(newuser, cancellationToken);
            return await Ctx.SaveChangesAsync(cancellationToken) > 0;
        }
        catch (Exception ex)
        {
            // Log the exception (you can use a logging framework here)
            Console.WriteLine($"An error occurred while adding a user: {ex.Message}");
            return false;
        }
    }
}