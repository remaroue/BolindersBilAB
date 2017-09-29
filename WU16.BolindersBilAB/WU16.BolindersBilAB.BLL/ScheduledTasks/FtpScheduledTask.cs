using DNTScheduler.Core.Contracts;
using System.Threading.Tasks;
using WU16.BolindersBilAB.BLL.Services;

namespace WU16.BolindersBilAB.BLL.ScheduledTasks
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

            _ftpService.Run();
        }
    }
}

