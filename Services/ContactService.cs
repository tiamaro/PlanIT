using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities;

namespace PlanIT.API.Services;

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

    // Oppretter en ny Contact basert på DTO fra klienten.
    public async Task<ContactDTO?> CreateAsync(ContactDTO newContactDTO)
    {
        _logger.LogCreationStart("contact");

        // Konverterer newContactDTO til Contact-modellen for lagring
        var newContact = _contactMapper.MapToModel(newContactDTO);

        // Forsøker å legge til ny Contact i databasen
        var addedContact = await _contactRepository.AddAsync(newContact);
        if (addedContact == null)
        {
            _logger.LogCreationFailure("contact");
            throw ExceptionHelper.CreateOperationException("contact", 0, "create");
        }

        // Logger at Contact ble vellykket opprettet med tilhørende ID
        _logger.LogOperationSuccess("created", "contact", addedContact.Id);

        // Returnerer Contact konvertert tilbake til DTO-format
        return _contactMapper.MapToDTO(addedContact);
        
    }

    // Henter alle Contacts med paginering
    public async Task<ICollection<ContactDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        // Henter Contacts fra repository med paginering
        var contactsFromRepository = await _contactRepository.GetAllAsync(1, 10);


        // Mapper Contactsinformasjon til contactsDTO-format
        var contactDTOs = contactsFromRepository.Select(contactEntity => _contactMapper.MapToDTO(contactEntity)).ToList();
        return contactDTOs;
    }


    // Henter en spesifikt Contact basert på dets ID og sjekker at brukeren har tilgang.
    public async Task<ContactDTO?> GetByIdAsync(int userIdFromToken, int contactId)
    {
        _logger.LogDebug($"Attempting to retrieve event with ID {contactId} for user ID {userIdFromToken}.");
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

    // Oppdaterer en eksisterende Contact og sikrer at brukeren har rettigheter til dette.
    public async Task<ContactDTO?> UpdateAsync(int userIdFromToken, int contactId, ContactDTO contactDTO)
    {
        _logger.LogDebug($"Attempting to update contact with ID {contactId} for user ID {userIdFromToken}.");

        // Forsøker å hente en Contact basert på ID for å sikre at det faktisk eksisterer før oppdatering.
        var exsistingContact = await _contactRepository.GetByIdAsync(contactId);
        if (exsistingContact == null)
        {
            _logger.LogNotFound("contact", contactId);
            throw ExceptionHelper.CreateNotFoundException("contact", contactId);
        }


        // Sjekker om brukeren som prøver å oppdatere Contact er den samme brukeren som opprettet det.
        if (exsistingContact.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("contact", contactId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("contact", contactId);

        }


        // Mapper til DTO og sørger for at ID forblir den samme under oppdateringen
        var contactToUpdate = _contactMapper.MapToModel(contactDTO);
        contactToUpdate.Id = contactId;


        // Prøver å oppdatere Contact i databasen
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
        _logger.LogDebug($"Attempting to delete contact with ID {contactId} for user ID {userIdFromToken}.");

        // Forsøker å hente en Contact basert på ID for å sikre at det faktisk eksisterer før sletting.
        var contactToDelete = await _contactRepository.GetByIdAsync(contactId);
        if (contactToDelete == null)
        {
            _logger.LogNotFound("contact", contactId);
            throw ExceptionHelper.CreateNotFoundException("contact", contactId);
        }


        // Sjekker om brukeren som prøver å slette Contact er den samme brukeren som opprettet det.
        if (contactToDelete.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("contact", contactId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("contact", contactId);

        }


        // Prøver å slette Contact fra databasen
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
