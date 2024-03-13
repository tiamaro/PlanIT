namespace PlanIT.API.Models.DTOs;

public class UserDTO
{
    public UserDTO(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;


}
