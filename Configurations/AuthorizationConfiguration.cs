using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace PlanIT.API.Configurations;

public static class AuthorizationConfiguration
{
    // An extension method for IServiceCollection to configure authorization services.
    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        // Adds authorization configurations.
        services.AddAuthorization(options =>
        {
            // Defines a new authorization policy named "Bearer".
            options.AddPolicy("Bearer", policy =>
            {
                // Specifies that this policy uses JwtBearerAuthenticationScheme.
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);

                // Requests passing through endpoints using this policy
                // must contain a valid JWT token to gain access.
                policy.RequireAuthenticatedUser();

            });
        });
    }
}