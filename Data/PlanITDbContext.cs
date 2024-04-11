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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "perhansen@mail.com",
                Name = "Per",
                HashedPassword = BCrypt.Net.BCrypt.HashPassword("Per123!", salt),
                Salt = salt
            });

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 2,
                Email = "olanordmann@mail.com",
                Name = "Ola",
                HashedPassword = BCrypt.Net.BCrypt.HashPassword("Per123!", salt),
                Salt = salt
            });

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 3,
                Email = "karinordmann@mail.com",
                Name = "Kari",
                HashedPassword = BCrypt.Net.BCrypt.HashPassword("Kari123!", salt),
                Salt = salt
            });


    }

}