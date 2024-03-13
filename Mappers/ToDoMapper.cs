using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;

namespace PlanIT.API.Mappers;

public class ToDoMapper : IMapper<ToDo, ToDoDTO>
{
    public ToDoDTO MapToDTO(ToDo model)
    {
        return new ToDoDTO(model.Id, model.UserId, model.Name, model.Date);
    }

    public ToDo MapToModel(ToDoDTO dto)
    {
        return new ToDo 
        { 
            Id = dto.Id,
            UserId = dto.UserId,
            Name = dto.Name,
            Date = dto.Date 
        };
    }
}