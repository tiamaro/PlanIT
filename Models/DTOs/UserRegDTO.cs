namespace PlanIT.API.Models.DTOs;

public class UserRegDTO
{
    public UserRegDTO(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;


}
