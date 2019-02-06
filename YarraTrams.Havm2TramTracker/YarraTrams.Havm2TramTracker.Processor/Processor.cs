using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Models;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Data;
using YarraTrams.Havm2TramTracker.Logger;

[assembly: InternalsVisibleTo("YarraTrams.Havm2TramTracker.Tests")]
namespace YarraTrams.Havm2TramTracker.Processor
{
    public class Processor
    {
        #region Public methods

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
        /// Saves HAVM2 trip information to the T_Temp_Trips in the TramTracker database
        /// </summary>
        /// <param name="trips"></param>
        public static void SaveTripsToT_Temp_Trips(List<Models.HavmTrip> trips)
        {
            var tripsDT = CopyTripsToT_Temp_TripsDataTable(trips);
            SaveTripDataToDatabase("T_Temp_Trips", tripsDT);
        }

        /// <summary>
        /// Saves HAVM2 trip information to the T_Temp_Schedules in the TramTracker database
        /// </summary>
        /// <param name="trips"></param>
        public static void SaveTripsToT_Temp_Schedules(List<Models.HavmTrip> trips)
        {
            var tripsDT = CopyTripsToT_Temp_SchedulesDataTable(trips);
            SaveTripDataToDatabase("T_Temp_Schedules", tripsDT);
        }

        #endregion

        #region Data Tramsformation and Mapping
        
