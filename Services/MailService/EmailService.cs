using PlanIT.API.Utilities;
using System.Net.Mail;

namespace PlanIT.API.Services.MailService;
// Service for sending emails using SMTP

public class EmailService : IEmailService
{
    private readonly SmtpClientFactory _smtpClientFactory;
    private readonly LoggerService _logger;

    public EmailService(SmtpClientFactory smtpClientFactory, LoggerService logger)
    {
        _smtpClientFactory = smtpClientFactory;
        _logger = logger;
    }

    // Sends an email asynchronously to the specified recipient with the given subject and body
    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        try
        {
            // Create a new SMTP client using the factory
            using (var client = _smtpClientFactory.CreateSmtpClient())
            {
                // Create a new MailMessage with sender, recipient, subject, and body
                var message = new MailMessage(_smtpClientFactory.GetSmtpUsername(), recipient)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                await client.SendMailAsync(message);
                _logger.LogInfo($"Email sent successfully to {recipient}. Subject: {subject}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogException(ex, $"Failed to send email to {recipient}");
            throw;
        }
    }
}