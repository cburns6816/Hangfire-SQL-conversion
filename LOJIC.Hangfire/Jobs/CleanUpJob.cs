using System;
using System.Linq;
using Hangfire.Console;
using Hangfire.Server;
using LOJIC.Orchestration.Data;
using Microsoft.Extensions.Configuration;

namespace LOJIC.Hangfire.Jobs
{
    class CleanUpJob : BaseJob
    {
        private readonly OrchestrationMetadataContext _context;
        private readonly IConfiguration _configuration;

        public CleanUpJob(OrchestrationMetadataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public void DeleteOldHeartbeats(PerformContext context)
        {
            context.WriteLine($"CleanupJob - Delete OldHeartbeats: {Environment.MachineName}");
            var oldServerHeartbeats = _context.ArcgisServerOracleHeartbeats.Where(heartbeat =>
                heartbeat.Created < DateTimeOffset.Now.AddDays(30));
            context.WriteLine($"CleanupJob - Old Heartbeat Count: {oldServerHeartbeats.Count()}");
            _context.RemoveRange(oldServerHeartbeats);
            var oldEnterpriseHeartbeats= _context.ArcgisEnterpriseOracleHeartbeats.Where(heartbeat =>
                heartbeat.Created < DateTimeOffset.Now.AddDays(30));
            context.WriteLine($"CleanupJob - Delete OldEnterpriseHeartbeats: {oldEnterpriseHeartbeats.Count()}");
            _context.RemoveRange(oldEnterpriseHeartbeats);
            context.WriteLine($"CleanupJob - Heartbeat Cleanup Completed: {Environment.MachineName}");
            _context.SaveChanges();

        }
    }
}
