using PlanIT.API.Models.Entities;

namespace PlanIT.API.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByEmailAsync(string email);
}
