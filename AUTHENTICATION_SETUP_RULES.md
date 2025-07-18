# Authentication Setup Rules for ASP.NET Core Blazor Server Applications

## Overview
This document provides comprehensive rules and guidelines for implementing a secure authentication system in ASP.NET Core Blazor Server applications, based on the analysis of the Hotel Revenue Tracker project.

## Architecture Principles

### 1. Clean Architecture Structure
```
Project/
├── Domain/                    # Domain entities and enums
├── Application/               # Business logic and CQRS
├── Infrastructure/           # Data access and external services
└── Presentation.BlazorServer/ # UI layer and authentication
```

### 2. Authentication Flow
- **CQRS Pattern**: Use MediatR for authentication commands/queries
- **Cookie Authentication**: Primary authentication mechanism
- **Token-based Flow**: Secure login tokens for session management
- **Role-based Authorization**: Multiple user roles with specific permissions

## Domain Layer Setup

### 1. User Entity
```csharp
public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Timezone { get; set; }
    
    public string FullName => $"{FirstName} {LastName}".Trim();
}
```

### 2. User Roles Enum
```csharp
public enum UserRole
{
    [Display(Name = "None")]
    None = 0,
    
    [Display(Name = "Super Admin")]
    SuperAdmin = 1,
    
    [Display(Name = "Portfolio Manager")]
    PortfolioManager = 2,
    
    [Display(Name = "Hotel Staff")]
    HotelStaff = 3,
    
    [Display(Name = "Investor")]
    Investor = 4
}
```

### 3. Row Status Enum
```csharp
public enum RowStatus : byte
{
    [Display(Name = "Inactive")]
    Inactive = 0,
    
    [Display(Name = "Active")]
    Active = 1,
    
    [Display(Name = "Deleted")]
    Deleted = 255
}
```

### 4. Password Reset Token Entity
```csharp
public class PasswordResetToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string ResetToken { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
}
```

## Application Layer Setup

### 1. Authentication DTOs
```csharp
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

public class LoginResponseDto
{
    public UserInfo? User { get; set; }
    public string? LoginToken { get; set; }
}

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
```

### 2. CQRS Authentication Commands/Queries
```csharp
// Login Request
public sealed record LoginRequest(LoginDto LoginData) : IRequest<LoginResponse>;
public sealed class LoginResponse : AppResult<LoginResponseDto>;

// Get Current User Request
public sealed record GetCurrentUserRequest(GetCurrentUserDto UserData) : IRequest<GetCurrentUserResponse>;
public sealed class GetCurrentUserResponse : AppResult<UserInfo>;

// Parse Login Token Request
public sealed record ParseLoginTokenRequest(ParseLoginTokenDto TokenData) : IRequest<ParseLoginTokenResponse>;
public sealed class ParseLoginTokenResponse : AppResult<LoginTokenData>;
```

### 3. Password Reset DTOs
```csharp
public class ForgetPasswordDto
{
    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => _email = value?.ToLowerInvariant() ?? string.Empty;
    }
}

public class ResetPasswordDto
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string MasterKey { get; set; } = string.Empty;
}

public class ValidateResetTokenDto
{
    public string Token { get; set; } = string.Empty;
}
```

## Infrastructure Layer Setup

### 1. Password Encryption Service
```csharp
public interface IPasswordEncryptionService
{
    Task<string> EncryptPasswordAsync(string plainTextPassword, string masterKey);
    Task<string> DecryptPasswordAsync(string encryptedPassword, string masterKey);
}

public class PasswordEncryptionService : IPasswordEncryptionService
{
    private const int KeySize = 32; // 256 bits
    private const int IvSize = 12; // 96 bits for GCM
    private const int SaltSize = 16; // 128 bits
    private const int TagSize = 16; // 128 bits for GCM authentication tag
    private const int IterationCount = 100000; // PBKDF2 iterations
    
    // Implementation using AES-256-GCM with PBKDF2 key derivation
}
```

### 2. Email Service Interface
```csharp
public interface IEmailSender
{
    Task<bool> SendResetPasswordEmailAsync(string email, string resetLink, string userName);
}
```

### 3. Database Context Configuration
```csharp
public class AppDbContext : DbContext, IAppDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new PasswordResetTokenConfiguration());
    }
}
```

## Presentation Layer Setup

### 1. Program.cs Configuration
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
        
        // Session timeout (30 minutes)
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        
        // Cookie security settings
        options.Cookie.Name = "YourAppAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.IsEssential = true;
        options.Cookie.Path = "/";
        
        // Event handlers
        options.Events.OnSigningIn = context =>
        {
            context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30);
            context.Properties.IsPersistent = false;
            return Task.CompletedTask;
        };
        
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

