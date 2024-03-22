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


    // Legger til ny ImportantDate
    public async Task<ImportantDate?> AddAsync(ImportantDate newImportantDate)
    {
        var addedImportantDate = await _dbContext.ImportantDates.AddAsync(newImportantDate);
        await _dbContext.SaveChangesAsync();

        return addedImportantDate?.Entity;
    }

    // Henter alle ImportantDates med paginering 
    public  async Task<ICollection<ImportantDate>> GetAllAsync(int pageNr, int pageSize)
    {
        var pagination = new PaginationUtility(_dbContext);

        IQueryable<ImportantDate> importantDatesQuery = _dbContext.ImportantDates.OrderBy(x => x.Id);
        return await _pagination.GetPageAsync(importantDatesQuery, pageNr, pageSize);
    }


    // Henter ImportantDate basert på ID
    public async Task<ImportantDate?> GetByIdAsync(int importantDateId)
    {
        var importantDateById = await _dbContext.ImportantDates.FirstOrDefaultAsync(x => x.Id == importantDateId);
        return importantDateById is null ? null : importantDateById;
    }


    // Oppdaterer ImportantDate
    public async Task<ImportantDate?> UpdateAsync(int dateID, ImportantDate updatedImportantDate)
    {
        var importantDateRows = await _dbContext.ImportantDates.Where(x => x.Id == dateID)
            .ExecuteUpdateAsync(setters => setters
            .SetProperty(x => x.Name, updatedImportantDate.Name)
            .SetProperty(x => x.Date, updatedImportantDate.Date));

        await _dbContext.SaveChangesAsync();

        if (importantDateRows == 0) return null;
        return updatedImportantDate;
        
    }


    // Sletter ImportantDate
    public async Task<ImportantDate?> DeleteAsync(int importantDateId)
    {
        var importantDateById = await _dbContext.ImportantDates.FirstOrDefaultAsync( x => x.Id == importantDateId);
        if (importantDateById == null) return null;

        var deletedImportantDate = _dbContext.ImportantDates.Remove(importantDateById);
        await _dbContext.SaveChangesAsync();

        return deletedImportantDate?.Entity;
        
    }
}
