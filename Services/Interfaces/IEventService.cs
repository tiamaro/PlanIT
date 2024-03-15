using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Services.Interfaces;

public interface IEventService : IService<EventDTO>
{
    Task<EventDTO?> CreateEventAsync(EventDTO newEventDTO);
}