using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Services;

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
        // Mapper EventDTO til Event-modellen
        var newEvent = _eventMapper.MapToModel(newEventDTO);

        // Legger til det nye arrangementet i databasen og henter resultatet
        var addedEvent = await _eventRepository.AddAsync(newEvent);

        // Mapper det nye arrangementet til EventDTO og returnerer den
        return addedEvent != null ? _eventMapper.MapToDTO(addedEvent) : null;   
    }


    // Henter alle arrangementer med paginering
    public async Task<ICollection<EventDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        // Henter arrangementinformasjon fra repository med paginering
        var eventsFromRepository = await _eventRepository.GetAllAsync(1, 10);

        // Mapper arrangementdataene til eventDTO-format
        var eventDTOs = eventsFromRepository.Select(eventEntity => _eventMapper.MapToDTO(eventEntity)).ToList();
        return eventDTOs;
    }


    // Henter arrangement basert på ID
    public async Task<EventDTO?> GetByIdAsync(int eventId)
    {
        var eventFromRepository = await _eventRepository.GetByIdAsync(eventId);
        return eventFromRepository != null ? _eventMapper.MapToDTO(eventFromRepository) : null;
    }


    // Oppdaterer arrangement
    public async Task<EventDTO?> UpdateAsync(int eventId, EventDTO eventDTO)
    {
        var existingEvent = await _eventRepository.GetByIdAsync(eventId);
        if (existingEvent == null) return null;
        

        // Mapper og oppdaterer arrangementsinformasjon
        var eventToUpdate = _eventMapper.MapToModel(eventDTO);
        eventToUpdate.Id = eventId;

        var updatedEvent = await _eventRepository.UpdateAsync(eventId, eventToUpdate);

        return updatedEvent != null ? _eventMapper.MapToDTO(updatedEvent) : null;
    }


    // Sletter arrangement
    public async Task<EventDTO?> DeleteAsync(int eventId)
    {
        var eventToDelete = await _eventRepository.GetByIdAsync(eventId);

        // Sjekker om arrangementet eksisterer
        if (eventToDelete == null) return null;

        // Sletter arrangementet fra databasen og mapper den til EventDTO for retur                 
        var deletedEvent = await _eventRepository.DeleteAsync(eventId);
        return deletedEvent != null ? _eventMapper.MapToDTO(eventToDelete) : null;
    }

}