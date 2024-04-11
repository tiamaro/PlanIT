using System.Security.Claims;

namespace PlanIT.API.Middleware;

public class JWTClaimsValidationMiddleware
{
    private readonly RequestDelegate _next;

    public JWTClaimsValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Sjekker kun for autentiserte forespørsler
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            // Claims som må være med
            var requiredClaimTypes = new[] { ClaimTypes.NameIdentifier, ClaimTypes.Email };


            // Sjekker om alle påkrevde claim-types finnes for den autentiserte brukeren
            bool hasAllRequiredClaims = requiredClaimTypes.All(requiredClaimType =>
                context.User.HasClaim(c => c.Type == requiredClaimType));

            if (!hasAllRequiredClaims)
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Access Denied. User does not have the required claims.");
                return;
            }
        }

        await _next(context);
    }
}
