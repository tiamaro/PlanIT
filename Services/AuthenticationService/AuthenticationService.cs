using Microsoft.IdentityModel.Tokens;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlanIT.API.Services.AuthenticationService;

// Service responsible for user authentication and JWT token generation.
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

        // Checks if user was found and password matches the hashed password in the database.
        if (user != null && VerifyPasswordHash(password, user.HashedPassword))
        {
            // Authentication successful
            return user;
        }

        // Authentication failed.
        return null;
    }


    // Generates a JWT token for an authenticated user.
    // This method is synchronous but returns a Task to conform to an asynchronous interface.
    public Task<string> GenerateJwtTokenAsync(User user)
    {
        // Creates a new instance of JwtSecurityTokenHandler to handle the creation of the JWT token.
        var tokenHandler = new JwtSecurityTokenHandler();

        // Retrieves the secret key for JWT
        var jwtSecret = _configuration["JwtSecret"];

        if (string.IsNullOrEmpty(jwtSecret))
        {
            _logger.LogError("JWT Secret is not configured correctly.");
            throw new InvalidOperationException("JWT Secret is not configured correctly.");
        }

        _logger.LogInformation("Starting JWT token generation for user: {UserId}", user.Id);

        // Converts the secret key into a byte array and creates a SymmetricSecurityKey.
        var key = Encoding.ASCII.GetBytes(jwtSecret);
        var symmetricKey = new SymmetricSecurityKey(key);

        // Retrieves information about the token issuer, audience, and expiration time from the configuration.
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpiryInMinutes", 60);


        // Sets up the token description with necessary information such as the user's ID and email,
        // as well as expiration time, issuer, audience, and signature settings.
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        }),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature)
        };

        // Creates the token and returns it.
        var token = tokenHandler.CreateToken(tokenDescriptor);
        _logger.LogInformation("JWT token generated for user: {UserId}", user.Id);

        return Task.FromResult(tokenHandler.WriteToken(token));
    }



    // Verifies a password against a stored hash.
    private bool VerifyPasswordHash(string password, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }

}