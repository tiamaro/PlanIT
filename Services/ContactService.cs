using Org.BouncyCastle.Cms;
using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Services;

// Service class for handling contacts information.
// Exceptions are caught by a middleware: HandleExceptionFilter

public class ContactService : IService<ContactDTO>
{
    private readonly IMapper<Contact, ContactDTO> _contactMapper;
    private readonly IRepository<Contact> _contactRepository;
    private readonly LoggerService _logger;

    public ContactService(IMapper<Contact, ContactDTO> contactMapper,
        IRepository<Contact> contactRepository,
        LoggerService loggerService)
    {
        _contactMapper = contactMapper;
        _contactRepository = contactRepository;
        _logger = loggerService;
    }

   
    public async Task<ContactDTO?> CreateAsync(int userIdFromToken,ContactDTO newContactDTO)
    {
        _logger.LogCreationStart("contact");

        
        var newContact = _contactMapper.MapToModel(newContactDTO);
        newContact.UserId = userIdFromToken;

        
        var addedContact = await _contactRepository.AddAsync(newContact);
        if (addedContact == null)
        {
            _logger.LogCreationFailure("contact");
            throw ExceptionHelper.CreateOperationException("contact", 0, "create");
        }

        
        _logger.LogOperationSuccess("created", "contact", addedContact.Id);

        
        return _contactMapper.MapToDTO(addedContact);
        
    }

    
    public async Task<ICollection<ContactDTO>> GetAllAsync(int userIdFromToken, int pageNr, int pageSize)
    {
        _logger.LogDebug($"Retrieving all contacts for user {userIdFromToken}.");

        var contactsFromRepository = await _contactRepository.GetAllAsync(1, 10);


        
        var filteredContacts = contactsFromRepository.Where(contact => contact.UserId == userIdFromToken);

        

        return filteredContacts.Select(contactEntity => _contactMapper.MapToDTO(contactEntity)).ToList();

        
    }


    
    public async Task<ContactDTO?> GetByIdAsync(int userIdFromToken, int contactId)
    {
        _logger.LogDebug($"Retrieving contact with ID {contactId} for user {userIdFromToken}.");
        var contactFromRepository = await _contactRepository.GetByIdAsync(contactId);
        if (contactFromRepository == null)
        {
            _logger.LogNotFound("contact", contactId);
            throw ExceptionHelper.CreateNotFoundException("contact", contactId);
        }

        if (contactFromRepository.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("contact", contactId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("contact", contactId) ;
        }

        _logger.LogOperationSuccess("retrieved", "contact", contactId);
        return _contactMapper.MapToDTO(contactFromRepository);

    }

    
    public async Task<ContactDTO?> UpdateAsync(int userIdFromToken, int contactId, ContactDTO contactDTO)
    {
        _logger.LogDebug($"Updating dinner with ID {contactId} for user {userIdFromToken}.");


        var exsistingContact = await _contactRepository.GetByIdAsync(contactId);
        if (exsistingContact == null)
        {
            _logger.LogNotFound("contact", contactId);
            throw ExceptionHelper.CreateNotFoundException("contact", contactId);
        }


        
        if (exsistingContact.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("contact", contactId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("contact", contactId);

        }


        
        var contactToUpdate = _contactMapper.MapToModel(contactDTO);
        contactToUpdate.Id = contactId;


        
        var updatedContact = await _contactRepository.UpdateAsync(contactId, contactToUpdate);
        if (updatedContact == null)
        {
            _logger.LogOperationFailure("update", "contact", contactId);
            throw ExceptionHelper.CreateOperationException("contact", contactId, "update");
        }


        _logger.LogOperationSuccess("updated", "contact", contactId);
        return _contactMapper.MapToDTO(updatedContact);


    }


    public async Task<ContactDTO?> DeleteAsync(int userIdFromToken, int contactId)
    {
        _logger.LogDebug($"Deleting dinner with ID {contactId} for user {userIdFromToken}.");


        var contactToDelete = await _contactRepository.GetByIdAsync(contactId);
        if (contactToDelete == null)
        {
            _logger.LogNotFound("contact", contactId);
            throw ExceptionHelper.CreateNotFoundException("contact", contactId);
        }


        
        if (contactToDelete.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("contact", contactId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("contact", contactId);

        }


        
        var deletedContact = await _contactRepository.DeleteAsync(contactId);
        if (deletedContact == null)
        {
            _logger.LogOperationFailure("delete", "contact", contactId);
            throw ExceptionHelper.CreateOperationException("contact", contactId, "delete");

        }

        _logger.LogOperationSuccess("deleted", "contact", contactId);
        return _contactMapper.MapToDTO(contactToDelete);

    }
}
