using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models
{
    public class TramTrackerSchedules : TramTrackerBase
    {
        public int TripID { get; set; }
        public string RunNo { get; set; }
        public string StopID { get; set; }
        public short RouteNo { get; set; }
        public bool OPRTimePoint { get; set; }
        public int Time { get; set; }
        public byte DayOfWeek { get; set; }
        public bool LowFloor { get; set; }
        public bool PublicTrip { get; set; }

        /// <summary>
        /// Populate data from HavmTrip object
        /// </summary>
        public void FromHavmTripAndStop(HavmTrip havmTrip, HavmTripStop havmStop, Dictionary<int, string> stopMapping)
        {
            this.TripID = havmTrip.HastusTripId;
            this.RunNo = this.GetRunNumberShortForm(havmTrip);
            this.RouteNo = this.GetRouteNumberUsingHeadboard(havmTrip);
            this.DayOfWeek = this.GetDayOfWeek(havmTrip);
            this.LowFloor = this.GetLowFloor(havmTrip);
            this.PublicTrip = havmTrip.IsPublic;
            this.OPRTimePoint = havmStop.IsMonitoredOPRReliability;
            this.StopID = this.GetStopId(havmStop, stopMapping);
            this.Time = (int)havmStop.PassingTime.TotalSeconds;
        }

         /// <summary>
        /// A StopId is a textual identifer for a defined tram stop. e.g. DD16Coll, U080Glen
        /// A StopNo is a numeric identifer for a stop, as defined by HASTUS.
        /// This routine converts a StopNo in to a StopId.
        /// It relies on mapping data defined in the TramTracker database.
        /// This mapping data must be populated (via Processor/Helpers/HastisStopMpper) prior to calling this routine.
        /// </summary>
        /// <param name="tripStop"></param>
        /// <returns></returns>
        public string GetStopId(HavmTripStop tripStop, Dictionary<int, string> stopMapping)
        {
            if (int.TryParse(tripStop.HastusStopId, out var stopID))
            {
                if (stopMapping.ContainsKey(stopID))
                {
                    return stopMapping[(stopID)];
                }
            }
            throw new Exception($"Unable to find mapping for stop with Hastus Id of {tripStop.HastusStopId}. Is the DB table empty? Is this a new or invalid stop?");
        }
    }
}
