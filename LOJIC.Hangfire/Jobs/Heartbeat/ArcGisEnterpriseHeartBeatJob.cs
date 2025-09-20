using System;
using System.Linq;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using LOJIC.Hangfire.Services.Heartbeat;
using LOJIC.Orchestration.Data;

namespace LOJIC.Hangfire.Jobs.Heartbeat
{
    [Queue("age_heartbeat")]
    class ArcGisEnterpriseHeartBeatJob : BaseJob
    {
        private readonly OrchestrationMetadataContext _context;
        private readonly GisEnterpriseHeartBeatService _heartbeat;
        private string _targetQueue = "age_heartbeat";

        public ArcGisEnterpriseHeartBeatJob(OrchestrationMetadataContext context, GisEnterpriseHeartBeatService heartbeat)
        {
            _context = context;
            _heartbeat = heartbeat;
        }

        public void Init(PerformContext context)
        {
            context.WriteLine($"Starting AGE_HEARTBEAT for {Environment.MachineName}");
            var heartbeat = _heartbeat.GetNewHeartbeat();
            //Log current state to SQL on startup to ensure there is heartbeat data.
            _heartbeat.AddHeartbeatEntry(heartbeat);
            context.WriteLine($"AGE_HEARTBEAT online for {Environment.MachineName}");
        }
        public void Run(PerformContext context)
        {
            context.WriteLine($"AGE_HEARTBEAT run {Environment.MachineName}");
            var c = JobStorage.Current.GetMonitoringApi();
            //Get the current machine name
            var machineName = Environment.MachineName.ToLower();

            //get the name of the first server pool for this machine
            var targetPool = c.Servers()
                .Where(dto => dto.Name.Contains(machineName) && dto.Queues.Contains(_targetQueue))
                .Select(dto => dto.Name.Split($"{machineName} - ")[1].Split(":")[0]).FirstOrDefault();
            context.WriteLine($"AGE_HEARTBEAT TargetPool: {targetPool}");
            //perform current heartbeat
            var serviceIsAvailable = _heartbeat.GetNewHeartbeat();
            context.WriteLine($"AGE_HEARTBEAT TargetPool Available: {serviceIsAvailable}");
            //Check last 15 heartbeat states from SQL
            var mostRecentHeartbeats = _context.ArcgisEnterpriseSqlHeartbeats.Take(15);
            
            //handle updates to service availability
            _heartbeat.HandleHeartbeatState(serviceIsAvailable, mostRecentHeartbeats, targetPool);

            //Log current state to SQL
            _heartbeat.AddHeartbeatEntry(serviceIsAvailable);
            context.WriteLine($"AGE_HEARTBEAT heartbeat check finished");
        }
    }
}
