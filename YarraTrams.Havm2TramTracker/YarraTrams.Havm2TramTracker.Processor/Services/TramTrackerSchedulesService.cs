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
        public List<TramTrackerSchedules> FromHavmTrips(List<HavmTrip> havmTrips, Dictionary<int, string> stopMapping, bool logRowsToFilePriorToInsert)
        {
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

            using (StreamWriter fileWriter = new StreamWriter(Properties.Settings.Default.LogFilePath + @"\" + string.Format("{0}_{1}_PriorToInsert.txt", DateTime.Now.ToString("yyyy-MM-dd"), serviceName), true))
            {
                foreach (HavmTrip havmTrip in havmTrips)
                {
                    tripCounter++;

                    if (logRowsToFilePriorToInsert)
                    {
                        fileWriter.Write(string.Format("\n\n{0}\nTrip {1}\n{0}", DateTime.Now, tripCounter, havmTrip.ToString())); fileWriter.Write(string.Format("\n\n{0}\nTrip {1}\n{0}", DateTime.Now, tripCounter, havmTrip.ToString()));
                    }

                    if (havmTrip.IsPublic)
                    {
                        foreach (HavmTripStop havmStop in havmTrip.Stops)
                        {
                            try
                            {
                                int stopID;
                                if (int.TryParse(havmStop.HastusStopId, out stopID))
                                {
                                    modelCounter++;

                                    TramTrackerSchedules schedules = new TramTrackerSchedules();
                                    schedules.FromHavmTripAndStop(havmTrip, havmStop, stopMapping);
                                    scheduless.Add(schedules);

                                    if (logRowsToFilePriorToInsert)
                                    {
                                        fileWriter.Write(string.Format("\nRow {0}\n{1}", modelCounter, schedules.ToString()));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //This is catching an error with the stop-level data.
                                errorMessages.Append(string.Format("Exception: {0}\n{1}\n", ex.Message, havmTrip.ToString()));
                                exceptionCounts[havmTrip.OperationalDay.DayOfWeek]++;
                            }
                        }
                    }
                }
            }

            // Set the "CatchTime" on each trip stop - this field tells the prediction calculations when we should start "catching" this trip stop and make predictions for it.
            scheduless = this.SetCatchTimes(scheduless, Properties.Settings.Default.NumberOfPredictionsPerTripStop);

            int totalErrors = exceptionCounts.Values.Sum();
            if (totalErrors == 0)
            {
                LogWriter.Instance.LogWithoutDelay(EventLogCodes.SCHEDULE_TRANSFORMATION_SUCCESS
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

                LogWriter.Instance.LogWithoutDelay(EventLogCodes.SCHEDULE_TRANSFORMATION_ERROR
                    , string.Format("Encountered {0} error{1} when transforming {2} HAVM trip{3} in the {4}.", totalErrors, (totalErrors == 1 ? "" : "s"), havmTrips.Count, (havmTrips.Count == 1 ? "" : "s"), serviceName)
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

        /// <summary>
        /// Adds a CatchTime to every schedule record.
        /// The CatchTime field tells the prediction calculations when we should start "catching" this trip stop and make predictions for it.
        /// </summary>
        /// <param name="scheduless">A list of schedules without their CatchTime set</param>
        /// <param name="numberOfPredictionsPerTripStop">Number of predictions we want to make for each Stop/Route/Direction combination</param>
        /// <returns></returns>
        public List<TramTrackerSchedules> SetCatchTimes(List<TramTrackerSchedules> scheduless, int numberOfPredictionsPerTripStop)
        {
            byte currentDayOfWeek = 255; // DayOfWeek is stored in the TramTracker DB as a TinyInt (a byte - 3 bits would, in fact, be enough to store 0,1,2,3,4,5 or 6!). We set the working variable to 255 so as the first trip is recognised as a "new" trip in the loop.
            short currentRouteNo = 0;
            string currentStopId = "0";
            bool currentUpDirection = true;
            Queue<int> passingTimes = new Queue<int>();

            foreach (TramTrackerSchedules tripStop in scheduless.OrderBy(s => s.DayOfWeek).ThenBy(s => s.RouteNo).ThenBy(s => s.StopID).ThenBy(s => s.UpDirection).ThenBy(s => s.Time))
            {
                // Upon reaching a new unique combination of DayOfWeek+RouteNo+StopID+Direction we set the next X CatchTimes to 0.
                if (currentUpDirection != tripStop.UpDirection || currentStopId != tripStop.StopID || currentRouteNo != tripStop.RouteNo || currentDayOfWeek != tripStop.DayOfWeek)
                {
                    currentDayOfWeek = tripStop.DayOfWeek;
                    currentRouteNo = tripStop.RouteNo;
                    currentStopId = tripStop.StopID;
                    currentUpDirection = tripStop.UpDirection;

                    passingTimes.Clear();
                    for (int ii = 0; ii < numberOfPredictionsPerTripStop; ii++)
                    {
                        passingTimes.Enqueue(0);
                    }
                }

                // Set the CatchTime on this trip stop to the earliest passing time in the queue, then put the current passing time on to the queue.
                tripStop.CatchTime = passingTimes.Dequeue();
                passingTimes.Enqueue(tripStop.Time);
            }
            return scheduless;
        }
    }
}
