using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Repositories;

public class DinnerRepository : IRepository<Dinner>
{
    private readonly PlanITDbContext _dbContext;
    private readonly PaginationUtility _pagination;

    public DinnerRepository(PlanITDbContext dbContext, PaginationUtility pagination)
    {
        _dbContext = dbContext;
        _pagination = pagination;
    }

    public async Task<Dinner?> AddAsync(Dinner dinner)
    {
        var addedDinner = await _dbContext.Dinners.AddAsync(dinner);
        await _dbContext.SaveChangesAsync();
        return addedDinner.Entity;
    }

    public async Task<ICollection<Dinner>> GetAllAsync(int pageNr, int pageSize)
    {
        IQueryable<Dinner> dinnersQuery = _dbContext.Dinners.OrderBy(x => x.Id);
        return await _pagination.GetPageAsync(dinnersQuery, pageNr, pageSize);
    }

    public async Task<Dinner?> GetByIdAsync(int id)
    {
        return await _dbContext.Dinners.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Dinner?> UpdateAsync(int id, Dinner updatedDinner)
    {
        var existingDinner = await _dbContext.Dinners.FindAsync(id);
        if (existingDinner == null)
        {
            return null;
        }

        _dbContext.Entry(existingDinner).CurrentValues.SetValues(updatedDinner);
        await _dbContext.SaveChangesAsync();
        return updatedDinner;
    }

    public async Task<Dinner?> DeleteAsync(int id)
    {
        var existingDinner = await _dbContext.Dinners.FindAsync(id);
        if (existingDinner == null)
        {
            return null;
        }

        _dbContext.Dinners.Remove(existingDinner);
        await _dbContext.SaveChangesAsync();
        return existingDinner;
    }
}
