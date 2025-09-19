using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Common.Helpers;
using EduSetu.Application.Common.Interfaces;
using EduSetu.Application.Common.Settings;
using EduSetu.Domain.Entities;
using EduSetu.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EduSetu.Application.Features.Authentication.Request;

public sealed record RegisterUserRequest(StudentDTOs UserData) : IRequest<RegisterUserResponse>;

public sealed class RegisterUserResponse : AppResult<string> { }

internal sealed class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, RegisterUserResponse>
{
    public IAppDbContext _Ctx { get; }
    public IPasswordEncryptionService _PasswordEncryptionService { get; }
    public IOptions<EncryptionSettings> _EncryptionSettings { get; } 

    public RegisterUserRequestHandler(IAppDbContext Ctx, IPasswordEncryptionService passwordEncryptionService, IOptions<EncryptionSettings> encryptionSettings)
    {
         _Ctx = Ctx;
        _PasswordEncryptionService = passwordEncryptionService;
        _EncryptionSettings = encryptionSettings;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        string hashedPassword = string.Empty;
        if (request.UserData.Password == string.Empty)
        {
            string tempPassword = CommonHelper.GenerateTemporaryPassword();
            hashedPassword = await _PasswordEncryptionService.EncryptPasswordAsync(tempPassword, _EncryptionSettings.Value.MasterKey);
        }
        else
        {
            hashedPassword = await _PasswordEncryptionService.EncryptPasswordAsync(request.UserData.Password!, _EncryptionSettings.Value.MasterKey);
        }
        RegisterUserResponse result = new RegisterUserResponse();

        // Check if user already exists
        var exists = await _Ctx.Users.AnyAsync(u => u.Email == request.UserData.Email && u.RowStatus !=RowStatus.Deleted, cancellationToken);
        if (exists)
        {
            result.Failure(ErrorCode.Conflict, "User already exists");
            return result;
        }
        var user = new User
        {
            RowStatus = RowStatus.Active,
            Email = request.UserData.Email,
            FirstName = request.UserData.FirstName,
            LastName = request.UserData.LastName,
            DateOfBirth = request.UserData.DateOfBirth,
            PhoneNumber = request.UserData.PhoneNumber,
            Role = request.UserData.Role,
            Password = hashedPassword ?? string.Empty // For Google, can be empty or random
        };
        _Ctx.Users.Add(user);
        await _Ctx.SaveChangesAsync(cancellationToken);
        result.Success(user.Id.ToString());
        return result;
    }
}
