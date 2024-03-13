using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;

namespace PlanIT.API.Mappers;

public class DinnerMapper : IMapper<Dinner, DinnerDTO>
{
    public DinnerDTO MapToDTO(Dinner model)
    {
        return new DinnerDTO(model.Id, model.UserId, model.Date, model.Name);
    }

    public Dinner MapToModel(DinnerDTO dto)
    {
        return new Dinner
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Date = dto.Date,
            Name = dto.Name
        };
    }
}