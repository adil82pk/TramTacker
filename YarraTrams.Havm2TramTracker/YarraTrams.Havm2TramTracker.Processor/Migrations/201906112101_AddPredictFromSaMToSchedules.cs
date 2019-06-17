using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace YarraTrams.Havm2TramTracker.TestComparisons
{
    [Tags("TTBU")]
    [Migration(201906112101)]
    public class DB_201906112101_AddComparisonTables : Migration
    {
        public override void Up()
        {
            Alter.Table("T_Schedules").AddColumn("PredictFromSaM").AsInt32().NotNullable().SetExistingRowsTo(0);
            Alter.Table("T_Temp_Schedules").AddColumn("PredictFromSaM").AsInt32().NotNullable().SetExistingRowsTo(0);
        }

        public override void Down()
        {
            Delete.Column("PredictFromSaM").FromTable("T_Schedules");
            Delete.Column("PredictFromSaM").FromTable("T_Temp_Schedules");
        }
    }
}
