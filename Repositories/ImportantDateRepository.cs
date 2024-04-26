using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Repositories;

public class ImportantDateRepository : IRepository<ImportantDate>
{
    private readonly PlanITDbContext _dbContext;
    private readonly PaginationUtility _pagination;

    public ImportantDateRepository(PlanITDbContext dbContext, PaginationUtility pagination)
    {
        _dbContext = dbContext;
        _pagination = pagination;
    }


   
    public async Task<ImportantDate?> AddAsync(ImportantDate newImportantDate)
    {
        var addedImportantDate = await _dbContext.ImportantDates.AddAsync(newImportantDate);
        await _dbContext.SaveChangesAsync();

        return addedImportantDate?.Entity;
    }

    
    public async Task<ICollection<ImportantDate>> GetAllAsync(int pageNr, int pageSize)
    {
        var pagination = new PaginationUtility(_dbContext);

        IQueryable<ImportantDate> importantDatesQuery = _dbContext.ImportantDates.OrderBy(x => x.Id);
        return await _pagination.GetPageAsync(importantDatesQuery, pageNr, pageSize);
    }


    
    public async Task<ImportantDate?> GetByIdAsync(int importantDateId)
    {
        var importantDateById = await _dbContext.ImportantDates.FirstOrDefaultAsync(x => x.Id == importantDateId);
        return importantDateById is null ? null : importantDateById;
    }


   
    public async Task<ImportantDate?> UpdateAsync(int importantDateId, ImportantDate updatedImportantDate)
    {

        var exsistingDate = await _dbContext.ImportantDates.FirstOrDefaultAsync(x => x.Id == importantDateId);
        if (exsistingDate == null) return null;

        exsistingDate.Name = string.IsNullOrEmpty(updatedImportantDate.Name) ? exsistingDate.Name : updatedImportantDate.Name;
        exsistingDate.Date = updatedImportantDate.Date != DateOnly.MinValue ? updatedImportantDate.Date : exsistingDate.Date;

        await _dbContext.SaveChangesAsync();
        return exsistingDate;


    }


    
    public async Task<ImportantDate?> DeleteAsync(int importantDateId)
    {
        var importantDateById = await _dbContext.ImportantDates.FirstOrDefaultAsync(x => x.Id == importantDateId);
        if (importantDateById == null) return null;

        var deletedImportantDate = _dbContext.ImportantDates.Remove(importantDateById);
        await _dbContext.SaveChangesAsync();

        return deletedImportantDate?.Entity;

    }
}
