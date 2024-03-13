namespace PlanIT.API.Models.DTOs;

public class EventDTO
{
    public EventDTO(int id, int userId, string name, DateOnly date, string time, string location)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Date = date;
        Time = time;
        Location = location;
    }

    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateOnly Date { get; set; }

    public string Time { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;
}
