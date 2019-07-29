using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace YarraTrams.Havm2TramTracker.TestComparisons
{
    [Tags("TTBU")]
    [Migration(201907241041)]
    public class DB_201907241041_Add2FieldsToTrips : Migration
    {
        public override void Up()
        {
            Alter.Table("T_Trips").AddColumn("HavmPartnerTimetableId").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Trips").AddColumn("RunHasDoubleUps").AsBoolean().NotNullable().SetExistingRowsTo(false);
            
            Alter.Table("T_Temp_Trips").AddColumn("HavmPartnerTimetableId").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Temp_Trips").AddColumn("RunHasDoubleUps").AsBoolean().NotNullable().SetExistingRowsTo(false);
            
            Alter.Table("T_Temp_Overlap_Trips").AddColumn("HavmPartnerTimetableId").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Temp_Overlap_Trips").AddColumn("RunHasDoubleUps").AsBoolean().NotNullable().SetExistingRowsTo(false);
        }

        public override void Down()
        {
            Delete.Column("HavmPartnerTimetableId").FromTable("T_Trips");
            Delete.Column("RunHasDoubleUps").FromTable("T_Trips");

            Delete.Column("HavmPartnerTimetableId").FromTable("T_Temp_Trips");
            Delete.Column("RunHasDoubleUps").FromTable("T_Temp_Trips");

            Delete.Column("HavmPartnerTimetableId").FromTable("T_Temp_Overlap_Trips");
            Delete.Column("RunHasDoubleUps").FromTable("T_Temp_Overlap_Trips");
        }
    }
}
