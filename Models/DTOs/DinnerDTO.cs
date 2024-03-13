namespace PlanIT.API.Models.DTOs;

public class DinnerDTO
{
    public DinnerDTO(int id, int userId, DateOnly date, string name)
    {
        Id = id;
        UserId = userId;
        Date = date;
        Name = name;
    }

    public int Id { get; init; }

    public int UserId { get; init; }
    public DateOnly Date { get; init;}
    public string Name { get; init; } = string.Empty;

}

