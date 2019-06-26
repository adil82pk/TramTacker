@echo off
set TTBUDBHostName=%1
set TTBUDBName=%2
set UseActiveDirectory=%3
set TTBUDBUserName=%4
set TTBUDBPassword=%5

if "%UseActiveDirectory%"=="true" (
	echo "Running Havm2TramTracker TTBU DB migrations up using ActiveDirectory..."
	"../../packages/FluentMigrator.Console.3.2.1/net461/x64/Migrate.exe" /tag "TTBU" /conn "Data Source=%TTBUDBHostName%;Persist Security Info=True;database=%TTBUDBName%;Integrated Security=SSPI;" /provider sqlserver2012 /assembly "C:/havm2tramtracker/bin/YarraTrams.Havm2TramTracker.Processor.exe" /verbose=true --task migrate
)

if "%UseActiveDirectory%"=="false" (
	echo "Running Havm2TramTracker TTBU DB migrations up using username/password..."
	"../../packages/FluentMigrator.Console.3.2.1/net461/x64/Migrate.exe" /tag "TTBU" /conn "Data Source=%TTBUDBHostName%;Persist Security Info=True;User ID=%TTBUDBUserName%;Password=%TTBUDBPassword%;database=%TTBUDBName%;" /provider sqlserver2012 /assembly "C:/havm2tramtracker/bin/YarraTrams.Havm2TramTracker.Processor.exe" /verbose=true --task migrate
)

pause