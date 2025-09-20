$service = Get-Service -Name "LojicOrchestrator" -ErrorAction SilentlyContinue
if ($null -ne $service) {
    # Service Exists
    Start-Service "LojicOrchestrator"
} else {
    # New server, run setup script
    $serviceConfig = @{
        Name = "LojicOrchestrator"
        BinaryPathName = "C:\Hangfire\LOJIC.Orchestration.exe"
        StartupType = "Automatic"
        Description = "LojicOrchestrator"
    }
    New-Service @serviceConfig
    Start-Service "LojicOrchestrator"
}