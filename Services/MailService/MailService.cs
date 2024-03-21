using PlanIT.API.Models.Entities;
using PlanIT.API.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace PlanIT.API.Services.MailService;

public class MailService : IMailService
{
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
                message.Body = "<h1> Hello you have been invited to an event </h1>";
                message.IsBodyHtml = true;

                client.Send(message);


            }


        }

        catch (Exception ex) 
        {
            Console.WriteLine($"Error sending email: {ex.ToString()}");

        }
        

    }
   
}
