using Microsoft.OpenApi.Models;
using System.Reflection;

namespace PlanIT.API.Extensions;

public static class WebAppExtensions
{

    // Method for registering Services in Dependency Injection
    public static void RegisterServicesFromConfiguration(this IServiceCollection services, Assembly assembly, IConfiguration configuration)
    {
        // Load namespaces and exclusions from configuration
        var namespaces = configuration.GetSection("DependencyInjection:Namespaces").Get<string[]>() ?? [];
        var exclusions = configuration.GetSection("DependencyInjection:Exclusions").Get<string[]>() ?? [];

        // Process each type and interface based on the loaded namespaces and exclusions
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToList();

        foreach (var type in types)
        {
            if (exclusions.Contains(type.Name))
            {
                Console.WriteLine($"Skipped registration of {type.Name}");
                continue;
            }

            if (!namespaces.Any(ns => type.Namespace?.Contains(ns) == true))
                continue;

            // Register each interface implemented by the class that is not system-defined.
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.Namespace == null || interfaceType.Namespace.StartsWith("System"))
                    continue;

                services.AddScoped(interfaceType, type);
                Console.WriteLine($"Registered {interfaceType} to {type}");
            }
        }
    }


    // Retrieves and validates a user ID from HttpContext; throws an exception if invalid.
    public static int GetValidUserId(HttpContext httpContext)
    {
        var userIdValue = httpContext.Items["UserId"] as string;
        if (int.TryParse(userIdValue, out var userId) && userId != 0)
        {
            return userId;
        }
        throw new ArgumentException("Invalid user ID.");
    }


    // Configures Swagger to include JWT authentication for secure API documentation and testing.
    public static void AddSwaggerWithJwtAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
        });
    }
}