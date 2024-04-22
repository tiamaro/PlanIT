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
            // Krav til claims som må være til stede
            var requiredClaimTypes = new[] { ClaimTypes.NameIdentifier, ClaimTypes.Email };

            // Sjekk om alle påkrevde claim-typer er til stede for den autentiserte brukeren
            bool hasAllRequiredClaims = requiredClaimTypes.All(requiredClaimType =>
                context.User.HasClaim(c => c.Type == requiredClaimType));

            if (!hasAllRequiredClaims)
            {
                context.Response.StatusCode = 403; // Forbudt
                await context.Response.WriteAsync("Access Denied. User does not have the required claims.");
                return;
            }

            // Hvis de nødvendige claims er til stede, hent og lagre UserId-claim for senere bruk
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                context.Items["UserId"] = userIdClaim.Value;
            }
        }

        await _next(context);
    }
}