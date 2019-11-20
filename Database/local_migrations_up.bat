@echo off
set MIGRATION_PATH="%~dp0%..\YarraTrams.Havm2TramTracker\YarraTrams.Havm2TramTracker.Processor\bin\Debug\YarraTrams.Havm2TramTracker.Processor.exe"
set TTBUDBHostName=IOP\SQLEXPRESS
set TTBUDBName=TramTracker

echo "Running Havm2TramTracker TTBU DB migrations UP. Continue?"
pause
	"../YarraTrams.Havm2TramTracker/packages/FluentMigrator.1.6.2/tools/Migrate.exe" /tag "TTBU" /conn "Data Source=%TTBUDBHostName%;Persist Security Info=True;database=%TTBUDBName%;Integrated Security=SSPI;" /provider sqlserver2012 /assembly "%MIGRATION_PATH%" /verbose=true --task migrate --timeout 300
pause