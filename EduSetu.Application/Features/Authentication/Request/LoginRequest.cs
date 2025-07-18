using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Common.Interfaces;
using EduSetu.Domain.Entities;
using EduSetu.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduSetu.Application.Features.Authentication.Request;

public sealed record LoginRequest(LoginDto LoginData) : IRequest<LoginResponse>;

public sealed class LoginResponse : AppResult<LoginResponseDto>;

internal sealed class LoginRequestHandler(
    IAppDbContext _ctx,
    IPasswordEncryptionService _passwordEncryptionService
    ) : IRequestHandler<LoginRequest, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {

        LoginResponse result = new LoginResponse();

        User? user = await _ctx.Users
            .FirstOrDefaultAsync(u => u.Email == request.LoginData.Email &&
            u.RowStatus != RowStatus.Deleted &&
                            (u.Role == UserRole.SuperAdmin ||
        u.Role == UserRole.Teacher ||
        u.Role == UserRole.Student), cancellationToken);

        if (user == null)
        {
            result.Failure(ErrorCode.NotFound, "Invalid email or password");
            return result;
        }

        string decryptedPassword = await _passwordEncryptionService.DecryptPasswordAsync(
            user.Password, request.LoginData.MasterKey);

        if (decryptedPassword != request.LoginData.Password)
        {
            result.Failure(ErrorCode.Unauthorized, "Invalid email or password");
            return result;
        }

        if (user.RowStatus != RowStatus.Active)
        {
            result.Failure(ErrorCode.Forbidden, "Account is inactive");
            return result;
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _ctx.SaveChangesAsync(cancellationToken);

        string loginToken = CreateLoginToken(request.LoginData.Email, request.LoginData.RememberMe);

        LoginResponseDto loginResponse = new LoginResponseDto
        {
            User = new UserInfo
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                Role = user.Role.ToString()
            },
            LoginToken = loginToken
        };

        result.Success(loginResponse);
        return result;
    }

    private static string CreateLoginToken(string email, bool rememberMe)
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string tokenData = $"{email}|{rememberMe}|{timestamp}";
        byte[] tokenBytes = Encoding.UTF8.GetBytes(tokenData);
        string token = Convert.ToBase64String(tokenBytes);
        return Uri.EscapeDataString(token);
    }
}