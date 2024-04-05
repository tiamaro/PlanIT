using Microsoft.EntityFrameworkCore;
using PlanIT.API.Models.Entities;

namespace PlanIT.API.Data;

public class PlanITDbContext : DbContext
{
    // For konfigurering av databasen
    public PlanITDbContext(DbContextOptions<PlanITDbContext> options)
        : base(options) { }

    // Tabeller til databasen
    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Invite> Invites { get; set; }
    public DbSet<Dinner> Dinners { get; set; }
    public DbSet<ToDo> Todos { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    public DbSet<ImportantDate> ImportantDates { get; set; }

}