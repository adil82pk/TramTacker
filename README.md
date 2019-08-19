havm2tramtracker
============

## Overview

havm2tramtracker is 1 of 3 components in the tramTRACKER Uplift Project (TTBU).
It is responsible for collectecting near schedule/timetable data from HAVM2, and inserting it into relrevant tramTRACKER database tables (that are later used for predictions).

---

## Technology

* Backend language: MS .Net / C# 5.0
* Backend framework: [.Net v4.6.1](The devloper pack is required on each host server - https://www.microsoft.com/en-au/download/details.aspx?id=49978)
* Database: [MS SQL Server](https://www.microsoft.com/en-au/sql-server/sql-server-downloads)
  * [FluentMigrator](https://fluentmigrator.github.io/)
* JSON: [Newtonsoft.JSON v12.0.2](https://github.com/JamesNK/Newtonsoft.Json)

---

## Setup

### Prerequisites

Requires access to a HAVM2 instance with the ExternalAPI running.

Requires a local TramTracker database containing the following objects:

#### Tables

| Name  | With Data?  | Notes  |
|---|---|---|
|  DailyRunoutTimes | No  |   |
|  PredictionCacheRefreshLog | No   |   |
|  Predictions | No   |   |
|  PredictionsCache | No   |   |
|  StopMessages | No   |   |
|  T_DestinationLookUps | Yes  |   |
|  T_Preferences | Yes  |   |
|  T_RouteLookUps | Yes  |   |
|  T_Routes_Stops | Yes  |   |
|  T_Schedules | No   |   |
|  T_Stops | Yes  |   |
|  T_SubVehicleInformation | No   |   |
|  T_Temp_Schedules | No   |   |
|  T_Temp_SchedulesDetails | No   | Havm2TT populates this but it isn't used for anything else  |
|  T_Temp_SchedulesMaster  | No   | Havm2TT populates this but it isn't used for anything else  |
|  T_Temp_Trips | No   |   |
|  T_Trams | Yes  |   |
|  T_Trips | No   |   |
|  T_VehicleInformation | No   |   |
|  T_VehicleRunAssignments | No   |   |

Note: The following tables will be created when the FluentMigrator scripts are run -
| Name  | Notes  |
|---|---|
VersionInfoAvmis2TT | FluentMigrator versioning  |
VersionInfoHavm2TT  | FluentMigrator versioning  |
VersionInfoTTPS  | FluentMigrator versioning  |

#### Stored Procedures

| Name | Can be stubbed? | Notes |
|---|---|---|
|  CreateDailyData2.5 | Yes  |   |
|  CreatePredictionSpecialEventRouteStops | Yes  | If testing Stop Message functionality then this needs the real code, and further table dependancies, and perhaps other jobs to run  |
|  GetStopMessages (can be stubbed, if you're not dealing with stop messages) | Yes  | If testing Stop Message functionality then this needs the real code, and further table dependancies, and perhaps other jobs to run  |
|  SetDayOfWeek | Yes  |   |

### Configuration

The Windows services has the following config settings available:

- CopyTodaysDataToLiveDueTime - the time that the service will wake up and copy the data from the temp tables to the live tables
- RefreshTempWithTomorrowsDataDueTime -  the time that the service will wake up and refresh the tmep tables with new data from Havm2
- Havm2TramTrackerAPI - full address of the HAVM2 ExternalAPI
- Havm2TramTrackerAPITimeoutSeconds - timeout for the API call, should be set to a high number
- MaxGetDataFromHavm2RetryCount - the number of times this application will retry following a failure when pulling data down from HAVM2
- GapBetweenGetDataFromHavm2RetriesInSecs - the time to wait between retries following a failure when pulling data down from HAVM2
- TramTrackerDB - DB connection string
- DBCommandTimeoutSeconds - timeout for every command executed by Havm2TramTracker against the TramTracker database
- MaxCopyToLiveRetryCount - the number of times this application will retry following a failure when copying the data from the temp tables to the live tables 
- GapBetweenCopyToLiveRetriesInSecs - the time to wait between retries following a failure when copying the data from the temp tables to the live tables 
- NumberOfPredictionsPerTripStop - the number of predictions that the TTPS will calculate for a stop at once
- NumberDailyTimetablesToRetrieve - the number of daily timetables to pull down from HAVM2
- DbTableSuffix - For testing, this application can insert into test tables (with this suffix), instead of the real tables
- LogFilePath - the path that log files are written to
- LogFilePathMaxSizeInBytes - the maximium size allowed for the log folder
- LogFilePathWarnSizeInBytesExceedsInBytes - the sise the log folder must get to prior to us warning about its size
- LogT_Temp_TripRowsToFilePriorToInsert - True/False - controls whether we log this HAVM2 and TT data to file as we convert it.
- LogT_Temp_SchedulesRowsToFilePriorToInsert - True/False - controls whether we log this HAVM2 and TT data to file as we convert it.
- LogT_Temp_SchedulesMasterRowsToFilePriorToInsert - True/False - controls whether we log this HAVM2 and TT data to file as we convert it.
- LogT_Temp_SchedulesDetailsRowsToFilePriorToInsert - True/False - controls whether we log this HAVM2 and TT data to file as we convert it

### Development setup
* IDE: [Visual Studio Enterprise >= 2013](https://visualstudio.microsoft.com/vs/enterprise/)
* Database admin: [Server Management Studio (SSMS) >= 2012](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-2017)

#### Local Database setup
1. Create (empty) TramTracker database
2. Create (base) tables - run `database/BaseTables.sql`
3. Create (base) store proc _stubs - run `database/BaseProcs.sql`
4. Run the FluentMigrator migrations - run `database/local_migrations_up.bat`
5. Pull down standing (stop, vehicle ,etc) data from a TramTracker database on the Yarra Trams network:
   1. Connect to the Yarra Trams VPN
   2. Locate a server hosting a TramTracker database with up-to-date standing data
   3. Add this server name/IP to your local HOSTS file
   4. Open SQL Server Management Studio on your local machine
   5. Right click the TramTracker database and select *Tasks/Import Data*
   6. Follow the wizard steps:
      1. Select Data Source as:
         *  SQL Server Native
         *  The server details you added to your HOSTS file
         *  The TramTracker database
      2. Select the Destination as:
         *  SQL Server Native
         *  localhost/SQLEXPRESS
         *  The (local) TramTracker database
      3. Copy data from one or more tables
      4. Tick the following tables:
         *  T_DestinationLookUps
         *  T_Preferences
         *  T_RouteLookUps
         *  T_Routes_Stops
         *  T_Stops
         *  T_Trams

### Comparison Testing
Havm2TramTracker has a TestComparison feature that compares tables populated by two different pieces of code.
Further information on this tool can be found here `YarraTrams.Havm2TramTracker/YarraTrams.Havm2TramTracker.TestComparisons/README.md`

## Deployment

Steps are outlined in Confluence under [YT Knowledge base/tramTRACKER Backend Uplift/TTBU Solution Overview/1.3. New Solution/1.3.1. HAVM 2.0 to tramTRACKER Import Service](https://inoutput.atlassian.net/wiki/spaces/YKB/pages/767787154/Havm2TramTracker+Deployment+Steps)

## Known Issues

None
