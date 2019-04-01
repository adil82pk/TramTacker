using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.TestComparisons.Models
{
    internal class T_Temp_SchedulesDetailsComparer : TramTrackerComparer
    {
        public T_Temp_SchedulesDetailsComparer()
        {
            base.TableName = "T_Temp_SchedulesDetails";
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_SchedulesDetails tables, finding data in the first that's not in the second
        /// - Inserts results in to Havm2TTComparison_T_Temp_SchedulesDetails_MissingFromNew
        /// </summary>
        public override string GetMissingFromNewSql(int runId)
        {
            string sql = string.Format(@"INSERT Havm2TTComparison_T_Temp_SchedulesDetails_MissingFromNew
                                        SELECT {0},
                                            live.ArrivalTime,
                                            live.StopID,
                                            live.TripID,
                                            live.RunNo,
                                            live.OPRTimePoint
                                        FROM T_Temp_SchedulesDetails live
                                        LEFT JOIN T_Temp_SchedulesDetails{1} new ON RTRIM(LTRIM(new.TripID)) = RTRIM(LTRIM(live.TripID))
                                            AND RTRIM(LTRIM(new.StopID)) = RTRIM(LTRIM(live.StopID))
                                        WHERE new.TripID IS NULL", runId, Processor.Helpers.SettingsExposer.DbTableSuffix());

            return sql;
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_SchedulesDetails tables, finding data in the second that's not in the first
        /// - Inserts results in to Havm2TTComparison_T_Temp_SchedulesDetails_ExtraInNew
        /// </summary>
        public override string GetExtraInNewSql(int runId)
        {
            string sql = string.Format(@"INSERT Havm2TTComparison_T_Temp_SchedulesDetails_ExtraInNew
                                        SELECT {0},
                                            new.ArrivalTime,
                                            new.StopID,
                                            new.TripID,
                                            new.RunNo,
                                            new.OPRTimePoint
                                        FROM T_Temp_SchedulesDetails{1} new
                                        LEFT JOIN T_Temp_SchedulesDetails live ON RTRIM(LTRIM(live.TripID)) = RTRIM(LTRIM(new.TripID))
                                            AND RTRIM(LTRIM(live.StopID)) = RTRIM(LTRIM(new.StopID))
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
            string sql = string.Format(@"CREATE TABLE #Diffs (Id uniqueidentifier, TripID varchar(50) COLLATE database_default, StopID varchar(50) COLLATE database_default)

                                        INSERT #Diffs
                                        SELECT NewId(), *
                                        FROM
                                            (--This table can have duplicate trip/stop combos because there is no record of the date/day, so we filter the multi-day dupes.
                                            SELECT DISTINCT live.TripID, live.StopID
                                            FROM T_Temp_SchedulesDetails live
                                            JOIN T_Temp_SchedulesDetails{1} new ON new.TripID = live.TripID
                                                AND RTRIM(LTRIM(new.StopID)) = RTRIM(LTRIM(live.StopID))
                                            WHERE
                                            NOT (live.ArrivalTime = new.ArrivalTime
                                                AND live.StopID = new.StopID
                                                AND live.TripID = new.TripID
                                                AND live.RunNo = new.RunNo)
                                            ) x

                                        INSERT Havm2TTComparison_T_Temp_SchedulesDetails_Differing
                                        SELECT {0},
                                            #Diffs.Id,
                                            1,
                                            live.ArrivalTime,
                                            live.StopID,
                                            live.TripID,
                                            live.RunNo,
                                            live.OPRTimePoint
                                        FROM T_Temp_SchedulesDetails live
                                        JOIN #Diffs ON RTRIM(LTRIM(#Diffs.TripID)) = RTRIM(LTRIM(live.TripID))
                                            AND RTRIM(LTRIM(#Diffs.StopID)) = RTRIM(LTRIM(live.StopID))

                                        INSERT Havm2TTComparison_T_Temp_SchedulesDetails_Differing
                                        SELECT {0},
                                            #Diffs.Id,
                                            0,
                                            new.ArrivalTime,
                                            new.StopID,
                                            new.TripID,
                                            new.RunNo,
                                            new.OPRTimePoint
                                        FROM T_Temp_SchedulesDetails{1} new
                                        JOIN #Diffs ON RTRIM(LTRIM(#Diffs.TripID)) = RTRIM(LTRIM(new.TripID))
                                            AND RTRIM(LTRIM(#Diffs.StopID)) = RTRIM(LTRIM(new.StopID))", runId, Processor.Helpers.SettingsExposer.DbTableSuffix());

            return sql;
        }
    }
}
