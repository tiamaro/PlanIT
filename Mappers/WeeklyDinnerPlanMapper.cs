using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;

namespace PlanIT.API.Mappers;

public class WeeklyDinnerPlanMapper : IWeeklyDinnerPlanMapper
{
    private readonly IMapper<Dinner, DinnerDTO> _dinnerMapper;

    public WeeklyDinnerPlanMapper(IMapper<Dinner, DinnerDTO> dinnerMapper)
    {
        _dinnerMapper = dinnerMapper;
    }

    public WeeklyDinnerPlanDTO MapToDTO(IEnumerable<Dinner> dinners)
    {
        var plan = new WeeklyDinnerPlanDTO();
        foreach (var dinner in dinners)
        {
            switch (dinner.Date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    plan.Monday = _dinnerMapper.MapToDTO(dinner);
                    break;
                case DayOfWeek.Tuesday:
                    plan.Tuesday = _dinnerMapper.MapToDTO(dinner);
                    break;
                case DayOfWeek.Wednesday:
                    plan.Wednesday = _dinnerMapper.MapToDTO(dinner);
                    break;
                case DayOfWeek.Thursday:
                    plan.Thursday = _dinnerMapper.MapToDTO(dinner);
                    break;
                case DayOfWeek.Friday:
                    plan.Friday = _dinnerMapper.MapToDTO(dinner);
                    break;
                case DayOfWeek.Saturday:
                    plan.Saturday = _dinnerMapper.MapToDTO(dinner);
                    break;
                case DayOfWeek.Sunday:
                    plan.Sunday = _dinnerMapper.MapToDTO(dinner);
                    break;
            }
        }
        return plan;
    }
}