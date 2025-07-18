using EduSetu.Application.Common.Interfaces;
using EduSetu.Components;
using EduSetu.Infrastructure.Data.Contexts;
using EduSetu.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MediatR;
using EduSetu.Application.Common.Settings;
using EduSetu.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

// register connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the interface for dependency injection
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

//Register all services in your DI container.
builder.Services.AddScoped<EduSetu.Application.Common.Interfaces.IPasswordEncryptionService, PasswordEncryptionService>();

// Register MediatR for CQRS pattern
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(EduSetu.Application.Features.Authentication.Request.LoginRequest).Assembly));

// Register HttpContextAccessor for accessing HttpContext in components
builder.Services.AddHttpContextAccessor();

// Add Authentication with Cookie and Google support
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Google";
})
.AddCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/access-denied";

    // Default session timeout (30 minutes of inactivity)
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true; // Reset timeout on activity

    // Cookie security settings
    options.Cookie.Name = "EduSetuAuth";
    options.Cookie.HttpOnly = true; // Prevent XSS attacks
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Use HTTPS in production
    options.Cookie.SameSite = SameSiteMode.Lax; // Less restrictive for development
    options.Cookie.IsEssential = true; // Required for GDPR compliance
    options.Cookie.Path = "/"; // Ensure cookie is available for all paths

    // Enhanced event handlers for Blazor Server
    options.Events.OnSignedIn = context =>
    {
        return Task.CompletedTask;
    };

    options.Events.OnValidatePrincipal = context =>
    {
        return Task.CompletedTask;
    };

    // Handle session expiration - IMPORTANT: Don't auto-redirect for Blazor
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        // For Blazor Server SignalR connections, don't auto-redirect
        if (context.Request.Headers.ContainsKey("Connection") &&
            context.Request.Headers["Connection"].ToString().Contains("Upgrade"))
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
})
.AddGoogle("Google", options =>
{
    options.ClientId = "367131133436-9ulnv933benc9d4cl4v85ddiaps28ped.apps.googleusercontent.com"; //367131133436 - 9ulnv933benc9d4cl4v85ddiaps28ped.apps.googleusercontent.com
    options.ClientSecret = "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"; //GOCSPX - nQG4iWlck3QvPysxIS5tCSdiJ8IJ
    options.CallbackPath = "/profile";
    options.SaveTokens = true;    
});

builder.Services.Configure<EncryptionSettings>(
    builder.Configuration.GetSection("EncryptionSettings"));

// Add infrastructure and application services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

// Add controllers for API endpoints
builder.Services.AddControllers();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Add Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map API controllers
app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();