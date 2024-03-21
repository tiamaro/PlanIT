using PlanIT.API.Models.Entities;

namespace PlanIT.API.Services.Interfaces;

public interface IMailService
{
    void SendInviteEmail(Invite invite);
}
