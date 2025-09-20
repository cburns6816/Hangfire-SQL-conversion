using System;
using System.Linq;
using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using LOJIC.Hangfire.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LOJIC.Hangfire.Extensions
{
    public static class HangfireConfigurationExtensions
    {
        public static void AddHangfireConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            Common.ScriptLocation = configuration.GetSection("ScriptLocation").Value;
            services.AddHangfire(hangFireConfig => hangFireConfig
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseConsole()
                .UseFilter(new AutomaticRetryAttribute { Attempts = 1 })
                .UseFilter(new HangfirePreserverAttribute())
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));
        }
        public static void AddWorkerPoolConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var serverWorkerPoolConfiguration = WorkerPoolsConfig.GetPoolConfiguration(configuration);

            foreach (var workerpool in serverWorkerPoolConfiguration.WorkerPools)
            {
                if (workerpool.Name == "localmachine")
                    workerpool.SubscribedQueues.Add(Environment.MachineName.ToLower());
                
                var serverName = $"{Environment.MachineName.ToLower()} - {workerpool.Name}";
                services.AddHangfireServer(options =>
                {
                    options.WorkerCount = workerpool.WorkerCount;
                    options.Queues = workerpool.SubscribedQueues.ToArray();
                    options.ServerName = serverName;
                });
            }
        }
    }
}
