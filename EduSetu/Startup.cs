using EduSetu.Application.Common.Interfaces;
using EduSetu.Components;
using EduSetu.Infrastructure.Data.Contexts;
using EduSetu.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using EduSetu.Application.Common.Settings;
using EduSetu.Infrastructure.DependencyInjection;
using FluentValidation;
using EduSetu.Application.Features.Authentication;
using System.Security.Claims;

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
            .AddCircuitOptions(options => { options.DetailedErrors = true; });

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IPasswordEncryptionService, PasswordEncryptionService>();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(EduSetu.Application.Features.Authentication.Request.LoginRequest).Assembly));
        services.AddHttpContextAccessor();

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
            options.Events.OnSignedIn = context => Task.CompletedTask;
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
            options.CallbackPath = "/profile";
            options.SaveTokens = true;
        });

        services.Configure<EncryptionSettings>(
            Configuration.GetSection("EncryptionSettings"));

        services.AddInfrastructure(Configuration);
        services.AddApplicationServices();

        services.AddControllers();
        services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
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