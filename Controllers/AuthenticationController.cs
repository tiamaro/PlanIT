using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

// Denne kontrolleren håndterer brukerinnlogging og generering av JWT (JSON Web Token) for autentiserte brukere.

[Route("api/v1/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticationController> _logger;
    private readonly IWebHostEnvironment _environment;

    public AuthenticationController(IAuthService authService, 
        ILogger<AuthenticationController> logger,
        IWebHostEnvironment environment)
    {
        _authService = authService;
        _logger = logger;
        _environment = environment;
    }

    [AllowAnonymous]
    [HttpPost("login", Name = "Login")]
    public async Task<IActionResult> LoginAsync(UserLoginDTO userLoginDTO)
    {
        _logger.LogInformation("Login attempt for user {userEmail}", userLoginDTO.Email);

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

            _logger.LogInformation("Successful login for user {userEmail}", userLoginDTO.Email);

            //var cookieOptions = new CookieOptions
            //{
            //    HttpOnly = true,
            //    Secure = true,
            //    SameSite = SameSiteMode.Strict,
            //    Expires = DateTime.UtcNow.AddDays(7)
            //};

            //Response.Cookies.Append("jwtToken", token, cookieOptions);

            //// return Ok(new { Message = "Login successful" });

            return Ok(new { Token = token });

        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred during user login: {ex.Message}");
            return StatusCode(500, "An error occurred during user login");
        }
    }
}