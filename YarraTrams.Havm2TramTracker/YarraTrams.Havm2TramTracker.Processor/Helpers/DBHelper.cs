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
        /// 
        /// </summary>
        /// <param name="tripData">A typed DataTable. You can use the CopyTripsTo???DataTable routines to generate one.</param>
        public static void SaveTripDataToDatabase(string tableName, DataTable tripData)
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

        public static void CopyDataFromTempToLive()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.TramTrackerDB))
                {
                    string query = @"
                                BEGIN TRAN

                                -- It's all or nothing - we either insert 1 or more records into both tables or we abort completely,
                                BEGIN TRY
	                                DELETE dbo.T_Trips
	                                DECLARE @CountOfT_Trips int = @@ROWCOUNT
                                    
	                                INSERT dbo.T_Trips
	                                SELECT *
	                                FROM dbo.T_Temp_Trips
	                                DECLARE @CountOfT_Temp_Trips int = @@ROWCOUNT
                                    
	                                DELETE dbo.T_Schedules
	                                DECLARE @CountOfT_Schedules int = @@ROWCOUNT
                                    
	                                INSERT dbo.T_Schedules
	                                SELECT [TripID], [RunNo], [StopID], [RouteNo], [OPRTimePoint], [Time], [DayOfWeek], [LowFloor], [PublicTrip]
	                                FROM dbo.T_Temp_Schedules
	                                DECLARE @CountOfT_Temp_Schedules int = @@ROWCOUNT
                                    
	                                IF @CountOfT_Temp_Trips = 0
		                                RAISERROR('No trips to insert',16,1)
                                    
	                                IF @CountOfT_Temp_Schedules = 0
		                                RAISERROR('No schedules to insert',16,1)
                                    
	                                SELECT @CountOfT_Trips [TripsDeleted], @CountOfT_Temp_Trips [TripsAdded], @CountOfT_Schedules [SchedulesDeleted], @CountOfT_Temp_Schedules [SchedulesAdded]
                                    
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
                    cmd.CommandTimeout = 180;

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
