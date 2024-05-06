using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

// The AuthenticationController manages the authentication process for users.
// It supports login operations, handling user credentials to authenticate and generate JWT tokens.
// This controller is responsible for logging important information about the login attempts,
// successes, and failures. It also includes error handling to manage exceptions that occur during the login process.


[Route("api/v1/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticationController> _logger;
   

    public AuthenticationController(IAuthService authService, 
        ILogger<AuthenticationController> logger)
    {
        _authService = authService;
        _logger = logger;

    }


    [AllowAnonymous]
    [HttpPost("login", Name = "Login")]
    public async Task<IActionResult> LoginAsync(UserLoginDTO userLoginDTO)
    {
        _logger.LogInformation("Login attempt for user {userEmail}", userLoginDTO.Email);

        try
        {
            // Authenticate the user.
            var user = await _authService.AuthenticateUserAsync(userLoginDTO.Email, userLoginDTO.Password);


            // Check if user authentication was successful.
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            // Generate a JWT token for the authenticated user.
            var token = await _authService.GenerateJwtTokenAsync(user);

            _logger.LogInformation("Successful login for user {userEmail}", userLoginDTO.Email);


            // Return the generated token as a response.
            return Ok(new { Token = token });

        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred during user login: {ex.Message}");
            return StatusCode(500, "An error occurred during user login");
        }
    }
}