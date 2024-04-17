using Microsoft.EntityFrameworkCore;
using PlanIT.API.Data;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Repositories;

public class ContactRepository : IRepository<Contact>
{
    private readonly PlanITDbContext _dbContext;
    private readonly PaginationUtility _pagination;

    public ContactRepository(PlanITDbContext dbContext, PaginationUtility pagination)
    {
        _dbContext = dbContext;
        _pagination = pagination;
    }

    public async Task<Contact?> AddAsync(Contact newContact)
    {
        var addedContact = await _dbContext.Contacts.AddAsync(newContact);
        await _dbContext.SaveChangesAsync();
        return addedContact.Entity;
    }

   
    public async Task<ICollection<Contact>> GetAllAsync(int pageNr, int pageSize)
    {
        IQueryable<Contact> contactsQuery = _dbContext.Contacts.OrderBy(x => x.Id);
        return await _pagination.GetPageAsync(contactsQuery, pageNr, pageSize);
    }

    public async Task<Contact?> GetByIdAsync(int contactId)
    {
        var exsistingContact = await _dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == contactId);
        return exsistingContact is null ? null : exsistingContact;


    }

    public async Task<Contact?> UpdateAsync(int contactId, Contact updatedContact)
    {
        var exsistingContact = await _dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == contactId);
        if (exsistingContact == null) return null;

        exsistingContact.Name = string.IsNullOrEmpty(updatedContact.Name) ? exsistingContact.Name : updatedContact.Name;
        exsistingContact.Email = string.IsNullOrEmpty(updatedContact.Email) ? exsistingContact.Email : updatedContact.Email;

        await _dbContext.SaveChangesAsync();
        return exsistingContact;


    }

    public async Task<Contact?> DeleteAsync(int contactId)
    {
        var contactById = await _dbContext.Contacts.FirstOrDefaultAsync(x =>x.Id == contactId);
        if (contactById == null) return null;

        var deletedContact = _dbContext.Contacts.Remove(contactById);
        await _dbContext.SaveChangesAsync();

        return deletedContact.Entity;
        
    }
}
