using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Services;

// Serviceklasse for håndtering av arrangementsinformasjon.
// Exceptions blir fanget av en middleware: HandleExceptionFilter
public class EventService : IService<EventDTO>
{
    private readonly IMapper<Event, EventDTO> _eventMapper;
    private readonly IRepository<Event> _eventRepository;
    private readonly ILogger<EventService> _logger;

    public EventService(IMapper<Event,
        EventDTO> eventMapper,
        IRepository<Event> eventRepository,
        ILogger<EventService> logger)
    {
        _eventMapper = eventMapper;
        _eventRepository = eventRepository;
        _logger = logger;
    }


    // Oppretter nytt arrangement
    public async Task<EventDTO?> CreateAsync(EventDTO newEventDTO)
    {
        _logger.LogInformation("Starting to create a new event.");

        // Mapper EventDTO til Event-modellen
        var newEvent = _eventMapper.MapToModel(newEventDTO);

        // Legger til det nye arrangementet i databasen og henter resultatet
        var addedEvent = await _eventRepository.AddAsync(newEvent);
        if (addedEvent == null)
        {
            _logger.LogWarning("Failed to create new event.");
            throw new InvalidOperationException("The event could not be created due to invalid data or state.");
        }

        _logger.LogInformation("New event created successfully with ID {EventId}.", addedEvent.Id);
        return _eventMapper.MapToDTO(addedEvent);
    }



    // *********NB!! FJERNE??? *************
    //
    // Henter alle arrangementer med paginering
    public async Task<ICollection<EventDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        // Henter arrangementinformasjon fra repository med paginering
        var eventsFromRepository = await _eventRepository.GetAllAsync(1, 10);

        // Mapper arrangementdataene til eventDTO-format
        var eventDTOs = eventsFromRepository.Select(eventEntity => _eventMapper.MapToDTO(eventEntity)).ToList();
        return eventDTOs;
    }



    // Henter arrangement basert på eventID og brukerID
    public async Task<EventDTO?> GetByIdAsync(int userIdFromToken, int eventId)
    {
        _logger.LogDebug("Attempting to retrieve event with ID {EventId} for user ID {UserId}.", eventId, userIdFromToken);

        var eventFromRepository = await _eventRepository.GetByIdAsync(eventId);

        if (eventFromRepository == null)
        {
            _logger.LogWarning("Event with ID {EventId} not found.", eventId);
            throw new KeyNotFoundException($"Event with ID {eventId} not found.");
        }

        if (eventFromRepository.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized attempt to access event with ID {EventId} by user ID {UserId}.", eventId, userIdFromToken);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to access event ID {eventId}.");
        }

        _logger.LogInformation("Event with ID {EventId} retrieved successfully for user ID {UserId}.", eventId, userIdFromToken);
        return _eventMapper.MapToDTO(eventFromRepository);
    }


    // Oppdaterer arrangement
    public async Task<EventDTO?> UpdateAsync(int userIdFromToken, int eventId, EventDTO eventDTO)
    {
        _logger.LogDebug("Attempting to update event with ID {EventId} for user ID {UserId}.", eventId, userIdFromToken);

        // Sjekker om arrangementet eksisterer
        var existingEvent = await _eventRepository.GetByIdAsync(eventId);
        if (existingEvent == null)
        {
            _logger.LogWarning("Event with ID {EventId} not found.", eventId);
            throw new KeyNotFoundException($"Event with ID {eventId} not found.");
        }

        // Sjekker om brukeren har rettigheter til å oppdatere arrangementet
        if (existingEvent.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized update attempt by User ID {UserId} on Event ID {EventId}", userIdFromToken, eventId);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to update event ID {eventId}.");
        }

        var eventToUpdate = _eventMapper.MapToModel(eventDTO);
        eventToUpdate.Id = eventId; // Passer på at ikke ID endrer seg under oppdatering

        // Oppdaterer arrangementet
        var updatedEvent = await _eventRepository.UpdateAsync(eventId, eventToUpdate);
        if (updatedEvent == null)
        {
            _logger.LogError("Failed to update Event with ID {EventId}", eventId);
            throw new InvalidOperationException($"Failed to update event with ID {eventId}.");
        }

        _logger.LogInformation("Event with ID {EventId} updated successfully by User ID {UserId}.", eventId, userIdFromToken);
        return _eventMapper.MapToDTO(updatedEvent);
    }



    // Sletter arrangement
    public async Task<EventDTO?> DeleteAsync(int userIdFromToken, int eventId)
    {
        _logger.LogDebug("Attempting to delete event with ID {EventId} by user ID {UserId}.", eventId, userIdFromToken);

        var eventToDelete = await _eventRepository.GetByIdAsync(eventId);

        // Sjekker om arrangementet eksisterer
        if (eventToDelete == null)
        {
            _logger.LogWarning("Attempt to delete a non-existent event with ID {EventId}.", eventId);
            throw new KeyNotFoundException($"Event with ID {eventId} not found."); 
        }

        // Sjekker om brukeren har rettigheter til å oppdatere arrangementet
        if (eventToDelete.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized delete attempt by User ID {UserId} on Event ID {EventId}.", userIdFromToken, eventId);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to delete event ID {eventId}.");  
        }

        // Sletter arrangement fra databasen
        var deletedEvent = await _eventRepository.DeleteAsync(eventId);
        if (deletedEvent == null)
        {
            _logger.LogError("Failed to delete event with ID {EventId}.", eventId);
            throw new InvalidOperationException("Deletion failed, could not complete operation on the database.");
        }

        _logger.LogInformation("Event with ID {EventId} deleted successfully by User ID {UserId}.", eventId, userIdFromToken);
        return _eventMapper.MapToDTO(eventToDelete);
    }
}