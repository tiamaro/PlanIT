using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Services.Interfaces;

public interface IUserService
{
    // CREATE
    Task<UserDTO?> RegisterUserAsync(UserRegDTO userRegDto);

    // READ
    Task<UserDTO?> GetByIdAsync(int id);
    Task<ICollection<UserDTO>> GetAllAsync(int pageNr, int pageSize);

    // UPDATE
    Task<UserDTO?> UpdateAsync(int id, UserDTO dto);

    // DELETE
    Task<UserDTO?> DeleteAsync(int id);
}