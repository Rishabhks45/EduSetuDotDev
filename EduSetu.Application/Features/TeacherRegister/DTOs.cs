using EduSetu.Domain.Enums;
using FluentValidation;


namespace EduSetu.Application.Features.TeacherRegister;

public class CoachingDetailsDto
{

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
    
public class Institute
{
    public string InstituteName { get; set; } = string.Empty;
    public PreferredTeachingMode PreferredTeachingMode { get; set; }
    public int NumberOfStudents { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PinCode { get; set; } = string.Empty;
}

// fulent validation for TeacherRegister
public class TeacherRegisterValidator : AbstractValidator<CoachingDetailsDto>
{
    public TeacherRegisterValidator()
    {
        RuleFor(x => x.InstituteName)
            .NotEmpty().WithMessage("Institute Name is required.")
            .MaximumLength(100).WithMessage("Institute Name cannot exceed 100 characters.");
        RuleFor(x => x.PreferredTeachingMode)
            .IsInEnum().WithMessage("Preferred Teaching Mode is required.");
        RuleFor(x => x.NumberOfStudents)
            .GreaterThan(0).WithMessage("Number of Students must be greater than zero.");
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");
        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(50).WithMessage("City cannot exceed 50 characters.");
        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(50).WithMessage("State cannot exceed 50 characters.");
        RuleFor(x => x.PinCode)
            .NotEmpty().WithMessage("Pin Code is required.")
            .Matches(@"^\d{5,6}$").WithMessage("Pin Code must be 5 or 6 digits.");
    }
}