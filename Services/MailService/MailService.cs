using PlanIT.API.Models.Entities;
using PlanIT.API.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace PlanIT.API.Services.MailService;

public class MailService : IMailService
{
    private readonly ILogger<MailService> _logger;

    public MailService(ILogger<MailService> logger)
    {
        _logger = logger;
    }

    public void SendInviteEmail(Invite invite)
    {
        try
        {
            using (var client = new SmtpClient("smtp-mail.outlook.com", 587))
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("planit-event@outlook.com", "yVh1to*VzM2c");
                client.EnableSsl = true;


                var message = new MailMessage("planit-event@outlook.com", $"{invite.Email}");
                message.Subject = "You have been invited to an event!";

                // Insert guest information 
                message.Body = $"<h1>Hello {invite.Name} you have been invited to {invite?.Event?.Name} on {invite?.Event?.Date} at {invite?.Event?.Time} at {invite?.Event?.Location}, best wishes {invite?.Event?.User?.Name}</h1>";
                message.IsBodyHtml = true;


                //await Task.Run(() => client.Send(message));
                client.Send(message);


            }


        }

        catch (Exception ex)
        {
            _logger.LogError($"An error occured trying to send invite email:{ex.ToString()}");
            Console.WriteLine($"Error sending email:");

        }


    }

    public async Task SendReminderEmail(Invite invite)
    {
        if (invite?.Event?.Date.CompareTo(DateTime.Today) <= 3)
            try
            {
                using (var client = new SmtpClient("smtp-mail.outlook.com", 587))
                {
                    var message = new MailMessage("planit-event@outlook.com", $"{invite.Email}");
                    message.Subject = $"Reminder {invite.Name} in 3 days!";

                    message.Body = $"<h1>Hello {invite.Name},</h1>" +
                        $"<p>This is a reminder that the event '{invite?.Event?.Name}' is happening on {invite?.Event?.Date} at {invite?.Event?.Time} at {invite?.Event?.Location}.</p>" +
                        $"<p>We look forward to seeing you there!</p>" +
                        $"Sincerely,<br>{invite?.Event?.User?.Name}</p>";
                    message.IsBodyHtml = true;


                    // cant await 'void' error, wrap in Task block ?? 
                    await Task.Run(() => client.Send(message));


                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"An error occured trying to send reminder email:{ex.ToString()}");
                Console.WriteLine($"Error sending reminder email");
            }
    }
}

