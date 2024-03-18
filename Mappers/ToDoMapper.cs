using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;

namespace PlanIT.API.Mappers;

public class TodoMapper : IMapper<Todo, TodoDTO>
{
    public TodoDTO MapToDTO(Todo model)
    {
        return new TodoDTO(model.Id, model.UserId, model.Name, model.Date);
    }

    public Todo MapToModel(TodoDTO dto)
    {
        return new Todo 
        { 
            Id = dto.Id,
            UserId = dto.UserId,
            Name = dto.Name,
            Date = dto.Date 
        };
    }
}
