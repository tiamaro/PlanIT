using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities; // Inkluderer tilgang til LoggerService og ExceptionHelper

namespace PlanIT.API.Services;

// Serviceklasse for håndtering av arrangementsinformasjon.
// Exceptions blir fanget av en middleware: HandleExceptionFilter
public class EventService : IService<EventDTO>
{
    private readonly IMapper<Event, EventDTO> _eventMapper;
    private readonly IRepository<Event> _eventRepository;
    private readonly LoggerService _logger;

    public EventService(IMapper<Event, EventDTO> eventMapper,
        IRepository<Event> eventRepository,
        LoggerService logger)
    {
        _eventMapper = eventMapper;
        _eventRepository = eventRepository;
        _logger = logger;
    }


    // Oppretter et nytt arrangement basert på DTO fra klienten.
    public async Task<EventDTO?> CreateAsync(EventDTO newEventDTO)
    {
        _logger.LogCreationStart("event");

        // Konverterer newEventDTO til event-modellen for lagring
        var newEvent = _eventMapper.MapToModel(newEventDTO);

        // Forsøker å legge til det nye arrangementet i databasen
        var addedEvent = await _eventRepository.AddAsync(newEvent);
        if (addedEvent == null)
        {
            _logger.LogCreationFailure("event");
            throw ExceptionHelper.CreateOperationException("event", 0, "create");
        }

        // Logger at arrangementet ble vellykket opprettet med tilhørende ID
        _logger.LogOperationSuccess("created", "event", addedEvent.Id);

        // Returnerer arrangementet konvertert tilbake til DTO-format
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


    // Henter et spesifikt arrangement basert på dets ID og sjekker at brukeren har tilgang.
    public async Task<EventDTO?> GetByIdAsync(int userIdFromToken, int eventId)
    {
        _logger.LogDebug($"Attempting to retrieve event with ID {eventId} for user ID {userIdFromToken}.");
        var eventFromRepository = await _eventRepository.GetByIdAsync(eventId);
        if (eventFromRepository == null)
        {
            _logger.LogNotFound("event", eventId);
            throw ExceptionHelper.CreateNotFoundException("event", eventId);
        }

        if (eventFromRepository.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("event", eventId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("event", eventId);
        }

        _logger.LogOperationSuccess("retrieved", "event", eventId);
        return _eventMapper.MapToDTO(eventFromRepository);
    }


    // Oppdaterer et eksisterende arrangement og sikrer at brukeren har rettigheter til dette.
    public async Task<EventDTO?> UpdateAsync(int userIdFromToken, int eventId, EventDTO eventDTO)
    {
        _logger.LogDebug($"Attempting to update event with ID {eventId} for user ID {userIdFromToken}.");

        // Forsøker å hente et arrangemenr basert på ID for å sikre at det faktisk eksisterer før oppdatering.
        var existingEvent = await _eventRepository.GetByIdAsync(eventId);
        if (existingEvent == null)
        {
            _logger.LogNotFound("event", eventId);
            throw ExceptionHelper.CreateNotFoundException("event", eventId);
        }

        // Sjekker om brukeren som prøver å oppdatere arrangementet er den samme brukeren som opprettet det.
        if (existingEvent.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("event", eventId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("event", eventId);
        }

        // Mapper til DTO og sørger for at ID forblir den samme under oppdateringen
        var eventToUpdate = _eventMapper.MapToModel(eventDTO);
        eventToUpdate.Id = eventId;

        // Prøver å oppdatere arrangementet i databasen
        var updatedEvent = await _eventRepository.UpdateAsync(eventId, eventToUpdate);
        if (updatedEvent == null)
        {
            _logger.LogOperationFailure("update", "event", eventId);
            throw ExceptionHelper.CreateOperationException("event", eventId, "update");
        }

        _logger.LogOperationSuccess("updated", "event", eventId);
        return _eventMapper.MapToDTO(updatedEvent);
    }


    // Sletter et arrangement og sikrer at brukeren har autorisasjon til dette.
    public async Task<EventDTO?> DeleteAsync(int userIdFromToken, int eventId)
    {
        _logger.LogDebug($"Attempting to delete event with ID {eventId} by user ID {userIdFromToken}.");

        // Forsøker å hente en arrangement basert på ID for å sikre at det faktisk eksisterer før sletting
        var eventToDelete = await _eventRepository.GetByIdAsync(eventId);
        if (eventToDelete == null)
        {
            _logger.LogNotFound("event", eventId);
            throw ExceptionHelper.CreateNotFoundException("event", eventId);
        }

        // Sjekker om brukeren som prøver å slette arrangementet er den samme brukeren som opprettet det.
        if (eventToDelete.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("event", eventId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("event", eventId);
        }

        // Prøver å slette arrangementet fra databasen
        var deletedEvent = await _eventRepository.DeleteAsync(eventId);
        if (deletedEvent == null)
        {
            _logger.LogOperationFailure("delete", "event", eventId);
            throw ExceptionHelper.CreateOperationException("event", eventId, "delete");
        }

        _logger.LogOperationSuccess("deleted", "event", eventId);
        return _eventMapper.MapToDTO(eventToDelete);
    }
}