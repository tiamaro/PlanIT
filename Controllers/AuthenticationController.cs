using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

// Denne kontrolleren håndterer brukerinnlogging og generering av JWT (JSON Web Token) for autentiserte brukere.
// Ved å bruke [Authorize(Policy = "Bearer")], kreves det at alle handlinger i denne kontrolleren
// autentiseres med en gyldig JWT, med mindre [AllowAnonymous] er eksplisitt satt på en handling.

[Authorize(Policy = "Bearer")]
[Route("api/v1/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IAuthService authService, ILogger<AuthenticationController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("login", Name = "Login")]
    public async Task<IActionResult> LoginAsync(UserLoginDTO userLoginDTO)
    {
        try
        {
            // Autentiserer bruker
            var user = await _authService.AuthenticateUserAsync(userLoginDTO.Email, userLoginDTO.Password);

            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            // Genererer JWT token
            var token = await _authService.GenerateJwtTokenAsync(user);

            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred during user login: {ex.Message}");
            return StatusCode(500, "An error occurred during user login");
        }
    }
}