using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace PlanIT.API.Configurations;

public static class AuthenticationConfiguration
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration, Serilog.ILogger logger)
    {
        // Henter og validerer den hemmelige nøkkelen for JWT fra secret manager.
        var jwtSecret = configuration["JwtSecret"];
        if (string.IsNullOrEmpty(jwtSecret))
        {
            logger.Error("JWT Secret Key is not configured correctly.");
            throw new InvalidOperationException("JWT Secret Key is not configured.");
        }
        logger.Information("JWT configuration validated successfully.");

        // Konverterer den hemmelige nøkkelen til en byte-array.
        var key = Encoding.ASCII.GetBytes(jwtSecret);

        // Konfigurerer autentiseringstjenester med JWT Bearer som standard.
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key) // Angir signatur-nøkkelen.
            };

            // Hendelser for autentisering. Logging ved feil eller vellykket validering.
            options.Events = new JwtBearerEvents
            {
                //// Tilpasset hendelse for å hente token fra cookie
                //OnMessageReceived = context =>
                //{
                //    // Henter token fra en cookie i stedet for standard header
                //    context.Token = context.Request.Cookies["jwtToken"];
                //    return Task.CompletedTask;
                //},

                OnAuthenticationFailed = context =>
                {
                    logger.Error("Authentication failed: {ErrorMessage}", context.Exception?.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var claimsCount = context.Principal?.Claims.Count() ?? 0;
                    logger.Information("Token validated successfully with {ClaimsCount} claims.", claimsCount);
                    return Task.CompletedTask;
                }
            };
        });
    }
}