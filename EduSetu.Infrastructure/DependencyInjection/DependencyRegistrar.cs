using EduSetu.Application.Common.Behaviors;
using EduSetu.Application.Common.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using EduSetu.Infrastructure.Data.Contexts;
using EduSetu.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EduSetu.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering infrastructure services
/// </summary>
public static class DependencyRegistrar
{
    /// <summary>
    /// Registers infrastructure services and dependencies
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Get database settings from configuration
        IConfigurationSection databaseSettings = configuration.GetSection("DatabaseSettings");
        bool enableSensitiveDataLogging = bool.TryParse(databaseSettings["EnableSensitiveDataLogging"], out var sdl) ? sdl : false;
        bool enableDetailedErrors = bool.TryParse(databaseSettings["EnableDetailedErrors"], out var de) ? de : false;
        int commandTimeout = int.TryParse(databaseSettings["CommandTimeout"], out var ct) ? ct : 30;
        int maxRetryCount = int.TryParse(databaseSettings["MaxRetryCount"], out var mrc) ? mrc : 3;
        string maxRetryDelayString = databaseSettings["MaxRetryDelay"] ?? "00:00:30";
        TimeSpan maxRetryDelay = TimeSpan.TryParse(maxRetryDelayString, out var mrd) ? mrd : TimeSpan.FromSeconds(30);

        // Register DbContext with SQL Server and enhanced configuration
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), sqlServerOptions =>
            {
                sqlServerOptions.CommandTimeout(commandTimeout);
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: maxRetryCount,
                    maxRetryDelay: maxRetryDelay,
                    errorNumbersToAdd: null);

                // Enable advanced SQL Server features
                sqlServerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            if (enableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }

            if (enableDetailedErrors)
            {
                options.EnableDetailedErrors();
            }

            // Add query logging in development
            if (System.Diagnostics.Debugger.IsAttached)
            {
                options.LogTo(Console.WriteLine, LogLevel.Information);
            }
        });

        // Register context interface
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        // Register application services
        services.AddScoped<EduSetu.Application.Common.Interfaces.IPasswordEncryptionService, PasswordEncryptionService>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Get Application assembly reference for automatic service registration
        Assembly applicationAssembly = typeof(EduSetu.Application.Features.Authentication.Request.LoginRequest).Assembly;

        // Automatically register all MediatR handlers from Application assembly
        // This will find all Command, Query, and Request handlers across all Features folders
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
        });

        // Register pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Register FluentValidation - scan entire Application assembly
        services.AddValidatorsFromAssembly(applicationAssembly);

        // Automatically register all Repository classes from Features/*/Infrastructure folders
        // Note: Using TryAddScoped instead of Scan for better compatibility
        var repositoryTypes = applicationAssembly.GetTypes()
            .Where(type => type.Namespace != null &&
                   type.Namespace.Contains(".Infrastructure") &&
                   type.Name.Equals("Repository", StringComparison.OrdinalIgnoreCase) &&
                   !type.IsAbstract && !type.IsInterface)
            .ToList();

        foreach (var repositoryType in repositoryTypes)
        {
            services.TryAddScoped(repositoryType);
        }

        // Note: Request handlers in Features/*/Requests are already auto-registered by MediatR above
        // MediatR automatically discovers and registers all IRequestHandler implementations

        return services;
    }
}
