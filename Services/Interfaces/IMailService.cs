using PlanIT.API.Models.Entities;

namespace PlanIT.API.Services.Interfaces;

public interface IMailService
{
    Task SendInviteEmail(Invite invite);

    Task SendReminderEmail(Invite invite);


}
