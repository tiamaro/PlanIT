using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;


// Kontrolleren for API-versjon 1 som definerer adressen (URL) til API-et.
// [ApiController] indikerer at klassen fungerer som en kontroller i systemet og arver fra ControllerBase.
// Tar imot en EventService-innstans som en del av konstruktøren for å utføre arrangementrelaterte operasjoner.
// api/v1/Events

[Route("api/v1/[controller]")]
[ApiController]

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
        try
        {
            // Sjekk om modelltilstanden er gyldig etter modellbinding og validering
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state in AddEventAsync");

                // Hvis modelltilstanden er ugyldig, returner en BadRequest sammen med ModelState-feilene
                return BadRequest(ModelState);
            }

            // Registrer arrangementet ved å bruke de oppgitte detaljene for arrangementregistrering
            var addedEvent = await _eventService.CreateAsync(newEventDTO);

            // Sjekk om arrangementsregistreringen var vellykket
            return addedEvent != null
                ? Ok(addedEvent)
                : BadRequest("Failed to register new event");
        }
        catch (Exception ex) // Generell Exception-håndtering for uventede feil
        {
            _logger.LogError("An unknown error occured: " + ex.Message);
            return StatusCode(500, "An unknown error occured, please try again later");
        }
    }


    // Henter en liste over arrangementer i form av EventDTO-objekter med paginering.
    // Parametrene 'pageNr' og 'pageSize' angir henholdsvis sidenummer og størrelse på siden.
    // Returnerer en ActionResult med en IEnumerable av EventDTO-objekter.
    // GET: /api/v1/Events?pageNr=1&pageSize=10
    [HttpGet(Name = "GetEvents")]
    public async Task<ActionResult<IEnumerable<EventDTO>>> GetEventsAsync(int pageNr, int pageSize)
    {
        var allEvents = await _eventService.GetAllAsync(pageNr, pageSize);

        return allEvents != null
            ? Ok(allEvents)
            : NotFound("No registered events found.");
    }


    // Henter et arrangement basert på arrangementets ID ved hjelp av EventService.
    // Returnerer en ActionResult med en EventDTO hvis arrangementet ble funnet,
    // ellers returneres NotFound hvis arrangementet ikke ble funnet.
    // GET /api/v1/Events/1
    [HttpGet("{eventId}", Name = "GetEventsById")]
    public async Task<ActionResult<EventDTO>> GetEventsByIdASync(int eventId)
    {
        // Hent arrangement fra tjenesten
        var existingEvent = await _eventService.GetByIdAsync(eventId);

        return existingEvent != null
            ? Ok(existingEvent)
            : NotFound("Event not found");
    }


    // Oppdaterer et arrangement basert på arrangementets ID ved å bruke EventService.
    // Returnerer en ActionResult med en oppdatert EventDTO hvis oppdateringen var vellykket, 
    // ellers returneres NotFound hvis arrangementet ikke ble funnet.
    // PUT /api/v1/Events/4
    [HttpPut("{eventId}", Name = "UpdateEvent")]
    public async Task<ActionResult<EventDTO>> UpdateEventAsync(int eventId, EventDTO updatedEventDTO)
    {
        // Hent arrangement fra tjenesten
        var existingEvent = await _eventService.GetByIdAsync(eventId);

        // Sjekk om arrangementet eksisterer
        if (existingEvent == null) return NotFound("Event not found");

        try
        {
            // Hvis arrangement er riktig, fortsett med oppdateringen.
            var updatedEventResult = await _eventService.UpdateAsync(eventId, updatedEventDTO);
            return updatedEventResult != null
                ? Ok(updatedEventResult)
                : NotFound("Unable to update the event");
        }
        catch (Exception ex) // Generell Exception-håndtering for uventede feil
        {
            _logger.LogError("An unknown error occured: " + ex.Message);
            return StatusCode(500, "An unknown error occured, please try again later");
        }

    }


    // Sletter et arrangement basert på arrangementets ID ved å bruke EventService.
    // Returnerer en ActionResult med en slettet EventDTO hvis slettingen var vellykket, 
    // ellers returneres BadRequest hvis arrangement ikke kunne slettes.
    // DELETE /api/v1/Events/2
    [HttpDelete("{eventId}", Name = "DeleteEvent")]
    public async Task<ActionResult<EventDTO>> DeleteEventAsync(int eventId)
    {
        // Hent arrangement fra tjenesten
        var existingEvent = await _eventService.GetByIdAsync(eventId);

        // Sjekk om arrangement eksisterer
        if (existingEvent == null) return NotFound("Event not found");

        try
        {
            // Hvis arrangement er riktig, fortsett med slettingen.
            var deletedEventResult = await _eventService.DeleteAsync(eventId);
            return deletedEventResult != null
                ? Ok(deletedEventResult)
                : BadRequest("Unable to delete event");
        }
        catch (Exception ex) // Generell Exception-håndtering for uventede feil
        {
            _logger.LogError("An unknown error occured: " + ex.Message);
            return StatusCode(500, "An unknown error occured, please try again later");
        }

    }
}