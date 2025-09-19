using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Features.Profile.Infrastructure;
using EduSetu.Application.Features.TeacherRegister.Infrastructure;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduSetu.Application.Features.Profile.Requests;

public sealed record GetUserProfileRequest(Session Session) : IRequest<GetUserProfileResponse> { }

public sealed class GetUserProfileResponse : AppResult<UpdateProfileDto> { }

internal sealed class GetUserProfileRequestHandler : IRequestHandler<GetUserProfileRequest, GetUserProfileResponse>
{
    private readonly Repository _repository;

    public GetUserProfileRequestHandler(Repository repository)
    {
        _repository = repository;
    }

    public async Task<GetUserProfileResponse> Handle(GetUserProfileRequest request, CancellationToken cancellationToken)
    {
        var result = new GetUserProfileResponse();

        var user = await _repository.GetUserByIdAsync(request.Session.UserId, cancellationToken);
        if (user == null)
        {
            result.Failure(ErrorCode.NotFound, "User profile not found");
            return result;
        }

        var dto = user.Adapt<UpdateProfileDto>();

        result.Success(dto);
        return result;
    }
}
