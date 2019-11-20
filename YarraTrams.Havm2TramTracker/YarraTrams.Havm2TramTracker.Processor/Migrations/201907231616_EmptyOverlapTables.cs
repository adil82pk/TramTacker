using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace YarraTrams.Havm2TramTracker.TestComparisons
{
    [Tags("TTBU")]
    [Migration(201907231616)]
    public class DB_201907231616_EmptyOverlapTables : Migration
    {
        public override void Up()
        {
            Execute.Sql("DELETE T_Temp_Overlap_Trips");
            Execute.Sql("DELETE T_Temp_Overlap_Schedules");
        }

        public override void Down()
        {
            
        }
    }
}
