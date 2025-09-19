using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Features.Profile.Infrastructure;
using EduSetu.Domain.Entities;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduSetu.Application.Features.Profile.Requests;

public sealed record UpdateUserProfileRequest(UpdateProfileDto Dto, Session Session) : IRequest<UpdateUserProfileResponse> { }

public sealed class UpdateUserProfileResponse : AppResult { }

internal sealed class UpdateUserProfileRequestHandler : IRequestHandler<UpdateUserProfileRequest, UpdateUserProfileResponse>
{
    private readonly Repository _repository;

    public UpdateUserProfileRequestHandler(Repository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateUserProfileResponse> Handle(UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        var result = new UpdateUserProfileResponse();

        var emailExists = await _repository.ExistsUserByEmailAsync(request.Dto.Email, request.Dto.Id, cancellationToken);
        if (emailExists)
        {
            result.Failure(ErrorCode.BadRequest, "Email address is already in use by another account");
            return result;
        }

        User user = request.Dto.Adapt<User>();

        var isUpdated = await _repository.UpdateUserAsync(user, request.Session, cancellationToken);
        if (!isUpdated)
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
