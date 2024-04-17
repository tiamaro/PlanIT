namespace PlanIT.API.Models.DTOs;

public class ContactDTO
{
    public ContactDTO(int id, int userId, string name, string email)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Email = email;
    }

    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;



}
