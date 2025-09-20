using System.Collections.ObjectModel;
using System.Management.Automation;

namespace LOJIC.Hangfire.Helpers
{
    public static class PowerShellJobHelper
    {
        public static Collection<T> ExecuteCommand<T>(PSCommand commands)
        {
            using PowerShell ps = PowerShell.Create(RunspaceMode.NewRunspace);
            ps.AddCommand("Set-ExecutionPolicy").AddParameter("ExecutionPolicy", "Unrestricted").Invoke(); 
            ps.Commands = commands;

            return ps.Invoke<T>();
        }
    }
}
