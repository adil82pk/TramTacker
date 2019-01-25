$source = "Havm2TramTracker"
$logName = "Havm2TramTracker.Events"

Write-Output "Creating Log Source $source if not exist..."
if ([System.Diagnostics.EventLog]::SourceExists($source) -eq $false) {
    [System.Diagnostics.EventLog]::CreateEventSource($source, $logName)
    Write-EventLog -LogName $logName -Source $source -EntryType Information -Message "Created Windows Eventlog Source $source" -EventId 3000
    Write-Output "Created Windows Eventlog Source $source"
}