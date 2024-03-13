namespace PlanIT.API.Models.DTOs;

public class UserDTO
{
    public UserDTO(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;


}
