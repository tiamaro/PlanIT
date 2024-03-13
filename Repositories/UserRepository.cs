using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;

namespace PlanIT.API.Repositories;

public class UserRepository : IRepository<User>
{
    private readonly PlanITDbContext _dbContext;

    public UserRepository(PlanITDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Legger til ny bruker i databasen
    public async Task<User?> AddAsync(User newUser)
    {
        var addedUserEntry = await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();

        return addedUserEntry?.Entity;
    }


    // Henter alle brukere
    public async Task<ICollection<User>> GetAllAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }


    // Henter bruker basert på ID
    public async Task<User?> GetByIdAsync(int userId)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        return existingUser is null ? null : existingUser;
    }

   
    // Oppdaterer bruker
    public async Task<User?> UpdateAsync(int userId, User updatedUser)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (existingUser == null) return null;

        // Oppdaterer feltene med de nye verdiene
        existingUser.Name = string.IsNullOrEmpty(updatedUser.Name) ? existingUser.Name : updatedUser.Name;
        existingUser.Email = string.IsNullOrEmpty(updatedUser.Email) ? existingUser.Email : updatedUser.Email;

        await _dbContext.SaveChangesAsync();
        return existingUser;
    }


    // Sletter bruker
    public async Task<User?> DeleteAsync(int userId)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (existingUser == null) return null;

        var deletedUser = _dbContext.Users.Remove(existingUser);
        await _dbContext.SaveChangesAsync();

        return deletedUser?.Entity;
    }
}