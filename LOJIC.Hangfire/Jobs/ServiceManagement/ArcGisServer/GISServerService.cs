using System;
using System.Management.Automation;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using LOJIC.Hangfire.Config;
using LOJIC.Hangfire.Helpers;

namespace LOJIC.Hangfire.Jobs.ServiceManagement
{
    class GisServerService : BaseJob
    {
        public void Start(PerformContext context)
        {
            context.WriteLine($"GisServerService Start: {Environment.MachineName}");
            var results = PowerShellJobHelper.ExecuteCommand<PSObject>(new PSCommand().AddCommand($"{Common.ScriptLocation}/Shared/StartGISService.ps1"));
            
            foreach (var lineItem in results)
            {
                context.WriteLine($"GisServerService Start PSLog: {lineItem}");
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void Stop(PerformContext context)
        {
            context.WriteLine($"GisServerService Stop: {Environment.MachineName}");
            var results = PowerShellJobHelper.ExecuteCommand<PSObject>(new PSCommand().AddCommand($"{Common.ScriptLocation}/Shared/StopGISService.ps1"));

            foreach (var lineItem in results)
            {
                context.WriteLine($"GisServerService Stop PSLog: {lineItem}");
            }
        }
    }
}
