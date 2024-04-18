using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;

// ImportantDateController - API Controller for håndtering av viktige datoer:
// - Kontrolleren håndterer alle forespørsler relatert til vikitge datoer, inkludert registrering,
//   oppdatering, sletting og henting av datoinformasjon. Den tar imot en instans av IService
//   som en del av konstruktøren for å utføre operasjoner relatert til viktige datoer.
//
// Policy:
// - "Bearer": Krever at alle kall til denne kontrolleren er autentisert med et gyldig JWT-token
//   som oppfyller kravene definert i "Bearer" autentiseringspolicy. Dette sikrer at bare
//   autentiserte brukere kan aksessere endepunktene definert i denne kontrolleren.
//
// HandleExceptionFilter:
// - Dette filteret er tilknyttet kontrolleren for å fange og behandle unntak på en sentralisert måte.
//
// Forespørsler som starter med "api/v1/ImportantDate" vil bli rutet til metoder definert i denne kontrolleren.


[Authorize(Policy = "Bearer")]
[Route("api/v1/[controller]")]
[ApiController]
[ServiceFilter(typeof(HandleExceptionFilter))]
public class ImportantDateController : ControllerBase
{
    private readonly IService<ImportantDateDTO> _dateService;
    private readonly ILogger<ImportantDateController> _logger;

    public ImportantDateController(IService<ImportantDateDTO> dateService,
        ILogger<ImportantDateController> logger)
    {
        _dateService = dateService;
        _logger = logger;
    }


    // Endepunkt for registrering av ny viktig dato
    // POST /api/v1/Events/register
    [HttpPost("register", Name = "AddImportantDate")]
    public async Task<ActionResult<ImportantDateDTO>> AddImportantDateAsync(ImportantDateDTO newImportantDateDTO)
    {
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Sjekk om modelltilstanden er gyldig etter modellbinding og validering
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddImportantDateAsync");
            return BadRequest(ModelState);
        }

        // Registrerer vikitg dato
        var addedImportantDate = await _dateService.CreateAsync(userId, newImportantDateDTO);

        return addedImportantDate != null
            ? Ok(addedImportantDate)
            : BadRequest("Failed to register new ImportantDate");

    }


    // !!!!!! NB! FJERNE ELLER ADMIN RETTIGHETER??? !!!!!!!!!!!!!!!
    //
    [HttpGet(Name = "GetImportantDates")]
    public async Task<ActionResult<IEnumerable<ImportantDateDTO>>> GetImportantDatesAsync(int pageNr, int pageSize)
    {
        var userId = WebAppExtensions.GetValidUserId(HttpContext);


        var allImportantDates = await _dateService.GetAllAsync(userId, pageNr, pageSize);

        return allImportantDates != null
            ? Ok(allImportantDates)
            : NotFound("No registered ImportantDates found.");

    }


    // Henter datoer basert på importantDateId
    // GET /api/v1/ImportantDates/1
    [HttpGet("{importantDateId}", Name = "GetImportantDatesById")]
    public async Task<ActionResult<ImportantDateDTO>> GetImportantDatesByIdAsync(int importantDateId)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Hent viktig dato fra tjenesten, filtrert etter brukerens ID
        var exsistingImportantDate = await _dateService.GetByIdAsync(userId, importantDateId);

        return exsistingImportantDate != null
            ? Ok(exsistingImportantDate)
            : NotFound("ImportantDate not found");
    }



    // Oppdaterer vikitg dato basert på importantDateId.
    // PUT /api/v1/ImportantDate/4
    [HttpPut("{importantDateId}", Name = "UpdateImportantDate")]
    public async Task<ActionResult<ImportantDateDTO>> updateImportantDateAsync(int importantDateId, ImportantDateDTO updatedImportantDateDTO)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Prøver å oppdatere arrangementet med den nye informasjonen
        var updatedImportantDate = await _dateService.UpdateAsync(userId, importantDateId, updatedImportantDateDTO);


        return updatedImportantDate != null
            ? Ok(updatedImportantDate)
            : NotFound("Unable to update the important date item or the item does not belong to the user");

    }


    // Sletter et vikitg dato basert på ID
    // DELETE /api/v1/ImportantDate/2
    [HttpDelete("{importantDateId}", Name = "DeleteImportantDate")]
    public async Task<ActionResult<ImportantDateDTO>> DeleteImportantDateAsync(int importantDateId)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Prøver å slette viktig dato
        var deletedImportantDateResult = await _dateService.DeleteAsync(userId, importantDateId);

        return deletedImportantDateResult != null
            ? Ok(deletedImportantDateResult)
            : BadRequest("Unable to delete important date item or the item does not belong to the user");

    }

}