# Detailed Authentication Setup Steps for ASP.NET Core Blazor Server

This guide provides a comprehensive, step-by-step process for implementing authentication in a new project, following the patterns from the Hotel Revenue Tracker project.

---

## 1. Create BaseEntity
**Purpose:** Provides common properties for all entities (Id, CreatedDate, etc.)
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedDate { get; set; }
}
```

---

## 2. Create Domain Entities

### a. User Entity
**Purpose:** Represents a user in the system.
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

### b. PasswordResetToken Entity
**Purpose:** Stores password reset tokens for secure password reset flows.
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

### c. Enums
**Purpose:** Define user roles and row status.
```csharp
public enum UserRole
{
    None = 0,
    SuperAdmin = 1,
    PortfolioManager = 2,
    HotelStaff = 3,
    Investor = 4
}

public enum RowStatus : byte
{
    Inactive = 0,
    Active = 1,
    Deleted = 255
}
```

---

## 3. Create DTOs for Authentication
**Purpose:** Transfer data between layers.
```csharp
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string MasterKey { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

public class LoginResponseDto
{
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Role { get; set; }
    public string? LoginToken { get; set; }
}

public class ForgetPasswordDto
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string MasterKey { get; set; } = string.Empty;
}
```

---

## 4. Implement Password Encryption Service
**Purpose:** Securely encrypt and decrypt passwords.
```csharp
public interface IPasswordEncryptionService
{
    Task<string> EncryptPasswordAsync(string plainTextPassword, string masterKey);
    Task<string> DecryptPasswordAsync(string encryptedPassword, string masterKey);
}

public class PasswordEncryptionService : IPasswordEncryptionService
{
    // Use AES-256-GCM with PBKDF2 (see your project for full implementation)
}
```

---

## 5. Configure Database Context
**Purpose:** Manage database access and entity sets.
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

---

## 6. Implement CQRS Handlers for Authentication
**Purpose:** Use MediatR for login, get user, password reset, etc.
- Create `LoginRequest`, `GetCurrentUserRequest`, `ForgotPasswordRequest`, `ResetPasswordRequest` handlers.
- Each handler should validate input, interact with the database, and return results via DTOs.

---

## 7. Configure Dependency Injection
**Purpose:** Register all services in your DI container.
```csharp
services.AddDbContext<AppDbContext>(...);
services.AddScoped<IPasswordEncryptionService, PasswordEncryptionService>();
services.AddScoped<IEmailSender, SendGridEmailSender>();
services.AddMediatR(typeof(YourApplicationAssembly));
```

---

## 8. Setup Cookie Authentication in Program.cs
**Purpose:** Enable and configure authentication/authorization middleware.
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.Name = "YourAppAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.IsEssential = true;
        options.Cookie.Path = "/";
    });
builder.Services.AddAuthorization();
```

---

## 9. Create Authentication Controller
**Purpose:** Handle sign-in, sign-out, and password reset endpoints.
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) { _mediator = mediator; }

    [HttpGet("signin")]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn(string token, string? returnUrl = null)
    {
        // Parse token, get user, create claims, sign in, redirect
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

---

## 10. Create Blazor Components
**Purpose:** UI for login, forgot password, reset password, etc.
- Use `EditForm` for forms.
- Inject `IMediator` for CQRS calls.
- Use `AuthenticationStateProvider` for state management.

---

## 11. Add Validation
**Purpose:** Ensure data integrity and security.
- Use FluentValidation for DTOs (e.g., `LoginDtoValidator`, `ResetPasswordRequestDtoValidator`).

---

## 12. Database Migration
**Purpose:** Create tables for Users and PasswordResetTokens.
- Use EF Core migrations or SQL scripts:
```sql
CREATE TABLE "Users" (...);
CREATE TABLE "PasswordResetTokens" (...);
```

---

## 13. Security Best Practices
- Use secure cookies (HttpOnly, Secure, SameSite).
- Enforce strong password policies.
- Never store plain text passwords.
- Implement rate limiting for login and password reset.
- Log authentication events and errors.
- Use HTTPS in production.

---

**You can copy and expand each step as needed for your new project. If you need full code for any handler, service, or component, let me know!** 