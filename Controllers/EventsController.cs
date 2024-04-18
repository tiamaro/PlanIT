using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;


// EventController - API Controller for arrangementhåndtering:
// - Kontrolleren håndterer alle forespørsler relatert til arrangementdata, inkludert registrering,
//   oppdatering, sletting og henting av arrangementinformasjon. Den tar imot en instans av IService
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

    public EventsController(IService<EventDTO> eventService,
        ILogger<EventsController> logger)
    {
        _eventService = eventService;
        _logger = logger;
    }


    // Endepunkt for registrering av nytt arrangement
    // POST /api/v1/Events/register
    [HttpPost("register", Name = "AddEvent")]
    public async Task<ActionResult<EventDTO>> AddEventAsync(EventDTO newEventDTO)
    {
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Sjekk om modelltilstanden er gyldig etter modellbinding og validering
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddEventAsync");
            return BadRequest(ModelState);
        }

        // Registrer arrangementet
        var addedEvent = await _eventService.CreateAsync(userId, newEventDTO);

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
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        var allEvents = await _eventService.GetAllAsync(userId,pageNr, pageSize);

        return allEvents != null
            ? Ok(allEvents)
            : NotFound("No registered events found.");
    }


    // Henter arrangementet basert på eventId
    // GET /api/v1/Events/1
    [HttpGet("{eventId}", Name = "GetEventsById")]
    public async Task<ActionResult<EventDTO>> GetEventsByIdASync(int eventId)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Hent arrangement fra tjenesten, filtrert etter brukerens ID
        var existingEvent = await _eventService.GetByIdAsync(userId, eventId);

        return existingEvent != null
            ? Ok(existingEvent)
            : NotFound("Event not found");
    }


    // Oppdaterer arrangementet basert på eventID.
    // PUT /api/v1/Events/4
    [HttpPut("{eventId}", Name = "UpdateEvent")]
    public async Task<ActionResult<EventDTO>> UpdateEventAsync(int eventId, EventDTO updatedEventDTO)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Prøver å oppdatere arrangementet med den nye informasjonen
        var updatedEventResult = await _eventService.UpdateAsync(userId, eventId, updatedEventDTO);

        // Returnerer oppdatert arrangementdata, eller en feilmelding hvis oppdateringen mislykkes
        return updatedEventResult != null
            ? Ok(updatedEventResult)
            : NotFound("Unable to update the event or the event does not belong to the user");
    }


    // Sletter et arrangement basert på arrangementets ID
    // DELETE /api/v1/Events/2
    [HttpDelete("{eventId}", Name = "DeleteEvent")]
    public async Task<ActionResult<EventDTO>> DeleteEventAsync(int eventId)
    {
        // Henter brukerens ID fra HttpContext.Items som ble lagt til av middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Prøver å slette arrangementet
        var deletedEventResult = await _eventService.DeleteAsync(userId, eventId);

        return deletedEventResult != null
            ? Ok(deletedEventResult)
            : BadRequest("Unable to delete event or event does not belong to the user");
    }
}