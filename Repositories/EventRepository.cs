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

    
    public async Task<Event?> AddAsync(Event newEvent)
    {
        var addedEvent = await _dbContext.Events.AddAsync(newEvent);
        await _dbContext.SaveChangesAsync();

        return addedEvent?.Entity;
    }



  
    public async Task<ICollection<Event>> GetAllAsync(int pageNr, int pageSize)
    {
        var pagination = new PaginationUtility(_dbContext);

        IQueryable<Event> eventsQuery = _dbContext.Events.OrderBy(x => x.Id);
        return await _pagination.GetPageAsync(eventsQuery, pageNr, pageSize);

    }


    
    public async Task<Event?> GetByIdAsync(int eventId)
    {
        var exsistingEvent = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id == eventId);
        return exsistingEvent is null ? null : exsistingEvent;
    }

    
    public async Task<Event?> UpdateAsync(int id, Event updatedEvent)
    {
        var exsistingEvent = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == id);
        if (exsistingEvent == null) return null;

        exsistingEvent.Name = string.IsNullOrEmpty(updatedEvent.Name) ? exsistingEvent.Name : updatedEvent.Name;
        exsistingEvent.Date = updatedEvent.Date != DateOnly.MinValue ? updatedEvent.Date : exsistingEvent.Date;
        exsistingEvent.Time = updatedEvent.Time != TimeOnly.MinValue ? updatedEvent.Time : exsistingEvent.Time;
        exsistingEvent.Location = string.IsNullOrEmpty(updatedEvent.Location) ? exsistingEvent.Location : updatedEvent.Location;

        await _dbContext.SaveChangesAsync();
        return exsistingEvent;


    }


    
    public async Task<Event?> DeleteAsync(int eventId)
    {
        var eventById = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
        if (eventById == null) return null;

        var deletedEvent = _dbContext.Events.Remove(eventById);
        await _dbContext.SaveChangesAsync();

        return deletedEvent.Entity;

    }
}