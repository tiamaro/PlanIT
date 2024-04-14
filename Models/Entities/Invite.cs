using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanIT.API.Models.Entities;

public class Invite
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("EventId")]
    public int EventId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public bool IsReminderSent { get; set; }


    [Required]
    public bool Coming { get; set; }


    public virtual Event? Event { get; set; }
}
