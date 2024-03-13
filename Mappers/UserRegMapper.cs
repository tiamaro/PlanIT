using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;

namespace PlanIT.API.Mappers;

public class UserRegMapper : IMapper<User, UserRegDTO>
{
    public UserRegDTO MapToDTO(User model)
    {
        return new UserRegDTO(model.Name, model.Email, model.HashedPassword);
    }

    public User MapToModel(UserRegDTO dto)
    {
        return new User
        { 
            Name = dto.Name,
            Email = dto.Email,
            HashedPassword = dto.Password
        };
    }
}