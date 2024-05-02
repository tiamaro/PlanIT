using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Repositories;

public class InviteRepository : IInviteRepository
{
    private readonly PlanITDbContext _dbContext;
    private readonly PaginationUtility _pagination;

    public InviteRepository(PlanITDbContext dbContext, PaginationUtility pagination)
    {
        _dbContext = dbContext;
        _pagination = pagination;
    }


    
    public async Task<Invite?> AddAsync(Invite newInvite)
    {
        var addedInviteEntry = await _dbContext.Invites.AddAsync(newInvite);
        await _dbContext.SaveChangesAsync();

        return addedInviteEntry?.Entity;
    }


    
    public async Task<ICollection<Invite>> GetAllAsync(int pageNr, int pageSize)
    {
        var pagination = new PaginationUtility(_dbContext);

        IQueryable<Invite> invitesQuery = _dbContext.Invites.OrderBy(x => x.Id);
        return await _pagination.GetPageAsync(invitesQuery, pageNr, pageSize);
    }


    
    public async Task<Invite?> GetByIdAsync(int inviteId)
    {
        var existingInvite = await _dbContext.Invites.FirstOrDefaultAsync(x => x.Id == inviteId);
        return existingInvite is null ? null : existingInvite;
    }


    public async Task<ICollection<Invite>> GetInvitesByEventIdAsync(int eventId, int pageNr, int pageSize)
    {
        IQueryable<Invite> query = _dbContext.Invites
            .Where(invite => invite.EventId == eventId)
            .OrderBy(invite => invite.Id);

        return await _pagination.GetPageAsync(query, pageNr, pageSize);
    }


    public async Task<Invite?> UpdateAsync(int inviteId, Invite updatedInvite)
    {
        var exsistingInvite = await _dbContext.Invites.FirstOrDefaultAsync(x => x.Id == inviteId);
        if (exsistingInvite == null) return null;

        exsistingInvite.Name = string.IsNullOrEmpty(updatedInvite.Name) ? exsistingInvite.Name : updatedInvite.Name;
        exsistingInvite.Email = string.IsNullOrEmpty(updatedInvite.Email) ? exsistingInvite.Email : updatedInvite.Email;
        exsistingInvite.Coming = updatedInvite.Coming;

        await _dbContext.SaveChangesAsync();
        return exsistingInvite;

    }


   
    public async Task<Invite?> DeleteAsync(int inviteId)
    {
        var existingInvite = await _dbContext.Invites.FirstOrDefaultAsync(x => x.Id == inviteId);
        if (existingInvite == null) return null;

        var deletedInvite = _dbContext.Invites.Remove(existingInvite);
        await _dbContext.SaveChangesAsync();

        return deletedInvite?.Entity;
    }
}