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

    public int Id { get; set; }

    public int UserId { get; set; }
    public DateOnly Date { get; set;}
    public string Name { get; set; } = string.Empty;

}

