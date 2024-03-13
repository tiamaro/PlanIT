using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;

namespace PlanIT.API.Repositories;

public class EventRepository : IRepository<Event>
{
    private readonly PlanITDbContext _context;

    public EventRepository(PlanITDbContext context)
    {
        _context = context;
    }

    public async Task<Event?> AddAsync(Event entity)
    {
        var addedEvent = await _context.Events.AddAsync(entity);
        await _context.SaveChangesAsync();

        if (addedEvent == null) return null;
        return addedEvent.Entity;
    }

    public async Task<Event?> DeleteAsync(int id)
    {
        var eventById = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
        if (eventById == null) return null;

        var deletedEvent = _context.Events.Remove(eventById);
        await _context.SaveChangesAsync();
        if (deletedEvent == null) return null;
        return deletedEvent.Entity;

    }


    // paginering ? 
    public async Task<ICollection<Event>> GetAllAsync()
    {
        var eventsCount = _context.Events.Count();
        if (eventsCount == 0)
        {
            return Enumerable.Empty<Event>().ToList();

        }

        return await _context.Events.ToListAsync();

    }

    public async Task<Event?> GetByIdAsync(int id)
    {
        var eventById = await _context.Events.FirstOrDefaultAsync(x =>x.Id == id);
        if (eventById == null) return null;
        return eventById;
    }

    public async Task<Event?> UpdateAsync(int id, Event entity)
    {    

        var eventRows = await _context.Events.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters => setters
            .SetProperty(x => x.Name, entity.Name)
            .SetProperty(x => x.Time, entity.Time)
            .SetProperty(x => x.Location, entity.Location)
            .SetProperty(x => x.Date, entity.Date));

        await _context.SaveChangesAsync();

        if (eventRows == 0) return null;
        return entity;

    }
}

