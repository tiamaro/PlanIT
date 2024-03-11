using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace PlanIT.API.Models.Entities;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string HashedPassword { get; set; } = string.Empty;

    [Required]
    public string salt { get; set; } = string.Empty;




    public virtual ICollection<Event> Events { get; set; } = new HashSet<Event>();

    public virtual ICollection<ToDo> Todos { get; set; } = new HashSet<ToDo>();

    public virtual ICollection<Dinner> Dinners { get; set; } = new HashSet<Dinner>();

    public virtual ICollection<ShoppingList> ShoppingList { get; set; } = new HashSet<ShoppingList>();


}
