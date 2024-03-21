using PlanIT.API.Models.Entities;

namespace PlanIT.API.Services.Interfaces;

public interface IAuthService
{
    Task<User?> AuthenticateUserAsync(string email, string password);
    Task<string> GenerateJwtTokenAsync(User user);
}