        /// <summary>
        /// Copies trip objects in to a new DataTable that matches the structure of the T_Temp_Trips table in the TramTracker database.
        /// 
        /// This routine also performs the required data transformations.
        /// 
        /// The returned DataTable can be bulk-copied in to the TramTRACKER database using the SaveTripsToDatabase routine.
        /// </summary>
        /// <param name="trips"></param>
        /// <returns></returns>
        internal static TramTrackerDataSet.T_Temp_TripsDataTable CopyTripsToT_Temp_TripsDataTable(List<Models.HavmTrip> trips)
        {
            var tripDataTable = new TramTrackerDataSet.T_Temp_TripsDataTable();
            var exceptionCounts = new Dictionary<System.DayOfWeek, int>();
            foreach(var dayOfWeek in Enum.GetValues(typeof(System.DayOfWeek)).Cast<System.DayOfWeek>())
            {
                exceptionCounts.Add(dayOfWeek, 0);
            }

            bool logRowsToFilePriorToInsert = Properties.Settings.Default.LogT_Temp_TripRowsToFilePriorToInsert;
            int tripCounter = 0;
            int rowCounter = 0;

            var errorLog = new StringBuilder();

            foreach (HavmTrip trip in trips)
            {
                tripCounter++;
                try
                {
                    TramTrackerDataSet.T_Temp_TripsRow tripsRow = (TramTrackerDataSet.T_Temp_TripsRow)tripDataTable.NewRow();
                    tripsRow.TripID = trip.HastusTripId;
                    tripsRow.RunNo = Transformations.GetRunNumber(trip);
                    tripsRow.RouteNo = Transformations.GetRouteNo(trip);
                    tripsRow.FirstTP = trip.StartTimepoint;
                    tripsRow.FirstTime = (int)trip.StartTime.TotalSeconds;
                    tripsRow.EndTP = trip.EndTimepoint;
                    tripsRow.EndTime = (int)trip.EndTime.TotalSeconds - 1;//Todo: Investigate this. Why is TT making all trips end 1 second early?
                    tripsRow.AtLayoverTime = Transformations.GetAtLayovertime(trip);
                    tripsRow.NextRouteNo = Transformations.GetNextRouteNo(trip);
                    tripsRow.UpDirection = Transformations.GetUpDirection(trip);
                    tripsRow.LowFloor = Transformations.GetLowFloor(trip);
                    tripsRow.TripDistance = Transformations.GetTripDistance(trip);
                    tripsRow.PublicTrip = trip.IsPublic; //Todo: Confirm whether we bother filtering non public trips or we trust HAVM2.
                    tripsRow.DayOfWeek = Transformations.GetDayOfWeek(trip);

                    rowCounter++;

                    if (logRowsToFilePriorToInsert)
                    {
                        Helpers.LogfileWriter.writeToFile("T_Temp_TripsRowsPriorToInsert", $"\n\n{DateTime.Now}\nTrip {tripCounter}\n{trip.ToString()}\nRow {rowCounter}\n{tripsRow.ToLogString()}", Properties.Settings.Default.LogFilePath);
                    }

                    tripDataTable.AddT_Temp_TripsRow(tripsRow);
                }
                catch (Exception ex)
                {
                    errorLog.Append($"Exception: {ex.Message}\n{trip.ToString()}\n");
                    
                    exceptionCounts[trip.OperationalDay.DayOfWeek]++;
                }
            }

            int totalErrors = exceptionCounts.Values.Sum();
            if (totalErrors == 0)
            {
                LogWriter.Instance.LogWithoutDelay(EventLogCodes.TRIP_TRANSFORMATION_SUCCESS
                    , $"{trips.Count} HAVM trip{(trips.Count == 1 ? "" : "s")} successfully transformed in to the TramTRACKER T_Temp_Trips format.");
            }
            else
            {
                var today = DateTime.Now;
                // Write exceptions to file
                string logFilePath = Properties.Settings.Default.LogFilePath;
                string filePostFix = "T_Temp_Trips";
                Helpers.LogfileWriter.writeToFile(filePostFix, errorLog.ToString(),logFilePath);

                // Log error message to event log
                var message = new StringBuilder();
                var todayDayOfWeek = today.DayOfWeek;
                
                // We print the number of exceptions separately for each of the next 7 days, starting with tomorrow.
                foreach (var dayOfWeek in exceptionCounts.OrderBy(pair => pair.Key<=todayDayOfWeek?pair.Key+7:pair.Key))
                {
                    message.AppendLine($"{dayOfWeek.Key.ToString()} errors: {dayOfWeek.Value}");
                }
                message.AppendLine($"See the \"{filePostFix}\" log file under {logFilePath} for more detail.");

                LogWriter.Instance.LogWithoutDelay(EventLogCodes.TRIP_TRANSFORMATION_ERROR
                    , $"Encountered {totalErrors} error{(totalErrors==1?"":"s")} when transforming {trips.Count} HAVM trip{(trips.Count == 1 ? "" : "s")} in to the TramTRACKER T_Temp_Trips format."
                    , message.ToString());
            }

            return tripDataTable;
        }

        
        /// <summary>
        /// Copies trip objects in to a new DataTable that matches the structure of the T_Temp_Schedules table in the TramTracker database.
        /// 
        /// This routine also performs the required data transformations.
        /// 
        /// The returned DataTable can be bulk-copied in to the TramTRACKER database using the SaveTripsToDatabase routine.
        /// </summary>
        /// <param name="trips"></param>
        /// <returns></returns>
        internal static TramTrackerDataSet.T_Temp_SchedulesDataTable CopyTripsToT_Temp_SchedulesDataTable(List<Models.HavmTrip> trips)
        {
            HastusStopMapper.Populate();

            var schedulesDataTable = new TramTrackerDataSet.T_Temp_SchedulesDataTable();
            var exceptionCounts = new Dictionary<System.DayOfWeek, int>();
            foreach (var dayOfWeek in Enum.GetValues(typeof(System.DayOfWeek)).Cast<System.DayOfWeek>())
            {
                exceptionCounts.Add(dayOfWeek, 0);
            }

            bool logRowsToFilePriorToInsert = Properties.Settings.Default.LogT_Temp_SchedulesRowsToFilePriorToInsert;
            int tripCounter = 0;
            int rowCounter = 0;

            var errorLog = new StringBuilder();

            foreach (HavmTrip trip in trips)
            {
                tripCounter++;
                try
                {

                    if (logRowsToFilePriorToInsert)
                    {
                        Helpers.LogfileWriter.writeToFile("T_Temp_SchedulesRowsPriorToInsert", $"\n\n{DateTime.Now}\nTrip {tripCounter}\n{trip.ToString()}", Properties.Settings.Default.LogFilePath);
                    }
                    int tripId = trip.HastusTripId;
                    string runNo = Transformations.GetRunNumber(trip);
                    short routeNo = Transformations.GetRouteNo(trip);
                    byte dayOfWeek = Transformations.GetDayOfWeek(trip);
                    bool lowFloor = Transformations.GetLowFloor(trip);
                    bool publicTrip = trip.IsPublic; //Todo: Confirm whether we bother filtering non public trips or we trust HAVM2.

                    foreach (HavmTripStop stop in trip.Stops)
                    {
                        TramTrackerDataSet.T_Temp_SchedulesRow schedulessRow = (TramTrackerDataSet.T_Temp_SchedulesRow)schedulesDataTable.NewRow();
                        schedulessRow.TripID = tripId;
                        schedulessRow.RunNo = runNo;
                        schedulessRow.RouteNo = routeNo;
                        schedulessRow.DayOfWeek = dayOfWeek;
                        schedulessRow.LowFloor = lowFloor;
                        schedulessRow.PublicTrip = publicTrip;
                        schedulessRow.OPRTimePoint = stop.IsMonitoredOPRReliability;
                        schedulessRow.StopID = Transformations.GetStopId(stop);
                        schedulessRow.Time = (int)stop.PassingTime.TotalSeconds;

                        rowCounter++;

                        if (logRowsToFilePriorToInsert)
                        {
                            Helpers.LogfileWriter.writeToFile("T_Temp_SchedulesRowsPriorToInsert", $"\nRow {rowCounter}\n{schedulessRow.ToLogString()}", Properties.Settings.Default.LogFilePath);
                        }

                        schedulesDataTable.AddT_Temp_SchedulesRow(schedulessRow);
                    }

                }
                catch (Exception ex)
                {
                    errorLog.Append($"Exception: {ex.Message}\n{trip.ToString()}\n");

                    exceptionCounts[trip.OperationalDay.DayOfWeek]++;
                }
            }

            int totalErrors = exceptionCounts.Values.Sum();
            if (totalErrors == 0)
            {
                LogWriter.Instance.LogWithoutDelay(EventLogCodes.TRIP_TRANSFORMATION_SUCCESS
                    , $"{trips.Count} HAVM trip{(trips.Count == 1 ? "" : "s")} successfully transformed in to the TramTRACKER T_Temp_Schedules format.");
            }
            else
            {
                var today = DateTime.Now;
                // Write exceptions to file
                string logFilePath = Properties.Settings.Default.LogFilePath;
                string filePostFix = "T_Temp_Schedules";
                Helpers.LogfileWriter.writeToFile(filePostFix, errorLog.ToString(), logFilePath);

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
                    , $"Encountered {totalErrors} error{(totalErrors == 1 ? "" : "s")} when transforming {trips.Count} HAVM trip{(trips.Count == 1 ? "" : "s")} in to the TramTRACKER T_Temp_Schedules format."
                    , message.ToString());
            }

            return schedulesDataTable;
        }

        #endregion

        #region Database
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripData">A typed DataTable. You can use the CopyTripsTo???DataTable routines to generate one.</param>
        private static void SaveTripDataToDatabase(string tableName,DataTable tripData)
        {
            //Dynmaically create SQL here, instead of bulkcopy. Makes error handling easier.
            
            // connect to SQL
            using (SqlConnection connection =
                    new SqlConnection(Properties.Settings.Default.TramTrackerDB))
            {
                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy
                    (
                    connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    externalTransaction: null
                    );

                bulkCopy.DestinationTableName = tableName;
                connection.Open();

                bulkCopy.WriteToServer(tripData);
                connection.Close();
            }
        }
        #endregion
    }
}
