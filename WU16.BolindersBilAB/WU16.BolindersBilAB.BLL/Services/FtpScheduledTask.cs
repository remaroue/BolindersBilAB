using DNTScheduler.Core.Contracts;
using System.Threading.Tasks;

namespace WU16.BolindersBilAB.BLL.Services
{
    public class FtpScheduledTask : IScheduledTask
    {
        private FtpService _ftpService;

        public FtpScheduledTask(FtpService ftpService)
        {
            _ftpService = ftpService;
        }

        public bool IsShuttingDown { get; set; }

        public async Task RunAsync()
        {
            if (this.IsShuttingDown)
            {
                return;
            }

            //_ftpService.Run();
        }
    }
}

