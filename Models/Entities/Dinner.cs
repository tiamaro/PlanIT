﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanIT.API.Models.Entities;

public class Dinner
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("UserId")]
    public int UserId { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;




    public virtual User? User { get; set; }
}