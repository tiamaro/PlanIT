using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;

namespace PlanIT.API.Repositories;

public class ImportantDateRepository : IRepository<ImportantDate>
{


    // Legger til ny ImportantDate
    public Task<ImportantDate?> AddAsync(ImportantDate entity)
    {
        throw new NotImplementedException();
    }

    // Henter alle ImportantDates med paginering 
    public Task<ICollection<ImportantDate>> GetAllAsync(int pageNr, int pageSize)
    {
        throw new NotImplementedException();
    }


    // Henter ImportantDate basert på ID
    public Task<ImportantDate?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }


    // Oppdaterer ImportantDate
    public Task<ImportantDate?> UpdateAsync(int id, ImportantDate entity)
    {
        throw new NotImplementedException();
    }


    // Sletter ImportantDate
    public Task<ImportantDate?> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
