namespace PlanIT.API.Models.DTOs;

public class WeeklyDinnerPlanDTO
{
    public DinnerDTO? Monday { get; set; }
    public DinnerDTO? Tuesday { get; set; }
    public DinnerDTO? Wednesday { get; set; }
    public DinnerDTO? Thursday { get; set; }
    public DinnerDTO? Friday { get; set; }
    public DinnerDTO? Saturday { get; set; }
    public DinnerDTO? Sunday { get; set; }

    public IEnumerable<DinnerDTO> ToDinnerDTOs()
    {
        // Only return DTOs for days that have been set
        if (Monday != null) yield return Monday;
        if (Tuesday != null) yield return Tuesday;
        if (Wednesday != null) yield return Wednesday;
        if (Thursday != null) yield return Thursday;
        if (Friday != null) yield return Friday;
        if (Saturday != null) yield return Saturday;
        if (Sunday != null) yield return Sunday;
    }
}