using System;
using YarraTrams.Havm2TramTracker.Logger;
using YarraTrams.Havm2TramTracker.TestComparisons.Helpers;

namespace YarraTrams.Havm2TramTracker.TestComparisons
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

                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, string.Format("Running side-by-side comparisons. Run Id is {0}. Suffix for new tables is '{1}'.", runId, Processor.Helpers.SettingsExposer.DbTableSuffix()));

                // T_Temp_Trips
                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, "Comparing T_Temp_Trips.");
                Models.T_Temp_TripsComparer tripsComparer = new Models.T_Temp_TripsComparer();
                tripsComparer.RunComparison(runId);

                // T_Temp_Schedules
                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, "Comparing T_Temp_Schedules.");
                Models.T_Temp_SchedulesComparer schedulesComparer = new Models.T_Temp_SchedulesComparer();
                schedulesComparer.RunComparison(runId);

                // T_Temp_SchedulesMaster
                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, "Comparing T_Temp_SchedulesMaster.");
                Models.T_Temp_SchedulesMasterComparer schedulesMasterComparer = new Models.T_Temp_SchedulesMasterComparer();
                schedulesMasterComparer.RunComparison(runId);

                // T_Temp_SchedulesDetails
                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, "Comparing T_Temp_SchedulesDetails.");
                Models.T_Temp_SchedulesDetailsComparer schedulesDetailsComparer = new Models.T_Temp_SchedulesDetailsComparer();
                schedulesDetailsComparer.RunComparison(runId);

                System.Console.WriteLine("Comparisons complete.");
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_ERROR, string.Format("Error when running side-by-side test:\n{0}\n\n{1}", ex.Message, ex.StackTrace));
            }
        }
    }
}
