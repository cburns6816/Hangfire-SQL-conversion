using Hangfire.Server;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace LOJIC.Hangfire.Jobs
{
    abstract class BasePowerShellJob :BaseJob
    {
        public Collection<PSObject> returnedObjects { get; set; }
        
        public virtual void Run(PerformContext context)
        {
            
        }
    }
}