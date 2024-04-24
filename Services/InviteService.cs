using PlanIT.API.Extensions;
using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Services.MailService;
using PlanIT.API.Utilities;
using System.ComponentModel;
using System.Runtime.CompilerServices; // For å inkludere LoggerService og ExceptionHelper


namespace PlanIT.API.Services;

// Serviceklasse for håndtering av invitasjonsinformasjon.
// Exceptions blir fanget av en middleware: HandleExceptionFilter
public class InviteService : IService<InviteDTO> , IInviteService
{
    private readonly IMapper<Invite, InviteDTO> _inviteMapper;
    private readonly IRepository<Invite> _inviteRepository;
    private readonly IRepository<Event> _eventRepository;
    private readonly IUserRepository _userRepository;

    private readonly LoggerService _logger;
    private readonly IMailService _mailService;

    public InviteService(IMapper<Invite, InviteDTO> inviteMapper,
        IRepository<Invite> inviteRepository,
        IRepository<Event> eventRepository,
        IUserRepository userRepository,

        LoggerService logger,
        IMailService mailService)
    {
        _inviteMapper = inviteMapper;
        _inviteRepository = inviteRepository;
        _eventRepository = eventRepository;
        _userRepository = userRepository;
        _logger = logger;
        _mailService = mailService;
    }

