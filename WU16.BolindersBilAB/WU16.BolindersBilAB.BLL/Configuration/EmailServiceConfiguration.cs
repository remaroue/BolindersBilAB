using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WU16.BolindersBilAB.BLL.Configuration
{
    public class EmailServiceConfiguration
    {
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpPassword { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
    }
}
