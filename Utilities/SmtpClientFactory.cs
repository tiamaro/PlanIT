using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;

namespace PlanIT.API.Utilities;

public class SmtpClientFactory
{
    private readonly SmtpSettings _smtpSettings;

    public SmtpClientFactory(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

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

    // Expose SMTP username safely
    public string GetSmtpUsername()
    {
        return _smtpSettings.SmtpUsername;
    }
}
