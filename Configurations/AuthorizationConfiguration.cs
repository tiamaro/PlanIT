using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace PlanIT.API.Configurations;

public static class AuthorizationConfiguration
{
    // En utvidelsesmetode for IServiceCollection for å konfigurere autorisasjonstjenester.
    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        // Legger til autorisasjonskonfigurasjoner.
        services.AddAuthorization(options =>
        {
            // Definerer en ny autorisasjonspolicy med navnet "Bearer".
            options.AddPolicy("Bearer", policy =>
            {
                // Spesifiserer at denne policyen bruker JwtBearerAuthenticationScheme.
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);

                // Forespørsler som passerer gjennom endepunkter som bruker denne policyen,
                // må inneholde en gyldig JWT-token for å få tilgang.
                policy.RequireAuthenticatedUser();

            });
        });
    }
}