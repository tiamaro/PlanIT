using PlanIT.API.Utilities;
using System.Net.Mail;

namespace PlanIT.API.Services.MailService;

public class EmailService : IEmailService
{
    private readonly SmtpClientFactory _smtpClientFactory;
    private readonly ILogger<EmailService> _logger;

    public EmailService(SmtpClientFactory smtpClientFactory, ILogger<EmailService> logger)
    {
        _smtpClientFactory = smtpClientFactory;
        _logger = logger;
    }


    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        using (var client = _smtpClientFactory.CreateSmtpClient())
        {
            var message = new MailMessage(_smtpClientFactory._smtpSettings.SmtpUsername, recipient)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            await client.SendMailAsync(message);
        }
    }
}
