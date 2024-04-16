using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;


namespace PlanIT.API.Controllers;

// ShoppinListController - API Controller for handlelistehåndtering:
// - Kontrolleren håndterer alle forespørsler relatert til handlelistedata, inkludert registrering,
//   oppdatering, sletting og henting av handlelisteinformasjon. Den tar imot en instans av IService
//   som en del av konstruktøren for å utføre operasjoner relatert til handleliste.
//
// Policy:
// - "Bearer": Krever at alle kall til denne kontrolleren er autentisert med et gyldig JWT-token
//   som oppfyller kravene definert i "Bearer" autentiseringspolicy. Dette sikrer at bare
//   autentiserte brukere kan aksessere endepunktene definert i denne kontrolleren.
//
// HandleExceptionFilter:
// - Dette filteret er tilknyttet kontrolleren for å fange og behandle unntak på en sentralisert måte.
//
// Forespørsler som starter med "api/v1/ShoppingList" vil bli rutet til metoder definert i denne kontrolleren.


[Authorize(Policy = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
[ServiceFilter(typeof(HandleExceptionFilter))]  // Bruker HandleExceptionFilter for å håndtere unntak
public class ShoppingListController : ControllerBase
{
    private readonly ILogger<ShoppingListController> _logger;
    private readonly IService<ShoppingListDTO> _shoppingListService;

    public ShoppingListController(ILogger<ShoppingListController> logger, 
        IService<ShoppingListDTO> shoppingListService)
    {
        _logger = logger;
        _shoppingListService = shoppingListService;
    }

    // Endepunkt for registrering av ny handleliste
    // POST /api/v1/ShoppinList/register
    [HttpPost("register", Name = "AddShoppingList")]
    public async Task<IActionResult> AddShoppingListAsync(ShoppingListDTO newShoppingListDto)
    {
        // Sjekk om modelltilstanden er gyldig etter modellbinding og validering
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddEventAsync");
            return BadRequest(ModelState);
        }

        // Registrer handlelisten
        var addedShoppingList = await _shoppingListService.CreateAsync(newShoppingListDto);

        // Sjekk om handlelisteregistreringen var vellykket
        return addedShoppingList != null
            ? Ok(addedShoppingList)
            : BadRequest("Failed to register new shoppingList");
    }


    // !!!!!! NB! FJERNE ELLER ADMIN RETTIGHETER??? !!!!!!!!!!!!!!!
    //
    // Henter en liste over handlelister
    // GET: /api/v1/ShoppingList?pageNr=1&pageSize=10
    [HttpGet(Name = "GetShoppingLists")]
    public async Task<ActionResult<IEnumerable<ShoppingListDTO>>> GetShoppingListsAsync(int pageNr, int pageSize)
    {
        var allShoppingLists = await _shoppingListService.GetAllAsync(pageNr, pageSize);

        return allShoppingLists != null
            ? Ok(allShoppingLists)
            : NotFound("No registered shoppingList found.");
    }


    // Henter arrangementet basert på shoppingListId
    // GET /api/v1/ShoppingList/1
    [HttpGet("{shoppingListId}", Name = "GetShoppingListById")]
    public async Task<ActionResult<ShoppingListDTO>> GetShoppingListByIdAsync(int shoppingListId)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Hent handleliste fra tjenesten, filtrert etter brukerens ID
        var existingShoppingList = await _shoppingListService.GetByIdAsync(userId, shoppingListId);

        return existingShoppingList != null
            ? Ok(existingShoppingList)
            : NotFound("ShoppingList not found");
    }


    // Oppdaterer handleliste basert på shoppingListID.
    // PUT /api/v1/ShoppingList/4
    [HttpPut("{shoppingListId}", Name = "UpdateShoppingList")]
    public async Task<ActionResult<ShoppingListDTO>> UpdateShoppingListAsync(int shoppingListId, ShoppingListDTO updatedShoppingListDTO)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Prøver å oppdatere handlelisten med den nye informasjonen
        var updatedShoppingListResult = await _shoppingListService.UpdateAsync(userId, shoppingListId, updatedShoppingListDTO);

        // Returnerer oppdatert handleliste, eller en feilmelding hvis oppdateringen mislykkes
        return updatedShoppingListResult != null
            ? Ok(updatedShoppingListResult)
            : NotFound("Unable to update the shoppingList or the shoppingList does not belong to the user");
    }


    // Sletter en handleliste basert på handlelistens ID
    // DELETE /api/v1/ShoppingList/2
    [HttpDelete("{shoppingListId}", Name = "DeleteShoppingList")]
    public async Task<ActionResult<ShoppingListDTO>> DeleteShoppingListAsync(int shoppingListId)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Prøver å slette handlelisten
        var deletedShoppingListResult = await _shoppingListService.DeleteAsync(userId, shoppingListId);

        return deletedShoppingListResult != null
            ? Ok(deletedShoppingListResult)
            : BadRequest("Unable to delete shoppinglist or shoppinglist does not belong to the user");
    }
}