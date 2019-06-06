﻿using System;
using System.Collections.Generic;
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
        /// Calls HAVM2 and returns trip and stop data for the next 7 days, starting from tomorrow.
        /// </summary>
        /// <param name="baseDate">ONLY USED FOR TESTING. Tells HAVM2 to retrieve data based on a custom date, instead of using today's date.</param>
        /// <returns>A JSON-formatted string.</returns>
        public static string GetDataFromHavm2(DateTime? baseDate, int retryCount = 0)
        {
            try
            {
                var result = Task.Run(() => {
                    return ApiHttpClient.GetDataFromHavm2(baseDate);
                }).Result;

                return result;
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.HAVM2_API_ERROR
                            , ex.Message + ((retryCount > 0) ? string.Format("\nRetry count = {0}", retryCount) : ""));

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
    }
}
