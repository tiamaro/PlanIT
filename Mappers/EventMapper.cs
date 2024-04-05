using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;

namespace PlanIT.API.Mappers;

public class EventMapper : IMapper<Event, EventDTO>
{
    public EventDTO MapToDTO(Event model)
    {
        return new EventDTO(model.Id, model.UserId, model.Name, model.Date, model.Time, model.Location);
    }

    public Event MapToModel(EventDTO dto)
    {
        return new Event
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Name = dto.Name,
            Date = dto.Date,
            Time = dto.Time,
            Location = dto.Location

        };
    }
}