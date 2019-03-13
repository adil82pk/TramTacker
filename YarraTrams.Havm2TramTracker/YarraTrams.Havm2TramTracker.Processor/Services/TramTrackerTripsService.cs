﻿using System;
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
        public List<TramTrackerTrips> FromHavmTrips(List<HavmTrip> havmTrips, bool logRowsToFilePriorToInsert)
        {
            string serviceName = this.GetType().Name;

            List<TramTrackerTrips> trips = new List<TramTrackerTrips>();
            var exceptionCounts = new Dictionary<System.DayOfWeek, int>();

            foreach (var dayOfWeek in Enum.GetValues(typeof(System.DayOfWeek)).Cast<System.DayOfWeek>())
            {
                exceptionCounts.Add(dayOfWeek, 0);
            }

            int tripCounter = 0;
            int modelCounter = 0;

            StringBuilder errorMessages = new StringBuilder();

            using (StreamWriter fileWriter = new StreamWriter(Properties.Settings.Default.LogFilePath + @"\" + string.Format("{0}_{1}_PriorToInsert.txt", DateTime.Now.ToString("yyyy-MM-dd"), serviceName), true))
            {
                foreach (HavmTrip havmTrip in havmTrips)
                {
                    tripCounter++;

                    try
                    {
                        modelCounter++;

                        if (logRowsToFilePriorToInsert)
                        {
                            fileWriter.Write(string.Format("\n\n{0}\nTrip {1}\n{0}", DateTime.Now, tripCounter, havmTrip.ToString()));
                        }

                        TramTrackerTrips trip = new TramTrackerTrips();
                        trip.FromHavmTrip(havmTrip);
                        trips.Add(trip);


                        if (logRowsToFilePriorToInsert)
                        {
                            fileWriter.Write(string.Format("\nRow {0}\n{1}", modelCounter, trip.ToString()));
                        }
                    }
                    catch (Exception ex)
                    {
                        //This is catching an error with the trip-level data.
                        errorMessages.Append(string.Format("Exception: {0}\n{1}\n", ex.Message, havmTrip.ToString()));
                        exceptionCounts[havmTrip.OperationalDay.DayOfWeek]++;
                    }
                }
            }

            int totalErrors = exceptionCounts.Values.Sum();
            if (totalErrors == 0)
            {
                LogWriter.Instance.LogWithoutDelay(EventLogCodes.TRIP_TRANSFORMATION_SUCCESS
                    , String.Format("{0} HAVM trip{1} successfully transformed inside {2}.", havmTrips.Count, (havmTrips.Count == 1 ? "" : "s"), serviceName));
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
                    message.AppendLine(String.Format("{0} errors: {1}", dayOfWeek.Key.ToString(), dayOfWeek.Value));
                }

                message.AppendLine(string.Format("See the \"{0}\" log file under {1} for more detail.", serviceName, logFilePath));

                LogWriter.Instance.LogWithoutDelay(EventLogCodes.TRIP_TRANSFORMATION_ERROR
                    , string.Format("Encountered {0} error{1} when transforming {2} HAVM trip{3} in the {4}.", totalErrors, (totalErrors == 1 ? "" : "s"), havmTrips.Count, (havmTrips.Count == 1 ? "" : "s"), serviceName)
                  , message.ToString());
            }

            return trips;
        }

        /// <summary>
        /// Convert collection to a DataTable
        /// </summary>
        /// <param name="trips"></param>
        /// <returns></returns>
        public TramTrackerDataSet.T_Temp_TripsDataTable ToDataTable(List<TramTrackerTrips> trips)
        {

            TramTrackerDataSet.T_Temp_TripsDataTable dataTable = new TramTrackerDataSet.T_Temp_TripsDataTable();

            foreach(TramTrackerTrips trip in trips)
            {
                dataTable.Rows.Add(trip.ToDataRow().ItemArray);
            }

            return dataTable;
        }
    }
}
