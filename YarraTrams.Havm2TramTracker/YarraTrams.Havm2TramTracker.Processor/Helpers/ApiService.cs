using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Processor.Helpers
{
    public class ApiService
    {
        /// <summary>
        /// Calls HAVM2 and returns trip and stop data for the next 7 days, starting from tomorrow.
        /// </summary>
        /// <param name="baseDate">ONLY USED FOR TESTING. Tells HAVM2 to retrieve data based on a custom date, instead of using today's date.</param>
        /// <returns>A JSON-formatted string.</returns>
        public static string GetDataFromHavm2(DateTime? baseDate)
        {
            var result = Task.Run(() => {
                return ApiHttpClient.GetDataFromHavm2(baseDate);
            }).Result;

            return result;
        }
    }
}
