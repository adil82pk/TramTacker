using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models
{
    public class TramTrackerSchedulesDetails
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
            this.OPRTimePoint = havmStop.IsMonitoredOPRReliability ? "1" : "0";
        }

        /// <summary>
        /// Return DataRow
        /// </summary>
        /// <returns></returns>
        public TramTrackerDataSet.T_Temp_SchedulesDetailsRow ToDataRow()
        {
            TramTrackerDataSet.T_Temp_SchedulesDetailsDataTable detailsTable = new TramTrackerDataSet.T_Temp_SchedulesDetailsDataTable();
            TramTrackerDataSet.T_Temp_SchedulesDetailsRow detailsRow = detailsTable.NewT_Temp_SchedulesDetailsRow();
            detailsRow.ArrivalTime = this.ArrivalTime;
            detailsRow.StopID = this.StopID;
            detailsRow.TripID = this.TripID;
            detailsRow.RunNo = this.RunNo;
            detailsRow.OPRTimePoint = this.OPRTimePoint;

            return detailsRow;
        }

        /// <summary>
        /// Returns contents of the class as a string
        /// </summary>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.AppendFormat("Details StopId: {0}{1}", StopID, Environment.NewLine);
            output.AppendFormat("  ArrivalTime: {0}{1}", ArrivalTime, Environment.NewLine);
            output.AppendFormat("  RunNo: {0}{1}", RunNo, Environment.NewLine);
            output.AppendFormat("  OPRTimePoint: {0}{1}", OPRTimePoint, Environment.NewLine);

            return output.ToString();
        }

        /// <summary>
        /// ArrivalTime is a left-aligned fixed-length string of 8 characters, hh:mm format.
        /// A single digit minute gets padded with a zero on the left, a single digit hour with a space.
        /// </summary>
        public string GetArrivalTime(HavmTripStop tripStop)
        {
            TimeSpan passingTime = new TimeSpan(0, 0, tripStop.PassingTimeSam);
            string arrivalTime = ((int)passingTime.TotalHours).ToString().PadLeft(2) + ":" + passingTime.Minutes.ToString().PadLeft(2, '0');

            return arrivalTime.PadRight(8);
        }
    }
}