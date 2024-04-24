using Microsoft.IdentityModel.Tokens;
using PlanIT.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlanIT.API.Services.AuthenticationService;

public class JWTEmailAuth : IJWTEmailAuth
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;


    public JWTEmailAuth(IConfiguration configuration)
    {
        _secretKey = configuration["JwtEmailSecret"];
        _issuer = configuration["Jwt:Issuer"];
        _audience = configuration["Jwt:Audience"];
    }


    public string GenerateJwtToken(int inviteId, int eventId)
    {
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

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public ClaimsPrincipal DecodeToken(string token)
    {
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
                ClockSkew = TimeSpan.Zero  // Remove default clock skew of 5 minutes
            }, out SecurityToken validatedToken);

            return principal;  // Correctly return the ClaimsPrincipal obtained from ValidateToken
        }
        catch (Exception ex)
        {
            // Handle or log the exception as appropriate
            throw new SecurityTokenValidationException("Token validation failed.", ex);
        }
    }

   
}
