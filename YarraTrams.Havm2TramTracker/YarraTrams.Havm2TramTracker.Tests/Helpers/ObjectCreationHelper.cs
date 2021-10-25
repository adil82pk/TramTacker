using System;
using YarraTrams.Havm2TramTracker.Models;

namespace YarraTrams.Havm2TramTracker.Tests.Helpers
{
    public static class ObjectCreationHelper
    {
        /// <summary>
        /// Create TramTrackerSchedules object with default values.
        /// </summary>
        /// <param name="tripId">Trip identity.</param>
        /// <param name="runNo">Run number.</param>
        /// <param name="routeNo">Route number.</param>
        /// <param name="dayOfWeek">Day of week.</param>
        /// <param name="operationalDay">Operation day.</param>
        /// <param name="lowFloor">Low floor.</param>
        /// <param name="publicTrip">Public trip.</param>
        /// <param name="oprTimePoint">OPR time point.</param>
        /// <param name="stopId">Stop identity.</param>
        /// <param name="time">Time.</param>
        /// <param name="passingDateTime">Passing date time.</param>
        /// <param name="UpDirection">Up direction</param>
        /// <returns>Object if TramTrackerSchedules</returns>
        public static TramTrackerSchedules CreateTramTrackerScheduleWithDefaults(int tripId = 10, string runNo = "", short routeNo = 86, byte dayOfWeek = 0, DateTime operationalDay = default(DateTime),
            bool lowFloor = false, bool publicTrip = true, bool oprTimePoint = false, string stopId = "StopA", int time = 0, DateTime passingDateTime = default(DateTime), bool UpDirection = true)
        {
            return new Models.TramTrackerSchedules
            {
                TripID = tripId,
                RunNo = runNo,
                RouteNo = routeNo,
                DayOfWeek = dayOfWeek,
                OperationalDay = operationalDay,
                LowFloor = lowFloor,
                PublicTrip = publicTrip,
                OPRTimePoint = oprTimePoint,
                StopID = stopId,
                Time = time,
                PassingDateTime = passingDateTime,
                UpDirection = UpDirection
            };
        }
    }
}
