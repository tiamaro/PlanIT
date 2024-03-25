using PlanIT.API.Models.Entities;

namespace PlanIT.API.Services.Interfaces;

public interface IMailService
{
    void SendInviteEmail(Invite invite);

    Task SendReminderEmail(Invite invite);



    // Send out reminder to observers in list 
    //void SendReminderEmail(Invite invite);
}
