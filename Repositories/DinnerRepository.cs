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

    public async Task<Dinner?> AddAsync(Dinner newDinner)
    {
        var addedDinner = await _dbContext.Dinners.AddAsync(newDinner);
        await _dbContext.SaveChangesAsync();
        return addedDinner.Entity;
    }

    public async Task<ICollection<Dinner>> GetAllAsync(int pageNr, int pageSize)
    {
        IQueryable<Dinner> dinnersQuery = _dbContext.Dinners.OrderBy(x => x.Id);
        return await _pagination.GetPageAsync(dinnersQuery, pageNr, pageSize);
    }

    public async Task<Dinner?> GetByIdAsync(int dinnerId)
    {
        var exsistingDinner = await _dbContext.Dinners.FirstOrDefaultAsync(x => x.Id == dinnerId);
        return exsistingDinner is null ? null : exsistingDinner;
    }

    public async Task<Dinner?> UpdateAsync(int id, Dinner updatedDinner)
    {
        var exsistingDinner = await _dbContext.Dinners.FirstOrDefaultAsync(x => x.Id == id);
        if (exsistingDinner == null) return null;

        exsistingDinner.Name = string.IsNullOrEmpty(updatedDinner.Name) ? exsistingDinner.Name : updatedDinner.Name;
        // exsistingDinner.Date = updatedDinner.Date != DateOnly.MinValue ? updatedDinner.Date : exsistingDinner.Date;

        await _dbContext.SaveChangesAsync();
        return exsistingDinner;

    }



    public async Task<Dinner?> DeleteAsync(int dinnerId)
    {
        var existingDinner = await _dbContext.Dinners.FindAsync(dinnerId);
        if (existingDinner == null) return null;

        var deletedDinner = _dbContext.Dinners.Remove(existingDinner);
        await _dbContext.SaveChangesAsync();

        return deletedDinner?.Entity;
    }
}
