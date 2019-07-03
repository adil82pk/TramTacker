using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace YarraTrams.Havm2TramTracker.TestComparisons
{
    [Tags("TTBU")]
    [Migration(201907022047)]
    public class DB_201907022047_AddOperationalDayToTrips : Migration
    {
        public override void Up()
        {
            Alter.Table("T_Trips").AddColumn("OperationalDay").AsDateTime().NotNullable().SetExistingRowsTo(new DateTime(1983,9,26));

            Alter.Table("T_Temp_Trips").AddColumn("OperationalDay").AsDateTime().NotNullable().SetExistingRowsTo(new DateTime(1983, 9, 26));

            Alter.Table("T_Temp_Overlap_Trips").AddColumn("OperationalDay").AsDateTime().NotNullable().SetExistingRowsTo(new DateTime(1983, 9, 26));
        }

        public override void Down()
        {
            Delete.Column("OperationalDay").FromTable("T_Trips");

            Delete.Column("OperationalDay").FromTable("T_Temp_Trips");

            Delete.Column("OperationalDay").FromTable("T_Temp_Overlap_Trips");
        }
    }
}
