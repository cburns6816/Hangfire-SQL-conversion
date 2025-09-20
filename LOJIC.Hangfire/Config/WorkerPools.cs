using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace LOJIC.Hangfire.Config
{
    public class WorkerPoolConfiguration
    {
        public Workerpool[] WorkerPools { get; set; }
    }

    public class Workerpool
    {
        public string Name { get; set; }
        public int WorkerCount { get; set; }

        public HashSet<string> SubscribedQueues { get; set; }
    }
    

    public static class WorkerPoolsConfig
    {
        private static List<Workerpool> _workerpools = new List<Workerpool>();

        public static void Add(Workerpool pool)
        {
            _workerpools.Add(pool);
        }
        public static WorkerPoolConfiguration GetPoolConfiguration(IConfiguration configuration) => new WorkerPoolConfiguration
        {
            WorkerPools = configuration.GetSection("HangfireServer:Pools").Get<Workerpool[]>()
        };

    }
}
