havm2tramtracker
============

## Overview

havm2tramtracker is 1 of 3 components in the tramTRACKER Uplift Project (TTBU).
It is responsible for collectecting near schedule/timetable data from HAVM2, and inserting it into relrevant tramTRACKER database tables (that are later used for predictions).

---

## Technology

* Backend language: MS .Net / C# 5.0
* Backend framework: [.Net v4.5.1](https://www.microsoft.com/en-au/download/details.aspx?id=40773)
* Database: [MS SQL Server](https://www.microsoft.com/en-au/sql-server/sql-server-downloads)
* JSON: [Newtonsoft.JSON v6.0.3](https://github.com/JamesNK/Newtonsoft.Json)
  * This old version of Newtonsoft.JSON is being used because our production deployment uses an old version of nuget and this version of nuget doesn't support recent versions of Newtonsoft.JSON. See [Deployment](#deployment).

---

## Setup

### Prerequisites

Requires access to a HAVM2 instance with the ExternalAPI running.

### Configuration

The Windows services has the following config settings available:

- DueTime - the service kicks of processing once per day at this time.
- Havm2TramTrackerAPI - full address of the HAVM2 ExternalAPI
- Havm2TramTrackerTimeoutSeconds - timeout for the API call, should be set to a high number.
- TramTrackerDB - DB connection string
- DbTableSuffix - For testing, this application can insert into test tables (with this suffix), instead of the real tables.
- LogFilePath
- LogT_Temp_TripRowsToFilePriorToInsert - True/False - controls whether we log this HAVM2 and TT data to file as we convert it.
- LogT_Temp_SchedulesRowsToFilePriorToInsert - True/False - controls whether we log this HAVM2 and TT data to file as we convert it.
- LogT_Temp_SchedulesMasterRowsToFilePriorToInsert - True/False - controls whether we log this HAVM2 and TT data to file as we convert it.
- LogT_Temp_SchedulesDetailsRowsToFilePriorToInsert - True/False - controls whether we log this HAVM2 and TT data to file as we convert it.

### Development setup
* IDE: [Visual Studio Enterprise >= 2013](https://visualstudio.microsoft.com/vs/enterprise/)
* Database admin: [Server Management Studio (SSMS) >= 2012](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-2017)

#### Local Database setup
1. Create TramTracker database
2. Create tables - run `database/Create_OT_Temp_xxx_TTBU_tables.sql`

### Comparison Testing
Havm2TramTracker has a TestComparison feature that compares tables populated by two different pieces of code.
Further information on this tool can be found here `YarraTrams.Havm2TramTracker/YarraTrams.Havm2TramTracker.TestComparisons/README.md`

### Testing server setup
TODO

### Production server setup
TODO

## Deployment

Steps are outlined in Confluence under [YT Knowledge base/tramTRACKER Backend Uplift/TTBU Solution Overview/1.3. New Solution/1.3.1. HAVM 2.0 to tramTRACKER Import Service](https://inoutput.atlassian.net/wiki/spaces/YKB/pages/767787154/Havm2TramTracker+Deployment+Steps)

## Known Issues
TODO
