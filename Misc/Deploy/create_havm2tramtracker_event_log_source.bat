:: Must be run as Administrator
:: Only run this once during deployment to create the event source
powershell -ExecutionPolicy ByPass -f "%~dp0\create_havm2tramtracker_event_log_source.ps1"