$service = Get-Service -Name "LojicOrchestrator" -ErrorAction SilentlyContinue
if ($null -ne $service) {
    # Service Exists, stopping
    Stop-Service -Name "LojicOrchestrator"
} else {
    Write-Debug -Message "Service LogicOrchestrator does not exist. Is this a new server?"
}