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

    public AuthenticationService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<User?> AuthenticateUserAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user != null && VerifyPasswordHash(password, user.HashedPassword, user.Salt))
        {
            // Bruker autensisering er vellykket
            return user;
        }

        // Autentisering feilet
        return null;
    }

    // Metode for å generere JWT-token 
    public async Task<string> GenerateJwtTokenAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSecret = _configuration["Jwt:Secret"] ?? "secret_key"; // Henter JWT-key fra konfigurasjonen eller bruker standardkey
        var keySizeInBits = int.Parse(_configuration["Jwt:KeySizeInBits"]); // Henter nøkkelstørrelsen fra konfigurasjonen
        var key = Encoding.ASCII.GetBytes(jwtSecret); // Konverterer til byte-array

        // Opprett SymmetricSecurityKey med riktig nøkkelstørrelse
        var symmetricKey = new SymmetricSecurityKey(key.Take(keySizeInBits / 8).ToArray());

        var issuer = _configuration["Jwt:Issuer"]; // Henter utsteder fra konfigurasjonen

        if (!int.TryParse(_configuration["Jwt:ExpiryInMinutes"], out int expirationMinutes))
        {
            expirationMinutes = 60; // Standard utløpstid for token
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Email) // Bruk e-post som identifikator
            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes), // Angi tokenets utløpstid
            Issuer = issuer, // Angi tokenets utsteder
            SigningCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature) // Angi signering av tokenet
        };

        var token = tokenHandler.CreateToken(tokenDescriptor); // Opprett JWT-token
        return await Task.FromResult(tokenHandler.WriteToken(token)); // Returner JWT-token som streng
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