using PlanIT.API.Models.Entities;
using System.Net;
using System.Net.Mail;

namespace PlanIT.API.Services.MailService;

public class MailService
{
    public static void SendEmailInvite()
    {
        try
        {
            using (var client = new SmtpClient("smtp-mail.outlook.com", 587))
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("planit-event@outlook.com", "yVh1to*VzM2c");
                client.EnableSsl = true;


                var message = new MailMessage("planit-event@outlook.com", "hhanna.persson@hotmail.com");
                message.Subject = "You have been invited to an event!";

                // Insert guest information 
                //message.Body = $"<h1>Hello, you have been invited to {Event.Name} on {Event.Date} at {Event.Time} at {Event.Location}, best wishes {User.Name}</h1>";
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
