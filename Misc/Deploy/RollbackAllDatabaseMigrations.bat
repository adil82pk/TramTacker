@echo off
set MIGRATION_PATH="%~dp0%..\havm2tramtracker\bin\YarraTrams.Havm2TramTracker.Processor.exe"
set TTBUDBHostName=<DB server name here>
set TTBUDBName=TramTracker

echo "Rolling back all Havm2TramTracker TTBU DB migrations (i.e. we're taking the database back to early 2019, pre inoutput!). Continue?"
pause
	"havm2tramtracker/YarraTrams.Havm2TramTracker/packages/FluentMigrator.Console.3.2.1/net461/x64/Migrate.exe" /tag "TTBU" /conn "Data Source=%TTBUDBHostName%;Persist Security Info=True;database=%TTBUDBName%;Integrated Security=SSPI;" /provider sqlserver2012 /assembly "%MIGRATION_PATH%" /verbose=true --task rollback:all
pause