// Custom Authentication State Provider
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
```

### 2. Authentication Controller
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("signin")]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn(string token, string? returnUrl = null)
    {
        // Parse login token using CQRS
        var parseTokenRequest = new ParseLoginTokenRequest(new ParseLoginTokenDto { Token = token });
        var parseTokenResponse = await _mediator.Send(parseTokenRequest);
        
        if (parseTokenResponse.HasError || parseTokenResponse.Payload == null)
        {
            return Redirect("/login");
        }

        // Get user info
        var getUserRequest = new GetCurrentUserRequest(new GetCurrentUserDto { Email = parseTokenResponse.Payload.Email });
        var getUserResponse = await _mediator.Send(getUserRequest);
        
        if (getUserResponse.HasError || getUserResponse.Payload == null)
        {
            return Redirect("/login");
        }

        // Create claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, getUserResponse.Payload.UserId ?? ""),
            new(ClaimTypes.Name, getUserResponse.Payload.FullName ?? ""),
            new(ClaimTypes.Email, getUserResponse.Payload.Email ?? ""),
            new(ClaimTypes.GivenName, getUserResponse.Payload.FirstName ?? ""),
            new(ClaimTypes.Surname, getUserResponse.Payload.LastName ?? ""),
            new(ClaimTypes.Role, getUserResponse.Payload.Role ?? "")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            IsPersistent = false,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            authProperties);

        // Redirect based on role or return URL
        string redirectUrl = !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) 
            ? returnUrl 
            : GetDashboardUrlByRole(getUserResponse.Payload.Role);

        return Redirect(redirectUrl);
    }

    [HttpGet("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/login");
    }
}
```

### 3. Custom Authentication State Provider
```csharp
public class ServerAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ServerAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory scopeFactory) : base(loggerFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(5);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        if (!authenticationState.User.Identity?.IsAuthenticated ?? true)
        {
            return false;
        }

        string? userId = authenticationState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? email = authenticationState.User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
        {
            return false;
        }

        using IServiceScope scope = _scopeFactory.CreateScope();
        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var authStatusRequest = new GetAuthStatusRequest(new GetAuthStatusDto 
        { 
            UserId = userId, 
            Email = email 
        });
        
        var authStatusResponse = await mediator.Send(authStatusRequest, cancellationToken);

        return !authStatusResponse.HasError && authStatusResponse.Payload?.IsAuthenticated == true;
    }
}
```

### 4. Login Page Component
```csharp
public partial class Login : ComponentBase
{
    [Inject] private IMediator Mediator { get; set; } = default!;
    [Inject] private IOptions<EncryptionSettings> EncryptionOptions { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;

    private LoginDto loginRequest = new();
    private string errorMessage = string.Empty;
    private bool isLoading = false;

    private async Task HandleLogin()
    {
        isLoading = true;
        errorMessage = string.Empty;

        var encryptionSettings = EncryptionOptions.Value;
        if (!encryptionSettings.IsValid())
        {
            errorMessage = "Configuration error. Please contact support.";
            isLoading = false;
            return;
        }

        var loginDto = new LoginDto
        {
            Email = loginRequest.Email,
            Password = loginRequest.Password,
            RememberMe = loginRequest.RememberMe,
            MasterKey = encryptionSettings.MasterKey
        };

        var loginResponse = await Mediator.Send(new LoginRequest(loginDto));

        if (loginResponse.HasError)
        {
            errorMessage = loginResponse.Errors.Count > 0
                ? loginResponse.Errors[0].Message
                : "Invalid email or password";
            isLoading = false;
            return;
        }

        // Redirect to authentication endpoint
        string signInUrl = $"/api/auth/signin?token={loginResponse.Payload?.LoginToken}";
        Navigation.NavigateTo(signInUrl, replace: true);
    }
}
```

## Configuration Settings

### 1. appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_Connection_String_Here"
  },
  "EncryptionSettings": {
    "MasterKey": "Your_Base64_Encoded_Master_Key_Here"
  },
  "SendGridSettings": {
    "FromEmail": "noreply@yourapp.com",
    "FromName": "Your App Name",
    "APIKey": "Your_SendGrid_API_Key"
  },
  "ApplicationSettings": {
    "MobileAppLoginUrl": "https://mobile.yourapp.com/login",
    "WebAppLoginUrl": "https://web.yourapp.com/login"
  }
}
```

### 2. Settings Classes
```csharp
public class EncryptionSettings
{
    public string MasterKey { get; set; } = string.Empty;
    
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(MasterKey) && MasterKey.Length >= 32;
    }
}

