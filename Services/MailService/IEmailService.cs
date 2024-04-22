namespace PlanIT.API.Services.MailService;

public interface IEmailService
{
    Task SendEmailAsync(string recipient, string subject, string body);

}
