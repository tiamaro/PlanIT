﻿using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Services;

public class InviteService : IInviteService
{
    private readonly IMapper<Invite, InviteDTO> _inviteMapper;
    private readonly IRepository<Invite> _inviteRepository;
    private readonly ILogger<InviteService> _logger;

    public InviteService(IMapper<Invite, InviteDTO> inviteMapper,
        IRepository<Invite> inviteRepository,
        ILogger<InviteService> logger)
    {
        _inviteMapper = inviteMapper;
        _inviteRepository = inviteRepository;
        _logger = logger;
    }


    // Oppretter ny invitasjon
    public async Task<InviteDTO?> CreateInviteAsync(InviteDTO newInviteDTO)
    {
        // Mapper InviteDTO til Invite-modellen
        var newInvite = _inviteMapper.MapToModel(newInviteDTO);

        // Legger til den nye invitasjonen i databasen og henter resultatet
        var addedInvite = await _inviteRepository.AddAsync(newInvite);

        // Mapper den nye invitasjonen til InviteDTO og returnerer den
        return _inviteMapper.MapToDTO(addedInvite!);
    }

   
    // Henter alle invitasjoner
    public async Task<ICollection<InviteDTO>> GetAllAsync(int pageNr, int pageSize)
    {
        // Henter invitasjonsinformasjon fra repository med paginering
        var invitesFromRepository = await _inviteRepository.GetAllAsync(1, 10);

        // Mapper invitasjonsdataene til inviteDTO-format
        var inviteDTOs = invitesFromRepository.Select(invite => _inviteMapper.MapToDTO(invite)).ToList();
        return inviteDTOs;
    }


    // Henter invitasjoner basert på ID
    public async Task<InviteDTO?> GetByIdAsync(int inviteId)
    {
        var inviteFromRepository = await _inviteRepository.GetByIdAsync(inviteId);
        return inviteFromRepository != null ? _inviteMapper.MapToDTO(inviteFromRepository) : null;
    }


    // Oppdaterer invitasjon
    public async Task<InviteDTO?> UpdateAsync(int inviteId, InviteDTO inviteDTO)
    {
        var existingInvite = await _inviteRepository.GetByIdAsync(inviteId);

        if (existingInvite == null)
        {
            return null; // Invitasjon ikke funnet
        }

        // Mapper og oppdaterer invitasjonsinformasjon
        var inviteToUpdate = _inviteMapper.MapToModel(inviteDTO);
        inviteToUpdate.Id = inviteId;

        var updatedInvite = await _inviteRepository.UpdateAsync(inviteId, inviteToUpdate);

        return updatedInvite != null ? _inviteMapper.MapToDTO(updatedInvite) : null;
    }


    // Sletter invitasjon
    public async Task<InviteDTO?> DeleteAsync(int inviteId)
    {
        var inviteToDelete = await _inviteRepository.GetByIdAsync(inviteId);

        // Sjekker om invitasjonen eksisterer
        if (inviteToDelete == null) return null;

        // Sletter invitasjonen fra databasen og mapper den til InviteDTO for retur                 
        var isDeleted = await _inviteRepository.DeleteAsync(inviteId);
        return isDeleted != null ? _inviteMapper.MapToDTO(inviteToDelete) : null;
    }
}