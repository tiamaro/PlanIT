﻿using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PlanITDbContext _dbContext;
    private readonly PaginationUtility _pagination;

    public UserRepository(PlanITDbContext dbContext, PaginationUtility pagination)
    {
        _dbContext = dbContext;
        _pagination = pagination;
    }

    
    public async Task<User?> AddAsync(User newUser)
    {
        var addedUserEntry = await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();

        return addedUserEntry?.Entity;
    }

   
    public async Task<ICollection<User>> GetAllAsync(int pageNr, int pageSize)
    {
        IQueryable<User> usersQuery = _dbContext.Users.OrderBy(x => x.Id);
        return await _pagination.GetPageAsync(usersQuery, pageNr, pageSize);
    }

    
    public async Task<User?> GetByIdAsync(int userId)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        return existingUser is null ? null : existingUser;
    }

    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

        return user;
    }


    
    public async Task<User?> UpdateAsync(int userId, User updatedUser)
    {
        var exsistingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (exsistingUser == null) return null;

        exsistingUser.Name = string.IsNullOrEmpty(updatedUser.Name) ? exsistingUser.Name : updatedUser.Name;
        exsistingUser.Email = string.IsNullOrEmpty(updatedUser.Email) ? exsistingUser.Email : updatedUser.Email;

        await _dbContext.SaveChangesAsync();
        return exsistingUser;

    }


    
    public async Task<User?> DeleteAsync(int userId)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (existingUser == null) return null;

        var deletedUser = _dbContext.Users.Remove(existingUser);
        await _dbContext.SaveChangesAsync();

        return deletedUser?.Entity;
    }

}