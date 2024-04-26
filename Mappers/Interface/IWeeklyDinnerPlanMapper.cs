using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;

namespace PlanIT.API.Mappers.Interface;

public interface IWeeklyDinnerPlanMapper
{
    WeeklyDinnerPlanDTO MapToDTO(IEnumerable<Dinner> dinners);
}