public class SendGridSettings
{
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string APIKey { get; set; } = string.Empty;
}
```

## Database Setup

### 1. Users Table
```sql
CREATE TABLE "Users" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "FirstName" VARCHAR(100) NOT NULL,
    "LastName" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(255) NOT NULL UNIQUE,
    "Password" VARCHAR(500) NOT NULL,
    "Role" INTEGER NOT NULL DEFAULT 0,
    "RowStatus" SMALLINT NOT NULL DEFAULT 1,
    "LastLoginAt" TIMESTAMP WITH TIME ZONE NULL,
    "ProfilePictureUrl" VARCHAR(500) NULL,
    "PhoneNumber" VARCHAR(20) NULL,
    "Timezone" VARCHAR(100) NULL,
    "CreatedDate" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "LastModifiedDate" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

### 2. Password Reset Tokens Table
```sql
CREATE TABLE "PasswordResetTokens" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserId" UUID NOT NULL,
    "ResetToken" VARCHAR(500) NOT NULL UNIQUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ExpiresAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "IsUsed" BOOLEAN NOT NULL DEFAULT FALSE,
    FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);
```

## Security Best Practices

### 1. Password Security
- Use AES-256-GCM encryption for passwords
- Implement PBKDF2 key derivation with 100,000 iterations
- Store master key securely in configuration
- Use cryptographically secure random number generation

### 2. Session Security
- Set appropriate cookie security flags (HttpOnly, Secure, SameSite)
- Implement sliding expiration for sessions
- Use secure token generation for password resets
- Implement rate limiting for authentication attempts

### 3. Authorization
- Implement role-based access control
- Use claims-based authorization
- Validate user status on each request
- Implement proper redirect handling for unauthorized access

### 4. Error Handling
- Don't expose sensitive information in error messages
- Log authentication failures for monitoring
- Implement proper exception handling middleware
- Use consistent error response formats

## Validation Rules

### 1. Login Validation
```csharp
public sealed class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("Invalid email address format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long");
    }
}
```

### 2. Password Reset Validation
```csharp
public sealed class ResetPasswordRequestDtoValidator : AbstractValidator<ResetPasswordRequestDto>
{
    public ResetPasswordRequestDtoValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Reset token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
    }
}
```

## Testing Guidelines

### 1. Unit Tests
- Test authentication handlers in isolation
- Mock dependencies (database, email service)
- Test validation rules
- Test encryption/decryption functionality

### 2. Integration Tests
- Test complete authentication flow
- Test password reset flow
- Test authorization rules
- Test session management

### 3. Security Tests
- Test for common vulnerabilities (SQL injection, XSS)
- Test password strength requirements
- Test session timeout behavior
- Test rate limiting functionality

## Deployment Considerations

### 1. Environment Configuration
- Use different master keys for each environment
- Configure appropriate connection strings
- Set up proper logging levels
- Configure HTTPS in production

### 2. Database Migration
- Use Entity Framework migrations
- Test migrations in staging environment
- Backup database before applying migrations
- Monitor migration performance

### 3. Monitoring
- Log authentication events
- Monitor failed login attempts
- Track session statistics
- Set up alerts for security events

## Common Issues and Solutions

### 1. Session Timeout Issues
- Ensure proper cookie configuration
- Check sliding expiration settings
- Verify authentication state provider configuration

### 2. Password Reset Token Issues
- Verify token expiration logic
- Check email service configuration
- Ensure proper URL encoding/decoding

### 3. Authorization Issues
- Verify role assignments
- Check claims configuration
- Ensure proper authorization policies

### 4. Performance Issues
- Optimize database queries
- Use appropriate indexes
- Implement caching where appropriate
- Monitor authentication performance

## Maintenance Tasks

### 1. Regular Cleanup
- Remove expired password reset tokens
- Clean up old user sessions
- Archive old authentication logs
- Update security dependencies

### 2. Security Updates
- Rotate master keys periodically
- Update encryption algorithms as needed
- Review and update security policies
- Conduct security audits

### 3. Performance Monitoring
- Monitor authentication response times
- Track database query performance
- Monitor memory usage
- Review session statistics

This comprehensive authentication setup provides a secure, scalable, and maintainable authentication system for ASP.NET Core Blazor Server applications. 