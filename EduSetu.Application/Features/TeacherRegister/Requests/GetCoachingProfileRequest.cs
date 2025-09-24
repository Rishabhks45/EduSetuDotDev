using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Features.TeacherRegister.Infrastructure;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EduSetu.Application.Features.TeacherRegister.Requests;

public sealed record GetCoachingProfileRequest(Session Session) : IRequest<GetCoachingProfileResponse> { }

public sealed class GetCoachingProfileResponse : AppResult<CoachingDetailsDto> { }

internal sealed class GetCoachingProfileRequestHandler : IRequestHandler<GetCoachingProfileRequest, GetCoachingProfileResponse>
{
    private readonly repository _repository;

    public GetCoachingProfileRequestHandler(repository repository)
    {
        _repository = repository;
    }

    public async Task<GetCoachingProfileResponse> Handle(GetCoachingProfileRequest request, CancellationToken cancellationToken)
    {
        var result = new GetCoachingProfileResponse();

        var coachingEntity = await _repository._Ctx.CoachingDetails
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.TeacherId == request.Session.UserId, cancellationToken);

        if (coachingEntity is null)
        {
            //result.Failure(ErrorCode.NotFound, "Coaching profile not found");
            return result;
        }

        var dto = coachingEntity.Adapt<CoachingDetailsDto>();
        result.Success(dto);
        return result;
    }
}
