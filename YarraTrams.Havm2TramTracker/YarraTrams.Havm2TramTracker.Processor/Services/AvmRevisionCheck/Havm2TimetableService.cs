using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Processor.Helpers;
using YarraTrams.Havm2TramTracker.Models;
using Newtonsoft.Json;

namespace YarraTrams.Havm2TramTracker.Processor.Services
{
    public class Havm2TimetableService
    {
        /// <summary>
        /// Connects to the HAVM2 API and checks which timetable revision is due to run tomorrow.
        /// </summary>
        /// <returns>The export timestamp of the revision, as an integer.</returns>
        public int GetTomorrowsLatestHavm2TimetableRevision()
        {
            DateTime tomorrow = DateTime.Now.Date.AddDays(1);

            // Get the timetable from HAVM2 - there should only ever be a single one returned in the list.
            List<HavmTimetable> timetables = GetTimetables(tomorrow, tomorrow);

            return GetTimestampFromTimetables(timetables);
        }

        /// <summary>
        /// Returns a list of timetables from HAVM2.
        /// </summary>
        /// <returns>A structured list of timetables.</returns>
        public List<HavmTimetable> GetTimetables(DateTime startDate, DateTime endDate)
        {
            var result = Task.Run(() => {
                return ApiHttpClient.GetTimetablesFromHavm2(startDate, endDate);
            }).Result;

            List<HavmTimetable> timetables = CopyJsonToTimetables(result);

            return timetables;
        }

        /// <summary>
        /// Converts the HAVM2 response to an object.
        /// </summary>
        /// <param name="jsonString">The JSON returned from the HAVM2 API.</param>
        /// <returns>>A structured list of timetables.</returns>
        public List<HavmTimetable> CopyJsonToTimetables(string jsonString)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Ignore; //Ignore any extra fields we find in the json

            List<HavmTimetable> timetables = JsonConvert.DeserializeObject<List<HavmTimetable>>(jsonString, settings);

            return timetables;
        }

        /// <summary>
        /// Extracts tomorrow's timetable export timestamp from the HAVM2 data.
        /// </summary>
        /// <param name="timetables">A structured list of timetables - we expect there to be only a single timetable in this list, otherise an exception is thrown.</param>
        /// <returns>The export timestamp, as an integer.</returns>
        public int GetTimestampFromTimetables(List<HavmTimetable> timetables)
        {
            if (timetables.Count == 1)
            {
                return timetables.First().ExportTimestamp;
            }
            else
            {
                throw new ArgumentException($"Expecting HAVM2 to return 1 timetable but found {timetables.Count}.");
            }
        }
    }
}
