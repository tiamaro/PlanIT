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

    public int Id { get; init; }

    public int EventId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public bool Coming { get; init; }


    //public bool IsReminderSent { get; set; }
}
