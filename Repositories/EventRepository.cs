using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Repositories;

public class EventRepository : IRepository<Event>
{
    private readonly PlanITDbContext _dbContext;
    private readonly PaginationUtility _pagination;

    public EventRepository(PlanITDbContext dbContext, PaginationUtility pagination)
    {
        _dbContext = dbContext;
        _pagination = pagination;
    }

    // Legger til nytt arrangement
    public async Task<Event?> AddAsync(Event newEvent)
    {
        var addedEvent = await _dbContext.Events.AddAsync(newEvent);
        await _dbContext.SaveChangesAsync();

        return addedEvent?.Entity;
    }



    // Henter alle arrangementer med paginering 
    public async Task<ICollection<Event>> GetAllAsync(int pageNr, int pageSize)
    {
        var pagination = new PaginationUtility(_dbContext);

        IQueryable<Event> eventsQuery = _dbContext.Events.OrderBy(x => x.Id);
        return await _pagination.GetPageAsync(eventsQuery, pageNr, pageSize);

    }


    // Henter arrangementer basert på ID
    public async Task<Event?> GetByIdAsync(int eventId)
    {
        var eventById = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
        return eventById is null ? null : eventById;
    }

    // Oppdaterer arrangementsinformasjon
    public async Task<Event?> UpdateAsync(int id, Event updatedEvent)
    {
        var exsistingEvent = await _dbContext.Dinners.FirstOrDefaultAsync(x => x.Id == id);
        if (exsistingEvent == null) return null;

        exsistingEvent.Name = string.IsNullOrEmpty(updatedDinner.Name) ? exsistingEvent.Name : updatedDinner.Name;
        exsistingEvent.Date = updatedDinner.Date != DateOnly.MinValue ? updatedDinner.Date : exsistingEvent.Date;

        await _dbContext.SaveChangesAsync();
        return exsistingEvent;


    }

    //var exsistingEvent = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == id);
    //    if (exsistingEvent == null) return null;

    //    exsistingEvent.Name = string.IsNullOrEmpty(updatedEvent.Name) ? exsistingEvent.Name : updatedEvent.Name;

    //    await _dbContext.SaveChangesAsync();
    //    return exsistingEvent;




    // Sletter Arrangement
    public async Task<Event?> DeleteAsync(int eventId)
    {
        var eventById = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
        if (eventById == null) return null;

        var deletedEvent = _dbContext.Events.Remove(eventById);
        await _dbContext.SaveChangesAsync();

        return deletedEvent?.Entity;

    }
}