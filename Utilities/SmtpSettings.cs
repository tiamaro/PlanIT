namespace PlanIT.API.Utilities;
// Represents the SMTP settings required for configuring an SMTP client

public class SmtpSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
}
