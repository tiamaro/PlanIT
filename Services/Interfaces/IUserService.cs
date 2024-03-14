using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Services.Interfaces;

public interface IUserService : IService<UserDTO>
{
    // CREATE
    Task<UserDTO?> RegisterUserAsync(UserRegDTO userRegDto);
}