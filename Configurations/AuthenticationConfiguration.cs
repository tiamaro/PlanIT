using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace PlanIT.API.Configurations;

public static class AuthenticationConfiguration
{
    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration, Serilog.ILogger logger)
    {
        var jwtSecret = configuration["Jwt:Secret"];
        logger.Information($"JWT Secret Key for Token Validation: {jwtSecret}");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"], // Set issuer from configuration
                ValidAudience = configuration["Jwt:Audience"], // Set expected audience
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)) // Set the key for validating the signature
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    logger.Error("Authentication failed: {ErrorMessage}", context.Exception.Message);
                    // Log the token causing authentication failure
                    logger.Debug("Token: {Token}", context.Request.Headers["Authorization"]);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    logger.Information("Token validated: {SecurityToken}", context.SecurityToken);
                    // Log the claims extracted from the token
                    logger.Debug("Claims: {Claims}", context.Principal.Claims);
                    return Task.CompletedTask;
                }
            };
        });
    }
}