namespace PlanIT.API.Models.DTOs;

public class UserRegDTO
{
    public UserRegDTO(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public string Name { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;


}
