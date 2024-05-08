using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace PlanIT.API.Configurations;

public static class AuthenticationConfiguration
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration, Serilog.ILogger logger)
    {

        // Fetches and validates the secret key for JWT from the secret manager.
        var jwtSecret = configuration["JwtSecret"] ?? Environment.GetEnvironmentVariable("JWT_SECRET_FILE");
        if (string.IsNullOrEmpty(jwtSecret))
        {
            logger.Error("JWT Secret Key is not configured correctly.");
            throw new InvalidOperationException("JWT Secret Key is not configured.");
        }
        logger.Information("JWT configuration validated successfully.");

        // Converts the secret key to a byte array.
        var key = Encoding.ASCII.GetBytes(jwtSecret);

        // Configures authentication services with JWT Bearer as the default.
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
                IssuerSigningKey = new SymmetricSecurityKey(key) 
            };

            // Logging in case of failure or successful validation.
            options.Events = new JwtBearerEvents
            {
                
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