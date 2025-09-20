using System.Threading;
using System.Threading.Tasks;
using LOJIC.Hangfire;
using Microsoft.Extensions.Hosting;

namespace LOJIC.Orchestration
{
    public class ServiceHost : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await JobSchedule.ConfigureRecurringJobs();
        }
    }
}
