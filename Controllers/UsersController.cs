using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;
using System.Security.Claims;

namespace PlanIT.API.Controllers;

// Kontrolleren for API-versjon 1 som definerer adressen (URL) til API-et.
// [ApiController] indikerer at klassen fungerer som en kontroller i systemet og arver fra ControllerBase.
// Tar imot en UserService-innstans som en del av konstruktøren for å utføre brukerrelaterte operasjoner.
// api/v1/Users

[Authorize(Policy = "Bearer")]
[Route("api/v1/[controller]")]
[ApiController]
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
        try
        {
            // Sjekk om modelltilstanden er gyldig etter modellbinding og validering
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state in AddUserAsync");

                // Hvis modelltilstanden er ugyldig, returner en BadRequest sammen med ModelState-feilene
                return BadRequest(ModelState);
            }

            // Registrer brukeren ved å bruke de oppgitte detaljene for brukerregistrering
            var userDTO = await _userService.RegisterUserAsync(userRegDTO);

            // Sjekk om brukerregistreringen var vellykket
            return userDTO != null
                ? Ok(userDTO)
                : BadRequest("Failed to register new user");
        }
        catch (Exception ex) // Generell Exception-håndtering for uventede feil
        {
            _logger.LogError("An unknown error occured: " + ex.Message);
            return StatusCode(500, "An unknown error occured, please try again later");
        }
    }


    // Henter en liste over brukere i form av UserDTO-objekter med paginering.
    // Parametrene 'pageNr' og 'pageSize' angir henholdsvis sidenummer og størrelse på siden.
    // Returnerer en ActionResult med en IEnumerable av USerDTO-objekter.
    // GET: /api/v1/Users?pageNr=1&pageSize=10
    [HttpGet(Name = "GetUsers")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsersAsync(int pageNr, int pageSize)
    {
        var users = await _userService.GetAllAsync(pageNr, pageSize);

        return users != null
            ? Ok(users)
            : NotFound("No registered users found.");
    }


    // Henter en bruker basert på brukerens ID ved hjelp av UserService.
    // Returnerer en ActionResult med en UserDTO hvis brukeren ble funnet,
    // ellers returneres NotFound hvis brukeren ikke ble funnet.
    // GET /api/v1/Users/1
    [HttpGet("{userId}", Name = "GetUsersById")]
    public async Task<ActionResult<UserDTO>> GetUsersByIdASync(int userId)
    {
        // Hent bruker fra tjenesten
        var user = await _userService.GetByIdAsync(userId);

        return user != null
            ? Ok(user)
            : NotFound("User not found");
    }


    // Oppdaterer informasjonen for den innloggede brukeren basert på data fra en JWT-token.
    // Brukerens unike ID hentes fra den autentiserte brukerens claims i JWT-tokenet,
    // og sikrer dermed at brukere kun kan oppdatere sin egen informasjon.
    // PUT /api/v1/Users/4
    [HttpPut(Name = "UpdateUser")]
    public async Task<ActionResult<UserDTO>> UpdateUserAsync(UserDTO updatedUserDTO)
    {
        // Hent brukerens unike ID fra de autentiserte brukerens claims
        var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Sjekk om brukerens ID ble funnet i tokenet
        if (!int.TryParse(userIdValue, out var userId))
        {
            _logger.LogError("Invalid or missing user ID in JWT token.");
            return Unauthorized("Invalid user ID.");
        }

        // Hent bruker fra tjenesten
        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning($"User with ID {userId} not found.");
            return NotFound("User not found");
        }

        var updatedUserResult = await _userService.UpdateAsync(userId, updatedUserDTO);
        return updatedUserResult != null ? Ok(updatedUserResult) : NotFound("Unable to update the user");
       
    }


    // Sletter en bruker basert på brukerens ID ved å bruke UserService.
    // Returnerer en ActionResult med en slettet UserDTO hvis slettingen var vellykket, 
    // ellers returneres BadRequest hvis bruker ikke kunne slettes.
    // DELETE /api/v1/Users/2
    [HttpDelete("{userId}", Name = "DeleteUser")]
    public async Task<ActionResult<UserDTO>> DeleteUserAsync(int userId)
    {
        // Hent bruker fra tjenesten
        var user = await _userService.GetByIdAsync(userId);

        // Sjekk om bruker eksisterer
        if (user == null) return NotFound("User not found");

        try
        {
            // Hvis bruker er riktig, fortsett med slettingen.
            var deletedUSerResult = await _userService.DeleteAsync(userId);
            return deletedUSerResult != null
                ? Ok(deletedUSerResult)
                : BadRequest("Unable to delete user");
        }
        catch (Exception ex) // Generell Exception-håndtering for uventede feil
        {
            _logger.LogError("An unknown error occured: " + ex.Message);
            return StatusCode(500, "An unknown error occured, please try again later");
        }
    }
}
