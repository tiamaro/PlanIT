using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PlanIT.API.Models.Entities;

public class Event
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("UserId")]
    public int UserId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public TimeOnly Time { get; set; } 

    [Required]
    public string Location { get; set; } = string.Empty;



    public virtual User? User { get; set; }
    public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();
}
