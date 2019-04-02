using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace YarraTrams.Havm2TramTracker.TestComparisons
{
    [Tags("TestComparison")]
    [Migration(201904011600)]
    public class DB_201904011600_AddComparisonTables : Migration
    {
        public override void Up()
        {
            #region Parent Tables
            // Havm2TTComparisonRun
            Create.Table("Havm2TTComparisonRun")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("RunTime").AsDateTime().NotNullable();

            // Havm2TTComparisonRunTable
            Create.Table("Havm2TTComparisonRunTable")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("Havm2TTComparisonRunId").AsInt32().NotNullable()
                .WithColumn("TableName").AsAnsiString().NotNullable()
                .WithColumn("TotalRecordsExisting").AsInt32().NotNullable()
                .WithColumn("TotalRecordsNew").AsInt32().NotNullable()
                .WithColumn("RecordsIdentical").AsInt32().NotNullable()
                .WithColumn("RecordsMissingFromNew").AsInt32().NotNullable()
                .WithColumn("RecordsExtraInNew").AsInt32().NotNullable()
                .WithColumn("RecordsDiffering").AsInt32().NotNullable();

            Create.ForeignKey("FK_Havm2TTComparisonRunTable_Havm2TTComparisonRun")
                .FromTable("Havm2TTComparisonRunTable").ForeignColumn("Havm2TTComparisonRunId")
                .ToTable("Havm2TTComparisonRun").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);
            #endregion

            #region T_Temp_Trips
            // Havm2TTComparison_T_Temp_Trips_MissingFromNew
            Create.Table("Havm2TTComparison_T_Temp_Trips_MissingFromNew")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("Havm2TTComparisonRunId").AsInt32().NotNullable()
                .WithColumn("TripID").AsInt32().Nullable()
                .WithColumn("RunNo").AsFixedLengthAnsiString(5).NotNullable()
                .WithColumn("RouteNo").AsInt16().NotNullable()
                .WithColumn("FirstTP").AsFixedLengthAnsiString(4).NotNullable()
                .WithColumn("FirstTime").AsInt32().NotNullable()
                .WithColumn("EndTP").AsFixedLengthAnsiString(4).NotNullable()
                .WithColumn("EndTime").AsInt32().NotNullable()
                .WithColumn("AtLayoverTime").AsInt16().NotNullable()
                .WithColumn("NextRouteNo").AsInt16().NotNullable()
                .WithColumn("UpDirection").AsBoolean().NotNullable()
                .WithColumn("LowFloor").AsBoolean().NotNullable()
                .WithColumn("TripDistance").AsDecimal(6,3).NotNullable()
                .WithColumn("PublicTrip").AsBoolean().NotNullable()
                .WithColumn("DayOfWeek").AsByte().NotNullable();

            Create.ForeignKey("FK_Havm2TTComparison_T_Temp_Trips_MissingFromNew_Havm2TTComparisonRun")
                .FromTable("Havm2TTComparison_T_Temp_Trips_MissingFromNew").ForeignColumn("Havm2TTComparisonRunId")
                .ToTable("Havm2TTComparisonRun").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);

            // Havm2TTComparison_T_Temp_Trips_ExtraInNew
            Create.Table("Havm2TTComparison_T_Temp_Trips_ExtraInNew")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("Havm2TTComparisonRunId").AsInt32().NotNullable()
                .WithColumn("TripID").AsInt32().Nullable()
                .WithColumn("RunNo").AsFixedLengthAnsiString(5).NotNullable()
                .WithColumn("RouteNo").AsInt16().NotNullable()
                .WithColumn("FirstTP").AsFixedLengthAnsiString(4).NotNullable()
                .WithColumn("FirstTime").AsInt32().NotNullable()
                .WithColumn("EndTP").AsFixedLengthAnsiString(4).NotNullable()
                .WithColumn("EndTime").AsInt32().NotNullable()
                .WithColumn("AtLayoverTime").AsInt16().NotNullable()
                .WithColumn("NextRouteNo").AsInt16().NotNullable()
                .WithColumn("UpDirection").AsBoolean().NotNullable()
                .WithColumn("LowFloor").AsBoolean().NotNullable()
                .WithColumn("TripDistance").AsDecimal(6, 3).NotNullable()
                .WithColumn("PublicTrip").AsBoolean().NotNullable()
                .WithColumn("DayOfWeek").AsByte().NotNullable();

            Create.ForeignKey("FK_Havm2TTComparison_T_Temp_Trips_ExtraInNew_Havm2TTComparisonRun")
                .FromTable("Havm2TTComparison_T_Temp_Trips_ExtraInNew").ForeignColumn("Havm2TTComparisonRunId")
                .ToTable("Havm2TTComparisonRun").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);

            // Havm2TTComparison_T_Temp_Trips_Differing
            Create.Table("Havm2TTComparison_T_Temp_Trips_Differing")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("Havm2TTComparisonRunId").AsInt32().NotNullable()
                .WithColumn("PairIdentifier").AsGuid().NotNullable()
                .WithColumn("IsExisting").AsBoolean().NotNullable()
                .WithColumn("TripID").AsInt32().Nullable()
                .WithColumn("RunNo").AsFixedLengthAnsiString(5).NotNullable()
                .WithColumn("RouteNo").AsInt16().NotNullable()
                .WithColumn("FirstTP").AsFixedLengthAnsiString(4).NotNullable()
                .WithColumn("FirstTime").AsInt32().NotNullable()
                .WithColumn("EndTP").AsFixedLengthAnsiString(4).NotNullable()
                .WithColumn("EndTime").AsInt32().NotNullable()
                .WithColumn("AtLayoverTime").AsInt16().NotNullable()
                .WithColumn("NextRouteNo").AsInt16().NotNullable()
                .WithColumn("UpDirection").AsBoolean().NotNullable()
                .WithColumn("LowFloor").AsBoolean().NotNullable()
                .WithColumn("TripDistance").AsDecimal(6, 3).NotNullable()
                .WithColumn("PublicTrip").AsBoolean().NotNullable()
                .WithColumn("DayOfWeek").AsByte().NotNullable();

            Create.ForeignKey("FK_Havm2TTComparison_T_Temp_Trips_Differing_Havm2TTComparisonRun")
                .FromTable("Havm2TTComparison_T_Temp_Trips_Differing").ForeignColumn("Havm2TTComparisonRunId")
                .ToTable("Havm2TTComparisonRun").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);
            #endregion

            #region T_Temp_Schedules
            // Havm2TTComparison_T_Temp_Schedules_MissingFromNew
            Create.Table("Havm2TTComparison_T_Temp_Schedules_MissingFromNew")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("Havm2TTComparisonRunId").AsInt32().NotNullable()
                .WithColumn("TripID").AsInt32().Nullable()
                .WithColumn("RunNo").AsFixedLengthAnsiString(5).NotNullable()
                .WithColumn("StopID").AsFixedLengthAnsiString(8).NotNullable()
                .WithColumn("RouteNo").AsInt16().NotNullable()
                .WithColumn("OPRTimePoint").AsBoolean().NotNullable()
                .WithColumn("Time").AsInt32().NotNullable()
                .WithColumn("DayOfWeek").AsByte().NotNullable()
                .WithColumn("LowFloor").AsBoolean().NotNullable()
                .WithColumn("PublicTrip").AsBoolean().NotNullable();

            Create.ForeignKey("FK_Havm2TTComparison_T_Temp_Schedules_MissingFromNew_Havm2TTComparisonRun")
                .FromTable("Havm2TTComparison_T_Temp_Schedules_MissingFromNew").ForeignColumn("Havm2TTComparisonRunId")
                .ToTable("Havm2TTComparisonRun").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);

            // Havm2TTComparison_T_Temp_Schedules_ExtraInNew
            Create.Table("Havm2TTComparison_T_Temp_Schedules_ExtraInNew")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("Havm2TTComparisonRunId").AsInt32().NotNullable()
                .WithColumn("TripID").AsInt32().Nullable()
                .WithColumn("RunNo").AsFixedLengthAnsiString(5).NotNullable()
                .WithColumn("StopID").AsFixedLengthAnsiString(8).NotNullable()
                .WithColumn("RouteNo").AsInt16().NotNullable()
                .WithColumn("OPRTimePoint").AsBoolean().NotNullable()
                .WithColumn("Time").AsInt32().NotNullable()
                .WithColumn("DayOfWeek").AsByte().NotNullable()
                .WithColumn("LowFloor").AsBoolean().NotNullable()
                .WithColumn("PublicTrip").AsBoolean().NotNullable();

            Create.ForeignKey("FK_Havm2TTComparison_T_Temp_Schedules_ExtraInNew_Havm2TTComparisonRun")
                .FromTable("Havm2TTComparison_T_Temp_Schedules_ExtraInNew").ForeignColumn("Havm2TTComparisonRunId")
                .ToTable("Havm2TTComparisonRun").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);

            // Havm2TTComparison_T_Temp_Schedules_Differing
            Create.Table("Havm2TTComparison_T_Temp_Schedules_Differing")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("Havm2TTComparisonRunId").AsInt32().NotNullable()
                .WithColumn("PairIdentifier").AsGuid().NotNullable()
                .WithColumn("IsExisting").AsBoolean().NotNullable()
                .WithColumn("TripID").AsInt32().Nullable()
                .WithColumn("RunNo").AsFixedLengthAnsiString(5).NotNullable()
                .WithColumn("StopID").AsFixedLengthAnsiString(8).NotNullable()
                .WithColumn("RouteNo").AsInt16().NotNullable()
                .WithColumn("OPRTimePoint").AsBoolean().NotNullable()
                .WithColumn("Time").AsInt32().NotNullable()
                .WithColumn("DayOfWeek").AsByte().NotNullable()
                .WithColumn("LowFloor").AsBoolean().NotNullable()
                .WithColumn("PublicTrip").AsBoolean().NotNullable();

            Create.ForeignKey("FK_Havm2TTComparison_T_Temp_Schedules_Differing_Havm2TTComparisonRun")
                .FromTable("Havm2TTComparison_T_Temp_Schedules_Differing").ForeignColumn("Havm2TTComparisonRunId")
                .ToTable("Havm2TTComparisonRun").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);
            #endregion

            #region T_Temp_SchedulesMaster
            // Havm2TTComparison_T_Temp_SchedulesMaster_MissingFromNew
            Create.Table("Havm2TTComparison_T_Temp_SchedulesMaster_MissingFromNew")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("Havm2TTComparisonRunId").AsInt32().NotNullable()
                .WithColumn("TramClass").AsAnsiString(50).Nullable()
                .WithColumn("HeadboardNo").AsAnsiString(50).Nullable()
                .WithColumn("RouteNo").AsAnsiString(50).Nullable()
                .WithColumn("RunNo").AsAnsiString(50).Nullable()
                .WithColumn("StartDate").AsAnsiString(50).Nullable()
                .WithColumn("TripNo").AsAnsiString(15).Nullable()
                .WithColumn("PublicTrip").AsAnsiString(50).Nullable();

            Create.ForeignKey("FK_Havm2TTComparison_T_Temp_SchedulesMaster_MissingFromNew_Havm2TTComparisonRun")
                .FromTable("Havm2TTComparison_T_Temp_SchedulesMaster_MissingFromNew").ForeignColumn("Havm2TTComparisonRunId")
                .ToTable("Havm2TTComparisonRun").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);

            // Havm2TTComparison_T_Temp_SchedulesMaster_ExtraInNew
            Create.Table("Havm2TTComparison_T_Temp_SchedulesMaster_ExtraInNew")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("Havm2TTComparisonRunId").AsInt32().NotNullable()
                .WithColumn("TramClass").AsAnsiString(50).Nullable()
                .WithColumn("HeadboardNo").AsAnsiString(50).Nullable()
                .WithColumn("RouteNo").AsAnsiString(50).Nullable()
                .WithColumn("RunNo").AsAnsiString(50).Nullable()
                .WithColumn("StartDate").AsAnsiString(50).Nullable()
                .WithColumn("TripNo").AsAnsiString(15).Nullable()
                .WithColumn("PublicTrip").AsAnsiString(50).Nullable();

            Create.ForeignKey("FK_Havm2TTComparison_T_Temp_SchedulesMaster_ExtraInNew_Havm2TTComparisonRun")
                .FromTable("Havm2TTComparison_T_Temp_SchedulesMaster_ExtraInNew").ForeignColumn("Havm2TTComparisonRunId")
                .ToTable("Havm2TTComparisonRun").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);

            // Havm2TTComparison_T_Temp_SchedulesMaster_Differing
            Create.Table("Havm2TTComparison_T_Temp_SchedulesMaster_Differing")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("Havm2TTComparisonRunId").AsInt32().NotNullable()
                .WithColumn("PairIdentifier").AsGuid().NotNullable()
                .WithColumn("IsExisting").AsBoolean().NotNullable()
                .WithColumn("TramClass").AsAnsiString(50).Nullable()
                .WithColumn("HeadboardNo").AsAnsiString(50).Nullable()
                .WithColumn("RouteNo").AsAnsiString(50).Nullable()
                .WithColumn("RunNo").AsAnsiString(50).Nullable()
                .WithColumn("StartDate").AsAnsiString(50).Nullable()
                .WithColumn("TripNo").AsAnsiString(15).Nullable()
                .WithColumn("PublicTrip").AsAnsiString(50).Nullable();

            Create.ForeignKey("FK_Havm2TTComparison_T_Temp_SchedulesMaster_Differing_Havm2TTComparisonRun")
                .FromTable("Havm2TTComparison_T_Temp_SchedulesMaster_Differing").ForeignColumn("Havm2TTComparisonRunId")
                .ToTable("Havm2TTComparisonRun").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);
            #endregion

            #region T_Temp_SchedulesDetails
            // Havm2TTComparison_T_Temp_SchedulesDetails_MissingFromNew
            Create.Table("Havm2TTComparison_T_Temp_SchedulesDetails_MissingFromNew")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("Havm2TTComparisonRunId").AsInt32().NotNullable()
                .WithColumn("ArrivalTime").AsAnsiString(50).Nullable()
                .WithColumn("StopID").AsAnsiString(50).Nullable()
                .WithColumn("TripID").AsAnsiString(50).Nullable()
                .WithColumn("RunNo").AsAnsiString(50).Nullable()
                .WithColumn("OPRTimePoint").AsAnsiString(50).Nullable();

            Create.ForeignKey("FK_Havm2TTComparison_T_Temp_SchedulesDetails_MissingFromNew_Havm2TTComparisonRun")
                .FromTable("Havm2TTComparison_T_Temp_SchedulesDetails_MissingFromNew").ForeignColumn("Havm2TTComparisonRunId")
                .ToTable("Havm2TTComparisonRun").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);

            // Havm2TTComparison_T_Temp_SchedulesDetails_ExtraInNew
            Create.Table("Havm2TTComparison_T_Temp_SchedulesDetails_ExtraInNew")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("Havm2TTComparisonRunId").AsInt32().NotNullable()
                .WithColumn("ArrivalTime").AsAnsiString(50).Nullable()
                .WithColumn("StopID").AsAnsiString(50).Nullable()
                .WithColumn("TripID").AsAnsiString(50).Nullable()
                .WithColumn("RunNo").AsAnsiString(50).Nullable()
                .WithColumn("OPRTimePoint").AsAnsiString(50).Nullable();

            Create.ForeignKey("FK_Havm2TTComparison_T_Temp_SchedulesDetails_ExtraInNew_Havm2TTComparisonRun")
                .FromTable("Havm2TTComparison_T_Temp_SchedulesDetails_ExtraInNew").ForeignColumn("Havm2TTComparisonRunId")
                .ToTable("Havm2TTComparisonRun").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);

            // Havm2TTComparison_T_Temp_SchedulesDetails_Differing
            Create.Table("Havm2TTComparison_T_Temp_SchedulesDetails_Differing")
                .WithColumn("Id").AsInt32().Identity().PrimaryKey().Indexed().NotNullable()
                .WithColumn("Havm2TTComparisonRunId").AsInt32().NotNullable()
                .WithColumn("PairIdentifier").AsGuid().NotNullable()
                .WithColumn("IsExisting").AsBoolean().NotNullable()
                .WithColumn("ArrivalTime").AsAnsiString(50).Nullable()
                .WithColumn("StopID").AsAnsiString(50).Nullable()
                .WithColumn("TripID").AsAnsiString(50).Nullable()
                .WithColumn("RunNo").AsAnsiString(50).Nullable()
                .WithColumn("OPRTimePoint").AsAnsiString(50).Nullable();

            Create.ForeignKey("FK_Havm2TTComparison_T_Temp_SchedulesDetails_Differing_Havm2TTComparisonRun")
                .FromTable("Havm2TTComparison_T_Temp_SchedulesDetails_Differing").ForeignColumn("Havm2TTComparisonRunId")
                .ToTable("Havm2TTComparisonRun").PrimaryColumn("Id")
                .OnDelete(System.Data.Rule.None);
            #endregion
        }

        public override void Down()
        {
            Delete.Table("Havm2TTComparison_T_Temp_SchedulesDetails_Differing");
            Delete.Table("Havm2TTComparison_T_Temp_SchedulesDetails_ExtraInNew");
            Delete.Table("Havm2TTComparison_T_Temp_SchedulesDetails_MissingFromNew");

            Delete.Table("Havm2TTComparison_T_Temp_SchedulesMaster_Differing");
            Delete.Table("Havm2TTComparison_T_Temp_SchedulesMaster_ExtraInNew");
            Delete.Table("Havm2TTComparison_T_Temp_SchedulesMaster_MissingFromNew");

            Delete.Table("Havm2TTComparison_T_Temp_Schedules_Differing");
            Delete.Table("Havm2TTComparison_T_Temp_Schedules_ExtraInNew");
            Delete.Table("Havm2TTComparison_T_Temp_Schedules_MissingFromNew");

            Delete.Table("Havm2TTComparison_T_Temp_Trips_Differing");
            Delete.Table("Havm2TTComparison_T_Temp_Trips_ExtraInNew");
            Delete.Table("Havm2TTComparison_T_Temp_Trips_MissingFromNew");

            Delete.Table("Havm2TTComparisonRunTable");
            Delete.Table("Havm2TTComparisonRun");
        }
    }
}
