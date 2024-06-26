﻿using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Services.MailService;
using PlanIT.API.Utilities;

namespace PlanIT.API.Services;

// Service class for handling invite information.
// Exceptions are caught by a middleware: HandleExceptionFilter
public class InviteService : IInviteService
{
    private readonly IMapper<Invite, InviteDTO> _inviteMapper;
    private readonly IInviteRepository _inviteRepository;
    private readonly IRepository<Event> _eventRepository;
    private readonly IUserRepository _userRepository;

    private readonly LoggerService _logger;
    private readonly IMailService _mailService;

    public InviteService(IMapper<Invite, InviteDTO> inviteMapper,
        IInviteRepository inviteRepository,
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

       // Sends a email to the new invite using mailService
        await _mailService.SendInviteEmail(addedInvite, user.Name);

        _logger.LogOperationSuccess("created", "invite", addedInvite.Id);
        return _inviteMapper.MapToDTO(addedInvite);
    }


    
    public async Task<ICollection<InviteDTO>> GetAllAsync(int userIdFromToken, int pageNr, int pageSize)
    {
        _logger.LogDebug($"Retrieving all invites for user {userIdFromToken}.");

        var invitesFromRepository = await _inviteRepository.GetAllAsync(1, 10);

        
        List<Invite> filteredInvites = new List<Invite>();

        
        foreach (var invite in invitesFromRepository)
        {
            var eventAssociated = await _eventRepository.GetByIdAsync(invite.EventId);
            if (eventAssociated != null && eventAssociated.UserId == userIdFromToken)
            {
                filteredInvites.Add(invite);
            }
        }

       
        var inviteDTOs = filteredInvites.Select(invite => _inviteMapper.MapToDTO(invite)).ToList();
        return inviteDTOs;
    }


    public async Task<InviteDTO?> GetByIdAsync(int userIdFromToken, int inviteId)
    {
        _logger.LogDebug($"Retrieving invite with ID {inviteId} for user {userIdFromToken}.");

        var invite = await _inviteRepository.GetByIdAsync(inviteId);
        if (invite == null)
        {
            _logger.LogNotFound("invite", inviteId);
            throw ExceptionHelper.CreateNotFoundException("invite", inviteId);
        }

        
        var eventAssociated = await _eventRepository.GetByIdAsync(invite.EventId);
        if (eventAssociated == null || eventAssociated.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("invite", inviteId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("invite", inviteId);
        }

        _logger.LogOperationSuccess("retrieved", "invite", inviteId);
        return _inviteMapper.MapToDTO(invite);
    }


    public async Task<ICollection<InviteDTO>> GetInvitesForEventAsync(int userId, int eventId, int pageNr, int pageSize)
    {
        _logger.LogDebug($"Retrieving invites for event {eventId} for user {userId}.");

        // Ensure the event exists and the user has access to it
        var eventDetails = await _eventRepository.GetByIdAsync(eventId);
        if (eventDetails == null)
        {
            _logger.LogNotFound("Event", eventId);
            throw ExceptionHelper.CreateNotFoundException("Event", eventId);
        }
        if (eventDetails.UserId != userId)
        {
            _logger.LogUnauthorizedAccess("Event", eventId, userId);
            throw ExceptionHelper.CreateUnauthorizedException("Event", eventId);
        }

        // Fetch all invites specifically for this event with pagination
        var allInvites = await _inviteRepository.GetInvitesByEventIdAsync(eventId, pageNr, pageSize);
        _logger.LogDebug($"Fetched {allInvites.Count} invites from repository.");

        var inviteDTOs = allInvites.Select(invite => _inviteMapper.MapToDTO(invite)).ToList();
        if (inviteDTOs.Count == 0)
        {
            _logger.LogWarning($"No invites found for event {eventId}.");
        }

        return inviteDTOs;
    }



    public async Task<InviteDTO?> UpdateAsync(int userIdFromToken, int inviteId, InviteDTO inviteDTO)
    {
        _logger.LogDebug($"Updating invite with ID {inviteId} for user {userIdFromToken}.");


        var existingInvite = await _inviteRepository.GetByIdAsync(inviteId);
        if (existingInvite == null)
        {
            _logger.LogNotFound("invite", inviteId);
            throw ExceptionHelper.CreateNotFoundException("invite", inviteId);
        }

        
        var associatedEvent = await _eventRepository.GetByIdAsync(existingInvite.EventId);
        if (associatedEvent == null || associatedEvent.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("invite", inviteId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("invite", inviteId);
        }

        
        var inviteToUpdate = _inviteMapper.MapToModel(inviteDTO);
        inviteToUpdate.Id = inviteId;

       
        var updatedInvite = await _inviteRepository.UpdateAsync(inviteId, inviteToUpdate);
        if (updatedInvite == null)
        {
            _logger.LogOperationFailure("update", "invite", inviteId);
            throw ExceptionHelper.CreateOperationException("invite", inviteId, "update");
        }

        _logger.LogOperationSuccess("updated", "invite", inviteId);
        return _inviteMapper.MapToDTO(updatedInvite);
    }


    
    public async Task<InviteDTO?> DeleteAsync(int userIdFromToken, int inviteId)
    {
        _logger.LogDebug($"Deleting invite with ID {inviteId} for user {userIdFromToken}.");


        var inviteToDelete = await _inviteRepository.GetByIdAsync(inviteId);
        if (inviteToDelete == null)
        {
            _logger.LogNotFound("invite", inviteId);
            throw ExceptionHelper.CreateNotFoundException("invite", inviteId);
        }

        
        var associatedEvent = await _eventRepository.GetByIdAsync(inviteToDelete.EventId);
        if (associatedEvent == null || associatedEvent.UserId != userIdFromToken)
        {
            _logger.LogUnauthorizedAccess("invite", inviteId, userIdFromToken);
            throw ExceptionHelper.CreateUnauthorizedException("invite", inviteId);
        }

       
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
        _logger.LogDebug($"Confirming invite for ID {inviteId} for event {eventId}.");

        var invitedGuest = await _inviteRepository.GetByIdAsync(inviteId);

        
        if (invitedGuest == null || invitedGuest.EventId != eventId)
        {
            throw ExceptionHelper.CreateNotFoundException("Invite or Event", inviteId);
        }

        
        if (invitedGuest.Coming)
        {
            _logger.LogWarning($"Invite ID {inviteId} for Event ID {eventId} is already confirmed.");
            return false; 
        }

        
        invitedGuest.Coming = true;
        await _inviteRepository.UpdateAsync(inviteId, invitedGuest);
        _logger.LogInfo($"Invite ID {inviteId} for Event ID {eventId} has been confirmed successfully.");
        return true;
    }

}