    // Oppretter ny invitasjon basert på data mottatt fra klienten
    public async Task<InviteDTO?> CreateAsync(int userIdFromToken, InviteDTO newInviteDTO)
    {
        _logger.LogCreationStart("invite");

        var eventAssociated = await _eventRepository.GetByIdAsync(newInviteDTO.EventId);
        if (eventAssociated == null)
        {
            _logger.LogNotFound("event", newInviteDTO.EventId);
            throw ExceptionHelper.CreateNotFoundException("event", newInviteDTO.EventId);
        }

        if (eventAssociated.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("event", newInviteDTO.EventId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("event", newInviteDTO.EventId);
        }

        // Fetch user details
        var user = await _userRepository.GetByIdAsync(userIdFromToken);
        if (user == null)
        {
            _logger.LogError("User not found with ID {UserId}", userIdFromToken);
            throw new InvalidOperationException("User not found.");
        }

        var newInvite = _inviteMapper.MapToModel(newInviteDTO);
        var addedInvite = await _inviteRepository.AddAsync(newInvite);
        if (addedInvite == null)
        {
            _logger.LogCreationFailure("invite");
            throw ExceptionHelper.CreateOperationException("invite", 0, "create");
        }

        // Pass user details to the mail service
        await _mailService.SendInviteEmail(addedInvite, user.Name);
        _logger.LogOperationSuccess("created", "invite", addedInvite.Id);
        return _inviteMapper.MapToDTO(addedInvite);
    }


    // Henter alle invitasjoner som tilhører den innloggede brukeren
    public async Task<ICollection<InviteDTO>> GetAllAsync(int userIdFromToken, int pageNr, int pageSize)
    {
        // Henter invitasjonsinformasjon fra repository med paginering
        var invitesFromRepository = await _inviteRepository.GetAllAsync(1, 10);

        // Liste som lagrer invites basert på filtreringen
        List<Invite> filteredInvites = new List<Invite>();

        // Sjekker hver invite for å se om det assosierte arrangementet er koblet til den innloggede brukeren
        foreach (var invite in invitesFromRepository)
        {
            var eventAssociated = await _eventRepository.GetByIdAsync(invite.EventId);
            if (eventAssociated != null && eventAssociated.UserId == userIdFromToken)
            {
                filteredInvites.Add(invite);
            }
        }

        // Mapper invitasjonsdataene til inviteDTO-format
        var inviteDTOs = filteredInvites.Select(invite => _inviteMapper.MapToDTO(invite)).ToList();
        return inviteDTOs;
    }


    // Henter en invitasjon basert på dens ID og brukerens ID for å sikre at brukeren har tilgang
    public async Task<InviteDTO?> GetByIdAsync(int userIdFromToken, int inviteId)
    {
        _logger.LogDebug("Attempting to retrieve invite with ID {InviteId} for user ID {UserId}.", inviteId, userIdFromToken);
        var invite = await _inviteRepository.GetByIdAsync(inviteId);
        if (invite == null)
        {
            _logger.LogNotFound("invite", inviteId);
            throw ExceptionHelper.CreateNotFoundException("invite", inviteId);
        }

        // Henter det assosierte arrangementet for å kunne sjekke mot riktig brukerID
        var eventAssociated = await _eventRepository.GetByIdAsync(invite.EventId);
        if (eventAssociated == null || eventAssociated.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("invite", inviteId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("invite", inviteId);
        }

        _logger.LogOperationSuccess("retrieved", "invite", inviteId);
        return _inviteMapper.MapToDTO(invite);
    }


    // Oppdaterer en eksisterende invitasjon etter å ha validert at brukeren har nødvendige rettigheter
    public async Task<InviteDTO?> UpdateAsync(int userIdFromToken, int inviteId, InviteDTO inviteDTO)
    {
        _logger.LogDebug("Attempting to update invite with ID {InviteId} for user ID {UserId}.", inviteId, userIdFromToken);

        // Forsøker å hente en invitasjon basert på ID for å sikre at det faktisk eksisterer før oppdatering.
        var existingInvite = await _inviteRepository.GetByIdAsync(inviteId);
        if (existingInvite == null)
        {
            _logger.LogNotFound("invite", inviteId);
            throw ExceptionHelper.CreateNotFoundException("invite", inviteId);
        }

        // Henter det assosierte arrangementet for å kunne sjekke mot riktig brukerID
        var associatedEvent = await _eventRepository.GetByIdAsync(existingInvite.EventId);
        if (associatedEvent == null || associatedEvent.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("invite", inviteId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("invite", inviteId);
        }

        // Mapper til DTO og sørger for at ID forblir den samme under oppdateringen
        var inviteToUpdate = _inviteMapper.MapToModel(inviteDTO);
        inviteToUpdate.Id = inviteId;

        // Prøver å oppdatere invitasjonen i databasen
        var updatedInvite = await _inviteRepository.UpdateAsync(inviteId, inviteToUpdate);
        if (updatedInvite == null)
        {
            _logger.LogOperationFailure("update", "invite", inviteId);
            throw ExceptionHelper.CreateOperationException("invite", inviteId, "update");
        }

        _logger.LogOperationSuccess("updated", "invite", inviteId);
        return _inviteMapper.MapToDTO(updatedInvite);
    }


    // Sletter en invitasjon etter å ha validert brukerens tilgangsrettigheter
    public async Task<InviteDTO?> DeleteAsync(int userIdFromToken, int inviteId)
    {
        _logger.LogDebug("Attempting to delete invite with ID {InviteId}.", inviteId);

        // Forsøker å hente en invitasjon basert på ID for å sikre at det faktisk eksisterer før sletting.
        var inviteToDelete = await _inviteRepository.GetByIdAsync(inviteId);
        if (inviteToDelete == null)
        {
            _logger.LogNotFound("invite", inviteId);
            throw ExceptionHelper.CreateNotFoundException("invite", inviteId);
        }

        // Henter det assosierte arrangementet for å kunne sjekke mot riktig brukerID
        var associatedEvent = await _eventRepository.GetByIdAsync(inviteToDelete.EventId);
        if (associatedEvent == null || associatedEvent.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("invite", inviteId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("invite", inviteId);
        }

        // Prøver å slette invitasjonen fra databasen
        var deletedInvite = await _inviteRepository.DeleteAsync(inviteId);
        if (deletedInvite == null)
        {
            _logger.LogOperationFailure("delete", "invite", inviteId);
            throw ExceptionHelper.CreateOperationException("invite", inviteId, "delete");
        }

        _logger.LogOperationSuccess("deleted", "invite", inviteId);
        return _inviteMapper.MapToDTO(deletedInvite);
    }

    public async Task<bool> ConfirmInvite(int inviteId, int eventId)
    {
        var invitedGuest = await _inviteRepository.GetByIdAsync(inviteId);

        // Check if the invited guest exists and the event ID matches if necessary.
        if (invitedGuest == null || invitedGuest.EventId != eventId)
        {
            throw ExceptionHelper.CreateNotFoundException("Invite or Event", inviteId);
        }

        // Check if the guest has already confirmed coming.
        if (invitedGuest.Coming)
        {
            _logger.LogWarning($"Invite ID {inviteId} for Event ID {eventId} is already confirmed.");
            return false; 
        }

        // Mark the guest as coming.
        invitedGuest.Coming = true;
        await _inviteRepository.UpdateAsync(inviteId, invitedGuest);
        _logger.LogInfo($"Invite ID {inviteId} for Event ID {eventId} has been confirmed successfully.");
        return true;
    }
}