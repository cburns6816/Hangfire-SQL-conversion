using System;
using System.Management.Automation;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using LOJIC.Hangfire.Config;
using LOJIC.Hangfire.Helpers;

namespace LOJIC.Hangfire.Jobs.ServiceManagement.ArcGisServer
{
    class GisServerAppsService : BaseJob
    {
        public void Start(PerformContext context)
        {
            context.WriteLine($"GisServerAppsService Start: {Environment.MachineName}");
            var results = PowerShellJobHelper.ExecuteCommand<PSObject>(new PSCommand().AddCommand($"{Common.ScriptLocation}/Shared/DisableRedirect.ps1"));

            foreach (var lineItem in results)
            {
                context.WriteLine($"GisServerAppsService Start PSLog: {lineItem}");
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void Stop(PerformContext context)
        {
            context.WriteLine($"GisServerAppsService Stop: {Environment.MachineName}");
            var results = PowerShellJobHelper.ExecuteCommand<PSObject>(new PSCommand().AddCommand($"{Common.ScriptLocation}/Shared/EnableRedirect.ps1"));

            foreach (var lineItem in results)
            {
                context.WriteLine($"GisServerAppsService Stop PSLog: {lineItem}");
            }
        }
    }
}