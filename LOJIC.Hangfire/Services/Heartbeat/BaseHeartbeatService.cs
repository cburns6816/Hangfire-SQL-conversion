using System.Linq;
using LOJIC.Orchestration.Data.Models;

namespace LOJIC.Hangfire.Services.Heartbeat
{
    public abstract class BaseHeartbeatService : IOrchestrationService
    {
        public abstract bool GetNewHeartbeat();
        public abstract void AddHeartbeatEntry(bool serviceIsAvailable);
        public abstract void ServiceUp<T>(IQueryable<T> mostRecentHeartbeats, string targetPool) where T : BaseServiceHeartbeat;
        public abstract void ServiceDown<T>(bool serviceIsAvailable, IQueryable<T> mostRecentHeartbeats, string targetPool) where T : BaseServiceHeartbeat;
        public void HandleHeartbeatState<T>(bool serviceIsAvailable, IQueryable<T> mostRecentHeartbeats, string targetPool) where T : BaseServiceHeartbeat
        {
            //check if the service was previously available - If there is no log of this service it is presumed to previously be up.
            var serviceWasAvailable = mostRecentHeartbeats.OrderByDescending(heartbeat => heartbeat.Created)
                .FirstOrDefault()?.ServiceIsAvailable ?? true;
            if (serviceIsAvailable) //if the database is up we should check to see if we need to start services
            {
                if(!serviceWasAvailable)
                    ServiceUp(mostRecentHeartbeats,targetPool);
            }
            else //if the database is down we should stop services if the service was just online. Otherwise, we should have already called the stop services before.
            {
                if (serviceWasAvailable)
                    ServiceDown(serviceIsAvailable, mostRecentHeartbeats, targetPool);
            }
        }
    }
}
