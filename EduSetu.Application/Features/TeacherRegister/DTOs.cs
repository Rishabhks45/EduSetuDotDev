using EduSetu.Domain.Enums;
using FluentValidation;
using System.ComponentModel.DataAnnotations;


namespace EduSetu.Application.Features.TeacherRegister;


//Education Details
public class EducationDetailsDtos
{
    public QualificationType qualificationType { get; set; }

    public Specialization Specialization { get; set; }

    public TeachingExperience teachingExperience { get; set; }

    public Certifications certifications { get; set; }
}
// FluentValidationValidator for EducationDetailsDtos
public class EducationDetailsDtosValidator : AbstractValidator<EducationDetailsDtos>
{
    public EducationDetailsDtosValidator()
    {
        // QualificationType → Specialization (dependent)
        RuleFor(x => x.qualificationType)
            .NotNull().WithMessage("Qualification type is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.Specialization)
                    .NotNull().WithMessage("Specialization is required when qualification type is set");
            });
        // TeachingExperience → Certifications (dependent)
        RuleFor(x => x.teachingExperience)
            .NotNull().WithMessage("Teaching experience is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.certifications)
                    .NotNull().WithMessage("Certifications are required when teaching experience is provided");
            });
    }
}

#region Register Teacher Dtos
public class TeacherRegister
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";

    public string Email { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Username { get; set; } = "";

    public string Password { get; set; } = "";

    public UserRole UserRole { get; set; } = UserRole.Teacher;

    public string ConfirmPassword { get; set; } = "";

    public QualificationType qualificationType { get; set; }

    public Specialization Specialization { get; set; }

    public TeachingExperience teachingExperience { get; set; }

    public Certifications certifications { get; set; }

}

// FluentValidationValidator for TeacherRegister dependent on UserRole
public class TeacherRegisterValidator : AbstractValidator<TeacherRegister>
{
    public TeacherRegisterValidator()
    {
        // First Name
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(100)
            .WithMessage("First name must not exceed 100 characters");

        // Last Name
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(100)
            .WithMessage("Last name must not exceed 100 characters");

        // Email (nested DependentRules)
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.Email)
                    .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")
                    .WithMessage("Invalid email format")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.Email)
                            .MaximumLength(255)
                            .WithMessage("Email must not exceed 255 characters");
                    });
            });

        // Phone Number
        RuleFor(x => x.PhoneNumber)
     .NotEmpty().WithMessage("Phone number is required")
     .DependentRules(() =>
     {
         RuleFor(x => x.PhoneNumber)
             .Matches(@"^[0-9]{10}$")
             .WithMessage("Phone number must be exactly 10 digits")
             .DependentRules(() =>
             {
                 RuleFor(x => x.PhoneNumber)
                     .Must(num => !new[] { "0000000000", "1234567890" }.Contains(num))
                     .WithMessage("Phone number is invalid");
             });
     });


        // Password with nested DependentRules
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.Password)
                    .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                    .MaximumLength(32).WithMessage("Password must not exceed 32 characters")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.Password)
                            .Matches(@"[A-Z]").WithMessage("Password must contain at least 1 uppercase letter")
                            .DependentRules(() =>
                            {
                                RuleFor(x => x.Password)
                                    .Matches(@"[a-z]").WithMessage("Password must contain at least 1 lowercase letter")
                                    .DependentRules(() =>
                                    {
                                        RuleFor(x => x.Password)
                                            .Matches(@"\d").WithMessage("Password must contain at least 1 number")
                                            .DependentRules(() =>
                                            {
                                                RuleFor(x => x.Password)
                                                    .Matches(@"[@$!%*?&]").WithMessage("Password must contain at least 1 special character");
                                            });
                                    });
                            });
                    });
            });

        // Confirm Password
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirm password is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.Password)
                    .WithMessage("Passwords do not match");
            });

        //// QualificationType → Specialization (dependent)
        //RuleFor(x => x.qualificationType)
        //    .NotNull().WithMessage("Qualification type is required")
        //    .DependentRules(() =>
        //    {
        //        RuleFor(x => x.Specialization)
        //            .NotNull().WithMessage("Specialization is required when qualification type is set");
        //    });

        //// TeachingExperience → Certifications (dependent)
        //RuleFor(x => x.teachingExperience)
        //    .NotNull().WithMessage("Teaching experience is required")
        //    .DependentRules(() =>
        //    {
        //        RuleFor(x => x.certifications)
        //            .NotNull().WithMessage("Certifications are required when teaching experience is provided");
        //    });
    }
}


#endregion
public class CoachingDetailsDto
{
    public Guid Id { get; set; }

    public Guid TeacherId { get; set; }

    public string InstituteName { get; set; } = string.Empty;

    public PreferredTeachingMode PreferredTeachingMode { get; set; }

    public int NumberOfStudents { get; set; }

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PinCode { get; set; } = string.Empty;

    public string NumberOfStudentsText
    {
        get => NumberOfStudents == 0 ? string.Empty : NumberOfStudents.ToString();
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                NumberOfStudents = 0;
            }
            else if (int.TryParse(value, out var result) && result >= 0)
            {
                NumberOfStudents = result;
            }
        }
    }
}

// FluentValidationValidator for CoachingDetailsDto

public class CoachingDetailsDtoValidator : AbstractValidator<CoachingDetailsDto>
{
    public CoachingDetailsDtoValidator()
    {
        // Institute Name
        RuleFor(x => x.InstituteName)
            .NotEmpty()
            .WithMessage("Institute name is required")
            .MaximumLength(200)
            .WithMessage("Institute name must not exceed 200 characters");

        // Preferred Teaching Mode (enum check)
        RuleFor(x => x.PreferredTeachingMode)
            .IsInEnum()
            .WithMessage("Preferred teaching mode is invalid");

        // Number of Students (dependent rules)
        RuleFor(x => x.NumberOfStudents)
            .GreaterThan(0).WithMessage("Number of students must be greater than 0")
            .DependentRules(() =>
            {
                RuleFor(x => x.NumberOfStudents)
                    .LessThanOrEqualTo(10000)
                    .WithMessage("Number of students cannot exceed 10,000");
            });

        // Address
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required")
            .MaximumLength(300).WithMessage("Address must not exceed 300 characters");

        // City
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(100).WithMessage("City must not exceed 100 characters");

        // State
        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(100).WithMessage("State must not exceed 100 characters");

        // PinCode with nested dependent rules
        RuleFor(x => x.PinCode)
            .NotEmpty().WithMessage("Pin code is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.PinCode)
                    .Matches(@"^\d{6}$").WithMessage("Pin code must be exactly 6 digits")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.PinCode)
                            .Must(code => code != "000000")
                            .WithMessage("Pin code cannot be all zeros");
                    });
            });
    }
}

