using PlanIT.API.Models.Entities;
using PlanIT.API.Services.Interfaces;
using PlanIT.API.Utilities;
using System.Net.Mail;
namespace PlanIT.API.Services.MailService;

public class MailService : IMailService
{
    private readonly SmtpClientFactory _smtpClientFactory;
    private readonly LoggerService _logger;
    private readonly IJWTEmailAuth _emailAuth;

    public MailService(SmtpClientFactory smtpClientFactory,
        LoggerService logger,
        IJWTEmailAuth emailAuth)
    {
        _smtpClientFactory = smtpClientFactory;
        _logger = logger;
        _emailAuth = emailAuth;
    }

    public async Task SendInviteEmail(Invite invite, string userName)
    {
        if (invite == null)
        {
            _logger.LogError("Invite is null.");
            throw new ArgumentNullException(nameof(invite), "Invite cannot be null.");
        }

        if (invite.Event == null)
        {
            _logger.LogError("Event details are missing in the invite.");
            throw new ArgumentException("Event details are missing in the invite.", nameof(invite));
        }

        // Checking for any critical event details that might be null
        var eventName = invite.Event.Name ?? "Unnamed Event";
        var eventDate = invite.Event.Date.ToString("yyyy-MM-dd");   // Assuming Date is non-nullable
        var eventTime = invite.Event.Time.ToString("HH:mm");        // Assuming Time is non-nullable
        var eventLocation = invite.Event.Location ?? "Location not specified";

        var token = _emailAuth.GenerateJwtToken(invite.Id, invite.EventId);
        var confirmationLink = $"https://localhost:7019/api/v1/inviteresponse/confirm-invite?token={token}";
        try
        {

            using (var client = _smtpClientFactory.CreateSmtpClient())
            {
                var message = new MailMessage(_smtpClientFactory.GetSmtpUsername(), invite.Email)
                {
                    Subject = $"You have been invited to an event: {eventName}!",
                    Body = $"<h1>Hello {invite.Name},</h1>" +
                           $"<p>You have been invited to '{eventName}'.</p>" +
                           $"<p>The event details are:</p>" +
                           $"<ul>" +
                           $"<li>Date: {eventDate}</li>" +
                           $"<li>Time: {eventTime}</li>" +
                           $"<li>Location: {eventLocation}</li>" +
                           $"</ul>" +
                           $"<p>Please confirm your attendance by clicking <a href='{confirmationLink}'>here</a>.</p>" +
                           $"<p>We look forward to seeing you there!</p>" +
                           $"Sincerely,<br>{userName}",
                    IsBodyHtml = true
                };
                await client.SendMailAsync(message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogException(ex, "An error occurred trying to send an invite email");
            throw;
        }
    }

    public async Task SendReminderEmail(Invite invite)
    {
        if (invite == null || invite.Event == null)
        {
            _logger.LogError("Failed to send reminder email: Invite or event data is missing.");
            throw new ArgumentNullException("Email could not be sent with missing Invite data");
        }

        try
        {
            using (var client = _smtpClientFactory.CreateSmtpClient())
            {
                var message = new MailMessage(_smtpClientFactory.GetSmtpUsername(), invite.Email)
                {
                    Subject = $"Reminder: {invite.Event.Name} in 3 days",
                    Body = $"<h1>Hello {invite.Name},</h1>" +
                           $"<p>This is a friendly reminder about the event '{invite.Event.Name}'.</p>" +
                           $"<p>The event details are:</p>" +
                           $"<ul>" +
                           $"<li>Date: {invite.Event.Date}</li>" +
                           $"<li>Time: {invite.Event.Time}</li>" +
                           $"<li>Location: {invite.Event.Location}</li>" +
                           $"</ul>" +
                           $"<p>We look forward to seeing you there!</p>" +
                           $"Sincerely,<br>{invite.Event.User?.Name}</p>",
                    IsBodyHtml = true
                };
                await client.SendMailAsync(message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogException(ex, "An error occurred trying to send a reminder email");
            throw;
        }
    }
}