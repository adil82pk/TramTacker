using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.TestComparisons.Models
{
    internal class T_Temp_SchedulesMasterComparer : TramTrackerComparer
    {
        public T_Temp_SchedulesMasterComparer()
        {
            base.TableName = "T_Temp_SchedulesMaster";
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_SchedulesMaster tables, finding data in the first that's not in the second
        /// - Inserts results in to Havm2TTComparison_T_Temp_SchedulesMaster_MissingFromNew
        /// </summary>
        public override string GetMissingFromNewSql(int runId)
        {
            string sql = string.Format(@"INSERT Havm2TTComparison_T_Temp_SchedulesMaster_MissingFromNew
                                        SELECT {0},
                                            live.[TramClass],
                                            live.[HeadboardNo],
                                            live.[RouteNo],
                                            live.[RunNo],
                                            live.[StartDate],
                                            live.[TripNo],
                                            live.[PublicTrip]
                                        FROM T_Temp_SchedulesMaster live
                                        LEFT JOIN T_Temp_SchedulesMaster{1} new ON new.TripNo = live.TripNo
                                            AND new.StartDate = live.StartDate
                                        WHERE new.TripNo IS NULL", runId, Processor.Helpers.SettingsExposer.DbTableSuffix());

            return sql;
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_SchedulesMaster tables, finding data in the second that's not in the first
        /// - Inserts results in to Havm2TTComparison_T_Temp_SchedulesMaster_ExtraInNew
        /// </summary>
        public override string GetExtraInNewSql(int runId)
        {
            string sql = string.Format(@"INSERT Havm2TTComparison_T_Temp_SchedulesMaster_ExtraInNew
                                        SELECT {0},
                                            new.[TramClass],
                                            new.[HeadboardNo],
                                            new.[RouteNo],
                                            new.[RunNo],
                                            new.[StartDate],
                                            new.[TripNo],
                                            new.[PublicTrip]
                                        FROM T_Temp_SchedulesMaster{1} new
                                        LEFT JOIN T_Temp_SchedulesMaster live ON live.TripNo = new.TripNo
                                            AND live.StartDate = new.StartDate
                                        WHERE live.TripNo IS NULL", runId, Processor.Helpers.SettingsExposer.DbTableSuffix());

            return sql;
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_SchedulesMaster tables, finding data between that two with matching keys but non matching detail
        /// - Inserts results in to Havm2TTComparison_T_Temp_SchedulesMaster_Differing
        /// </summary>
        public override string GetDifferingSql(int runId)
        {
            string sql = string.Format(@"CREATE TABLE #Diffs (Id uniqueidentifier, TripNo int, StartDate datetime)

                                    INSERT #Diffs
                                    SELECT NewID(), live.TripNo, live.StartDate
                                    FROM T_Temp_SchedulesMaster live
                                    JOIN T_Temp_SchedulesMaster{1} new ON new.TripNo = live.TripNo
	                                    AND new.StartDate  = live.StartDate
                                    WHERE
                                    NOT (live.[TramClass] = new.[TramClass]
                                        AND live.[HeadboardNo] = new.[HeadboardNo]
                                        AND live.[RouteNo] = new.[RouteNo]
                                        AND live.[RunNo] = new.[RunNo]
                                        AND live.[StartDate] = new.[StartDate]
                                        AND live.[TripNo] = new.[TripNo]
                                        AND live.[PublicTrip] = new.[PublicTrip])

                                    INSERT Havm2TTComparison_T_Temp_SchedulesMaster_Differing
                                    SELECT {0},
                                    #Diffs.Id,
                                        1,
                                        live.[TramClass],
                                        live.[HeadboardNo],
                                        live.[RouteNo],
                                        live.[RunNo],
                                        live.[StartDate],
                                        live.[TripNo],
                                        live.[PublicTrip]
                                    FROM T_Temp_SchedulesMaster live
                                    JOIN #Diffs ON #Diffs.TripNo = live.TripNo
                                        AND #Diffs.StartDate = live.StartDate

                                    INSERT Havm2TTComparison_T_Temp_SchedulesMaster_Differing
                                    SELECT {0},
                                        #Diffs.Id,
                                        0,
                                        new.[TramClass],
                                        new.[HeadboardNo],
                                        new.[RouteNo],
                                        new.[RunNo],
                                        new.[StartDate],
                                        new.[TripNo],
                                        new.[PublicTrip]
                                    FROM T_Temp_SchedulesMaster{1} new
                                    JOIN #Diffs ON #Diffs.TripNo = new.TripNo
                                       AND #Diffs.StartDate = new.StartDate", runId, Processor.Helpers.SettingsExposer.DbTableSuffix());

            return sql;
        }
    }
}
