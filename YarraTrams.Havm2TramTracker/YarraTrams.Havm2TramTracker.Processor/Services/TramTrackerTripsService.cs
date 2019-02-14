using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YarraTrams.Havm2TramTracker.Models;
using YarraTrams.Havm2TramTracker.Logger;

namespace YarraTrams.Havm2TramTracker.Processor.Services
{
    public class TramTrackerTripsService
    {
        /// <summary>
        /// Convert collection of HavmTrips into a collection of TramTrackerTrips.
        /// </summary>
        /// <param name="havmTrips"></param>
        /// <returns></returns>
        public List<TramTrackerTrip> FromHavmTrips(List<HavmTrip> havmTrips, bool logRowsToFilePriorToInsert)
        {
            string serviceName = this.GetType().Name;

            List<TramTrackerTrip> trips = new List<TramTrackerTrip>();
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

                    try
                    {
                        modelCounter++;

                        if (logRowsToFilePriorToInsert)
                        {
                            fileWriter.Write($"\n\n{DateTime.Now}\nTrip {tripCounter}\n{havmTrip.ToString()}");
                        }

                        TramTrackerTrip trip = new TramTrackerTrip();
                        trip.FromHavmTrip(havmTrip);
                        trips.Add(trip);


                        if (logRowsToFilePriorToInsert)
                        {
                           fileWriter.Write($"\nRow {modelCounter}\n{trip.ToString()}");
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

            return trips;
        }

        /// <summary>
        /// Convert collection to a DataTable
        /// </summary>
        /// <param name="trips"></param>
        /// <returns></returns>
        public TramTrackerDataSet.T_Temp_TripsDataTable ToDataTable(List<TramTrackerTrip> trips)
        {

            TramTrackerDataSet.T_Temp_TripsDataTable dataTable = new TramTrackerDataSet.T_Temp_TripsDataTable();

            foreach(TramTrackerTrip trip in trips)
            {
                dataTable.Rows.Add(trip.ToDataRow().ItemArray);
            }

            return dataTable;
        }
    }
}
