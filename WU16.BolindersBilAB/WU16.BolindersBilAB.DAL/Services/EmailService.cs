using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WU16.BolindersBilAB.DAL.Services
{
    public class EmailService
    {
        private string _smtpUserName;
        private string _smtpPassword;
        private string _host;
        private int _port;
        private bool _enableSsl;
        private string _senderEmail;
        private string _senderName;


        public EmailService(IConfiguration configuration)
        {
            var section = configuration.GetSection("EmailService").GetChildren().ToDictionary(x => x.Key, x => x.Value);

            _senderEmail = section["SenderEmail"];
            _senderName = section["SenderName"];
            _smtpUserName = section["SmtpUserName"];
            _smtpPassword = section["SmtpPassword"];
            _host = section["Host"];
            _port = Convert.ToInt32(section["Port"]);
            _enableSsl = bool.Parse(section["EnableSsl"]);
        }

        public bool SendTo(string recipient, string subject, string message, string sender = "", bool isBodyHtml = false)
        {
            if (string.IsNullOrEmpty(sender)) sender = _senderEmail;

            var credentials = new NetworkCredential(_smtpUserName, _smtpPassword);

            using (var client = new SmtpClient(_host, _port)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                EnableSsl = false
                //Credentials = credentials
            })
            {
                try
                {
                    var mail = new MailMessage()
                    {
                        Subject = subject,
                        From = new MailAddress(sender.Trim()),
                        Body = message,
                        IsBodyHtml = isBodyHtml
                    };

                    mail.To.Add(new MailAddress(recipient.Trim()));

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

