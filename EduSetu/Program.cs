using EduSetu.Application.Common.Interfaces;
using EduSetu.Components;
using EduSetu.Infrastructure.Data.Contexts;
using EduSetu.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// register connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the interface for dependency injection
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

//Register all services in your DI container.
builder.Services.AddScoped<EduSetu.Application.Common.Interfaces.IPasswordEncryptionService, PasswordEncryptionService>();


// Add Cookie Authentication with enhanced session management
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
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

        // Handle authentication events for remember me functionality
        options.Events.OnSigningIn = context =>
        {
            // Standard 30-minute session
            context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30);
            context.Properties.IsPersistent = false;

            return Task.CompletedTask;
        };

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
    });

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
