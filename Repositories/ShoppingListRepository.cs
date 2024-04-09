using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Repositories;

public class ShoppingListRepository : IRepository<ShoppingList>
{
    private readonly PlanITDbContext _dbContext;
    private readonly PaginationUtility _pagination;

    public ShoppingListRepository(PlanITDbContext dbContext, PaginationUtility pagination)
    {
        _dbContext = dbContext;
        _pagination = pagination;
    }


    // Legger til ny handleliste
    public async Task<ShoppingList?> AddAsync(ShoppingList newShoppingList)
    {
        var addedShoppingListEntry = await _dbContext.ShoppingLists.AddAsync(newShoppingList);
        await _dbContext.SaveChangesAsync();

        return addedShoppingListEntry?.Entity;
    }

    // Henter alle handlelister med paginering
    public async Task<ICollection<ShoppingList>> GetAllAsync(int pageNr, int pageSize)
    {
        var pagination = new PaginationUtility(_dbContext);

        IQueryable<ShoppingList> shoppingListQuery = _dbContext.ShoppingLists.OrderBy(x => x.Id);
        return await _pagination.GetPageAsync(shoppingListQuery, pageNr, pageSize);
    }


    // Henter handelister basert på ID
    public async Task<ShoppingList?> GetByIdAsync(int shoppingListId)
    {
        var existingShoppingList = await _dbContext.ShoppingLists.FirstOrDefaultAsync(x => x.Id == shoppingListId);
        return existingShoppingList is null ? null : existingShoppingList;
    }

    // Oppdaterer handleliste
    public async Task<ShoppingList?> UpdateAsync(int id, ShoppingList updatedShoppingList)
    {
        var exsistingShoppingList = await _dbContext.ShoppingLists.FirstOrDefaultAsync(x => x.Id == id);
        if (exsistingShoppingList == null) return null;

        exsistingShoppingList.Name = string.IsNullOrEmpty(updatedShoppingList.Name) ? exsistingShoppingList.Name : updatedShoppingList.Name;

        await _dbContext.SaveChangesAsync();
        return exsistingShoppingList;


    }


    // Slettter handeliste
    public async Task<ShoppingList?> DeleteAsync(int shoppingListId)
    {
        var existingShoppingList = await _dbContext.ShoppingLists.FirstOrDefaultAsync(x => x.Id == shoppingListId);
        if (existingShoppingList == null) return null;

        var deletedShoppingList = _dbContext.ShoppingLists.Remove(existingShoppingList);
        await _dbContext.SaveChangesAsync();

        return deletedShoppingList?.Entity;
    }
}