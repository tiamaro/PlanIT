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
    public DbSet<Contact> Contacts { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ensures unique email addresses for user, contact and invite. 
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Contact>().HasIndex(c => c.Email).IsUnique();
        modelBuilder.Entity<Invite>().HasIndex(i => i.Email).IsUnique();

        // uncomment before new migrations, replace unique invite email with this to make sure
        // that invite email is unique per event.
       // modelBuilder.Entity<Invite>().HasIndex(i => new { i.Email, i.EventId }).IsUnique();


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