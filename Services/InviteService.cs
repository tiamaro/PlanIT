using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Services;

// Serviceklasse for håndtering av invitasjonsinformasjon.
// Exceptions blir fanget av en middleware: HandleExceptionFilter
public class InviteService : IService<InviteDTO>
{
    private readonly IMapper<Invite, InviteDTO> _inviteMapper;
    private readonly IRepository<Invite> _inviteRepository;
    private readonly IRepository<Event> _eventRepository;
    private readonly ILogger<InviteService> _logger;
    private readonly IMailService _mailService;

    public InviteService(IMapper<Invite, InviteDTO> inviteMapper,
        IRepository<Invite> inviteRepository,
        IRepository<Event> eventRepository,
        ILogger<InviteService> logger,
        IMailService mailService
        )
    {
        _inviteMapper = inviteMapper;
        _inviteRepository = inviteRepository;
        _eventRepository = eventRepository;
        _logger = logger;
        _mailService = mailService;
    }


    // Oppretter ny invitasjon
    public async Task<InviteDTO?> CreateAsync(InviteDTO newInviteDTO)
    {
        _logger.LogInformation("Starting to create a new invite.");

        // Mapper InviteDTO til Invite-modellen
        var newInvite = _inviteMapper.MapToModel(newInviteDTO);

        // Legger til den nye invitasjonen i databasen og henter resultatet
        var addedInvite = await _inviteRepository.AddAsync(newInvite);
        if (addedInvite == null)
        {
            _logger.LogWarning("Failed to create new invite.");
            throw new InvalidOperationException("The invite could not be created due to invalid data or state.");
        }
        //_mailService.SendInviteEmail(addedInvite);

        _logger.LogInformation("New event created successfully with ID {EventId}.", addedInvite.Id);
        return _inviteMapper.MapToDTO(addedInvite);
    }


    // *********NB!! FJERNE??? *************
    //
    // Henter alle invitasjoner
    public async Task<ICollection<InviteDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        // Henter invitasjonsinformasjon fra repository med paginering
        var invitesFromRepository = await _inviteRepository.GetAllAsync(1, 10);

        // Mapper invitasjonsdataene til inviteDTO-format
        var inviteDTOs = invitesFromRepository.Select(inviteEntity => _inviteMapper.MapToDTO(inviteEntity)).ToList();
        return inviteDTOs;
    }


    // Henter invitasjoner basert på inviteID og brukerID
    public async Task<InviteDTO?> GetByIdAsync(int userIdFromToken, int inviteId)
    {
        _logger.LogDebug("Attempting to retrieve invite with ID {InviteId} for user ID {UserId}.", inviteId, userIdFromToken);

        var inviteFromRepository = await _inviteRepository.GetByIdAsync(inviteId);

        if (inviteFromRepository == null)
        {
            _logger.LogWarning("Invite with ID {InviteId} not found.", inviteId);
            throw new KeyNotFoundException($"Invite with ID {inviteId} not found.");
        }

        // Henter event tilknyttet invite
        var eventFromRepository = await _eventRepository.GetByIdAsync(inviteFromRepository.EventId);
        if (eventFromRepository == null)
        {
            _logger.LogWarning("Event associated with invite ID {InviteId} not found.", inviteId);
            throw new KeyNotFoundException($"Event associated with invite ID {inviteId} not found.");
        }

        // Sjekker om userId er lik som den innloggede brukeren
        if (eventFromRepository.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized attempt to access invite with ID {InviteId} by user ID {UserId}.", inviteId, userIdFromToken);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to access invite ID {inviteId}.");
        }

        _logger.LogInformation("Invite with ID {inviteId} retrieved successfully for user ID {UserId}.", inviteId, userIdFromToken);
        return _inviteMapper.MapToDTO(inviteFromRepository);
    }


    // Oppdaterer invitasjon
    public async Task<InviteDTO?> UpdateAsync(int userIdFromToken, int inviteId, InviteDTO inviteDTO)
    {
        _logger.LogDebug("Attempting to update invite with ID {InviteId} for user ID {UserId}.", inviteId, userIdFromToken);

        // Sjekker om invitasjonen eksisterer
        var existingInvite = await _inviteRepository.GetByIdAsync(inviteId);
        if (existingInvite == null)
        {
            _logger.LogWarning("Invite with ID {InviteId} not found.", inviteId);
            throw new KeyNotFoundException($"Invite with ID {inviteId} not found.");
        }

        // Henter det assosierte arrangementet for å kunne sjekke mot riktig brukerID
        var associatedEvent = await _eventRepository.GetByIdAsync(existingInvite.EventId);
        if (associatedEvent == null)
        {
            _logger.LogError("Event associated with invite ID {InviteId} not found.", inviteId);
            throw new KeyNotFoundException($"Event associated with invite ID {inviteId} not found.");
        }

        if (associatedEvent.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized update attempt by User ID {UserId} on Invite ID {InviteId}", userIdFromToken, inviteId);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to update invite ID {inviteId}.");
        }

        // Mapper til DTO, og passer på at ID blir uforandret 
        var inviteToUpdate = _inviteMapper.MapToModel(inviteDTO);
        inviteToUpdate.Id = inviteId;

        // Oppdaterer invitasjonen
        var updatedInvite = await _inviteRepository.UpdateAsync(inviteId, inviteToUpdate);
        if (updatedInvite == null)
        {
            _logger.LogError("Failed to update Invite with ID {InviteId}", inviteId);
            throw new InvalidOperationException($"Failed to update invite with ID {inviteId}.");
        }

        _logger.LogInformation("Invite with ID {InviteId} updated successfully by User ID {UserId}.", inviteId, userIdFromToken);
        return _inviteMapper.MapToDTO(updatedInvite);
    }


    // Sletter invitasjon
    public async Task<InviteDTO?> DeleteAsync(int userIdFromToken, int inviteId)
    {
        _logger.LogDebug("Attempting to delete invite with ID {InviteId}.", inviteId);

      
        var inviteToDelete = await _inviteRepository.GetByIdAsync(inviteId);

        // Sjekker om invitasjonen eksisterer
        if (inviteToDelete == null)
        {
            _logger.LogWarning("Invite with ID {InviteId} not found.", inviteId);
            throw new KeyNotFoundException($"Invite with ID {inviteId} not found.");
        }

        // Henter det assosierte arrangementet for å kunne sjekke mot riktig brukerID
        var associatedEvent = await _eventRepository.GetByIdAsync(inviteToDelete.EventId);
        if (associatedEvent == null)
        {
            _logger.LogError("Event associated with invite ID {InviteId} not found.", inviteId);
            throw new KeyNotFoundException($"Event associated with invite ID {inviteId} not found.");
        }

        if (associatedEvent.UserId != userIdFromToken)
        {
            _logger.LogWarning("Unauthorized delete attempt by User ID {UserId} on Invite ID {InviteId}", userIdFromToken, inviteId);
            throw new UnauthorizedAccessException($"User ID {userIdFromToken} is not authorized to delete invite ID {inviteId}.");
        }

        // Sletter invitasjonen fra databasen
        var deletedInvite = await _inviteRepository.DeleteAsync(inviteId);
        if (deletedInvite == null)
        {
            _logger.LogError("Failed to delete Invite with ID {InviteId}", inviteId);
            throw new InvalidOperationException($"Failed to delete invite with ID {inviteId}.");
        }

        _logger.LogInformation("Invite with ID {InviteId} deleted successfully.", inviteId);
        return _inviteMapper.MapToDTO(inviteToDelete);
    }
}