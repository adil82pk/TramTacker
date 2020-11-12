using System;
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
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("YarraTrams.Havm2TramTracker.Tests")]
namespace YarraTrams.Havm2TramTracker.Processor
{
    public class Processor
    {
        #region Public methods

        /// <summary>
        /// Copies data from the T_Temp... tables to the equivalent T_... tables.
        /// One of two main entry points for Havm2TramTracker processes (the other is RefreshTemp).
        /// </summary>
        public static void CopyToLive()
        {
            try
            {
                DBHelper.CopyDataFromTempToLive();
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.FATAL_ERROR, String.Format("An error has occured\n\nMessage: {0}\n\nStacktrace:{1}", Helpers.ExceptionHelper.GetExceptionMessagesRecursive(ex), ex.StackTrace));
            }
        }

        /// <summary>
        /// Truncates the T_Temp... tables and repopulates them with the latest HAVM2 data.
        /// One of two main entry points for Havm2TramTracker processes (the other is CopyToLive).
        /// </summary>
        public static void RefreshTemp()
        {
            try
            {
                // Get schedule data from HAVM2
                string json = Helpers.ApiService.GetDataFromHavm2(null);


                // Create Havm model from JSON
                List<Models.HavmTrip> havmTrips = CopyJsonToTrips(json);

                // Populate 4 temp tables
                SaveToTrips(havmTrips);
                SaveToSchedules(havmTrips);
                SaveToSchedulesMaster(havmTrips);
                SaveToSchedulesDetails(havmTrips);
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.FATAL_ERROR, String.Format("An error has occured\n\nMessage: {0}\n\nStacktrace:{1}", Helpers.ExceptionHelper.GetExceptionMessagesRecursive(ex), ex.StackTrace));
            }
        }

        /// <summary>
        /// Takes a JSON string and converts it to in-memory HavmTrip/Stop objects.
        /// The string can contain more fields than expected but must not be missing
        /// any of the fields marked as Required on the HavmTrip and HavmStop models.
        /// </summary>
        /// <param name="jsonString">A JSON string that matches the format defined in ????.apibp.</param>
        public static List<Models.HavmTrip> CopyJsonToTrips(string jsonString)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Ignore; //Ignore any extra fields we find in the json

            List<Models.HavmTrip> trips = JsonConvert.DeserializeObject<List<Models.HavmTrip>>(jsonString, settings);
            
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
            DBHelper.TruncateThenSaveTripDataToDatabase(DBHelper.GetDbTableName(Enums.TableNames.TempTrips), dataTable);
        }

        /// <summary>
        /// ves HAVM2 trip information to the T_Temp_Schedules table in the TramTracker database
        /// </summary>
        public static void SaveToSchedules(List<HavmTrip> havmTrips)
        {
            TramTrackerSchedulesService service = new TramTrackerSchedulesService();
            List<TramTrackerSchedules> schedules = service.FromHavmTrips(havmTrips, Helpers.HastusStopMapper.GetMapping(), Properties.Settings.Default.LogT_Temp_SchedulesRowsToFilePriorToInsert);
            DataTable dataTable = service.ToDataTable(schedules);
            DBHelper.TruncateThenSaveTripDataToDatabase(DBHelper.GetDbTableName(Enums.TableNames.TempSchedules), dataTable);
        }

        /// <summary>
        /// Saves HAVM2 trip information to the T_Temp_SchedulesMaster table in the TramTracker database
        /// </summary>
        public static void SaveToSchedulesMaster(List<HavmTrip> havmTrips)
        {
            TramTrackerSchedulesMasterService service = new TramTrackerSchedulesMasterService();
            List<TramTrackerSchedulesMaster> schedulesMasters = service.FromHavmTrips(havmTrips, Properties.Settings.Default.LogT_Temp_SchedulesMasterRowsToFilePriorToInsert);
            DataTable dataTable = service.ToDataTable(schedulesMasters);
            DBHelper.TruncateThenSaveTripDataToDatabase(DBHelper.GetDbTableName(Enums.TableNames.TempSchedulesMaster), dataTable);
        }

        /// <summary>
        /// ves HAVM2 trip information to the T_Temp_SchedulesDetails table in the TramTracker database
        /// </summary>
        public static void SaveToSchedulesDetails(List<HavmTrip> havmTrips)
        {
            TramTrackerSchedulesDetailsService service = new TramTrackerSchedulesDetailsService();
            List<TramTrackerSchedulesDetails> schedulesDetailss = service.FromHavmTrips(havmTrips, Properties.Settings.Default.LogT_Temp_SchedulesDetailsRowsToFilePriorToInsert);
            DataTable dataTable = service.ToDataTable(schedulesDetailss);
            DBHelper.TruncateThenSaveTripDataToDatabase(DBHelper.GetDbTableName(Enums.TableNames.TempSchedulesDetails), dataTable);
        }

        /// <summary>
        /// Checks that the revision of the timetable loaded into AVM for tomorrow matches the last one exported from HAVM2.
        /// </summary>
        public static void CheckAvmTimetableRevision()
        {
            try
            {
                AvmTimetableService service = new AvmTimetableService();
                int avmTimetableTimestamp = service.GetTomorrowsAvmTimetableRevision();
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.FATAL_ERROR, String.Format("An error has occured when checking the AVM timetable revision.\n\nMessage: {0}\n\nStacktrace:{1}", Helpers.ExceptionHelper.GetExceptionMessagesRecursive(ex), ex.StackTrace));
            }
        }

        #endregion
    }
}
