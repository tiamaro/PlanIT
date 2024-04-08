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

    // Autentiserer en bruker basert på e-post og passord.
    public async Task<User?> AuthenticateUserAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        // Sjekker om en bruker ble funnet og at passordet som ble oppgitt matcher brukerens lagrede passordhash.
        if (user != null && VerifyPasswordHash(password, user.HashedPassword, user.Salt))
        {
            // Autentiseringen var vellykket.
            return user;
        }

        // Autentiseringen feilet.
        return null;
    }


    // Metode for å generere en JWT (JSON Web Token) for en gitt bruker.
    // Denne metoden er synkron, men returnerer en Task for å tilpasse seg et asynkront grensesnitt.
    public Task<string> GenerateJwtTokenAsync(User user)
    {
        // Oppretter en ny instans av JwtSecurityTokenHandler for å håndtere opprettelsen av JWT-tokenet.
        var tokenHandler = new JwtSecurityTokenHandler();

        // Henter den hemmelige nøkkelen for JWT fra appsettings.
        var jwtSecret = _configuration["Jwt:Secret"];

        if (string.IsNullOrEmpty(jwtSecret))
        {
            _logger.LogError("JWT Secret is not configured correctly.");
            throw new InvalidOperationException("JWT Secret is not configured correctly.");
        }

        _logger.LogInformation("Starting JWT token generation for user: {UserId}", user.Id);

        // Konverterer den hemmelige nøkkelen til en byte-array, og oppretter SymmetricSecurityKey
        var key = Encoding.ASCII.GetBytes(jwtSecret);
        var symmetricKey = new SymmetricSecurityKey(key);

        // Henter informasjon om token-utsteder, mottaker og utløpstid fra konfigurasjonen.
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpiryInMinutes", 60);

        // Setter opp tokenbeskrivelsen med nødvendig informasjon som brukerens ID og e-post,
        // samt utløpstid, utsteder, mottaker og signaturinnstillinger.
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

        // Oppretter tokenet, og returnerer det.
        var token = tokenHandler.CreateToken(tokenDescriptor);
        _logger.LogInformation("JWT token generated for user: {UserId}", user.Id);

        return Task.FromResult(tokenHandler.WriteToken(token));
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