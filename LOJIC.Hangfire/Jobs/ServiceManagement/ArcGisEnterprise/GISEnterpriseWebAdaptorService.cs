using System;
using System.Management.Automation;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using LOJIC.Hangfire.Config;
using LOJIC.Hangfire.Helpers;

namespace LOJIC.Hangfire.Jobs.ServiceManagement.ArcGisEnterprise
{
    class GISEnterpriseWebAdaptorService : BaseJob
    {
        public void Start(PerformContext context)
        {
            context.WriteLine($"GISEnterpriseWebAdaptorService Start: {Environment.MachineName}");
            var results = PowerShellJobHelper.ExecuteCommand<PSObject>(new PSCommand().AddCommand($"{Common.ScriptLocation}/Shared/DisableRedirect.ps1"));

            foreach (var lineItem in results)
            {
                context.WriteLine($"GISEnterpriseWebAdaptorService Start PSLog: {lineItem}");
            }
        }

        [AutomaticRetry(Attempts = 0)]
        public void Stop(PerformContext context)
        {
            context.WriteLine($"GISEnterpriseWebAdaptorService Stop: {Environment.MachineName}");
            var results = PowerShellJobHelper.ExecuteCommand<PSObject>(new PSCommand().AddCommand($"{Common.ScriptLocation}/Shared/EnableRedirect.ps1"));
            
            foreach (var lineItem in results)
            {
                context.WriteLine($"GISEnterpriseWebAdaptorService Stop PSLog: {lineItem}");
            }
        }
    }
}
