Havm2TramTracker TestComparisons
============

Note: Read the core Havm2TramTracker README.md before reading this one.

# Overview
The TestComparisons functionality of Havm2TramTracker compares *existing* data with *new* and writes the results to some database tables.

# Concepts

The project assumes that *existing* data is present in the T_Temp_Trips table and the T_Temp_Schedules tables, and assumes that *new* data is present in tables of the same name but with a configurable suffix (e.g. T_Temp_Trips_TTBU & T_Temp_Schedules_TTBU). It assumes all these tables are in the save database. Scripts to create the *_TTBU* tables can be found under source control in Database\Comparisons\Create_OT_Temp_xxx_TTBU_tables.sql

Each time the comparisons run they run for every defined table (T_Temp_Trips & T_Temp_Schedules).

## Console app

The comparisons are run via a console app - YarraTrams.Havm2TramTracker.Console.exe. (This console app is also used for other Havm2TramTracker functions, such as testing and Production support.)

There are 7 config settings relevant to the TestComparisons:

Under YarraTrams.Havm2TramTracker.Processor.Properties.Settings

1. DbTableSuffix - the suffix for the *new* tables (e.g. *_TTBU*)

Under YarraTrams.Havm2TramTracker.TestComparisons.Properties.Settings

2. TramTrackerDB - the database connection string
3. ComparisonSummaryEmailFrom
4. ComparisonSummaryEmailsTo - mutliple addresses delimited by commas or semi-colons
5. SmtpHost
6. SmtpUsername
7. SmtpPassword

You can use the IOP Mailgun details in LastPass for the Smtp settings.

## Database

The project uses 8 database tables to store comparison results. It assumes these tables are present in the same database as the tables being compared (i.e. a TramTracker database on a dev server).

### Havm2TTComparisonRun

- One record is added here each time the comparisons are run. This record serves as a parent to the records in the other tables.

### Havm2TTComparisonRunTable

- A record is added here for each table comparison that occurs in a run - one record for T_Temp_Trips and one for T_Temp_Schedules. The records summarise the comparison differences.

### Havm2TTComparison_T_Temp_Trips_MissingFromNew

- Every record that is in the *existing* T_Temp_Trips table but not in the *new* T_Temp_Trips table is written here.

### Havm2TTComparison_T_Temp_Trips_ExtraInNew

- Every record that is in the *new* T_Temp_Trips table but not in the *existing* T_Temp_Trips table is written here.

### Havm2TTComparison_T_Temp_Trips_Differing

- Every record that is matched between *existing* and *new*, but differs in some way is added to this table. Both the record from the *existing* table and the record from the *new* table is saved.

### Havm2TTComparison_T_Temp_Schedules_MissingFromNew

- Every record that is in the *existing* T_Temp_Schedules table but not in the *new* T_Temp_Schedules table is written here.

### Havm2TTComparison_T_Temp_Schedules_ExtraInNew

- Every record that is in the *new* T_Temp_Schedules table but not in the *existing* T_Temp_Schedules table is written here.

### Havm2TTComparison_T_Temp_Schedules_Differing

- Every record that is matched between *existing* and *new*, but differs in some way is added to this table. Both the record from the *existing* table and the record from the *new* table is saved.

Scripts to create these tables can be found under source control in Database\Comparisons\CreateComparisonTables.sql

# Steps to execute

1. Create the required database tables.
   - Database\Comparisons\Create_OT_Temp_xxx_TTBU_tables.sql
   - Database\Comparisons\CreateComparisonTables.sql
2. Get Havm2TramTracker working. (See separate Havm2TramTracker README.md)
3. Populate the TestComparisons app settings.
4. Open the Windows command prompt and either:
   - Run YarraTrams.Havm2TramTracker.Console.exe with the *sidebyside* parameter ; **or**
   - Run YarraTrams.Havm2TramTracker.Console.exe without the parameter and choose the *Compare Existing and New data* menu item.
5. A "Run ID" will be printed to the console and you should now be able to analyse the differences by looking at the records in the 8 tables with this Run ID.
6. A summary email will also be sent to the defined emails address(es).