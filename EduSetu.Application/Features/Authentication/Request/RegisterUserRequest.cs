using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Common.Interfaces;
using EduSetu.Domain.Entities;
using EduSetu.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace EduSetu.Application.Features.Authentication.Request;

public sealed record RegisterUserRequest(RegisterUserDto UserData) : IRequest<RegisterUserResponse>;

public sealed class RegisterUserDto
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public string Password { get; set; } = string.Empty; // For Google, can be empty or random
}

public sealed class RegisterUserResponse : AppResult<string> { }

internal sealed class RegisterUserRequestHandler(
    IAppDbContext _ctx
    ) : IRequestHandler<RegisterUserRequest, RegisterUserResponse>
{
    public async Task<RegisterUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        RegisterUserResponse result = new RegisterUserResponse();

        // Check if user already exists
        var exists = await _ctx.Users.AnyAsync(u => u.Email == request.UserData.Email, cancellationToken);
        if (exists)
        {
            result.Failure(ErrorCode.Conflict, "User already exists");
            return result;
        }

        var user = new User
        {
            Email = request.UserData.Email,
            FirstName = request.UserData.FirstName,
            LastName = request.UserData.LastName,
            Role = request.UserData.Role,
            Password = request.UserData.Password ?? string.Empty // For Google, can be empty or random
        };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync(cancellationToken);
        result.Success(user.Id.ToString());
        return result;
    }
}
