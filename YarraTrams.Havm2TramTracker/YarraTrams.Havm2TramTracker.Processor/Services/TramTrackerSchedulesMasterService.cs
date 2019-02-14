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
    public class TramTrackerSchedulesMasterService
    {
        /// <summary>
        /// Convert collection of HavmTrips into a collection of TramTrackerSchedulesMasterService.
        /// </summary>
        /// <param name="havmTrips"></param>
        /// <returns></returns>
        public List<TramTrackerSchedulesMaster> FromHavmTrips(List<HavmTrip> havmTrips)
        {
            List<TramTrackerSchedulesMaster> schedulesMasters = new List<TramTrackerSchedulesMaster>();
            var exceptionCounts = new Dictionary<System.DayOfWeek, int>();

            foreach (var dayOfWeek in Enum.GetValues(typeof(System.DayOfWeek)).Cast<System.DayOfWeek>())
            {
                exceptionCounts.Add(dayOfWeek, 0);
            }

            bool logRowsToFilePriorToInsert = Properties.Settings.Default.LogT_Temp_SchedulesMasterDetailsRowsToFilePriorToInsert;
            int tripCounter = 0;
            int masterRowCounter = 0;

            StringBuilder errorMessages = new StringBuilder();

            using (StreamWriter fileWriter = new StreamWriter(Properties.Settings.Default.LogFilePath + @"\" + $"{DateTime.Now.ToString("yyyy-MM-dd")}-T_Temp_SchedulesMasterDetailsRowsPriorToInsert.txt", true))
            {
                foreach (HavmTrip havmTrip in havmTrips)
                {
                    tripCounter++;

                    try
                    {
                        masterRowCounter++;

                        if (logRowsToFilePriorToInsert)
                        {
                            fileWriter.Write($"\n\n{DateTime.Now}\nTrip {tripCounter}\n{havmTrip.ToString()}");
                        }

                        TramTrackerSchedulesMaster schedulesMaster = new TramTrackerSchedulesMaster();
                        schedulesMaster.FromHavmTrip(havmTrip);
                        schedulesMasters.Add(schedulesMaster);


                        if (logRowsToFilePriorToInsert)
                        {
                           fileWriter.Write($"\nRow {masterRowCounter}\n{schedulesMaster.ToString()}");
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
                    , $"{havmTrips.Count} HAVM trip{(havmTrips.Count == 1 ? "" : "s")} successfully transformed from HavmTtrips to TramTrackerMasterSchedules format.");
            }
            else
            {
                var today = DateTime.Now;
                // Write exceptions to file
                string logFilePath = Properties.Settings.Default.LogFilePath;
                string filePostFix = "TramTrackerMasterSchedules";
                Helpers.LogfileWriter.writeToFile(filePostFix, errorMessages.ToString(), logFilePath);

                // Log error message to event log
                var message = new StringBuilder();
                var todayDayOfWeek = today.DayOfWeek;

                // We print the number of exceptions separately for each of the next 7 days, starting with tomorrow.
                foreach (var dayOfWeek in exceptionCounts.OrderBy(pair => pair.Key <= todayDayOfWeek ? pair.Key + 7 : pair.Key))
                {
                    message.AppendLine($"{dayOfWeek.Key.ToString()} errors: {dayOfWeek.Value}");
                }

                message.AppendLine($"See the \"{filePostFix}\" log file under {logFilePath} for more detail.");

                LogWriter.Instance.LogWithoutDelay(EventLogCodes.TRIP_TRANSFORMATION_ERROR
                    , $"Encountered {totalErrors} error{(totalErrors == 1 ? "" : "s")} when transforming {havmTrips.Count} HAVM trip{(havmTrips.Count == 1 ? "" : "s")} in to the TramTRACKER T_Temp_SchedulesMaster/T_Temp_SchedulesDetails format."
                  , message.ToString());
            }

            return schedulesMasters;
        }

        /// <summary>
        /// Convert collection to a DataTable
        /// </summary>
        /// <param name="schedulesMasters"></param>
        /// <returns></returns>
        public TramTrackerDataSet.T_Temp_SchedulesMasterDataTable ToDataTable(List<TramTrackerSchedulesMaster> schedulesMasters)
        {

            TramTrackerDataSet.T_Temp_SchedulesMasterDataTable masterTable = new TramTrackerDataSet.T_Temp_SchedulesMasterDataTable();

            foreach(TramTrackerSchedulesMaster schedulesMaster in schedulesMasters)
            {
                masterTable.AddT_Temp_SchedulesMasterRow(schedulesMaster.ToDataRow());
            }

            return new TramTrackerDataSet.T_Temp_SchedulesMasterDataTable();
        }
    }
}
