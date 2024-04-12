using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;
using System.Security.Claims;

namespace PlanIT.API.Controllers;


// EventController - API Controller for arrangementhåndtering:
// - Kontrolleren håndterer alle forespørsler relatert til arrangementdata, inkludert registrering,
//   oppdatering, sletting og henting av arrangementinformasjon. Den tar imot en instans av eventService
//   som en del av konstruktøren for å utføre operasjoner relatert til arrangementer.
//
// Policy:
// - "Bearer": Krever at alle kall til denne kontrolleren er autentisert med et gyldig JWT-token
//   som oppfyller kravene definert i "Bearer" autentiseringspolicy. Dette sikrer at bare
//   autentiserte brukere kan aksessere endepunktene definert i denne kontrolleren.
//
// HandleExceptionFilter:
// - Dette filteret er tilknyttet kontrolleren for å fange og behandle unntak på en sentralisert måte.
//
// Forespørsler som starter med "api/v1/Events" vil bli rutet til metoder definert i denne kontrolleren.


[Authorize(Policy = "Bearer")]
[Route("api/v1/[controller]")]
[ApiController]
[ServiceFilter(typeof(HandleExceptionFilter))]  // Bruker HandleExceptionFilter for å håndtere unntak
public class EventsController : ControllerBase
{
    private readonly IService<EventDTO> _eventService;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IService<EventDTO> eventService, ILogger<EventsController> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }


    // Endepunkt for registrering av nytt arrangement
    // POST /api/v1/Events/register
    [HttpPost("register", Name = "AddEvent")]
    public async Task<ActionResult<EventDTO>> AddEventAsync(EventDTO newEventDTO)
    {
       
        // Sjekk om modelltilstanden er gyldig etter modellbinding og validering
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddEventAsync");
            return BadRequest(ModelState);
        }

        // Registrer arrangementet
        var addedEvent = await _eventService.CreateAsync(newEventDTO);

        // Sjekk om arrangementsregistreringen var vellykket
        return addedEvent != null
            ? Ok(addedEvent)
            : BadRequest("Failed to register new event");        
    }


    // !!!!!! NB! FJERNE ELLER ADMIN RETTIGHETER??? !!!!!!!!!!!!!!!
    //
    // Henter en liste over arrangementer
    // GET: /api/v1/Events?pageNr=1&pageSize=10
    [HttpGet(Name = "GetEvents")]
    public async Task<ActionResult<IEnumerable<EventDTO>>> GetEventsAsync(int pageNr, int pageSize)
    {
        var allEvents = await _eventService.GetAllAsync(pageNr, pageSize);

        return allEvents != null
            ? Ok(allEvents)
            : NotFound("No registered events found.");
    }


    // Henter et arrangement basert på arrangementets ID
    // GET /api/v1/Events/1
    [HttpGet("{eventId}", Name = "GetEventsById")]
    public async Task<ActionResult<EventDTO>> GetEventsByIdASync(int eventId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out var numericUserId))
        {
            return Unauthorized("Invalid user ID.");
        }

        // Hent arrangement fra tjenesten, filtrert etter brukerens ID
        var existingEvent = await _eventService.GetByIdAndUserIdAsync(eventId, numericUserId);

        return existingEvent != null
            ? Ok(existingEvent)
            : NotFound("Event not found");
    }


    // Oppdaterer et arrangement basert på arrangementets ID
    // PUT /api/v1/Events/4
    [HttpPut("{eventId}", Name = "UpdateEvent")]
    public async Task<ActionResult<EventDTO>> UpdateEventAsync(int eventId, EventDTO updatedEventDTO)
    {
        // Hent arrangement fra tjenesten
        var existingEvent = await _eventService.GetByIdAsync(eventId);

        // Sjekk om arrangementet eksisterer
        if (existingEvent == null) return NotFound("Event not found");

        
        // Hvis arrangement er riktig, fortsett med oppdateringen.
        var updatedEventResult = await _eventService.UpdateAsync(eventId, updatedEventDTO);
        return updatedEventResult != null
            ? Ok(updatedEventResult)
            : NotFound("Unable to update the event");
    }


    // Sletter et arrangement basert på arrangementets ID
    // DELETE /api/v1/Events/2
    [HttpDelete("{eventId}", Name = "DeleteEvent")]
    public async Task<ActionResult<EventDTO>> DeleteEventAsync(int eventId)
    {
        // Hent arrangement fra tjenesten
        var existingEvent = await _eventService.GetByIdAsync(eventId);

        // Sjekk om arrangement eksisterer
        if (existingEvent == null) return NotFound("Event not found");

        
        // Hvis arrangement er riktig, fortsett med slettingen.
        var deletedEventResult = await _eventService.DeleteAsync(eventId);
        return deletedEventResult != null
            ? Ok(deletedEventResult)
            : BadRequest("Unable to delete event");
    }
}