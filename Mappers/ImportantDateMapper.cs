using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;

namespace PlanIT.API.Mappers;

public class ImportantDateMapper : IMapper<ImportantDate, ImportantDateDTO>
{
    public ImportantDateDTO MapToDTO(ImportantDate model)
    {
        return new ImportantDateDTO(model.Id, model.UserId, model.Name, model.Date);
    }

    public ImportantDate MapToModel(ImportantDateDTO dto)
    {
        return new ImportantDate
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Name = dto.Name,
            Date = dto.Date
            
        };
    }
}
