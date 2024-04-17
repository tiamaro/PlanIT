using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;

namespace PlanIT.API.Mappers;

public class ContactMapper : IMapper<Contact, ContactDTO>
{
    public ContactDTO MapToDTO(Contact model)
    {
        return new ContactDTO(model.Id, model.UserId, model.Name, model.Email);
    }

    public Contact MapToModel(ContactDTO dto)
    {
        return new Contact
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Name = dto.Name,
            Email = dto.Email
        };
    }
}
