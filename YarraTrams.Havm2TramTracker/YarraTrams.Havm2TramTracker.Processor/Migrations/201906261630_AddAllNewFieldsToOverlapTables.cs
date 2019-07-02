using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace YarraTrams.Havm2TramTracker.TestComparisons
{
    [Tags("TTBU")]
    [Migration(201906261630)]
    public class DB_201906261630_AddAllNewFieldsToOverlapTables : Migration
    {
        public override void Up()
        {
            Alter.Table("T_Temp_Overlap_Trips").AddColumn("HavmTripId").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Temp_Overlap_Trips").AddColumn("HavmTimetableId").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Temp_Overlap_Trips").AddColumn("HastusPermanentTripNumber").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Temp_Overlap_Trips").AddColumn("RunSequenceNumber").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Temp_Overlap_Trips").AddColumn("AtLayoverTimePrevious").AsInt32().NotNullable().SetExistingRowsTo(0);

            Alter.Table("T_Temp_Overlap_Schedules").AddColumn("PredictFromSaM").AsInt32().NotNullable().SetExistingRowsTo(0);
        }

        public override void Down()
        {

            Delete.Column("HavmTripId").FromTable("T_Temp_Overlap_Trips");
            Delete.Column("HavmTimetableId").FromTable("T_Temp_Overlap_Trips");
            Delete.Column("HastusPermanentTripNumber").FromTable("T_Temp_Overlap_Trips");
            Delete.Column("RunSequenceNumber").FromTable("T_Temp_Overlap_Trips");
            Delete.Column("AtLayoverTimePrevious").FromTable("T_Temp_Overlap_Trips");

            Delete.Column("PredictFromSaM").FromTable("T_Temp_Overlap_Schedules");
        }
    }
}
