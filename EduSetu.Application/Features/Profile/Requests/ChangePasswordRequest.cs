using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Common.Settings;
using MediatR;
using Microsoft.Extensions.Options;
using EduSetu.Application.Common.Interfaces;
using EduSetu.Application.Features.Profile.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduSetu.Application.Features.Profile.Requests;


public sealed record ChangePasswordRequest(ChangePasswordDto Dto, Session Session) : IRequest<ChangePasswordResponse> { }

public sealed class ChangePasswordResponse : AppResult { }

internal sealed class ChangePasswordRequestHandler : IRequestHandler<ChangePasswordRequest, ChangePasswordResponse>
{
    private readonly Repository _repository;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    private readonly EncryptionSettings _encryptionSettings;

    public ChangePasswordRequestHandler(Repository repository, IPasswordEncryptionService passwordEncryptionService, IOptions<EncryptionSettings> encryptionSettings)
    {
        _repository = repository;
        _passwordEncryptionService = passwordEncryptionService;
        _encryptionSettings = encryptionSettings.Value;
    }

    public async Task<ChangePasswordResponse> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var result = new ChangePasswordResponse();

        var user = await _repository.GetUserByIdAsync(request.Session.UserId, cancellationToken);
        if (user == null)
        {
            result.Failure(ErrorCode.NotFound, "User not found");
            return result;
        }

        var decryptedPassword = await _passwordEncryptionService.DecryptPasswordAsync(user.Password, _encryptionSettings.MasterKey);
        if (decryptedPassword != request.Dto.CurrentPassword)
        {
            result.Failure(ErrorCode.Unauthorized, "Current password is incorrect");
            return result;
        }

        var encryptedNewPassword = await _passwordEncryptionService.EncryptPasswordAsync(request.Dto.NewPassword, _encryptionSettings.MasterKey);
        var isPasswordChanged = await _repository.ChangePasswordAsync(user.Id, encryptedNewPassword, request.Session, cancellationToken);
        if (!isPasswordChanged)
        {
            result.Failure(ErrorCode.InternalServerError, "Something went wrong, please try again later.");
        }
        else
        {
            result.Success();
        }

        return result;
    }
}
