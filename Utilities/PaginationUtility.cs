using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;

namespace PlanIT.API.Utilities;

// Utility class for data pagination
public class PaginationUtility
{
    private readonly PlanITDbContext _dbContext;

   
    public PaginationUtility(PlanITDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Retrieves a page of data based on page index and size
    // Returns a collection of data for the specified page
    public async Task<ICollection<T>> GetPageAsync<T>(IQueryable<T> query, int pageNr, int pageSize)
    {
        // Henter totalt antall elementer i spørringen
        int totalItemsCount = await query.CountAsync();

        // Calculate the total number of pages based on the number of items per page
        int totalPages = (int)Math.Ceiling((double)totalItemsCount / pageSize);

        // Validate the specified page number
        if (pageNr < 1 || pageNr > totalPages)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNr), "Ugyldig sidetall");
        }

        // Fetch and return the data for the specified page
        return await query.Skip((pageNr - 1) * pageSize).Take(pageSize).ToListAsync();
    }
}