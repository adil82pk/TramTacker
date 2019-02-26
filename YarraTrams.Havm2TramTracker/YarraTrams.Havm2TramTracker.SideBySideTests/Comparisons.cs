using SpreadsheetLight;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using YarraTrams.Havm2TramTracker.Logger;
using YarraTrams.Havm2TramTracker.SideBySideTests.Helpers;

namespace YarraTrams.Havm2TramTracker.SideBySideTests
{
    public class Comparisons
    {
        /// <summary>
        /// Entry method for running side-by-side comparisons.
        /// Triggers a comparison of all relevant tables.
        /// </summary>
        public void RunComparisons()
        {
            try
            {
                int runId;

                //Create Comparison Run parent record
                runId = DBHelper.ExecuteSQLReturnInt(string.Format("INSERT Havm2TTComparisonRun VALUES ('{0}'); SELECT scope_identity()", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF")));

                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, string.Format("Running side-by-side comparisons. Run Id is {0}.", runId));

                // T_Temp_Trips
                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, "Comparing T_Temp_Trips.");
                Models.T_Temp_TripsComparer tripsComparer = new Models.T_Temp_TripsComparer();
                tripsComparer.RunComparison(runId);

                // T_Temp_Schedules
                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, "Comparing T_Temp_Schedules.");
                Models.T_Temp_SchedulesComparer schedulesComparer = new Models.T_Temp_SchedulesComparer();
                schedulesComparer.RunComparison(runId);

                System.Console.WriteLine("Comparisons complete.");
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_ERROR, string.Format("Error when running side-by-side test:\n{0}\n\n{1}", ex.Message, ex.StackTrace));
            }
        }
    }
}
