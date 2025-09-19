using EduSetu.Domain.Enums;
using FluentValidation;

namespace EduSetu.Application.Features.Profile;


#region Profile Update DTOs
public class UpdateProfileDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    private string _email = string.Empty;
    public DateTime CreatedDate { get; set; }
    public RowStatus RowStatus { get; set; }
    public string Email
    {
        get => _email;
        set => _email = value?.ToLowerInvariant() ?? string.Empty;
    }

    public string? ImageBytes { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string FullName => $"{FirstName} {LastName}".Trim(); 
    public QualificationType? qualificationType { get; set; }
    public Specialization? Specialization { get; set; }
    public TeachingExperience? teachingExperience { get; set; }
    public Certifications? certifications { get; set; }
    private string? _phoneNo;
    public string? PhoneNumber
    {
        get => _phoneNo;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var digits = new string(value.Where(char.IsDigit).ToArray());
                if (digits.Length > 10)
                {
                    digits = digits.Substring(0, 10);
                }

                if (digits.Length == 10)
                {
                    _phoneNo = $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6)}";
                }
                else
                {
                    _phoneNo = value;
                }
            }
            else
            {
                _phoneNo = value;
            }
        }
    }
}

public sealed class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.FirstName)
                    .MaximumLength(100)
                    .WithMessage("First name must not exceed 100 characters")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.FirstName)
                            .Matches(@"^[a-zA-Z\s-']+$")
                            .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");
                    });
            });

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.LastName)
                    .MaximumLength(100)
                    .WithMessage("Last name must not exceed 100 characters")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.LastName)
                            .Matches(@"^[a-zA-Z\s-']+$")
                            .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");
                    });
            });

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email address is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.Email)
                    .EmailAddress()
                    .WithMessage("Please enter a valid email address")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.Email)
                            .MaximumLength(255)
                            .WithMessage("Email address must not exceed 255 characters");
                    });
            });

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\(\d{3}\) \d{3}-\d{4}$")
            .WithMessage("Mobile number must be in format (123) 456-7890")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

    }
}
#endregion

#region Change Password DTOs
public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Please enter current password");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Please enter new password")
            .DependentRules(() =>
            {
                RuleFor(x => x.NewPassword)
                    .MaximumLength(100).WithMessage("Password must not exceed 100 characters")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.NewPassword)
                            .MinimumLength(8).Configure(cfg => cfg.MessageBuilder = _ => null)
                            .DependentRules(() =>
                            {
                                RuleFor(x => x.NewPassword)
                                    .Matches(@"[A-Z]").Configure(cfg => cfg.MessageBuilder = _ => null)
                                    .DependentRules(() =>
                                    {
                                        RuleFor(x => x.NewPassword)
                                            .Matches(@"[a-z]").Configure(cfg => cfg.MessageBuilder = _ => null)
                                            .DependentRules(() =>
                                            {
                                                RuleFor(x => x.NewPassword)
                                                    .Matches(@"\d").Configure(cfg => cfg.MessageBuilder = _ => null)
                                                    .DependentRules(() =>
                                                    {
                                                        RuleFor(x => x.NewPassword)
                                                            .Matches(@"[@$!%*?&]").Configure(cfg => cfg.MessageBuilder = _ => null);
                                                    });
                                            });
                                    });
                            });
                    });
            });

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Please enter confirm password")
            .DependentRules(() =>
            {
                RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.NewPassword)
                    .WithMessage("Passwords do not match");
            });
    }
}

#endregion
