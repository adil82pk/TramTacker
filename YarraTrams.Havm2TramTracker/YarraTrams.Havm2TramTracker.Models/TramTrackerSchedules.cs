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
            this.Time = havmStop.PassingTimeSam;
        }

        /// <summary>
        /// Return DataRow
        /// </summary>
        /// <returns></returns>
        public TramTrackerDataSet.T_Temp_SchedulesRow ToDataRow()
        {
            TramTrackerDataSet.T_Temp_SchedulesDataTable dataTable = new TramTrackerDataSet.T_Temp_SchedulesDataTable();
            TramTrackerDataSet.T_Temp_SchedulesRow row = dataTable.NewT_Temp_SchedulesRow();
            row.TripID = this.TripID;
            row.RunNo = this.RunNo;
            row.RouteNo = this.RouteNo;
            row.DayOfWeek = this.DayOfWeek;
            row.LowFloor = this.LowFloor;
            row.PublicTrip = this.PublicTrip;
            row.OPRTimePoint = this.OPRTimePoint;
            row.StopID = this.StopID;
            row.Time = this.Time;

            return row;
        }

        /// <summary>
        /// Returns contents of the class as a string
        /// </summary>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.AppendFormat("Schedules StopId: {0}{1}", StopID, Environment.NewLine);
            output.AppendFormat("  TripID: {0}{1}", TripID, Environment.NewLine);
            output.AppendFormat("  RunNo: {0}{1}", RunNo, Environment.NewLine);
            output.AppendFormat("  RouteNo: {0}{1}", RouteNo, Environment.NewLine);
            output.AppendFormat("  OPRTimePoint: {0}{1}", OPRTimePoint, Environment.NewLine);
            output.AppendFormat("  Time: {0}{1}", Time, Environment.NewLine);
            output.AppendFormat("  LowFloor: {0}{1}", LowFloor, Environment.NewLine);
            output.AppendFormat("  PublicTrip: {0}{1}", PublicTrip, Environment.NewLine);

            return output.ToString();
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
            int stopID;
            if (int.TryParse(tripStop.HastusStopId, out stopID))
            {
                if (stopMapping.ContainsKey(stopID))
                {
                    return stopMapping[(stopID)];
                }
                else
                {
                    return stopID.ToString().PadRight(8);
                }
            }
            throw new Exception(string.Format("Unable to find mapping for stop with Hastus Id of {0}. Is the DB table empty? Is this a new or invalid stop?", tripStop.HastusStopId));
        }
    }
}
