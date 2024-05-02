using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Services.Interfaces;

public interface IInviteService : IService<InviteDTO>
{
    Task<bool> ConfirmInvite(int inviteId, int eventId);

    Task<ICollection<InviteDTO>> GetInvitesForEventAsync(int userId, int eventId, int pageNr, int pageSize);

}