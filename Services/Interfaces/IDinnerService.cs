using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Services.Interfaces;

public interface IDinnerService : IService<DinnerDTO>
{
    Task<WeeklyDinnerPlanDTO> GetWeeklyDinnerPlanAsync(int userId, DateOnly startDate, DateOnly endDate);
}