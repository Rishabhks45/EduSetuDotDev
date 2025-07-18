using FluentValidation;

namespace EduSetu.Application.Features.Authentication;

#region DTOs

/// <summary>
/// DTO for login data
/// </summary>
public class LoginDto
{
    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => _email = value?.ToLowerInvariant() ?? string.Empty;
    }
    public string Password { get; set; } = string.Empty;
    public string MasterKey { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

/// <summary>
/// Response DTO for login operations
/// </summary>
public class LoginResponseDto
{
    public UserInfo? User { get; set; }
    public string? RedirectUrl { get; set; }
    public string? LoginToken { get; set; }
}

/// <summary>
/// DTO for get current user requests
/// </summary>
public class GetCurrentUserDto
{
    public string UserId { get; set; } = string.Empty;
    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => _email = value?.ToLowerInvariant() ?? string.Empty;
    }
}

/// <summary>
/// DTO for authentication status checks
/// </summary>
public class GetAuthStatusDto
{
    public string UserId { get; set; } = string.Empty;
    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => _email = value?.ToLowerInvariant() ?? string.Empty;
    }
}

/// <summary>
/// Response DTO for authentication status checks
/// </summary>
public class AuthStatusResponse
{
    public bool IsAuthenticated { get; set; }
    public UserInfo? User { get; set; }
}

/// <summary>
/// DTO for forgot password requests
/// </summary>
public class ForgetPasswordDto
{
    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => _email = value?.ToLowerInvariant() ?? string.Empty;
    }
}

/// <summary>
/// DTO for reset password requests
/// </summary>
public class ResetPasswordDto
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string MasterKey { get; set; } = string.Empty;
}

/// <summary>
/// DTO for reset password form data
/// </summary>
public class ResetPasswordRequestDto
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// DTO for validate reset token requests
/// </summary>
public class ValidateResetTokenDto
{
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// DTO for parse login token request
/// </summary>
public class ParseLoginTokenDto
{
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// Data extracted from login token
/// </summary>
public class LoginTokenData
{
    public string Email { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}


/// <summary>
/// User information included in authentication responses
/// </summary>
public class UserInfo
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => _email = value?.ToLowerInvariant() ?? string.Empty;
    }
    public string Role { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime? LastLoginAt { get; set; }
}

#endregion

#region Validators

/// <summary>
/// Validator for LoginDto
/// </summary>
public sealed class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.Email)
                    .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")
                    .WithMessage("Invalid email address format")
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.Email)
                            .MaximumLength(255)
                            .WithMessage("Email must not exceed 255 characters");
                    });
            });

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}

/// <summary>
/// Validator for ForgetPasswordDto
/// </summary>
public sealed class ForgetPasswordDtoValidator : AbstractValidator<ForgetPasswordDto>
{
    public ForgetPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
             .NotEmpty()
             .WithMessage("Email address is required")
             .DependentRules(() =>
             {
                 RuleFor(x => x.Email)
                     .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")
                     .WithMessage("Invalid email address format")
                     .DependentRules(() =>
                     {
                         RuleFor(x => x.Email)
                             .MaximumLength(255)
                             .WithMessage("Email must not exceed 255 characters");
                     });
             });
    }
}

/// <summary>
/// Validator for ValidateResetTokenDto
/// </summary>
public sealed class ValidateResetTokenDtoValidator : AbstractValidator<ValidateResetTokenDto>
{
    public ValidateResetTokenDtoValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Reset token is required.");
    }
}

/// <summary>
/// Validator for ParseLoginTokenDto
/// </summary>
public sealed class ParseLoginTokenDtoValidator : AbstractValidator<ParseLoginTokenDto>
{
    public ParseLoginTokenDtoValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required.");
    }
}

/// <summary>
/// Validator for ResetPasswordRequestDto
/// </summary>
public sealed class ResetPasswordRequestDtoValidator : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestDtoValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Reset token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.NewPassword)
                    .MinimumLength(8).WithMessage("Password must be at least 8 characters long") //Password must be at least 8 characters long and include at least one uppercase letter, one number, and one special character.
                    .MaximumLength(32).WithMessage("Password must not exceed 32 characters") //Password must not exceed 32 characters
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.NewPassword)
                            .Matches(@"[A-Z]").WithMessage("Password must contain at least 1 uppercase letter")
                            .DependentRules(() =>
                            {
                                RuleFor(x => x.NewPassword)
                                    .Matches(@"[a-z]").WithMessage("Password must contain at least 1 lowercase letter")
                                    .DependentRules(() =>
                                    {
                                        RuleFor(x => x.NewPassword)
                                            .Matches(@"\d").WithMessage("Password must contain at least 1 number")
                                            .DependentRules(() =>
                                            {
                                                RuleFor(x => x.NewPassword)
                                                    .Matches(@"[@$!%*?&]").WithMessage("Password must contain at least 1 special character");
                                            });
                                    });
                            });
                    });
            });

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirm password is required.")
            .DependentRules(() =>
            {
                RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.NewPassword)
                    .WithMessage("Passwords do not match.");
            });
    }
}

#endregion
