using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Common.Interfaces;
using EduSetu.Application.Common.Settings;
using EduSetu.Application.Features.Profile.Infrastructure;
using MediatR;
using Microsoft.Extensions.Options;

namespace EduSetu.Application.Features.Profile.Requests;

public sealed record DeleteUserProfileRequest(DeleteAccountModel deleteAccountModel, Session Session) : IRequest<DeleteUserProfileResponse> { }

public sealed class DeleteUserProfileResponse : AppResult { }

internal sealed class DeleteUserProfileRequestHandler : IRequestHandler<DeleteUserProfileRequest, DeleteUserProfileResponse>
{
    private readonly Repository _repository;
    private readonly IPasswordEncryptionService _passwordEncryptionService;
    private readonly IOptions<EncryptionSettings> _encryptionSettings;

    public DeleteUserProfileRequestHandler(Repository repository, IPasswordEncryptionService passwordEncryptionService, IOptions<EncryptionSettings> encryptionSettings)
    {
        _repository = repository;
        _passwordEncryptionService = passwordEncryptionService;
        _encryptionSettings = encryptionSettings;
    }

    public async Task<DeleteUserProfileResponse> Handle(DeleteUserProfileRequest request, CancellationToken cancellationToken)
    {
        var result = new DeleteUserProfileResponse();

        var user = await _repository.GetUserByIdAsync(request.Session.UserId, cancellationToken);
        if (user == null)
        {
            result.Failure(ErrorCode.NotFound, "User not found");
            return result;
        }
        var decryptedPassword = await _passwordEncryptionService.DecryptPasswordAsync(user.Password, _encryptionSettings.Value.MasterKey);
        if (decryptedPassword != request.deleteAccountModel.Password )
        {
            result.Failure(ErrorCode.Unauthorized, "Current password is incorrect");
            return result;
        }
        var isDeleted = await _repository.DeleteUserAsync(user.Id, cancellationToken);
        if (!isDeleted)
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