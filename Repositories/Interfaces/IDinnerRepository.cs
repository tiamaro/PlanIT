using PlanIT.API.Models.Entities;

namespace PlanIT.API.Repositories.Interfaces;

public interface IDinnerRepository : IRepository<Dinner>
{
    Task<List<Dinner>?> GetByDateRangeAndUserAsync(int userId, DateOnly startDate, DateOnly endDate);
    
    Task<Dinner?> GetByDayAndUserAsync(int userId, DateOnly day);
}