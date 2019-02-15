using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YarraTrams.Havm2TramTracker.Models;
using YarraTrams.Havm2TramTracker.Logger;
using YarraTrams.Havm2TramTracker.Processor;

namespace YarraTrams.Havm2TramTracker.Processor.Services
{
    public class TramTrackerSchedulesService
    {
        /// <summary>
        /// Convert collection of HavmTrips into a collection of TramTrackerSchedules.
        /// </summary>
        /// <param name="havmTrips"></param>
        /// <returns></returns>
        public List<TramTrackerSchedules> FromHavmTrips(List<HavmTrip> havmTrips, bool logRowsToFilePriorToInsert)
        {
            Dictionary<int,string> stopMapping = Helpers.HastusStopMapper.GetMapping();

            string serviceName = this.GetType().Name;

            List<TramTrackerSchedules> scheduless = new List<TramTrackerSchedules>();
            var exceptionCounts = new Dictionary<System.DayOfWeek, int>();

            foreach (var dayOfWeek in Enum.GetValues(typeof(System.DayOfWeek)).Cast<System.DayOfWeek>())
            {
                exceptionCounts.Add(dayOfWeek, 0);
            }

            int tripCounter = 0;
            int modelCounter = 0;

            StringBuilder errorMessages = new StringBuilder();

            using (StreamWriter fileWriter = new StreamWriter(Properties.Settings.Default.LogFilePath + @"\" + $"{DateTime.Now.ToString("yyyy-MM-dd")}_{serviceName}_PriorToInsert.txt", true))
            {
                foreach (HavmTrip havmTrip in havmTrips)
                {
                    tripCounter++;

                    if (logRowsToFilePriorToInsert)
                    {
                        fileWriter.Write($"\n\n{DateTime.Now}\nTrip {tripCounter}\n{havmTrip.ToString()}");
                    }

                    foreach (HavmTripStop havmStop in havmTrip.Stops)
                    {
                        try
                        {
                            modelCounter++;

                            TramTrackerSchedules schedules = new TramTrackerSchedules();
                            schedules.FromHavmTripAndStop(havmTrip,havmStop,stopMapping);
                            scheduless.Add(schedules);


                            if (logRowsToFilePriorToInsert)
                            {
                               fileWriter.Write($"\nRow {modelCounter}\n{schedules.ToString()}");
                            }
                        }
                        catch (Exception ex)
                        {
                            //This is catching an error with the trip-level data.
                            errorMessages.Append($"Exception: {ex.Message}\n{havmTrip.ToString()}\n");
                            exceptionCounts[havmTrip.OperationalDay.DayOfWeek]++;
                        }
                    }
                }
            }

            int totalErrors = exceptionCounts.Values.Sum();
            if (totalErrors == 0)
            {
                LogWriter.Instance.LogWithoutDelay(EventLogCodes.TRIP_TRANSFORMATION_SUCCESS
                    , $"{havmTrips.Count} HAVM trip{(havmTrips.Count == 1 ? "" : "s")} successfully transformed from HavmTtrips inside {serviceName}.");
            }
            else
            {
                var today = DateTime.Now;
                // Write exceptions to file
                string logFilePath = Properties.Settings.Default.LogFilePath;
                Helpers.LogfileWriter.writeToFile(serviceName, errorMessages.ToString(), logFilePath);

                // Log error message to event log
                var message = new StringBuilder();
                var todayDayOfWeek = today.DayOfWeek;

                // We print the number of exceptions separately for each of the next 7 days, starting with tomorrow.
                foreach (var dayOfWeek in exceptionCounts.OrderBy(pair => pair.Key <= todayDayOfWeek ? pair.Key + 7 : pair.Key))
                {
                    message.AppendLine($"{dayOfWeek.Key.ToString()} errors: {dayOfWeek.Value}");
                }

                message.AppendLine($"See the \"{serviceName}\" log file under {logFilePath} for more detail.");

                LogWriter.Instance.LogWithoutDelay(EventLogCodes.TRIP_TRANSFORMATION_ERROR
                    , $"Encountered {totalErrors} error{(totalErrors == 1 ? "" : "s")} when transforming {havmTrips.Count} HAVM trip{(havmTrips.Count == 1 ? "" : "s")} in the {serviceName}."
                  , message.ToString());
            }

            return scheduless;
        }

        /// <summary>
        /// Convert collection to a DataTable
        /// </summary>
        /// <param name="schedulesMasters"></param>
        /// <returns></returns>
        public TramTrackerDataSet.T_Temp_SchedulesDataTable ToDataTable(List<TramTrackerSchedules> schedulesDetailss)
        {

            TramTrackerDataSet.T_Temp_SchedulesDataTable masterTable = new TramTrackerDataSet.T_Temp_SchedulesDataTable();

            foreach(TramTrackerSchedules schedulesDetails in schedulesDetailss)
            {
                masterTable.Rows.Add(schedulesDetails.ToDataRow().ItemArray);
            }

            return masterTable;
        }
    }
}
