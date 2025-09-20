using System;
using System.Linq;
using System.Linq.Expressions;
using Hangfire;
using Hangfire.States;
using LOJIC.Hangfire.Jobs;

namespace LOJIC.Hangfire.Extensions
{
    public static class BackgroundJobHelper
    {
        public static void Enqueue<T>(Expression<Action<T>> actionMethod)
        {
            BackgroundJob.Enqueue(actionMethod);
        }

        /// <summary>
        /// Will queue a Job in Hangfire to Run at the specified future date.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actionMethod">The method to be queued.</param>
        /// <param name="futureDate">The DateTime that the job should Run at.</param>
        /// <param name="hangfireId">The id of the job in Hangfire.</param>
        public static string EnqueueForLater<T>(Expression<Action<T>> actionMethod, DateTime futureDate) where T : BaseJob
        {
            return BackgroundJob.Schedule(actionMethod, DateTime.Now >= futureDate ? TimeSpan.Zero : futureDate.Subtract(DateTime.Now));
        }

        public static void EnqueueOnAllSubscribingServers<T>(Expression<Action<T>> actionMethod, string queueName, string poolName) where T : BaseJob
        {
            var c = JobStorage.Current.GetMonitoringApi();
            var servers = c.Servers().Where(s => s.Queues.Contains(queueName) && s.Name.Contains(poolName));
            var serverNames = servers.Select(dto => dto.Name.Substring(0, dto.Name.IndexOf($" - {poolName}")));
            
            foreach (var serverName in serverNames)
            {
                var client = new BackgroundJobClient();
                var state = new EnqueuedState(serverName.ToLower());
                client.Create(actionMethod, state);
            }
        }
    }
}
