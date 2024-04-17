using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanIT.API.Models.Entities;

public class Contact
{

    [Key]
    public int Id { get; set; }

    [ForeignKey("UserId")]
    public int UserId { get; set; }


    [Required]
    public string Name { get; set; } = string.Empty;


    [EmailAddress]
    public string Email { get; set; } = string.Empty;



    public virtual User? User { get; set; }


}
