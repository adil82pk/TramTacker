using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models
{
    class TramTrackerSchedulesDetails
    {
        public string ArrivalTime { get; set; }
        public string StopID { get; set; }
        public string TripID { get; set; }
        public string RunNo { get; set; }
        public string OPRTimePoint { get; set; }

        /// <summary>
        /// Populate data from HavmTrip object
        /// </summary>
        public void FromHavmTripAndStop(HavmTrip havmTrip, HavmTripStop havmStop)
        {
            this.ArrivalTime = this.GetArrivalTime(havmStop);
            this.StopID = havmStop.HastusStopId;
            this.TripID = havmTrip.HastusTripId.ToString().PadLeft(11);
            this.RunNo = havmTrip.Block;

            //TODO: How does hastus know this value?
            this.OPRTimePoint = "0";
        }

        /// <summary>
        /// ArrivalTime is a left-aligned fixed-length string of 8 characters.
        /// The hh:mm portion MUST be five characters long, even when we have a single-digit hour - a single digit hour gets padded with a space on the left.
        /// </summary>
        /// <param name="tripStop"></param>
        /// <returns></returns>
        public string GetArrivalTime(HavmTripStop tripStop)
        {
            string arrivalTime = tripStop.PassingTime.ToString(@"h\:mm");

            if (tripStop.PassingTime.Hours <= 9)
            {
                arrivalTime = " " + arrivalTime;
            }

            return arrivalTime.PadRight(8);
        }
    }
}