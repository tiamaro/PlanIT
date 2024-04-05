using Microsoft.IdentityModel.Tokens;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlanIT.API.Services.AuthenticationService;

public class AuthenticationService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthenticationService> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<User?> AuthenticateUserAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user != null && VerifyPasswordHash(password, user.HashedPassword, user.Salt))
        {
            // Authentication successful, return the user object
            return user;
        }

        // Authentication failed, return null
        return null;
    }


    public async Task<string> GenerateJwtTokenAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSecret = _configuration["Jwt:Secret"]; // Retrieve JWT key from configuration
        _logger.LogInformation("JWT Secret Key for Token Generation: {JwtSecret}", jwtSecret);

        var key = Encoding.ASCII.GetBytes(jwtSecret); // Convert to byte array directly

        // Use the entire secret key for creating SymmetricSecurityKey
        var symmetricKey = new SymmetricSecurityKey(key);

        var issuer = _configuration["Jwt:Issuer"]; // Retrieve issuer from configuration
        var audience = _configuration["Jwt:Audience"]; // Retrieve audience from configuration

        if (!int.TryParse(_configuration["Jwt:ExpiryInMinutes"], out int expirationMinutes))
        {
            expirationMinutes = 60; // Default token expiration time
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Use ID as identifier
            new Claim(ClaimTypes.Email, user.Email) // Include email in claims

            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes), // Set token expiration time
            Issuer = issuer, // Set token issuer
            Audience = audience, // Set audience
            SigningCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature) // Set token signing with the full key
        };

        var token = tokenHandler.CreateToken(tokenDescriptor); // Create JWT token
        return await Task.FromResult(tokenHandler.WriteToken(token)); // Return JWT token as string
    }

    // Hjelpemetode for å verifisere passordhash
    private bool VerifyPasswordHash(string password, string storedHash, string salt)
    {
        var computedHash = HashPassword(password, salt);
        return storedHash == computedHash;
    }

    // Hjelpemetode for å hashe passord
    private string HashPassword(string password, string salt)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }
}