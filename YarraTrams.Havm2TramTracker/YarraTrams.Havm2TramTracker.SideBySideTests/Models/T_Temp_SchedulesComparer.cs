using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.SideBySideTests.Models
{
    internal class T_Temp_SchedulesComparer : TramTrackerComparer
    {
        public T_Temp_SchedulesComparer()
        {
            base.TableName = "T_Temp_Schedules";
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_Schedules tables, finding data in the first that's not in the second
        /// - Inserts results in to Havm2TTComparison_T_Temp_Schedules_MissingFromNew
        /// </summary>
        public override string GetMissingFromNewSql(int runId)
        {
            return "";
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_Schedules tables, finding data in the second that's not in the first
        /// - Inserts results in to Havm2TTComparison_T_Temp_SchedulesExtraInNew
        /// </summary>
        public override string GetExtraInNewSql(int runId)
        {
            return "";
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_Schedules tables, finding data between that two with matching keys but non matching detail
        /// - Inserts results in to Havm2TTComparison_T_Temp_SchedulesDiffering
        /// </summary>
        public override string GetDifferingSql(int runId)
        {
            return "";
        }
    }
}
