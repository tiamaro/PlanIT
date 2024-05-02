using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanIT.API.Extensions;
using PlanIT.API.Middleware;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Controllers;


// EventController - API Controller for event management:
// - The controller handles all requests related to event data, including registration,
//   updating, deletion, and retrieval of event information. It receives an instance of IService
//   as part of the constructor to perform operations related to events.
//
// Policy:
// - "Bearer": Requires that all calls to this controller are authenticated with a valid JWT token
//   that meets the requirements defined in the "Bearer" authentication policy. This ensures that only
//   authenticated users can access the endpoints defined in this controller.
//
// HandleExceptionFilter:
// - This filter is attached to the controller to catch and handle exceptions in a centralized manner.
//
// Requests starting with "api/v1/Events" will be routed to methods defined in this controller.


[Authorize(Policy = "Bearer")]
[Route("api/v1/[controller]")]
[ApiController]
[ServiceFilter(typeof(HandleExceptionFilter))]  // Uses HandleExceptionFilter to handle exceptions
public class EventsController : ControllerBase
{
    private readonly IService<EventDTO> _eventService;
    private readonly IInviteService _inviteService;
    private readonly ILogger<EventsController> _logger;
    

    public EventsController(IService<EventDTO> eventService, 
        IInviteService inviteService,
        ILogger<EventsController> logger
        )
    {
        _eventService = eventService;
        _inviteService = inviteService;
        _logger = logger;
    }


    // Registers a new event
    // POST /api/v1/Events/register
    [HttpPost("register", Name = "AddEvent")]
    public async Task<ActionResult<EventDTO>> AddEventAsync(EventDTO newEventDTO)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        // Checks if the model state is valid after model binding and validation
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state in AddEventAsync");
            return BadRequest(ModelState);
        }

       
        var addedEvent = await _eventService.CreateAsync(userId, newEventDTO);


        // Returns new event, or an error message if registration fails
        return addedEvent != null
            ? Ok(addedEvent)
            : BadRequest("Failed to register new event");
    }


    // Retrieves a paginated list of events
    // GET: /api/v1/Events?pageNr=1&pageSize=10
    [HttpGet(Name = "GetEvents")]
    public async Task<ActionResult<IEnumerable<EventDTO>>> GetEventsAsync(int pageNr, int pageSize)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);


        var allEvents = await _eventService.GetAllAsync(userId,pageNr, pageSize);

        // Returns list of events, or an error message if not found
        return allEvents != null
            ? Ok(allEvents)
            : NotFound("No registered events found.");
    }


    // Retrieves a specific event by its ID.
    // GET /api/v1/Events/{eventId}
    [HttpGet("{eventId}", Name = "GetEventsById")]
    public async Task<ActionResult<EventDTO>> GetEventsByIdASync(int eventId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        
        var existingEvent = await _eventService.GetByIdAsync(userId, eventId);

        // Returns event, or an error message if not found
        return existingEvent != null
            ? Ok(existingEvent)
            : NotFound("Event not found");
    }


    // Retrieves invites associated with a specific event by eventID.
    // GET /api/v1/Events/{eventId}/invites
    [HttpGet("{eventId:int}/invites", Name = "GetEventInvites")]
    public async Task<ActionResult<IEnumerable<InviteDTO>>> GetEventInvitesAsync(int eventId, int pageNr, int pageSize)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

        var invites = await _inviteService.GetInvitesForEventAsync(userId, eventId, pageNr, pageSize);

        // Returns list of invites, or an error message if not found
        return invites != null
            ? Ok(invites)
            : NotFound("No invites found for the specified event.");
    }



    // Updates an event based on the provided ID.
    // PUT /api/v1/Events/{eventId}
    [HttpPut("{eventId}", Name = "UpdateEvent")]
    public async Task<ActionResult<EventDTO>> UpdateEventAsync(int eventId, EventDTO updatedEventDTO)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);

       
        var updatedEventResult = await _eventService.UpdateAsync(userId, eventId, updatedEventDTO);


        // Returns updated event, or an error message if update fails
        return updatedEventResult != null
            ? Ok(updatedEventResult)
            : NotFound("Unable to update the event or the event does not belong to the user");
    }


    // Deletes an event based on the provided ID.
    // DELETE /api/v1/Events/{evenetId}
    [HttpDelete("{eventId}", Name = "DeleteEvent")]
    public async Task<ActionResult<EventDTO>> DeleteEventAsync(int eventId)
    {
        // Retrieves the user's ID from HttpContext.Items which was added by middleware
        var userId = WebAppExtensions.GetValidUserId(HttpContext);


        
        var deletedEventResult = await _eventService.DeleteAsync(userId, eventId);


        // Returns deleted event, or an error message if deletion fails
        return deletedEventResult != null
            ? Ok(deletedEventResult)
            : BadRequest("Unable to delete event or event does not belong to the user");
    }
}