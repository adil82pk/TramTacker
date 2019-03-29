using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Logger;

namespace YarraTrams.Havm2TramTracker.Processor.Helpers
{
    public class DBHelper
    {
        /// <summary>
        /// Returns DB table name (with suffix appended if configured)
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string GetDbTableName(string table)
        {
            if (Properties.Settings.Default.DbTableSuffix != null)
            {
                table += Properties.Settings.Default.DbTableSuffix;
            }

            return table;
        }

        /// <summary>
        /// Execute the passed-in sql using the passed-in connection and passed-in transaction.
        /// </summary>
        private static void ExecuteSql(string sql, SqlConnection connection, SqlTransaction transaction)
        {
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Transaction = transaction;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = Properties.Settings.Default.DBCommandTimeoutSeconds;
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Execute the passed-in sql using the passed-in connection.
        /// </summary>
        private static void ExecuteSqlProc(string procName, SqlConnection connection)
        {
            SqlCommand cmd = new SqlCommand(procName, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = Properties.Settings.Default.DBCommandTimeoutSeconds;
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes all records from the destination table then inserts the records from the passed-in DataTable;
        /// </summary>
        /// <param name="tripData">A typed DataTable. You can use the .ToDataTable routines on the TramTracker objects to generate one.</param>
        public static void TruncateThenSaveTripDataToDatabase(string tableName, DataTable tripData)
        {
            // connect to SQL
            using (SqlConnection connection =
                    new SqlConnection(Properties.Settings.Default.TramTrackerDB))
            {
                connection.Open();

                using (SqlTransaction transaction =
                           connection.BeginTransaction())
                {
                    try
                    {
                        // Update Preferences and delete existing records
                        string deleteStatement = string.Format("DELETE FROM {0};", tableName);
                        ExecuteSql(deleteStatement, connection, transaction);

                        // Insert new records
                        using (SqlBulkCopy bulkCopy = new SqlBulkCopy(
                                   connection,
                                   SqlBulkCopyOptions.TableLock |
                                    SqlBulkCopyOptions.FireTriggers,
                                   transaction))
                        {
                            bulkCopy.DestinationTableName = tableName;
                            bulkCopy.BulkCopyTimeout = Properties.Settings.Default.DBCommandTimeoutSeconds;
                            bulkCopy.WriteToServer(tripData);
                        }

                        // Update Overlaps & Preferences (if required)
                        string updPreferencesStatement = GetPostUpdateTempSql(tableName);
                        if (!String.IsNullOrEmpty(updPreferencesStatement))
                        {
                            ExecuteSql(updPreferencesStatement, connection, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                        }
                        catch (Exception ex2)
                        {
                            // SQL Server may choose to rollback the transaction itself, so this code prevents the original error from being swallowed.
                             LogWriter.Instance.Log(EventLogCodes.DB_EXECUTE_ERROR, String.Format("Error encountered when rolling back transaction:\n\nMessage: {0}\n\nType:{1}", ex2.Message, ex2.GetType()));
                        }
                        throw ex;
                    }
                }

                connection.Close();

                LogWriter.Instance.LogWithoutDelay(EventLogCodes.SAVE_TO_DATABASE_SUCCESS
                    , String.Format("{0} record{1} saved to {2} table.", tripData.Rows.Count, (tripData.Rows.Count == 1 ? "" : "s"), tableName));
            }
        }

        /// <summary>
        /// Returns the sql to populated the overlaps and update the relevant T_Preferences field if this table is one that gets "copied to live".
        /// A table is deemed to "copy to live" if it is called T_Temp_Trips or T_Temp_Schedules, even if they have the DbTableSuffix applied (in a test environment).
        /// </summary>
        private static string GetPostUpdateTempSql(string tableName)
        {
            if (tableName == GetDbTableName("T_Temp_Trips"))
            {
                return @"UPDATE T_Preferences SET TripsLoaded = 1;
                        EXEC CopyOverlappingTempTrips;";
            }
            else if (tableName == GetDbTableName("T_Temp_Schedules"))
            {
                return @"UPDATE T_Preferences SET ScheduleLoaded = 1;
                        EXEC CopyOverlappingTempSchedules;";
            }
            else
            {
                return "";
            }
        }


        /// <summary>
        /// Deletes all records from T_Trips and T_Schedules
        /// then copies the records from T_Temp_Trips to T_Trips
        /// and copies the records from T_TemP_Schedules to T_Schedules.
        /// The whole operation either succeeds or fails. It never partially commits.
        /// 
        /// Exceptions are logged then re-thrown.
        /// </summary>
        public static void CopyDataFromTempToLive()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.TramTrackerDB))
                {
                    string query = @"
                                BEGIN TRAN
                                
                                -- It's all or nothing - we either insert 1 or more records into both tables or we abort completely.
                                BEGIN TRY
                                    UPDATE T_Preferences SET DataAvailable = 0;
                                    
                                    DELETE dbo.T_Trips;
                                    DECLARE @CountOfT_Trips int = @@ROWCOUNT
                                    
                                    INSERT dbo.T_Trips
                                    SELECT *
                                    FROM dbo.T_Temp_Trips;
                                    DECLARE @CountOfT_Temp_Trips int = @@ROWCOUNT
                                    
                                    DELETE dbo.T_Schedules;
                                    DECLARE @CountOfT_Schedules int = @@ROWCOUNT
                                    
                                    INSERT dbo.T_Schedules
                                    SELECT [TripID], [RunNo], [StopID], [RouteNo], [OPRTimePoint], [Time], [DayOfWeek], [LowFloor], [PublicTrip]
                                    FROM dbo.T_Temp_Schedules;
                                    DECLARE @CountOfT_Temp_Schedules int = @@ROWCOUNT
                                    
                                    IF @CountOfT_Temp_Trips = 0
                                       RAISERROR('No trips to insert',16,1)
                                    
                                    IF @CountOfT_Temp_Schedules = 0
                                       RAISERROR('No schedules to insert',16,1)
                                    
                                    SELECT @CountOfT_Trips [TripsDeleted], @CountOfT_Temp_Trips [TripsAdded], @CountOfT_Schedules [SchedulesDeleted], @CountOfT_Temp_Schedules [SchedulesAdded]
                                    
                                    -- We set Trips/ScheduleLoaded to 0 here. We set them back to one when we next populate the T_Temp_Trips/Schedules tables.
                                    UPDATE T_Preferences SET DataAvailable = 1, TripsLoaded = 0, ScheduleLoaded = 0;
                                    
                                    COMMIT TRAN
                                END TRY
                                BEGIN CATCH
                                    ROLLBACK TRAN
                                    
                                    DECLARE @ErrorMessage NVARCHAR(4000);  
                                    DECLARE @ErrorSeverity INT;  
                                    DECLARE @ErrorState INT;  
                                    
                                    SELECT
                                        @ErrorMessage = 'Line:' + CAST(ISNULL(ERROR_LINE(),0) as varchar(max)) + ' ' + ERROR_MESSAGE(), 
                                        @ErrorSeverity = ERROR_SEVERITY(),  
                                        @ErrorState = ERROR_STATE();  
                                    
                                    RAISERROR (@ErrorMessage, -- Message text.
                                               @ErrorSeverity, -- Severity.
                                               @ErrorState -- State.
                                               );
                                END CATCH";

                    conn.Open();

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = Properties.Settings.Default.DBCommandTimeoutSeconds;

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        int TripsDeleted = (int)reader["TripsDeleted"];
                        int TripsAdded = (int)reader["TripsAdded"];
                        int SchedulesDeleted = (int)reader["SchedulesDeleted"];
                        int SchedulesAdded = (int)reader["SchedulesAdded"];

                        string result = string.Format("{0} trips deleted and {1} trips added. ", TripsDeleted, TripsAdded) +
                                        string.Format("{0} schedules deleted and {1} schedules added.", SchedulesDeleted, SchedulesAdded);

                        LogWriter.Instance.Log(EventLogCodes.COPY_TO_LIVE_SUCCESS
                            , string.Format("Successfully copied Temp data to Live. {0}", result));
                    }
                    else
                    {
                        throw new Exception("No results returned from operation to copy temp data to live.");
                    }

                    reader.Close();

                    // Copy trips from T_Temp_Overlap_Trips to T_Trips, only if the trip doesn't already exist in T_Trips (based on DayOfWeek, RunNo, RouteNo and FirstTime).
                    // (T_Temp_Overlap_Trips was populated by CopyOverlappingTempTrips following the bulk insert in to T_Temp_Trips.)
                    ExecuteSqlProc("CopyOverlapTripsToTrips", conn);

                    // Copy trip stops from T_Temp_Overlap_Schedules to T_Schedules, only if the trip stop doesn't already exist in T_Schedules (based on DayOfWeek, RunNo, RouteNo, Time, TripID and StopID).
                    // (T_Temp_Overlap_Schedules was populated by CopyOverlappingTempSchedules following the bulk insert in to T_Temp_Trips.)
                    ExecuteSqlProc("CopyOverlapScheduleToSchedule", conn);

                    // Populate several tables with trip and schedule data.
                    // See documentation for more detail (Maybe https://inoutput.atlassian.net/wiki/spaces/YKB/pages/753926436/1.2.1.+tramTRACKER+Daily+Timetable+Import).
                    ExecuteSqlProc("[CreateDailyData2.5]", conn);
                    
                    // Sets the current day of the week, a number between 0 (Sunday) and 6 (Saturday), in the DayOfWeekSetting table. DayOfWeekSetting has one field and only ever one record.
                    ExecuteSqlProc("SetDayOfWeek", conn);

                    LogWriter.Instance.Log(EventLogCodes.COPY_TO_LIVE_SUBSEQUENT_PROC_SUCCESS
                            , "Successfully called all procs that run subsequent to copying the Temp data to Live.");
                }
            }
            catch (Exception ex)
            {
                // We make sure we load a very specific event log so that SCOM alerts can be configured in a granular way.
                LogWriter.Instance.Log(EventLogCodes.COPY_TO_LIVE_FAILED
                            , ex.Message);
                throw ex;
            }
        }
    }
}
