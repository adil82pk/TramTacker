using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.SideBySideTests.Models
{
    internal class T_Temp_TripsComparer : TramTrackerComparer
    {
        public T_Temp_TripsComparer()
        {
            base.TableName = "T_Temp_Trips";
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_trips tables, finding data in the first that's not in the second
        /// - Inserts results in to Havm2TTComparison_T_Temp_Trips_MissingFromNew
        /// </summary>
        public override string GetMissingFromNewSql(int runId)
        {
            string sql = string.Format(@"INSERT Havm2TTComparison_T_Temp_Trips_MissingFromNew
                                        SELECT {0},
                                        live.TripID,
                                        live.RunNo,
                                        live.RouteNo,
                                        live.FirstTP,
                                        live.FirstTime,
                                        live.EndTP,
                                        live.EndTime,
                                        live.AtLayoverTime,
                                        live.NextRouteNo,
                                        live.UpDirection,
                                        live.LowFloor,
                                        live.TripDistance,
                                        live.PublicTrip,
                                        live.[DayOfWeek]
                                        FROM T_Temp_Trips live
                                        LEFT JOIN T_Temp_Trips_TTBU new ON new.TripID = live.TripID
                                        WHERE new.TripID IS NULL", runId);

            return sql;
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_trips tables, finding data in the second that's not in the first
        /// - Inserts results in to Havm2TTComparison_T_Temp_Trips_ExtraInNew
        /// </summary>
        public override string GetExtraInNewSql(int runId)
        {
            string sql = string.Format(@"INSERT Havm2TTComparison_T_Temp_Trips_ExtraInNew
                                        SELECT {0},
                                        new.TripID,		
                                        new.RunNo,
                                        new.RouteNo,
                                        new.FirstTP,
                                        new.FirstTime,
                                        new.EndTP,
                                        new.EndTime,
                                        new.AtLayoverTime,
                                        new.NextRouteNo,
                                        new.UpDirection,
                                        new.LowFloor,
                                        new.TripDistance,
                                        new.PublicTrip,
                                        new.[DayOfWeek]		
                                        FROM T_Temp_Trips_TTBU new
                                        LEFT JOIN T_Temp_Trips live ON live.TripID = new.TripID
                                        WHERE live.TripID IS NULL", runId);

            return sql;
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_trips tables, finding data between that two with matching keys but non matching detail
        /// - Inserts results in to Havm2TTComparison_T_Temp_Trips_Differing
        /// </summary>
        public override string GetDifferingSql(int runId)
        {
            string sql = string.Format(@"CREATE TABLE #Diffs (Id uniqueidentifier, TripID int, [DayOfWeek] int)

                                    INSERT #Diffs
                                    SELECT NewID(), live.TripID, live.[DayOfWeek]	
                                    FROM T_Temp_Trips live
                                    JOIN T_Temp_Trips_TTBU new ON new.TripID = live.TripID
	                                    AND new.[DayOfWeek]  = live.[DayOfWeek]
                                    WHERE
                                    NOT (live.TripID = new.TripID
                                    AND live.RunNo = new.RunNo 
                                    AND live.RouteNo = new.RouteNo
                                    AND live.FirstTP = new.FirstTP
                                    AND live.FirstTime = new.FirstTime
                                    AND live.EndTP = new.EndTP 
                                    AND live.EndTime = new.EndTime
                                    AND live.AtLayoverTime = new.AtLayoverTime
                                    AND live.NextRouteNo = new.NextRouteNo
                                    AND live.UpDirection = new.UpDirection
                                    AND live.LowFloor = new.LowFloor
                                    AND live.TripDistance = new.TripDistance
                                    AND live.PublicTrip = new.PublicTrip
                                    AND live.[DayOfWeek] = new.[DayOfWeek])

                                    INSERT Havm2TTComparison_T_Temp_Trips_Differing
                                    SELECT {0},
                                    #Diffs.Id,
                                    1,
                                    live.TripID,
                                    live.RunNo,
                                    live.RouteNo,
                                    live.FirstTP,
                                    live.FirstTime,
                                    live.EndTP,
                                    live.EndTime,
                                    live.AtLayoverTime,
                                    live.NextRouteNo,
                                    live.UpDirection,
                                    live.LowFloor,
                                    live.TripDistance,
                                    live.PublicTrip,
                                    live.[DayOfWeek]
                                    FROM T_Temp_Trips live
                                    JOIN #Diffs ON #Diffs.TripID = live.TripID
			                                    AND #Diffs.[DayOfWeek] = live.[DayOfWeek]

                                    INSERT Havm2TTComparison_T_Temp_Trips_Differing
                                    SELECT {0},
                                    #Diffs.Id,
                                    0,
                                    new.TripID,
                                    new.RunNo,
                                    new.RouteNo,
                                    new.FirstTP,
                                    new.FirstTime,
                                    new.EndTP,
                                    new.EndTime,
                                    new.AtLayoverTime,
                                    new.NextRouteNo,
                                    new.UpDirection,
                                    new.LowFloor,
                                    new.TripDistance,
                                    new.PublicTrip,
                                    new.[DayOfWeek]		
                                    FROM T_Temp_Trips_TTBU new
                                    JOIN #Diffs ON #Diffs.TripID = new.TripID
			                        AND #Diffs.[DayOfWeek] = new.[DayOfWeek]", runId);

            return sql;
        }
    }
}
