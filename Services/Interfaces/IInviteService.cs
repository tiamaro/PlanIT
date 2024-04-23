namespace PlanIT.API.Services.Interfaces;

public interface IInviteService
{
    Task<bool> ConfirmInvite(int inviteId, int eventId);

}
