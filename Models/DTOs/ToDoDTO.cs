namespace PlanIT.API.Models.DTOs;

public class ToDoDTO
{
    public ToDoDTO(int id, int userId, string name, DateTime date)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Date = date;
    }

    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime Date { get; set; }
}
