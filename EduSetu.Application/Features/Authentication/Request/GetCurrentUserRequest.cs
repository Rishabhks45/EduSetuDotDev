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

public sealed record GetCurrentUserRequest(GetCurrentUserDto UserData) : IRequest<GetCurrentUserResponse>;

public sealed class GetCurrentUserResponse : AppResult<UserInfo>;

internal sealed class GetCurrentUserRequestHandler(
    IAppDbContext _ctx
    ) : IRequestHandler<GetCurrentUserRequest, GetCurrentUserResponse>
{
    public async Task<GetCurrentUserResponse> Handle(GetCurrentUserRequest request, CancellationToken cancellationToken)
    {
        GetCurrentUserResponse result = new GetCurrentUserResponse();

        User? user = null;

        // Search by UserId first if provided
        if (!string.IsNullOrEmpty(request.UserData.UserId) && Guid.TryParse(request.UserData.UserId, out Guid userId))
        {
            user = await _ctx.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.RowStatus == RowStatus.Active, cancellationToken);
        }
        // Fallback to email search
        else if (!string.IsNullOrEmpty(request.UserData.Email))
        {
            user = await _ctx.Users
                .FirstOrDefaultAsync(u => u.Email == request.UserData.Email && u.RowStatus == RowStatus.Active, cancellationToken);
        }

        if (user == null)
        {
            result.Failure(ErrorCode.NotFound, "User not found");
            return result;
        }

        UserInfo userInfo = new UserInfo
        {
            UserId = user.Id.ToString(),
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl,
            FullName = $"{user.FirstName} {user.LastName}",
            Role = user.Role.ToString()
        };

        result.Success(userInfo);
        return result;
    }
}
