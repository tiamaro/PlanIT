using Microsoft.IdentityModel.Tokens;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlanIT.API.Services.AuthenticationService;
// Service to manage email-based JWT authentication for invite and event operations.
public class EmailAuthService : IEmailAuth
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly ILoggerServiceFactory _loggerFactory;


    public EmailAuthService(IConfiguration configuration, ILoggerServiceFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        var logger = _loggerFactory.CreateLogger();

        // Default JWT settings.
        const string DefaultSecretKey = "default_secret_key";
        const string DefaultIssuer = "default_issuer";
        const string DefaultAudience = "default_audience";

        // Retrieve configuration values and fallback to defaults if null
        _secretKey = configuration["JwtEmailSecret"] ?? DefaultSecretKey;
        _issuer = configuration["Jwt:Issuer"] ?? DefaultIssuer;
        _audience = configuration["Jwt:Audience"] ?? DefaultAudience;

        // Log a warning if using default values
        if (_secretKey == DefaultSecretKey || _issuer == DefaultIssuer || _audience == DefaultAudience)
        {
            logger.LogWarning("Using default values for JWT configuration. Check appsettings.json for missing values.");
        }
    }

    // Generates a JWT token containing invite and event identifiers.
    // Returns a JWT token string
    public string GenerateJwtToken(int inviteId, int eventId)
    {
        var logger = _loggerFactory.CreateLogger();

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim("inviteId", inviteId.ToString()),
            new Claim("eventId", eventId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddHours(24), 
            signingCredentials: credentials
        );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        
        logger.LogInfo("JWT email-token generated successfully.");

        return jwtToken;
    }

    // Decodes and validates a given JWT token, returning the associated claims principal.
    public ClaimsPrincipal DecodeToken(string token)
    {
        var logger = _loggerFactory.CreateLogger();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                ClockSkew = TimeSpan.Zero  
            }, out SecurityToken validatedToken);

            logger.LogInfo("JWT email-token validation successful.");

            // Correctly return the ClaimsPrincipal obtained from ValidateToken
            return principal;  
        }
        catch (Exception ex)
        {
            logger.LogError("JWT token validation failed.");
            throw new SecurityTokenValidationException("Token validation failed.", ex);
        }
    }

    // Validates the token and extracts the invite and event identifiers.
    public (int inviteId, int eventId) ValidateAndExtractClaims(string token)
    {
        var logger = _loggerFactory.CreateLogger();

        var principal = DecodeToken(token);
        var inviteIdClaim = principal.FindFirst("inviteId")?.Value;
        var eventIdClaim = principal.FindFirst("eventId")?.Value;

        if (inviteIdClaim == null || eventIdClaim == null)
        {
            logger.LogWarning("Invalid token claims.");
            throw new ArgumentException("Invalid token email-claims.");
        }

        logger.LogInfo("Token claims validated and extracted successfully.");

        return (int.Parse(inviteIdClaim), int.Parse(eventIdClaim));
    }

}