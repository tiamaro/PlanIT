using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

// UsersController - API Controller for brukerhåndtering:
// - Kontrolleren håndterer alle forespørsler relatert til brukerdata, inkludert registrering,
//   oppdatering, sletting og henting av brukerinformasjon. Den tar imot en instans av IUserService
//   som en del av konstruktøren for å utføre operasjoner relatert til brukere.
//
// Policy:
// - "Bearer": Krever at alle kall til denne kontrolleren er autentisert med et gyldig JWT-token
//   som oppfyller kravene definert i "Bearer" autentiseringspolicy. Dette sikrer at bare
//   autentiserte brukere kan aksessere endepunktene definert i denne kontrolleren.
//
// HandleExceptionFilter:
// - Dette filteret er tilknyttet kontrolleren for å fange og behandle unntak på en sentralisert måte.
//
// Forespørsler som starter med "api/v1/Users" vil bli rutet til metoder definert i denne kontrolleren.

[Authorize(Policy = "Bearer")]
[Route("api/v1/[controller]")]
[ApiController]
[ServiceFilter(typeof(HandleExceptionFilter))]  // Bruker HandleExceptionFilter for å håndtere unntak
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }


    // Endepunkt for registrering av ny bruker
    // POST /api/v1/Users/register
    [AllowAnonymous]
    [HttpPost("register", Name = "AddUser")]
    public async Task<ActionResult<UserDTO>> AddUserAsync(UserRegDTO userRegDTO)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddUserAsync");
            return BadRequest(ModelState);
        }

        // Registerer brukeren
        var userDTO = await _userService.RegisterUserAsync(userRegDTO);

        // Sjekk om brukerregistreringen var vellykket
        return userDTO != null 
            ? Ok(userDTO) 
            : BadRequest("Failed to register new user");
    }


    // !!!!!!!!!NB! FJERNE ELLER ADMIN RETTIGHETER????!!!!!!!!!
    //
    // Henter en liste over brukere
    // GET: /api/v1/Users?pageNr=1&pageSize=10
    [HttpGet(Name = "GetUsers")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsersAsync(int pageNr, int pageSize)
    {
        var users = await _userService.GetAllAsync(pageNr, pageSize);
        return Ok(users);
    }


    // !!!!!! NB! FJERNE ELLER ADMIN RETTIGHETER??? !!!!!!!!!!!!!!!
    //
    // Henter en bruker basert på brukerens ID
    // GET /api/v1/Users/1
    [HttpGet("{userId}", Name = "GetUsersById")]
    public async Task<ActionResult<UserDTO>> GetUsersByIdASync(int userId)
    {
        var user = await _userService.GetByIdAsync(userId);

        return user != null 
            ? Ok(user) 
            : NotFound("User not found");
    }


    // Henter informasjon om den innloggede brukeren ved å bruke en bruker-ID
    // hentet fra JWT-tokenet, som deretter er lagret i HttpContext.Items av middleware.
    [HttpGet("profile", Name = "GetUserProfile")]
    public async Task<ActionResult<UserDTO>> GetUserProfileAsync()
    {
        // Henter UserId fra HttpContext.Items som ble lagt til av middleware
        var userIdValue = HttpContext.Items["UserId"] as string;

        if (!int.TryParse(userIdValue, out var userId) || userId == 0)
        {
            return Unauthorized("Invalid user ID.");
        }

        var user = await _userService.GetByIdAsync(userId);

        return user != null
            ? Ok(user)
            : NotFound("User not found");
    }


    // Oppdaterer den innloggede brukerens informasjon ved å bruke bruker-ID
    // hentet fra JWT-tokenet som deretter er lagret i HttpContext.Items av middleware.
    // PUT /api/v1/Users/4
    [HttpPut(Name = "UpdateUser")]
    public async Task<ActionResult<UserDTO>> UpdateUserAsync(UserDTO updatedUserDTO)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userIdValue = HttpContext.Items["UserId"] as string;

        if (!int.TryParse(userIdValue, out var userId) || userId == 0)
        {
            return Unauthorized("Invalid user ID.");
        }

        // Prøver å oppdatere brukeren med den nye informasjonen
        var updatedUserResult = await _userService.UpdateAsync(userId, updatedUserDTO);

        // Returnerer oppdatert brukerdata, eller en feilmelding hvis oppdateringen mislykkes
        return updatedUserResult != null
            ? Ok(updatedUserResult)
            : NotFound("Unable to update the user");
    }


    // Sletter en bruker basert på brukerens ID hentet fra JWT-tokenet,
    // som deretter er lagret i HttpContext.Items av middleware.
    // DELETE /api/v1/Users
    [HttpDelete(Name = "DeleteUser")]
    public async Task<IActionResult> DeleteUserAsync()
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userIdValue = HttpContext.Items["UserId"] as string;

        if (!int.TryParse(userIdValue, out var userId) || userId == 0)
        {
            return Unauthorized("Invalid user ID."); 
        }

        var deletedUserResult = await _userService.DeleteAsync(userId);

        // Returnerer slettet brukerdata, eller en feilmelding hvis sletting mislykkes
        return deletedUserResult != null
            ? Ok(deletedUserResult)
            : BadRequest("Unable to delete user"); 
    }
}