using EduSetu.Application.Common.Interfaces;
using EduSetu.Domain.Entities;
using EduSetu.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace EduSetu.Application.Features.TeacherRegister.Infrastructure;

public class repository
{
    public IAppDbContext _Ctx { get; }

    public repository(IAppDbContext Ctx)
    {
        _Ctx = Ctx;
    }

    // check if user already exists
    public async Task<bool> UserExistsAsync(Guid excludeId, string email, CancellationToken cancellationToken)
    {
        return await _Ctx.Users.AsNoTracking().AnyAsync(x => x.Email == email &&
                                                  x.Id != excludeId &&
                                                  x.RowStatus != RowStatus.Deleted, cancellationToken);
    }
    // check if coaching institute name already exists
    public async Task<bool> InstituteExistsAsync(Guid excludeId, string instituteName, CancellationToken cancellationToken)
    {
        return await _Ctx.Users.AsNoTracking().AnyAsync(x => x.InstituteName == instituteName &&
                                                  x.Id != excludeId &&
                                                  x.RowStatus != RowStatus.Deleted, cancellationToken);
    }


    // Register Teacher and Institute Details
    public async Task<bool> AddTeacherAsync(TeacherRegister TeacherReg, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;

        var user = TeacherReg.Adapt<User>();

        if (user.Id == Guid.Empty)
        {
            user.Id = Guid.NewGuid();
            user.CreatedDate = currentTime;
            user.Password = TeacherReg.Password;
            user.Role = UserRole.Teacher;
            _Ctx.Users.Add(user);
        }
        else
        {
            _Ctx.ChangeTracker.Clear();
            _Ctx.Users.Attach(user);
            var entry = _Ctx.Entry(user);
            
            entry.Property(nameof(User.FirstName)).IsModified = true;
            entry.Property(nameof(User.LastName)).IsModified = true;
            entry.Property(nameof(User.Email)).IsModified = true;
            entry.Property(nameof(User.PhoneNumber)).IsModified = true;
            entry.Property(nameof(User.Username)).IsModified = true;
            entry.Property(nameof(User.qualificationType)).IsModified = true;
            entry.Property(nameof(User.Specialization)).IsModified = true;
            entry.Property(nameof(User.teachingExperience)).IsModified = true;
            entry.Property(nameof(User.certifications)).IsModified = true;
            entry.Property(nameof(User.RowStatus)).IsModified = true;
            entry.Property(nameof(User.CreatedBy)).IsModified = true;
            entry.Property(nameof(User.LastModifiedBy)).IsModified = true;
            entry.Property(nameof(User.LastModifiedDate)).IsModified = true;
        }

        var rowsAffected = await _Ctx.SaveChangesAsync(cancellationToken);
        return rowsAffected > 0;
    }

    //Register Institute Details
    public async Task<bool> AddInstituteAsync(CoachingDetailsDto coachingDetails, CancellationToken cancellationToken)
    {
        DateTime currentTime = DateTime.UtcNow;
        var coaching = coachingDetails.Adapt<CoachingDetails>();
        if (coaching.Id == Guid.Empty)
        {
            coaching.Id = Guid.NewGuid();
            coaching.CreatedDate = currentTime;
            coaching.RowStatus = RowStatus.Active;
            coaching.CreatedBy = coachingDetails.TeacherId;
            _Ctx.CoachingDetails.Add(coaching);
        }
        else
        {
            _Ctx.ChangeTracker.Clear();
            _Ctx.CoachingDetails.Attach(coaching);
            var entry = _Ctx.Entry(coaching);
            entry.Property(nameof(CoachingDetails.InstituteName)).IsModified = true;
            entry.Property(nameof(CoachingDetails.PreferredTeachingMode)).IsModified = true;
            entry.Property(nameof(CoachingDetails.NumberOfStudents)).IsModified = true;
            entry.Property(nameof(CoachingDetails.Address)).IsModified = true;
            entry.Property(nameof(CoachingDetails.City)).IsModified = true;
            entry.Property(nameof(CoachingDetails.State)).IsModified = true;
            entry.Property(nameof(CoachingDetails.PinCode)).IsModified = true;
            entry.Property(nameof(CoachingDetails.RowStatus)).IsModified = true;
            entry.Property(nameof(CoachingDetails.CreatedBy)).IsModified = true;
            entry.Property(nameof(CoachingDetails.LastModifiedBy)).IsModified = true;
            entry.Property(nameof(CoachingDetails.LastModifiedDate)).IsModified = true;
        }
       var rowsAffected = await _Ctx.SaveChangesAsync(cancellationToken);
        return rowsAffected > 0;
    }
}
