using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Logger;

namespace YarraTrams.Havm2TramTracker.Processor.Helpers
{
    public class ApiService
    {
        /// <summary>
        /// Calls HAVM2 and returns trip and stop data for the next 8 (configurable) days, starting from today.
        /// </summary>
        /// <param name="baseDate">Used for testing and Production support. Tells HAVM2 to retrieve data based on a custom date, instead of using today's date.</param>
        /// <param name="retryCount">Number of retries attempted - no need to pass this.</param>
        /// <returns>A JSON-formatted string.</returns>
        public static string GetDataFromHavm2(DateTime? baseDate, int retryCount = 0)
        {
            if (baseDate == null)
            {
                baseDate = DateTime.Now.Date;
            }
            DateTime startDate = baseDate ?? DateTime.Now.Date;
            DateTime endDate = startDate.AddDays(Properties.Settings.Default.NumberDailyTimetablesToRetrieve-1);

            try
            {
                var result = Task.Run(() => {
                    return ApiHttpClient.GetDataFromHavm2(startDate, endDate);
                }).Result;

                LogResultAndCleanUp(result);

                return result;
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.HAVM2_API_ERROR
                            , "More information on this error may be available in the HAVM2 logs."
                                + string.Format("\n\nStartDate = {0:yyyy-MM-dd HH:mm:ss}\nEndDate = {1:yyyy-MM-dd HH:mm:ss}", startDate, endDate)
                                + string.Format("\n\nRetry {0} of {1}\n\n", retryCount, Properties.Settings.Default.MaxGetDataFromHavm2RetryCount)
                                + ex.Message
                            );

                // We retry a certain amount of times
                if (retryCount < Properties.Settings.Default.MaxGetDataFromHavm2RetryCount)
                {
                    retryCount++;
                    Thread.Sleep(Properties.Settings.Default.GapBetweenGetDataFromHavm2RetriesInSecs * 1000);
                    return GetDataFromHavm2(baseDate, retryCount);
                }
                else
                {
                    throw new Exception(string.Format("Error in GetDataFromHavm2 process, retried {0} times, waiting {1} seconds between each try. Further details may be available in the HAVM2 logs."
                                                    , retryCount, Properties.Settings.Default.GapBetweenGetDataFromHavm2RetriesInSecs), ex);
                }
            }
        }

        /// <summary>
        /// Write JSON result to file and clean up any old JSON result files.
        /// </summary>
        private static void LogResultAndCleanUp(string result)
        {
            string logFilePath = Properties.Settings.Default.LogFilePath;
            string logFilePrefix = "HAVM2_Result_";
            string logFileType = "json";
            
            // Clean up old files
            var dir = new DirectoryInfo(logFilePath);

            foreach (var file in dir.EnumerateFiles(logFilePrefix + "*." + logFileType))
            {
                file.Delete();
            }

            // Write JSON result to file
            string logFileName = logFilePrefix + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + logFileType;
            Helpers.LogfileWriter.writeToFile(logFilePath + @"\" + logFileName, result);
        }
    }
}
