@echo off
set TestComparisonsDBHostName=%1
set TestComparisonsDBName=%2
set UseActiveDirectory=%3
set TestComparisonsDBUserName=%4
set TestComparisonsDBPassword=%5

if "%UseActiveDirectory%"=="true" (
	echo "Running Havm2TramTracker TestComparisons DB migrations up using ActiveDirectory..."
	"../../packages/FluentMigrator.Console.3.2.1/net461/x64/Migrate.exe" /tag "TestComparison" /conn "Data Source=%TestComparisonsDBHostName%;Persist Security Info=True;database=%TestComparisonsDBName%;Integrated Security=SSPI;" /provider sqlserver2012 /assembly "../bin/Debug/YarraTrams.Havm2TramTracker.TestComparisons.dll" /verbose=true --task migrate
)

if "%UseActiveDirectory%"=="false" (
	echo "Running Havm2TramTracker TestComparisons DB migrations up using username/password..."
	"../../packages/FluentMigrator.Console.3.2.1/net461/x64/Migrate.exe" /tag "TestComparison" /conn "Data Source=%TestComparisonsDBHostName%;Persist Security Info=True;User ID=%TestComparisonsDBUserName%;Password=%TestComparisonsDBPassword%;database=%TestComparisonsDBName%;" /provider sqlserver2012 /assembly "../bin/Debug/YarraTrams.Havm2TramTracker.TestComparisons.dll" /verbose=true --task migrate
)

pause