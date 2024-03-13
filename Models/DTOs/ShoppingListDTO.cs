namespace PlanIT.API.Models.DTOs;

public class ShoppingListDTO
{
    public ShoppingListDTO(int id, int userId, string name)
    {
        Id = id;
        UserId = userId;
        Name = name;
    }

    public int Id { get; init; }

    public int UserId { get; init; }

    public string Name { get; init; } = string.Empty;
}
