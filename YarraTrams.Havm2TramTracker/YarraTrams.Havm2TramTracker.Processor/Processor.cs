﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Models;
using YarraTrams.Havm2TramTracker.Processor.Services;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Data;
using YarraTrams.Havm2TramTracker.Logger;
using System.IO;
using YarraTrams.Havm2TramTracker.Processor.Helpers;

[assembly: InternalsVisibleTo("YarraTrams.Havm2TramTracker.Tests")]
namespace YarraTrams.Havm2TramTracker.Processor
{
    public class Processor
    {
        #region Public methods

        /// <summary>
        /// Main entry point for Havm2TramTracker processes. This routine orchestrates all others.
        /// </summary>
        public static void Process()
        {
            try
            {
                // CopyToLive
                DBHelper.CopyDataFromTempToLive();

                // Get schedule data from HAVM2
                string json = Helpers.ApiService.GetDataFromHavm2();


                // Create Havm model from JSON
                List<Models.HavmTrip> havmTrips = CopyJsonToTrips(json);

                // Populate 4 temp tables
                SaveToTrips(havmTrips);
                SaveToSchedules(havmTrips);
                //SaveToSchedulesMaster(havmTrips);
                //SaveToSchedulesDetails(havmTrips);
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.FATAL_ERROR, String.Format("An error has occured\n\nMessage: {0}\n\nStacktrace:{1}", Helpers.ExceptionHelper.GetExceptionMessagesRecursive(ex) , ex.StackTrace));
            }
        }

        /// <summary>
        /// Takes a JSON string and converts it to in-memory objects.
        /// </summary>
        /// <param name="jsonString">A JSON string that matches the format defined in ????.apibp.</param>
        /// <returns></returns>
        public static List<Models.HavmTrip> CopyJsonToTrips(string jsonString)
        {
            List<Models.HavmTrip> trips = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Models.HavmTrip>>(jsonString);
            //Todo: investigate using automapper instead of Newtsonsoft
            //Todo: do this manually so we can get more granular errors? e.g. The KTDS datamap.
            return trips;
        }

        /// <summary>
        /// Saves HAVM2 trip information to the T_Temp_Trips table in the TramTracker database
        /// </summary>
        public static void SaveToTrips(List<HavmTrip> havmTrips)
        {
            TramTrackerTripsService service = new TramTrackerTripsService();
            List<TramTrackerTrips> trips = service.FromHavmTrips(havmTrips, Properties.Settings.Default.LogT_Temp_TripRowsToFilePriorToInsert);
            DataTable dataTable = service.ToDataTable(trips);
            DBHelper.SaveTripDataToDatabase("T_Temp_Trips", dataTable);
        }

        /// <summary>
        /// ves HAVM2 trip information to the T_Temp_Schedules table in the TramTracker database
        /// </summary>
        public static void SaveToSchedules(List<HavmTrip> havmTrips)
        {
            TramTrackerSchedulesService service = new TramTrackerSchedulesService();
            List<TramTrackerSchedules> schedules = service.FromHavmTrips(havmTrips, Properties.Settings.Default.LogT_Temp_SchedulesRowsToFilePriorToInsert);
            DataTable dataTable = service.ToDataTable(schedules);
            DBHelper.SaveTripDataToDatabase("T_Temp_Schedules", dataTable);
        }

        /// <summary>
        /// Saves HAVM2 trip information to the T_Temp_SchedulesMaster table in the TramTracker database
        /// </summary>
        public static void SaveToSchedulesMaster(List<HavmTrip> havmTrips)
        {
            TramTrackerSchedulesMasterService service = new TramTrackerSchedulesMasterService();
            List<TramTrackerSchedulesMaster> schedulesMasters = service.FromHavmTrips(havmTrips, Properties.Settings.Default.LogT_Temp_SchedulesMasterRowsToFilePriorToInsert);
            DataTable dataTable = service.ToDataTable(schedulesMasters);
            DBHelper.SaveTripDataToDatabase("T_Temp_SchedulesMaster", dataTable);
        }

        /// <summary>
        /// ves HAVM2 trip information to the T_Temp_SchedulesDetails table in the TramTracker database
        /// </summary>
        public static void SaveToSchedulesDetails(List<HavmTrip> havmTrips)
        {
            TramTrackerSchedulesDetailsService service = new TramTrackerSchedulesDetailsService();
            List<TramTrackerSchedulesDetails> schedulesDetailss = service.FromHavmTrips(havmTrips, Properties.Settings.Default.LogT_Temp_SchedulesDetailsRowsToFilePriorToInsert);
            DataTable dataTable = service.ToDataTable(schedulesDetailss);
            DBHelper.SaveTripDataToDatabase("T_Temp_SchedulesDetails", dataTable);
        }

        #endregion
    }
}
