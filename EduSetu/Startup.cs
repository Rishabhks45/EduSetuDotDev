using EduSetu.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using EduSetu.Application.Common.Settings;
using EduSetu.Infrastructure.DependencyInjection;
using System.Security.Claims;
using EduSetu.Services.Interfaces;
using EduSetu.Services.Implementations;
using Microsoft.AspNetCore.DataProtection;

public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddCircuitOptions(options => { options.DetailedErrors = true; })
            .AddHubOptions(x => { x.MaximumParallelInvocationsPerClient = 1; x.MaximumReceiveMessageSize = 1024 * 1024 * 5; });

        // Centralized registrations via Infrastructure/Application layers
        services.AddInfrastructure(Configuration);
        services.AddApplicationServices();

        services.AddHttpContextAccessor();

        // Add data protection services for OAuth state validation
        services.AddDataProtection()
            .SetApplicationName("EduSetu")
            .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

        // Add memory cache for OAuth state management
        services.AddMemoryCache();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/login";
            options.LogoutPath = "/logout";
            options.AccessDeniedPath = "/access-denied";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = true;
            options.Cookie.Name = "EduSetuAuth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.IsEssential = true;
            options.Cookie.Path = "/";
            options.Events.OnSignedIn = context =>
            {
                // If there's no return URL, default to the profile page
                if (string.IsNullOrEmpty(context.Properties?.RedirectUri))
                {
                    context.Properties.RedirectUri = "/profile";
                }
                return Task.CompletedTask;
            };
            options.Events.OnValidatePrincipal = async context =>
            {
                var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    context.RejectPrincipal();
                    return;
                }
                var email = context.Principal?.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    context.RejectPrincipal();
                    return;
                }
            };
            options.Events.OnRedirectToLogin = context =>
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                }
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
            options.ClientId = "367131133436-9ulnv933benc9d4cl4v85ddiaps28ped.apps.googleusercontent.com";
            options.ClientSecret = "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";
            options.CallbackPath = "/signin-google";
            options.SaveTokens = true;
            options.UsePkce = true;
            options.Events.OnCreatingTicket = context =>
            {
                return Task.CompletedTask;
            };
            options.Events.OnTicketReceived = context =>
            {
                // Redirect to profile page after successful authentication
                context.ReturnUri = "/api/auth/GoogleResponse";
                return Task.CompletedTask;
            };
            options.Events.OnRemoteFailure = context =>
            {
                context.Response.Redirect("/login?error=Google authentication failed");
                context.HandleResponse();
                return Task.CompletedTask;
            };
        });

        services.Configure<EncryptionSettings>(
            Configuration.GetSection("EncryptionSettings"));

        services.AddControllers();

        services.AddSingleton<INotificationService, NotificationService>();
        services.AddScoped<IFileUploadService, FileUploadService>();
    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}