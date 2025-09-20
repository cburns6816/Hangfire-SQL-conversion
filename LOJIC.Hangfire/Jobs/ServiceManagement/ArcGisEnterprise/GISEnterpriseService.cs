using System;
using System.Management.Automation;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using LOJIC.Hangfire.Config;
using LOJIC.Hangfire.Helpers;

namespace LOJIC.Hangfire.Jobs.ServiceManagement.ArcGisEnterprise
{
    class GISEnterpriseService :BaseJob
    {
        public void Start(PerformContext context)
        {
            context.WriteLine($"GISEnterpriseService Start: {Environment.MachineName}");
            var results = PowerShellJobHelper.ExecuteCommand<PSObject>(new PSCommand().AddCommand($"{Common.ScriptLocation}/Shared/StartGISService.ps1"));

            foreach (var lineItem in results)
            {
                context.WriteLine($"GISEnterpriseService Start PSLog: {lineItem}");
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void Stop(PerformContext context)
        {
            context.WriteLine($"GISEnterpriseService Stop: {Environment.MachineName}");
            var results = PowerShellJobHelper.ExecuteCommand<PSObject>(new PSCommand().AddCommand($"{Common.ScriptLocation}/Shared/StopGISService.ps1"));

            foreach (var lineItem in results)
            {
                context.WriteLine($"GISEnterpriseService Stop PSLog: {lineItem}");
            }
        }
    }
}
