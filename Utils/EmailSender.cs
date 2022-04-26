using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace BoxTrackLabel.API.Utils
{
    public class EmailSender : IEmailSender
    {
        private string _host;
        private int _port;
        private bool _enableSSL;
        private string _userName;
        private string _password;
        private string _fromAddress;
        private bool _useDefaultCredentials;

        public EmailSender(IConfiguration configuration)
        {
            _host = configuration["EmailSender:host"];
            _port = int.Parse(configuration["EmailSender:port"]);
            _enableSSL = bool.Parse(configuration["EmailSender:enablessl"]);
            _userName = configuration["EmailSender:username"];
            _password = configuration["EmailSender:password"];
            _useDefaultCredentials = bool.Parse(configuration["EmailSender:usedefaultcredentials"]);
            _fromAddress = configuration["EmailSender:fromaddress"];
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_host, _port)
            {
                UseDefaultCredentials = _useDefaultCredentials,
                Credentials = new NetworkCredential(_userName,_password),
                EnableSsl = _enableSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            return client.SendMailAsync(
                new MailMessage(_fromAddress, email, subject, htmlMessage) { IsBodyHtml = true }    
            );
        }
        public Task SendBulkEmailAsync(List<EmailAccount> emails, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_host, _port)
            {
                UseDefaultCredentials = _useDefaultCredentials,
                Credentials = new NetworkCredential(_userName,_password),
                EnableSsl = _enableSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
            var mailMessage = new MailMessage();
            mailMessage.Subject = subject;
            mailMessage.Body = htmlMessage;
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress(_fromAddress);

            foreach (EmailAccount emailAdress in emails)
            {
                if(!emailAdress.IsCopy)
                    mailMessage.To.Add(emailAdress.Email);
                else
                    mailMessage.CC.Add(emailAdress.Email);
            }
            return client.SendMailAsync(mailMessage);
        }
    }
}