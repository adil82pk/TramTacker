using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Logger;
using YarraTrams.Havm2TramTracker.SideBySideTests.Helpers;

namespace YarraTrams.Havm2TramTracker.SideBySideTests.Models
{
    abstract class TramTrackerComparer
    {
        protected string TableName;

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_[...] tables, finding data in the first that's not in the second
        /// - Inserts results in to Havm2TTComparison_T_Temp_[...]_MissingFromNew
        /// </summary>
        public abstract string GetMissingFromNewSql(int runId);

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_[...] tables, finding data in the second that's not in the first
        /// - Inserts results in to Havm2TTComparison_T_Temp_[...]ExtraInNew
        /// </summary>
        public abstract string GetExtraInNewSql(int runId);

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Compares data in two T_Temp_[...] tables, finding data between that two with matching keys but non matching detail
        /// - Inserts results in to Havm2TTComparison_T_Temp_[...]Differing
        /// </summary>
        public abstract string GetDifferingSql(int runId);

        /// <summary>
        /// Compare data between a tramTRACKER table populated via Existing means and a tramTRACKER table populated via New means.
        /// Looks for:
        /// - Rows in Existing that are missing from New
        /// - Rows in New that are not present in Existing
        /// - Rows with matching key data between Existing and New but with differing detail
        /// </summary>
        public void RunComparison(int runId)
        {
            LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, string.Format("Running GetMissingInNew SQL for {0}.", this.TableName));
            DBHelper.ExecuteSQL(GetMissingFromNewSql(runId));

            LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, string.Format("Running GetExtraInNew SQL for {0}.", this.TableName));
            DBHelper.ExecuteSQL(GetExtraInNewSql(runId));

            LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, string.Format("Running GetDiffering SQL for {0}.", this.TableName));
            DBHelper.ExecuteSQL(GetDifferingSql(runId));

            LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, string.Format("Running Summary SQL for {0}.", this.TableName));
            DBHelper.ExecuteSQL(GetSummaryInsertSql(runId));
        }

        /// <summary>
        /// Returns a SQL string that, when executed:
        /// - Inserts a summary record in to Havm2TTComparisonRunTable
        /// </summary>
        private string GetSummaryInsertSql(int runId)
        {
            string sql = string.Format(@"DECLARE @TotalExisting int = 0
                            SELECT @TotalExisting = COUNT(*) FROM {0}

                            DECLARE @TotalNew int = 0
                            SELECT @TotalNew = COUNT(*) FROM {0}{2}

                            DECLARE @MissingFromNew int = 0
                            SELECT @MissingFromNew = COUNT(*) FROM Havm2TTComparison_{0}_MissingFromNew WHERE Havm2TTComparisonRunId = {1}

                            DECLARE @ExtraInNew int = 0
                            SELECT @ExtraInNew = COUNT(*) FROM Havm2TTComparison_{0}_ExtraInNew WHERE Havm2TTComparisonRunId = {1}

                            DECLARE @Differing int = 0
                            SELECT @Differing = COUNT(*) FROM Havm2TTComparison_{0}_Differing WHERE Havm2TTComparisonRunId = {1} AND IsExisting = 1

                            DECLARE @Identical int = @TotalExisting - @MissingFromNew - @Differing

                            INSERT Havm2TTComparisonRunTable
                            VALUES ({1}, '{0}', @TotalExisting, @TotalNew, @Identical, @MissingFromNew, @ExtraInNew, @Differing)", this.TableName, runId, Processor.Helpers.SettingsExposer.DbTableSuffix());

            return sql;
        }

        /// <summary>
        /// Returns a DataTable showing every summary record for this table for the past X days.
        /// </summary>
        public DataTable GetSummaryHistory(int daysBack)
        {
            string sql = string.Format(@"SELECT rt.TableName,
                                        r.RunTime,
                                        TotalRecordsExisting,
                                        TotalRecordsNew,
                                        RecordsIdentical,
                                        RecordsMissingFromNew,
                                        RecordsExtraInNew,
                                        RecordsDiffering,
                                        r.Id [RunId]
                                        FROM Havm2TTComparisonRun r
                                        JOIN Havm2TTComparisonRunTable rt ON rt.Havm2TTComparisonRunId = r.Id
                                        WHERE DATEDIFF(day, r.RunTime, GETDATE()) < {0}
                                        AND rt.TableName = '{1}'
                                        ORDER BY r.RunTime DESC", daysBack, this.TableName);

            return DBHelper.GetData(sql);
        }
    }
}
