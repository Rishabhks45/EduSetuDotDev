using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Common.Interfaces;
using EduSetu.Application.Features.TeacherRegister.Infrastructure;
using MediatR;

namespace EduSetu.Application.Features.TeacherRegister.Requests;


public sealed record RegisterCoachingDetailsRequest(CoachingDetailsDto Dto) : IRequest<RegisterCoachingDetailsResponse>;

public sealed class RegisterCoachingDetailsResponse : AppResult { }

internal sealed class RegisterCoachingDetailsRequestHandler : IRequestHandler<RegisterCoachingDetailsRequest, RegisterCoachingDetailsResponse>
{
    public IAppDbContext _Ctx;
    public repository _Repository;

    public RegisterCoachingDetailsRequestHandler(IAppDbContext Ctx, repository repository)
    {
        _Ctx = Ctx;
        _Repository = repository;
    }

    public async Task<RegisterCoachingDetailsResponse> Handle(RegisterCoachingDetailsRequest request, CancellationToken cancellationToken)
    {
        var result = new RegisterCoachingDetailsResponse();

        var InstituteExists = await _Repository.InstituteExistsAsync(request.Dto.Id, request.Dto.InstituteName, cancellationToken);
        if (InstituteExists)
        {
            result.Failure(ErrorCode.Conflict, "Institute name already exists");
            return result;
        }

        var isUpserted = await _Repository.AddInstituteAsync(request.Dto, cancellationToken);
        if (!isUpserted)
        {
            result.Failure(ErrorCode.BadRequest, "Failed to add institute details");
            return result;
        }
        result.Success();
        return result;
    }
}
