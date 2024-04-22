using PlanIT.API.Utilities;
using System.Net.Mail;

namespace PlanIT.API.Services.MailService;

public class EmailService : IEmailService
{
    private readonly SmtpClientFactory _smtpClientFactory;
    private readonly LoggerService _logger;

    public EmailService(SmtpClientFactory smtpClientFactory, LoggerService logger)
    {
        _smtpClientFactory = smtpClientFactory;
        _logger = logger;
    }

    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        try
        {
            using (var client = _smtpClientFactory.CreateSmtpClient())
            {
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