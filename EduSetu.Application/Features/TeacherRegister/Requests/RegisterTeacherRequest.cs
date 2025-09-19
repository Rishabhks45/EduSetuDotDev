using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Common.Helpers;
using EduSetu.Application.Common.Interfaces;
using EduSetu.Application.Common.Settings;
using EduSetu.Application.Features.TeacherRegister.Infrastructure;
using MediatR;
using Microsoft.Extensions.Options;

namespace EduSetu.Application.Features.TeacherRegister.Requests;

public sealed record RegisterTeacherRequest(TeacherRegister teacherData) : IRequest<RegisterTeacherResponse>;

public sealed class RegisterTeacherResponse : AppResult<Guid> { }

internal sealed class RegisterTeacherRequestHandler : IRequestHandler<RegisterTeacherRequest, RegisterTeacherResponse>
{
    public IAppDbContext _Ctx { get; }
    public IPasswordEncryptionService _PasswordEncryptionService { get; }
    public IOptions<EncryptionSettings> _EncryptionSettings { get; }
    public repository _Repository { get; }

    public RegisterTeacherRequestHandler(IAppDbContext Ctx, IPasswordEncryptionService passwordEncryptionService, IOptions<EncryptionSettings> encryptionSettings, repository repository)
    {
        _Ctx = Ctx;
        _PasswordEncryptionService = passwordEncryptionService;
        _EncryptionSettings = encryptionSettings;
        _Repository = repository;
    }

    public async Task<RegisterTeacherResponse> Handle(RegisterTeacherRequest request, CancellationToken cancellationToken)
    {
        string tempPassword = string.Empty;
        RegisterTeacherResponse result = new RegisterTeacherResponse();

        var UserExists = await _Repository.UserExistsAsync(request.teacherData.Id, request.teacherData.Email, cancellationToken);
        var UserNameExists = await _Repository.UserNameExistsAsync( request.teacherData.Email, cancellationToken);
        if (UserExists)
        {
            result.Failure(ErrorCode.Conflict, "User already exists");
            return result;
        }
        if (UserNameExists)
        {
            result.Failure(ErrorCode.Conflict, "Username already exists");
            return result;
        }

        if (request.teacherData.Id == Guid.Empty)
        {
           // tempPassword = CommonHelper.GenerateTemporaryPassword();
            request.teacherData.Password = await _PasswordEncryptionService.EncryptPasswordAsync(request.teacherData.Password, _EncryptionSettings.Value.MasterKey);
        }
        else
        {
            tempPassword = await _PasswordEncryptionService.EncryptPasswordAsync(request.teacherData.Password!, _EncryptionSettings.Value.MasterKey);
        }

        Guid teacherId = await _Repository.AddTeacherAsync(request.teacherData, cancellationToken);
        if (teacherId == Guid.Empty)
        {
            result.Failure(ErrorCode.BadRequest, "Failed to add teacher details");
            return result;
        }

        result.Success(teacherId);
        return result;
    }
}
