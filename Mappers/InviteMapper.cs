using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;

namespace PlanIT.API.Mappers;

public class InviteMapper : IMapper<Invite, InviteDTO>
{
    public InviteDTO MapToDTO(Invite model)
    {
        return new InviteDTO(model.Id, model.EventId, model.Name, model.Email, model.Coming);
    }

    public Invite MapToModel(InviteDTO dto)
    {
        return new Invite
        {
            Id = dto.Id,
            EventId = dto.EventId,
            Name = dto.Name,
            Email = dto.Email,
            Coming = dto.Coming

        };
    }
}
