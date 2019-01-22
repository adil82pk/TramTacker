using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Models;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Data;

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

            foreach (HavmTrip trip in trips)
            {
                tripDataTable.AddT_Temp_TripsRow(TripID: trip.HastusTripId //Todo: Create all transformations
                                                , RunNo: trip.DisplayCode
                                                , RouteNo:          1
                                                , FirstTP:          trip.StartTimepoint
                                                , FirstTime:        (int)trip.StartTime.TotalSeconds
                                                , EndTP:            trip.EndTimepoint
                                                , EndTime:          (int)trip.EndTime.TotalSeconds
                                                , AtLayoverTime:    Transformations.GetAtLayovertime(trip)
                                                , NextRouteNo:      1
                                                , UpDirection:      Transformations.GetUpDirection(trip)
                                                , LowFloor:         false
                                                , TripDistance:     500
                                                , PublicTrip:       true
                                                , DayOfWeek:           1
                                                );
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
            throw new NotImplementedException();
        }

        #endregion

        #region Database
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tripData">A typed DataTable. You can use the CopyTripsTo???DataTable routines to generate one.</param>
        private static void SaveTripDataToDatabase(string tableName,DataTable tripData)
        {
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
