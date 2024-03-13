namespace PlanIT.API.Extensions;

public static class WebAppExtensions
{
    // Metode for å registrere klasser ved hjelp av Dependency Injection
    public static void RegisterOpenGenericType(this WebApplicationBuilder builder, Type openGenericType)
    {
        var assembly = openGenericType.Assembly;

        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericType))
            .ToList();

        foreach (var type in types)
        {
            var interfaceType = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericType);

            if (interfaceType != null)
            {
                builder.Services.AddScoped(interfaceType, type);
            }
        }
    }
}