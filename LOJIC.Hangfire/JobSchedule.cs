using System.Threading.Tasks;
using Hangfire;
using LOJIC.Hangfire.Extensions;
using LOJIC.Hangfire.Helpers;
using LOJIC.Hangfire.Jobs;
using LOJIC.Hangfire.Jobs.Heartbeat;
using LOJIC.Hangfire.Jobs.ServiceManagement.ArcGisEnterprise;

namespace LOJIC.Hangfire
{
    public static class JobSchedule
    {
        /// <summary>
        /// Adds all of the listed jobs to the scheduler. Only list jobs that can be scheduled or manually ran - not those executed by other jobs.
        /// </summary>
        public static async Task ConfigureRecurringJobs()
        {
            BackgroundJobHelper.Enqueue<ArcGisServerHeartBeatJob>(j=>j.Init(null));
            BackgroundJobHelper.Enqueue<ArcGisEnterpriseHeartBeatJob>(j => j.Init(null));

            RecurringJobHelper.AddOrUpdate<ArcGisServerHeartBeatJob>(j => j.Run(null), Cron.Minutely);
            RecurringJobHelper.AddOrUpdate<ArcGisEnterpriseHeartBeatJob>(j => j.Run(null), Cron.Minutely);
            RecurringJobHelper.AddOrUpdate<CleanUpJob>(j => j.DeleteOldHeartbeats(null), Cron.Monthly);
         
        }
    }
}
