using PlanIT.API.Mappers.Interface;
using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Repositories.Interfaces;
using PlanIT.API.Services.Interfaces;

namespace PlanIT.API.Services;

public class InviteService : IService<InviteDTO>
{
    private readonly IMapper<Invite, InviteDTO> _inviteMapper;
    private readonly IRepository<Invite> _inviteRepository;
    private readonly ILogger<InviteService> _logger;
    //private readonly IBackgroundJobClient _backgroundJobClient;
    public readonly IMailService _mailService;

    public InviteService(IMapper<Invite, InviteDTO> inviteMapper,
        IRepository<Invite> inviteRepository,
        ILogger<InviteService> logger,
        // IBackgroundJobClient backgroundJobClient
        IMailService mailService
        )
    {
        _inviteMapper = inviteMapper;
        _inviteRepository = inviteRepository;
        _logger = logger;
        //_backgroundJobClient = backgroundJobClient;
        _mailService = mailService;
    }


    // Oppretter ny invitasjon
    public async Task<InviteDTO?> CreateAsync(InviteDTO newInviteDTO)
    {
        // Mapper InviteDTO til Invite-modellen
        var newInvite = _inviteMapper.MapToModel(newInviteDTO);

        // Legger til den nye invitasjonen i databasen og henter resultatet
        var addedInvite = await _inviteRepository.AddAsync(newInvite);
        //_mailService.SendInviteEmail(addedInvite);


        // Unsure of what to do with invite null reference 
        // Unsure how this will work 
        //_backgroundJobClient.Schedule(() => _mailService.SendReminderEmail(addedInvite),
        //addedInvite.Event.Date.ToDateTime(TimeOnly.MinValue) >= DateTime.Today
        //? (addedInvite.Event.Date.ToDateTime(TimeOnly.MinValue) - DateTime.Today).TotalDays < 3
        //? TimeSpan.Zero  // Schedule immediately if less than 3 days
        //: (addedInvite.Event.Date.ToDateTime(TimeOnly.MinValue) - DateTime.Today)  // Schedule for remaining days
        //: addedInvite.Event.Date.ToDateTime(TimeOnly.MinValue).Subtract(DateTime.Today));





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