using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlanIT.API.Middleware;

public class JwtAuthMiddleware : IMiddleware
{
    private readonly IConfiguration _configuration;

    public JwtAuthMiddleware(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            await AttachUserToContext(context, token);
        }

        await next(context);
    }

    private async Task AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecret = _configuration["Jwt:Secret"] ?? "default_secret";
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            // Hent nøkkelstørrelsen fra konfigurasjonen
            var keySizeInBitsString = _configuration["Jwt:KeySizeInBits"];
            var keySizeInBits = !string.IsNullOrEmpty(keySizeInBitsString) ? int.Parse(keySizeInBitsString) : 128; // Endret standardverdien til 128

            var symmetricKey = new SymmetricSecurityKey(key.Take(keySizeInBits / 8).ToArray());

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                IssuerSigningKey = symmetricKey // Bruk den opprettede SymmetricSecurityKey
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userEmail = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value; // Hent ut brukerens e-post

            // Legg til brukeren i konteksten ved vellykket validering
            context.Items["User"] = userEmail; // Lagre brukerens e-post i konteksten
        }
        catch (Exception)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
        }
    }
}