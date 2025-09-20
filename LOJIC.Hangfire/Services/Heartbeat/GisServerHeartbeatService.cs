using System;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using LOJIC.Hangfire.Config;
using LOJIC.Hangfire.Extensions;
using LOJIC.Hangfire.Helpers;
using LOJIC.Hangfire.Jobs.ServiceManagement;
using LOJIC.Hangfire.Jobs.ServiceManagement.ArcGisServer;
using LOJIC.Hangfire.Services.Notifications;
using LOJIC.Orchestration.Data;

using LOJIC.Orchestration.Data.Models.Heartbeat;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace LOJIC.Hangfire.Services.Heartbeat
{
    public class GisServerHeartbeatService : BaseHeartbeatService
    {
        private readonly OrchestrationMetadataContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _email;
        private double delay = 0.5;

        public GisServerHeartbeatService(OrchestrationMetadataContext context, IConfiguration config, EmailService email)
        {
            _context = context;
            _configuration = config;
            _email = email;
            var webServiceDelay = _configuration.GetSection("HeartbeatTargets")["AGSServiceWebAppDelay"];
            if (!string.IsNullOrEmpty(webServiceDelay))
            {
                _ = double.TryParse(webServiceDelay, out delay);
            }
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
            var currentHeartbeat = new ARCGISServerOracleHeartbeat
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
                    Body = $"ArcGIS Server environment {targetPool} is back online.",
                    Subject = $"{targetPool} ArcGIS Server Online"
                });
            }

            BackgroundJobHelper.EnqueueOnAllSubscribingServers<GisServerService>(service => service.Start(null), "ags_gis", targetPool);
            BackgroundJobHelper.EnqueueOnAllSubscribingServers<GisServerWebService>(service => service.Start(null), "ags_webadaptor", targetPool);
            Thread.Sleep(TimeSpan.FromMinutes(delay));
            BackgroundJobHelper.EnqueueOnAllSubscribingServers<GisServerAppsService>(service => service.Start(null), "ags_apps", targetPool);
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
                    Body = $"ArcGIS Server environment {targetPool} is offline",
                    Subject = $"{targetPool} ArcGIS Server Offline"
                });
            }

  
            BackgroundJobHelper.EnqueueOnAllSubscribingServers<GisServerAppsService>(service => service.Stop(null), "ags_apps", targetPool);
            Thread.Sleep(TimeSpan.FromMinutes(delay));
            BackgroundJobHelper.EnqueueOnAllSubscribingServers<GisServerWebService>(service => service.Stop(null), "ags_webadaptor", targetPool);
            BackgroundJobHelper.EnqueueOnAllSubscribingServers<GisServerService>(service => service.Stop(null), "ags_gis", targetPool);
        }
    }
}
