using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Repositories;

public class InviteRepository : IRepository<Invite>
{
    private readonly PlanITDbContext _dbContext;
    private readonly PaginationUtility _pagination;

    public InviteRepository(PlanITDbContext dbContext, PaginationUtility pagination)
    {
        _dbContext = dbContext;
        _pagination = pagination;
    }


    // Legger til ny invitasjon
    public async Task<Invite?> AddAsync(Invite newInvite)
    {
        var addedInviteEntry = await _dbContext.Invites.AddAsync(newInvite);
        await _dbContext.SaveChangesAsync();

        return addedInviteEntry?.Entity;
    }

    
    // Henter alle invitasjoner med paginering
    public async Task<ICollection<Invite>> GetAllAsync(int pageNr, int pageSize)
    {
        var pagination = new PaginationUtility(_dbContext);

        IQueryable<Invite> invitesQuery = _dbContext.Invites.OrderBy(x => x.Id);
        return await _pagination.GetPageAsync(invitesQuery, pageNr, pageSize);
    }


    // Henter invitasjon basert på ID
    public async Task<Invite?> GetByIdAsync(int inviteId)
    {
        var existingInvite = await _dbContext.Invites.FirstOrDefaultAsync(x => x.Id == inviteId);
        return existingInvite is null ? null : existingInvite;
    }

    // Oppdaterer invitasjon
    public async Task<Invite?> UpdateAsync(int inviteId, Invite updatedInvite)
    {
        var inviteRows = await _dbContext.Invites.Where(x => x.Id == inviteId)
            .ExecuteUpdateAsync(setters => setters
            .SetProperty(x => x.Name, updatedInvite.Name)
            .SetProperty(x => x.Email, updatedInvite.Email)
            .SetProperty(x => x.Coming, updatedInvite.Coming));

        await _dbContext.SaveChangesAsync();

        if (inviteRows == 0) return null;
        return updatedInvite;
    }


    // Sletter invitasjon
    public async Task<Invite?> DeleteAsync(int inviteId)
    {
        var existingInvite = await _dbContext.Invites.FirstOrDefaultAsync(x => x.Id == inviteId);
        if (existingInvite == null) return null;

        var deletedInvite = _dbContext.Invites.Remove(existingInvite);
        await _dbContext.SaveChangesAsync();

        return deletedInvite?.Entity;
    }
}