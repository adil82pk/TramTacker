using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.TestComparisons.Models
{
    internal class T_Temp_SchedulesComparer : TramTrackerComparer
    {
        public T_Temp_SchedulesComparer()
        {
            base.TableName = "T_Temp_Schedules";
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_Schedules tables, finding data in the first that's not in the second
        /// - Inserts results in to Havm2TTComparison_T_Temp_Schedules_MissingFromNew
        /// </summary>
        public override string GetMissingFromNewSql(int runId)
        {
            string sql = string.Format(@"INSERT Havm2TTComparison_T_Temp_Schedules_MissingFromNew
                                        SELECT {0},
                                        live.TripID,
                                        live.RunNo,
                                        live.StopID,
                                        live.RouteNo,
                                        live.OPRTimePoint,
                                        live.Time,
                                        live.DayOfWeek,
                                        live.LowFloor,
                                        live.PublicTrip
                                        FROM T_Temp_Schedules live
                                        LEFT JOIN T_Temp_Schedules{1} new ON new.TripID = live.TripID
                                            AND RTRIM(LTRIM(new.StopID)) = RTRIM(LTRIM(live.StopID))
                                            AND new.DayOfWeek = live.DayOfWeek
                                        WHERE new.TripID IS NULL", runId, Processor.Helpers.SettingsExposer.DbTableSuffix());

            return sql;
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_Schedules tables, finding data in the second that's not in the first
        /// - Inserts results in to Havm2TTComparison_T_Temp_Schedules_ExtraInNew
        /// </summary>
        public override string GetExtraInNewSql(int runId)
        {
            string sql = string.Format(@"INSERT Havm2TTComparison_T_Temp_Schedules_ExtraInNew
                                        SELECT {0},
                                        new.TripID,
                                        new.RunNo,
                                        new.StopID,
                                        new.RouteNo,
                                        new.OPRTimePoint,
                                        new.Time,
                                        new.DayOfWeek,
                                        new.LowFloor,
                                        new.PublicTrip
                                        FROM T_Temp_Schedules{1} new
                                        LEFT JOIN T_Temp_Schedules live ON live.TripID = new.TripID
                                            AND RTRIM(LTRIM(live.StopID)) = RTRIM(LTRIM(new.StopID))
                                            AND live.DayOfWeek = new.DayOfWeek
                                        WHERE live.TripID IS NULL", runId, Processor.Helpers.SettingsExposer.DbTableSuffix());

            return sql;
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_Schedules tables, finding data between that two with matching keys but non matching detail
        /// - Inserts results in to Havm2TTComparison_T_Temp_Schedules_Differing
        /// </summary>
        public override string GetDifferingSql(int runId)
        {
            string sql = string.Format(@"CREATE TABLE #Diffs (Id uniqueidentifier, TripID int, StopID char(8) COLLATE database_default, [DayOfWeek] int)

                                        INSERT #Diffs
                                        SELECT NewID(), live.TripID, live.StopID, live.[DayOfWeek]	
                                        FROM T_Temp_Schedules live
                                        JOIN T_Temp_Schedules{1} new ON new.TripID = live.TripID
                                            AND RTRIM(LTRIM(new.StopID)) = RTRIM(LTRIM(live.StopID))
                                            AND new.[DayOfWeek]  = live.[DayOfWeek]
                                        WHERE
                                        NOT (live.TripID = new.TripID
                                        AND live.RunNo = new.RunNo 
                                        AND live.StopID = new.StopID
                                        AND live.RouteNo = new.RouteNo
                                        AND live.Time = new.Time 
                                        AND live.DayOfWeek = new.DayOfWeek
                                        AND live.LowFloor = new.LowFloor
                                        AND live.PublicTrip = new.PublicTrip)

                                        INSERT Havm2TTComparison_T_Temp_Schedules_Differing
                                        SELECT {0},
                                        #Diffs.Id,
                                        1,
                                        live.TripID,
                                        live.RunNo,
                                        live.StopID,
                                        live.RouteNo,
                                        live.OPRTimePoint,
                                        live.Time,
                                        live.DayOfWeek,
                                        live.LowFloor,
                                        live.PublicTrip
                                        FROM T_Temp_Schedules live
                                        JOIN #Diffs ON #Diffs.TripID = live.TripID
                                            AND RTRIM(LTRIM(#Diffs.StopID)) = RTRIM(LTRIM(live.StopID))
                                            AND #Diffs.[DayOfWeek] = live.[DayOfWeek]

                                        INSERT Havm2TTComparison_T_Temp_Schedules_Differing
                                        SELECT {0},
                                        #Diffs.Id,
                                        0,
                                        new.TripID,
                                        new.RunNo,
                                        new.StopID,
                                        new.RouteNo,
                                        new.OPRTimePoint,
                                        new.Time,
                                        new.DayOfWeek,
                                        new.LowFloor,
                                        new.PublicTrip
                                        FROM T_Temp_Schedules{1} new
                                        JOIN #Diffs ON #Diffs.TripID = new.TripID
                                            AND RTRIM(LTRIM(#Diffs.StopID)) = RTRIM(LTRIM(new.StopID))
                                            AND #Diffs.[DayOfWeek] = new.[DayOfWeek]", runId, Processor.Helpers.SettingsExposer.DbTableSuffix());

            return sql;
        }
    }
}
