using EduSetu.Application.Common.DTOs;
using EduSetu.Application.Common.Interfaces;
using EduSetu.Application.Features.TeacherRegister.Infrastructure;
using MediatR;

namespace EduSetu.Application.Features.TeacherRegister.Requests;


public sealed record UpsertCoachingDetailsRequest(CoachingDetailsDto Dto) : IRequest<RegisterCoachingDetailsResponse>;

public sealed class RegisterCoachingDetailsResponse : AppResult { }

internal sealed class RegisterCoachingDetailsRequestHandler : IRequestHandler<UpsertCoachingDetailsRequest, RegisterCoachingDetailsResponse>
{
    public IAppDbContext _Ctx;
    public repository _Repository;

    public RegisterCoachingDetailsRequestHandler(IAppDbContext Ctx, repository repository)
    {
        _Ctx = Ctx;
        _Repository = repository;
    }

    public async Task<RegisterCoachingDetailsResponse> Handle(UpsertCoachingDetailsRequest request, CancellationToken cancellationToken)
    {
        var result = new RegisterCoachingDetailsResponse();

        var InstituteNameExists = await _Repository.InstituteNameExistsAsync(request.Dto.InstituteName,request.Dto.Id, cancellationToken);       
        if (InstituteNameExists)
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
