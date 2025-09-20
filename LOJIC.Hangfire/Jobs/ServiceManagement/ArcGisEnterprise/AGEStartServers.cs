using System;
using System.Linq;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using LOJIC.Hangfire.Extensions;
using LOJIC.Hangfire.Services.Heartbeat;
using LOJIC.Orchestration.Data;

namespace LOJIC.Hangfire.Jobs.ServiceManagement.ArcGisEnterprise
{
    [Queue("age_heartbeat")]
    class AGEStartServers : BaseJob
    {
        private readonly OrchestrationMetadataContext _context;
        private readonly GisEnterpriseHeartBeatService _heartbeat;
        private string _targetQueue = "age_heartbeat";

        public AGEStartServers(OrchestrationMetadataContext context, GisEnterpriseHeartBeatService heartbeat)
        {
            _context = context;
            _heartbeat = heartbeat;
        }

        public void Run(PerformContext context)
        {
            context.WriteLine($"AGE_HEARTBEAT run StartServers {Environment.MachineName}");
            var c = JobStorage.Current.GetMonitoringApi();
            //Get the current machine name
            var machineName = Environment.MachineName.ToLower();

            //get the name of the first server pool for this machine
            var targetPool = c.Servers()
                .Where(dto => dto.Name.Contains(machineName) && dto.Queues.Contains(_targetQueue))
                .Select(dto => dto.Name.Split($"{machineName} - ")[1].Split(":")[0]).FirstOrDefault();
            context.WriteLine($"AGE_HEARTBEAT StartServers TargetPool: {targetPool}");
            //Check last 15 heartbeat states from SQL
            var mostRecentHeartbeats = _context.ArcgisEnterpriseOracleHeartbeats.Take(15);
            context.WriteLine($"AGE_HEARTBEAT StartServers - Creating Startup Jobs for age_gis and age_webadaptor");
            BackgroundJobHelper.EnqueueOnAllSubscribingServers<GISEnterpriseService>(service => service.Start(null), "age_gis", targetPool);
            BackgroundJobHelper.EnqueueOnAllSubscribingServers<GISEnterpriseWebAdaptorService>(service => service.Start(null), "age_webadaptor", targetPool);
        }
    }
}
