using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace WebSale.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("despacitovv@gmail.com", "ufhlaytdnnibhxrr")
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("despacitovv@gmail.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true 
            };
            mailMessage.To.Add(email); 
            return client.SendMailAsync(mailMessage);
        }
    }
}