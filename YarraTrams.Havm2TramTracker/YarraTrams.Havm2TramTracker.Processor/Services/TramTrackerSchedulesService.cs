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

            // Set the "PredictFromSaM" time on each trip stop - this field tells the prediction calculations when we should start making predictions for each trip stop.
            scheduless = this.SetPredictFromSaMTimeForEachTripStop(scheduless, Properties.Settings.Default.NumberOfPredictionsPerTripStop);

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

            foreach (TramTrackerSchedules schedulesDetails in schedulesDetailss)
            {
                masterTable.Rows.Add(schedulesDetails.ToDataRow().ItemArray);
            }

            return masterTable;
        }

        /// <summary>
        /// Adds a PredictFromSaM value to every schedule record.
        /// The PredictFromSaM field tells the prediction calculations when we should start making predictions for each trip stop.
        /// The PredictFromSam field could be negative. This is necessary for routes that only run on certain days. e.g. 
        /// 3a only runs on weekends, thus the first 3 Saturday trips become a prediction on the previous Sunday!
        /// </summary>
        /// <param name="scheduless">A list of schedules without their PredictFromSaM set</param>
        /// <param name="numberOfPredictionsPerTripStop">Number of predictions we want to make for each Stop/Route/Direction combination</param>
        /// <returns></returns>
        public List<TramTrackerSchedules> SetPredictFromSaMTimeForEachTripStop(List<TramTrackerSchedules> scheduless, int numberOfPredictionsPerTripStop)
        {
            if (scheduless.Count > 0)
            {
                short currentRouteNo = 0;
                string currentStopId = "0";
                bool currentUpDirection = true;
                DateTime currentOperationalDay = DateTime.MinValue;
                // Set the base time to 00:00 on the day of the first trip.
                DateTime baseDateTime = scheduless.OrderBy(s => s.PassingDateTime).First().PassingDateTime;
                baseDateTime = baseDateTime.AddTicks(-baseDateTime.Ticks % TimeSpan.TicksPerDay);

                Queue<DateTime> passingTimes = new Queue<DateTime>();

                foreach (TramTrackerSchedules tripStop in scheduless.OrderBy(s => s.RouteNo).ThenBy(s => s.StopID).ThenBy(s => s.UpDirection).ThenBy(s => s.PassingDateTime))
                {
                    // Upon reaching a new unique combination of Direction+StopID+RouteNo we set the next X PredictFromSaMs to 0.
                    if (currentUpDirection != tripStop.UpDirection || currentStopId != tripStop.StopID || currentRouteNo != tripStop.RouteNo)
                    {
                        currentRouteNo = tripStop.RouteNo;
                        currentStopId = tripStop.StopID;
                        currentUpDirection = tripStop.UpDirection;
                        currentOperationalDay = tripStop.OperationalDay;

                        passingTimes.Clear();
                        for (int ii = 0; ii < numberOfPredictionsPerTripStop; ii++)
                        {
                            passingTimes.Enqueue(baseDateTime);
                        }
                    }

                    // Upon reaching a new day we set the next x PredictFromSaMs to midnight of the day prior. This means, for instance, that the first three for Tues are predicted from start of Mon, but these predictions probably won't make the "top" 3 until late Mon.
                    if (currentOperationalDay != tripStop.OperationalDay)
                    {
                        passingTimes.Clear();
                        for (int ii = 0; ii < numberOfPredictionsPerTripStop; ii++)
                        {
                            passingTimes.Enqueue(currentOperationalDay);
                        }

                        currentOperationalDay = tripStop.OperationalDay;
                    }

                    // Set the PredictFromSaM field on this trip stop to the earliest passing time in the queue,
                    // then put the current passing time on to the queue.
                    tripStop.PredictFromDateTime = passingTimes.Dequeue();

                    // A single tick represents one hundred nanoseconds or one ten-millionth of a second. There are 10 million ticks in a second.
                    // tripStop.PassingDateTime.Ticks % TimeSpan.TicksPerDay remainder gives number of ticks passed in that day. For instance if tripStop.PassingDateTime is 9/11/1989 11:57 then the ticks are 627622558200000000
                    // and TimeSpan.TicksPerDay are 864000000000 so percentage of  627622558200000000 % 864000000000 = 862200000000. So if we subtract ticks will give us start of the day i.e 9/11/1989 12:00 am.
                    tripStop.PredictFromSaM = (int)(tripStop.PredictFromDateTime - tripStop.PassingDateTime.AddTicks(-tripStop.PassingDateTime.Ticks % TimeSpan.TicksPerDay)).TotalSeconds;
                    passingTimes.Enqueue(tripStop.PassingDateTime);
                }
            }
            return scheduless;
        }
    }
}
