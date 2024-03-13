using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;

namespace PlanIT.API.Mappers;

public class ShoppingListMapper : IMapper<ShoppingList, ShoppingListDTO>
{
    public ShoppingListDTO MapToDTO(ShoppingList model)
    {
        return new ShoppingListDTO(model.Id, model.UserId, model.Name);
    }

    public ShoppingList MapToModel(ShoppingListDTO dto)
    {
        return new ShoppingList
        {
            Id = dto.Id,
            UserId = dto.UserId,
            Name = dto.Name
        };
    }
}