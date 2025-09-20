using System;
using System.Linq;
using System.Management.Automation;
using LOJIC.Hangfire.Config;
using LOJIC.Hangfire.Extensions;
using LOJIC.Hangfire.Helpers;
using LOJIC.Hangfire.Jobs.ServiceManagement;
using LOJIC.Hangfire.Jobs.ServiceManagement.ArcGisEnterprise;
using LOJIC.Hangfire.Services.Notifications;
using LOJIC.Orchestration.Data;
using LOJIC.Orchestration.Data.Models.Heartbeat;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace LOJIC.Hangfire.Services.Heartbeat
{
    class GisEnterpriseHeartBeatService : BaseHeartbeatService
    {
        private readonly OrchestrationMetadataContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _email;

        public GisEnterpriseHeartBeatService(OrchestrationMetadataContext context, IConfiguration configuration, EmailService email)
        {
            _context = context;
            _configuration = configuration;
            _email = email;
        }

        public override bool GetNewHeartbeat()
        {
            
            var sqlServer = _configuration.GetSection("HeartbeatTargets")["SqlServer"];
            var connectionString = $"Server={sqlServer};Database=master;Integrated Security=True;";
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT 1", conn))
                    {
                        var result = cmd.ExecuteScalar();
                        return (result != null && Convert.ToInt32(result) == 1);
                    }
                }
            }
            catch
            {
                return false;
            }
        
        }

        public override void AddHeartbeatEntry(bool serviceIsAvailable)
        {
            var currentHeartbeat = new ARCGISEnterpriseOracleHeartbeat
            {
                ServiceIsAvailable = serviceIsAvailable,
                Created = DateTimeOffset.Now
            };
            _context.Add(currentHeartbeat);
            _context.SaveChanges();
        }

        public override void ServiceUp<T>(IQueryable<T> mostRecentHeartbeats, string targetPool)
        {
            var previousStatus = mostRecentHeartbeats.Take(15);
            var firstUpOccurrence = !previousStatus.Any(heartbeat => heartbeat.ServiceIsAvailable);

            //if this is the first time the service was up in the past 15 heartbeats then we send a notification
            if (firstUpOccurrence)
            {
                _email.Send(new EmailService.EmailDto()
                {
                    Body = $"ArcGIS Enterprise environment {targetPool} is back online.",
                    Subject = $"{targetPool} ArcGIS Enterprise Online"
                });
            }
            BackgroundJobHelper.EnqueueOnAllSubscribingServers<GISEnterpriseService>(service => service.Start(null), "age_gis", targetPool);
            BackgroundJobHelper.EnqueueOnAllSubscribingServers<GISEnterpriseWebAdaptorService>(service => service.Start(null), "age_webadaptor", targetPool);
        }

        public override void ServiceDown<T>(bool serviceIsAvailable, IQueryable<T> mostRecentHeartbeats, string targetPool)
        {
            var previousStatus = mostRecentHeartbeats.Take(15);
            var firstDownOccurrence = !previousStatus.Any(heartbeat => !heartbeat.ServiceIsAvailable);


            //if this is the first time the service was down in the past 15 heartbeats then we send a notification
            if (firstDownOccurrence)
            {
                _email.Send(new EmailService.EmailDto()
                {
                    Body = $"ArcGIS Enterprise environment {targetPool} is offline",
                    Subject = $"{targetPool} ArcGIS Enterprise Offline"
                });
            }
            BackgroundJobHelper.EnqueueOnAllSubscribingServers<GISEnterpriseService>(service => service.Stop(null), "age_gis", targetPool);
            BackgroundJobHelper.EnqueueOnAllSubscribingServers<GISEnterpriseWebAdaptorService>(service => service.Stop(null), "age_webadaptor", targetPool);

        }
    }
}
