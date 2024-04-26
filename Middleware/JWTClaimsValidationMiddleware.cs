using System.Security.Claims;
namespace PlanIT.API.Middleware;

// Middleware to validate JWT claims in the HTTP context for authenticated requests.
public class JWTClaimsValidationMiddleware
{
    private readonly RequestDelegate _next;

    public JWTClaimsValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }


    // Invokes the middleware to check for required JWT claims in the user's identity.
    public async Task Invoke(HttpContext context)
    {
        // Only check for authenticated requests
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            // Define required claims that must be present in the token
            var requiredClaimTypes = new[] { ClaimTypes.NameIdentifier, ClaimTypes.Email };

            // Check if all required claim types are present in the authenticated user's claims
            bool hasAllRequiredClaims = requiredClaimTypes.All(requiredClaimType =>
                context.User.HasClaim(c => c.Type == requiredClaimType));


            // If not all required claims are present, deny access and return a 403 Forbidden status
            if (!hasAllRequiredClaims)
            {
                context.Response.StatusCode = 403; 
                await context.Response.WriteAsync("Access Denied. User does not have the required claims.");
                return;
            }

            // If the necessary claims are present, retrieve and store the UserId claim for later use
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                context.Items["UserId"] = userIdClaim.Value;
            }
        }

        // Continue processing if the user is not authenticated or if all required claims are valid
        await _next(context);
    }
}