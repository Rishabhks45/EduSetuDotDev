using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Common.Helpers;
using EduSetu.Application.Common.Interfaces;
using EduSetu.Application.Common.Settings;
using EduSetu.Application.Features.Authentication.Infrastructure;
using EduSetu.Application.Features.TeacherRegister.Requests;
using MediatR;
using Microsoft.Extensions.Options;

namespace EduSetu.Application.Features.Authentication.Request;

public sealed record RegisterUserRequest(StudentDTOs UserData) : IRequest<RegisterUserResponse>;
public sealed class RegisterUserResponse : AppResult<string> { }

internal sealed class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, RegisterUserResponse>
{
    public IAppDbContext _Ctx { get; }
    public Repository _Repository { get; }
    public IPasswordEncryptionService _PasswordEncryptionService { get; }
    public IOptions<EncryptionSettings> _EncryptionSettings { get; } 

    public RegisterUserRequestHandler(IAppDbContext Ctx, Repository repository, IPasswordEncryptionService passwordEncryptionService, IOptions<EncryptionSettings> encryptionSettings)
    {
         _Ctx = Ctx;
        _Repository = repository;
        _PasswordEncryptionService = passwordEncryptionService;
        _EncryptionSettings = encryptionSettings;
    }

    public async Task<RegisterUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        string tempPassword = string.Empty;
        string hashedPassword = string.Empty;
        RegisterUserResponse result = new RegisterUserResponse();

        if (request.UserData.Password == string.Empty)
        {
            request.UserData.Password = await _PasswordEncryptionService.EncryptPasswordAsync(request.UserData.Password, _EncryptionSettings.Value.MasterKey);
        }
        else
        {
            hashedPassword = await _PasswordEncryptionService.EncryptPasswordAsync(request.UserData.Password!, _EncryptionSettings.Value.MasterKey);
        }

        // Check if user already exists (considering soft delete)
        var userexists = await _Repository.CheckUserExistsAsync(request.UserData.Email, cancellationToken);
        if (userexists)
        {
            result.Failure(ErrorCode.Conflict, "User already exists");
            return result;
        }
        if (request.UserData.id == Guid.Empty && request.UserData.Password== string.Empty)
        {
            request.UserData.Password = await _PasswordEncryptionService.EncryptPasswordAsync(request.UserData.Password, _EncryptionSettings.Value.MasterKey);
        }
        else
        {
            tempPassword = await _PasswordEncryptionService.EncryptPasswordAsync(request.UserData.Password!, _EncryptionSettings.Value.MasterKey);
        }
        var isRegistered = await _Repository.AddUserAsync(request.UserData, cancellationToken);
        if(!isRegistered)
        {
            result.Failure(ErrorCode.InternalServerError, "Something went wrong, please try again later.");
            return result;
        }
        result.Success();
        return result;

    }
}
