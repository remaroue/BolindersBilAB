using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using WU16.BolindersBilAB.BLL.Configuration;

namespace WU16.BolindersBilAB.BLL.Services
{
    public class EmailService
    {
        private EmailServiceConfiguration _config;

        public EmailService(IOptions<EmailServiceConfiguration> config)
        {
            _config = config.Value;
        }

        public bool SendTo(string recipient, string subject, string message, string sender = "", bool isBodyHtml = false)
        {
            if (string.IsNullOrEmpty(sender)) sender = _config.SenderEmail;

            NetworkCredential credentials = null;
            bool useDefaultCredentials = true;

            if (_config.Host == "localhost")
            {
                credentials = new NetworkCredential(_config.SenderEmail, _config.SmtpPassword);
                useDefaultCredentials = false;
            }

            using (var client = new SmtpClient(_config.Host, _config.Port)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = useDefaultCredentials,
                EnableSsl = _config.EnableSsl,
                Credentials = credentials
            })
            {
                try
                {
                    var mail = new MailMessage(new MailAddress(sender.Trim()), new MailAddress(recipient.Trim()))
                    {
                        Subject = subject,
                        Body = message,
                        IsBodyHtml = isBodyHtml
                    };

                    client.Send(mail);
                    return true;
                }
                catch(Exception)
                {
                    return false;
                }
            }
        }
    }
}

