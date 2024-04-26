using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Services;

// Service class for handling event information.
// Exceptions are caught by a middleware: HandleExceptionFilter
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


  
    public async Task<EventDTO?> CreateAsync(int userIdFromToken,EventDTO newEventDTO)
    {
        _logger.LogCreationStart("event");

        
        var newEvent = _eventMapper.MapToModel(newEventDTO);
        newEvent.UserId = userIdFromToken;

       
        var addedEvent = await _eventRepository.AddAsync(newEvent);
        if (addedEvent == null)
        {
            _logger.LogCreationFailure("event");
            throw ExceptionHelper.CreateOperationException("event", 0, "create");
        }

       
        _logger.LogOperationSuccess("created", "event", addedEvent.Id);

       
        return _eventMapper.MapToDTO(addedEvent);
    }


   
    public async Task<ICollection<EventDTO>> GetAllAsync(int userIdFromToken, int pageNr, int pageSize)
    {
        
        var eventsFromRepository = await _eventRepository.GetAllAsync(1, 10);

        
        var filteredDinners = eventsFromRepository.Where(@event => @event.UserId == userIdFromToken);

       
        return filteredDinners.Select(eventEntity => _eventMapper.MapToDTO(eventEntity)).ToList();
    }


    
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


    
    public async Task<EventDTO?> UpdateAsync(int userIdFromToken, int eventId, EventDTO eventDTO)
    {
        _logger.LogDebug($"Attempting to update event with ID {eventId} for user ID {userIdFromToken}.");

       
        var existingEvent = await _eventRepository.GetByIdAsync(eventId);
        if (existingEvent == null)
        {
            _logger.LogNotFound("event", eventId);
            throw ExceptionHelper.CreateNotFoundException("event", eventId);
        }

        
        if (existingEvent.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("event", eventId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("event", eventId);
        }

        
        var eventToUpdate = _eventMapper.MapToModel(eventDTO);
        eventToUpdate.Id = eventId;

        
        var updatedEvent = await _eventRepository.UpdateAsync(eventId, eventToUpdate);
        if (updatedEvent == null)
        {
            _logger.LogOperationFailure("update", "event", eventId);
            throw ExceptionHelper.CreateOperationException("event", eventId, "update");
        }

        _logger.LogOperationSuccess("updated", "event", eventId);
        return _eventMapper.MapToDTO(updatedEvent);
    }



    public async Task<EventDTO?> DeleteAsync(int userIdFromToken, int eventId)
    {
        _logger.LogDebug($"Attempting to delete event with ID {eventId} by user ID {userIdFromToken}.");

        
        var eventToDelete = await _eventRepository.GetByIdAsync(eventId);
        if (eventToDelete == null)
        {
            _logger.LogNotFound("event", eventId);
            throw ExceptionHelper.CreateNotFoundException("event", eventId);
        }

       
        if (eventToDelete.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("event", eventId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("event", eventId);
        }

       
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