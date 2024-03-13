namespace PlanIT.API.Models.DTOs;

public class InviteDTO
{
    public InviteDTO(int id, int eventId, string name, string email, bool coming)
    {
        Id = id;
        EventId = eventId;
        Name = name;
        Email = email;
        Coming = coming;
    }

    public int Id { get; set; }

    public int EventId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool Coming { get; set; }
}
