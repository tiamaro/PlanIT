using Microsoft.OpenApi.Models;
using System.Reflection;

namespace PlanIT.API.Extensions;

public static class WebAppExtensions
{

    // Metode for å registrere Services i Dependency Injection
    public static void RegisterServicesFromConfiguration(this IServiceCollection services, Assembly assembly, IConfiguration configuration)
    {
        // Last namespaces og exclusions fra konfigurasjon
        var namespaces = configuration.GetSection("DependencyInjection:Namespaces").Get<string[]>() ?? [];
        var exclusions = configuration.GetSection("DependencyInjection:Exclusions").Get<string[]>() ?? [];

        // // Behandle hver type og interface basert på de innlastede namespaces og exclusions
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

            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.Namespace == null || interfaceType.Namespace.StartsWith("System"))
                    continue;

                services.AddScoped(interfaceType, type);
                Console.WriteLine($"Registered {interfaceType} to {type}");
            }
        }
    }


    // Metode for å hente og validere brukerID fra HttpContext
    public static int GetValidUserId(HttpContext httpContext)
    {
        var userIdValue = httpContext.Items["UserId"] as string;
        if (int.TryParse(userIdValue, out var userId) && userId != 0)
        {
            return userId;
        }
        throw new ArgumentException("Invalid user ID.");
    }


    // Metode for å konfigurere Swagger med json web token autentisering
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