﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlanIT.API.Models.Entities;

public class ImportantDate
{

    [Key]
    public int Id { get; set; }

    [ForeignKey("UserId")]
    public int UserId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateOnly Date { get; set; }







    public virtual User? User { get; set; }



}
