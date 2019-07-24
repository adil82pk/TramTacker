using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace YarraTrams.Havm2TramTracker.TestComparisons
{
    [Tags("TTBU")]
    [Migration(201907191716)]
    public class DB_201907191716_UpdateTripsAndSchedulesPK : Migration
    {
        public override void Up()
        {
            // Add operational day field to Schedules tables
            Alter.Table("T_Schedules").AddColumn("OperationalDay").AsDateTime().Nullable().SetExistingRowsTo(new DateTime(1983, 9, 26));
            Alter.Table("T_Temp_Schedules").AddColumn("OperationalDay").AsDateTime().Nullable().SetExistingRowsTo(new DateTime(1983, 9, 26));

            // Populate OperationalDay with dummy data that ensures the uniqueness of each record
            Execute.Sql(@"UPDATE T_Temp_Schedules SET OperationalDay = DATEADD(day,[DayOfWeek],OperationalDay);");
            Execute.Sql(@"UPDATE T_Schedules SET OperationalDay = DATEADD(day,[DayOfWeek],OperationalDay);");

            // Make operational day field on Schedules tables mandatory
            Alter.Table("T_Schedules").AlterColumn("OperationalDay").AsDateTime().NotNullable();
            Alter.Table("T_Temp_Schedules").AlterColumn("OperationalDay").AsDateTime().NotNullable();

            // Change the PK constraint on T_Trips
            Execute.Sql(@"IF (EXISTS(SELECT * 
                                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                WHERE CONSTRAINT_NAME = 'PK_T_Trips'))
                            BEGIN
                                ALTER TABLE[dbo].[T_Trips] DROP CONSTRAINT[PK_T_Trips];
                            END");
            Create.PrimaryKey("PK_T_Trips").OnTable("T_Trips").Columns(new string[] { "RunNo", "RouteNo", "FirstTime", "OperationalDay" });

            // Change the PK constraint on T_Temp_Trips
            Execute.Sql(@"IF (EXISTS(SELECT * 
                                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                WHERE CONSTRAINT_NAME = 'PK_T_Temp_Trips'))
                            BEGIN
                                ALTER TABLE[dbo].[T_Temp_Trips] DROP CONSTRAINT[PK_T_Temp_Trips];
                            END");
            Create.PrimaryKey("PK_T_Temp_Trips").OnTable("T_Temp_Trips").Columns(new string[] { "RunNo", "RouteNo", "FirstTime", "OperationalDay" });

            // Change the PK constraint on T_Schedules
            Execute.Sql(@"IF (EXISTS(SELECT * 
                                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                WHERE CONSTRAINT_NAME = 'PK_T_Schedules_1'))
                            BEGIN
                                ALTER TABLE[dbo].[T_Schedules] DROP CONSTRAINT[PK_T_Schedules_1];
                            END");
            Create.PrimaryKey("PK_T_Schedules_1").OnTable("T_Schedules").Columns(new string[] { "TripID", "RunNo", "StopID", "RouteNo", "Time", "OperationalDay" });

            // Change the PK constraint on [T_Temp_Schedules]
            Execute.Sql(@"IF (EXISTS(SELECT * 
                                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                WHERE CONSTRAINT_NAME = 'PK_T_Temp_Schedules_1'))
                            BEGIN
                                ALTER TABLE[dbo].[T_Temp_Schedules] DROP CONSTRAINT[PK_T_Temp_Schedules_1];
                            END");
            Create.PrimaryKey("PK_T_Temp_Schedules_1").OnTable("[T_Temp_Schedules]").Columns(new string[] { "TripID", "RunNo", "StopID", "RouteNo", "Time", "OperationalDay" });
        }

        public override void Down()
        {
            // Revert the PK constraint on T_Trips
            Execute.Sql(@"IF (EXISTS(SELECT * 
                                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                WHERE CONSTRAINT_NAME = 'PK_T_Trips'))
                            BEGIN
                                ALTER TABLE[dbo].[T_Trips] DROP CONSTRAINT[PK_T_Trips];
                            END");
            Create.PrimaryKey("PK_T_Trips").OnTable("T_Trips").Columns(new string[] { "RunNo", "RouteNo", "FirstTime", "DayOfWeek" });
            
            // Revert the PK constraint on T_Temp_Trips
            Execute.Sql(@"IF (EXISTS(SELECT * 
                                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                WHERE CONSTRAINT_NAME = 'PK_T_Temp_Trips'))
                            BEGIN
                                ALTER TABLE[dbo].[T_Temp_Trips] DROP CONSTRAINT[PK_T_Temp_Trips];
                            END");
            Create.PrimaryKey("PK_T_Temp_Trips").OnTable("T_Temp_Trips").Columns(new string[] { "RunNo", "RouteNo", "FirstTime", "DayOfWeek" });

            // Revert the PK constraint on T_Schedules
            Execute.Sql(@"IF (EXISTS(SELECT * 
                                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                WHERE CONSTRAINT_NAME = 'PK_T_Schedules_1'))
                            BEGIN
                                ALTER TABLE[dbo].[T_Schedules] DROP CONSTRAINT[PK_T_Schedules_1];
                            END");
            Create.PrimaryKey("PK_T_Schedules_1").OnTable("T_Schedules").Columns(new string[] { "TripID", "RunNo", "StopID", "RouteNo", "Time", "DayOfWeek" });

            // Revert the PK constraint on [T_Temp_Schedules]
            Execute.Sql(@"IF (EXISTS(SELECT * 
                                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                                WHERE CONSTRAINT_NAME = 'PK_T_Temp_Schedules_1'))
                            BEGIN
                                ALTER TABLE[dbo].[T_Temp_Schedules] DROP CONSTRAINT[PK_T_Temp_Schedules_1];
                            END");
            Create.PrimaryKey("PK_T_Temp_Schedules_1").OnTable("[T_Temp_Schedules]").Columns(new string[] { "TripID", "RunNo", "StopID", "RouteNo", "Time", "DayOfWeek" });
            
            // Remove operational day field from Schedules tables
            Delete.Column("OperationalDay").FromTable("T_Schedules");
            Delete.Column("OperationalDay").FromTable("T_Temp_Schedules");
        }
    }
}
