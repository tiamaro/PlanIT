using PlanIT.API.Models.Entities;

namespace PlanIT.API.Services.MailService;

public interface IMailService
{
    Task SendInviteEmail(Invite invite);

    Task SendReminderEmail(Invite invite);


}
