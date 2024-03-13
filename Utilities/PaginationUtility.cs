using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;

namespace PlanIT.API.Utilities;

// Utility-klasse for paginering av data
public class PaginationUtility
{
    private readonly PlanITDbContext _dbContext;

    // Konstruktør for PaginationUtility-klasse som tar imot en databasekontekst
    public PaginationUtility(PlanITDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Henter en side av data basert på sideindeks og størrelse
    // Returnerer En kolleksjon av data for den angitte siden
    public async Task<ICollection<T>> GetPageAsync<T>(IQueryable<T> query, int pageNr, int pageSize)
    {
        // Henter totalt antall elementer i spørringen
        int totalItemsCount = await query.CountAsync();

        // Beregner totalt antall sider basert på antall elementer per side
        int totalPages = (int)Math.Ceiling((double)totalItemsCount / pageSize);

        // Sjekker om den angitte siden eksisterer
        if (pageNr < 1 || pageNr > totalPages)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNr), "Ugyldig sidetall");
        }

        // Henter data for den angitte siden basert på sideindeks og størrelse og
        // returnerer dataene for den angitte siden
        return await query.Skip((pageNr - 1) * pageSize).Take(pageSize).ToListAsync();
    }
}