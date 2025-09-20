using System;
using System.Management.Automation;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using LOJIC.Hangfire.Config;
using LOJIC.Hangfire.Helpers;

namespace LOJIC.Hangfire.Jobs.ServiceManagement.ArcGisServer
{
    class GisServerWebService : BaseJob
    {
        public void Start(PerformContext context)
        {
            context.WriteLine($"GisServerWebService Start: {Environment.MachineName}");
            var results = PowerShellJobHelper.ExecuteCommand<PSObject>(new PSCommand().AddCommand($"{Common.ScriptLocation}/Shared/DisableRedirect.ps1"));

            foreach (var lineItem in results)
            {
                context.WriteLine($"GisServerWebService Start PSLog: {lineItem}");
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void Stop(PerformContext context)
        {
            context.WriteLine($"GisServerWebService Stop: {Environment.MachineName}");
            var results = PowerShellJobHelper.ExecuteCommand<PSObject>(new PSCommand().AddCommand($"{Common.ScriptLocation}/Shared/EnableRedirect.ps1"));

            foreach (var lineItem in results)
            {
                context.WriteLine($"GisServerWebService Stop PSLog: {lineItem}");
            }
        }
    }
}
