using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using WU16.BolindersBilAB.BLL.Configuration;

namespace WU16.BolindersBilAB.BLL.Services
{
    public class FtpService
    {
        private FtpServiceConfiguration _config;

        public FtpService(IOptions<FtpServiceConfiguration> config)
        {
            _config = config.Value;
        }
    }
}
