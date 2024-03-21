using Microsoft.OpenApi.Models;

namespace PlanIT.API.Extensions;

public static class WebAppExtensions
{
    // Metode for å registrere klasser ved hjelp av Dependency Injection

    public static void RegisterOpenGenericTypeAndDerivatives(this WebApplicationBuilder builder, Type openGenericType)
    {
        var assembly = openGenericType.Assembly;

        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => IsDerivedFromInterface(i, openGenericType)))
            .ToList();

        foreach (var type in types)
        {
            var interfaceType = type.GetInterfaces()
                .FirstOrDefault(i => IsDerivedFromInterface(i, openGenericType));

            if (interfaceType != null)
            {
                builder.Services.AddScoped(interfaceType, type);
            }
        }
    }

    // Metode for å sjekke om grensesnittet eller noen av dets overordnede grensesnitt arver fra den angitte generiske typen.
    private static bool IsDerivedFromInterface(Type interfaceType, Type openGenericType)
    {
        return interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == openGenericType ||
               interfaceType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericType);
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