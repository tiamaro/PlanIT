using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace PlanIT.API.Utilities;
// Factory class for creating SMTP client instances

public class SmtpClientFactory
{
    private readonly SmtpSettings _smtpSettings;

    public SmtpClientFactory(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }


    // Creates and configures an SMTP client instance based on the provided SMTP settings
    // Returns the configured SMTP client
    public SmtpClient CreateSmtpClient()
    {
        return new SmtpClient(_smtpSettings.SmtpHost, _smtpSettings.SmtpPort)
        {
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_smtpSettings.SmtpUsername, _smtpSettings.SmtpPassword),
            EnableSsl = true
        };
    }

    // Safely retrieves the SMTP username
    public string GetSmtpUsername()
    {
        return _smtpSettings.SmtpUsername;
    }
}
