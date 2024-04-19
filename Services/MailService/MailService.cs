using PlanIT.API.Models.DTOs;
using PlanIT.API.Models.Entities;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities;
using System.Net;
using System.Net.Mail;

namespace PlanIT.API.Services.MailService;

public class MailService : IMailService
{
   // private readonly ILogger<MailService> _logger;
    private readonly LoggerService _logger;


    public MailService(LoggerService logger)
    {
        _logger = logger;      
    }

    public async Task SendInviteEmail(Invite invite)
    {
        //if (invite == null || invite.Event == null)
        //{

        //    _logger.LogError("Failed to send email: Invite or event data is missing.");
        //    throw new ArgumentNullException("Email could not be sent with missing Invite data");
        //}

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
                message.Body = $"<h1>Hello {invite.Name},</h1>" +
                  $"<p>You have been invite to '{invite.Event.Name}'.</p>" +
                  $"<p>The event details are:</p>" +
                  $"<ul>" +
                  $"<li>Date: {invite.Event.Date}</li>" +
                  $"<li>Time: {invite.Event.Time}</li>" +
                  $"<li>Location: {invite.Event.Location} </li>" +
                  $"</ul>" +
                  $"<p>We look forward to seeing you there! </p>" +
                  $"Sincerely,<br>{invite.Event.User?.Name} </p>";



                message.IsBodyHtml = true;
                await client.SendMailAsync(message);


            }


        }

        catch (Exception ex)
        {
            _logger.LogException(ex, "An error occured trying to send reminder email");
            throw;

        }


    }

    public async Task SendReminderEmail(Invite invite)
    {

        //if (invite == null || invite.Event == null)
        //{

        //    _logger.LogError("Failed to send reminder email: Invite or event data is missing.");
        //    throw new ArgumentNullException("Email could not be sent with missing Invite data");
        //}

        try
        {
            using (var client = new SmtpClient("smtp-mail.outlook.com", 587))
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("planit-event@outlook.com", "yVh1to*VzM2c");
                client.EnableSsl = true;


                var message = new MailMessage("planit-event@outlook.com", $"{invite.Email}");
                message.Subject = $"Reminder {invite?.Event?.Name} in 3 days";

                message.Body = $"<h1>Hello {invite?.Name},</h1>" +
                  $"<p>This is a friendly reminder about the event '{invite?.Event?.Name}'.</p>" +
                  $"<p>The event details are:</p>" +
                  $"<ul>" +
                  $"<li>Date: {invite?.Event?.Date} </li>" +
                  $"<li>Time: {invite?.Event?.Time} </li>" +
                  $"<li>Location: {invite?.Event?.Location} </li>" +
                  $"</ul>" +
                  $"<p>We look forward to seeing you there!</p>" +
                  $"Sincerely,<br> {invite?.Event?.User?.Name} </p>";



                message.IsBodyHtml = true;
                await client.SendMailAsync(message);


            }
        }

        catch (Exception ex)
        {
            _logger.LogException(ex ,"An error occured trying to send reminder email");
            throw;
        }


    }
}

