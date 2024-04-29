using Microsoft.EntityFrameworkCore;
using PlanIT.API.Models.Entities;

namespace PlanIT.API.Data;

public class PlanITDbContext : DbContext
{
    
    public PlanITDbContext(DbContextOptions<PlanITDbContext> options)
        : base(options) { }

    
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

        // Ensures each user has a unique email address in the User table
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();


        // Ensures that the email for a contact is unique per user in the Contact table
        modelBuilder.Entity<Contact>()
            .HasIndex(c => new { c.UserId, c.Email }) 
            .IsUnique();


        // Ensures that invites are unique per event based on the email in the Invite table
        modelBuilder.Entity<Invite>()
            .HasIndex(i => new { i.Email, i.EventId })
            .IsUnique();



       // Test data
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


        modelBuilder.Entity<Event>().HasData(
            new Event
            {
                Id = 1,
                UserId = 1,
                Name = "Birthday Party",
                Location = "at home",
                Time = new TimeOnly(18, 30, 0),
                Date = new DateOnly(2022, 06, 06)

            });

        modelBuilder.Entity<ToDo>().HasData(
            new ToDo
            {
                Id = 1,
                UserId = 1,
                Name = "Clean the car"
                
            });

        modelBuilder.Entity<ShoppingList>().HasData(
            new ShoppingList
            {
                Id = 1,
                UserId = 1,
                Name = "Milk"

            });

        modelBuilder.Entity<Invite>().HasData(
            new Invite
            {
                Id = 1,
                EventId = 1,
                Name = "Kari Nordmann",
                Email = "kari@mail.com",
                Coming = true,
                IsReminderSent = true
                

            });

        

        modelBuilder.Entity<ImportantDate>().HasData(
            new ImportantDate
            {
                Id = 1,
                UserId = 1,
                Name = "National Day",
                Date = new DateOnly(2022, 05, 17)

            });

        modelBuilder.Entity<Dinner>().HasData(
           new Dinner
           {
               Id = 1,
               UserId = 1,
               Name = "Pizza",
               Date = new DateOnly(2022, 05, 02)

           });




    }

}