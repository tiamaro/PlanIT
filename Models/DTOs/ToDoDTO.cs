namespace PlanIT.API.Models.DTOs;

public class TodoDTO
{
    public TodoDTO(int id, int userId, string name, DateOnly date)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Date = date;
    }

    public int Id { get; init; }

    public int UserId { get; init; }

    public string Name { get; init; } = string.Empty;

    public DateOnly Date { get; init; }

}