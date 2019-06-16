using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace YarraTrams.Havm2TramTracker.TestComparisons
{
    [Tags("TTBU")]
    [Migration(201906162255)]
    public class DB_201906162255_AddTripFieldsForNewPredictions : Migration
    {
        public override void Up()
        {
            Alter.Table("T_Trips").AddColumn("HavmTripId").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Trips").AddColumn("HavmTimetableId").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Trips").AddColumn("HastusPermanentTripNumber").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Trips").AddColumn("RunSequenceNumber").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Trips").AddColumn("HeadwayPreviousSeconds").AsInt32().NotNullable().SetExistingRowsTo(0);

            Alter.Table("T_Temp_Trips").AddColumn("HavmTripId").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Temp_Trips").AddColumn("HavmTimetableId").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Temp_Trips").AddColumn("HastusPermanentTripNumber").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Temp_Trips").AddColumn("RunSequenceNumber").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Temp_Trips").AddColumn("HeadwayPreviousSeconds").AsInt32().NotNullable().SetExistingRowsTo(0);
        }

        public override void Down()
        {
            Delete.Column("HavmTripId").FromTable("T_Trips");
            Delete.Column("HavmTimetableId").FromTable("T_Trips");
            Delete.Column("HastusPermanentTripNumber").FromTable("T_Trips");
            Delete.Column("RunSequenceNumber").FromTable("T_Trips");
            Delete.Column("HeadwayPreviousSeconds").FromTable("T_Trips");

            Delete.Column("HavmTripId").FromTable("T_Temp_Trips");
            Delete.Column("HavmTimetableId").FromTable("T_Temp_Trips");
            Delete.Column("HastusPermanentTripNumber").FromTable("T_Temp_Trips");
            Delete.Column("RunSequenceNumber").FromTable("T_Temp_Trips");
            Delete.Column("HeadwayPreviousSeconds").FromTable("T_Temp_Trips");
        }
    }
}
