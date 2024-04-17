namespace PlanIT.API.Models.DTOs;

public class EventDTO
{
    public EventDTO(int id, int userId, string name, DateOnly date, TimeOnly time, string location)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Date = date;
        Time = time;
        Location = location;
    }

    public int Id { get; init; }

    public int UserId { get; init; }

    public string Name { get; init; } = string.Empty;

    public DateOnly Date { get; init; }

    public TimeOnly Time { get; init; } 

    public string Location { get; init; } = string.Empty;
}
