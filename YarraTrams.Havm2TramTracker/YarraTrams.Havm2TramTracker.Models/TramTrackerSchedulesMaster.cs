using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models
{
    public class TramTrackerSchedulesMaster
    {
        public string TramClass { get; set; }
        public string HeadboardNo { get; set; }
        public string RouteNo { get; set; }
        public string RunNo { get; set; }
        public string StartDate { get; set; }
        public string TripNo { get; set; }
        public string PublicTrip { get; set; }

        /// <summary>
        /// Populate data from HavmTrip object
        /// </summary>
        /// <param name="trip"></param>
        public void FromHavmTrip(HavmTrip trip)
        {
            this.TramClass = trip.VehicleType;
            this.HeadboardNo = trip.Headboard;
            this.RouteNo = this.GetRouteNumberUsingRoute(trip).ToString().PadLeft(5);
            this.RunNo = this.GetRunNumberLongForm(trip);
            this.StartDate = trip.OperationalDay.ToString("dd/MM/yyyy");
            this.TripNo = trip.HastusTripId.ToString().PadLeft(11);
            this.PublicTrip = trip.IsPublic ? "1" : "0";
        }

        /// <summary>
        /// Return DataRow
        /// </summary>
        /// <returns></returns>
        public TramTrackerDataSet.T_Temp_SchedulesMasterRow ToDataRow()
        {
            TramTrackerDataSet.T_Temp_SchedulesMasterDataTable masterTable = new TramTrackerDataSet.T_Temp_SchedulesMasterDataTable();
            TramTrackerDataSet.T_Temp_SchedulesMasterRow masterRow = masterTable.NewT_Temp_SchedulesMasterRow();
            masterRow.TramClass = this.TramClass;
            masterRow.HeadboardNo = this.HeadboardNo;
            masterRow.RouteNo = this.RouteNo;
            masterRow.RunNo = this.RunNo;
            masterRow.StartDate = this.StartDate;
            masterRow.TripNo = this.TripNo;
            masterRow.PublicTrip = this.PublicTrip;

            return masterRow;
        }

        /// <summary>
        /// Returns contents of the class as a string
        /// </summary>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.AppendFormat("Trip TripNo: {0}{1}", TripNo, Environment.NewLine);
            output.AppendFormat("     TramClass: {0}{1}", TramClass, Environment.NewLine);
            output.AppendFormat("     HeadboardNo: {0}{1}", HeadboardNo, Environment.NewLine);
            output.AppendFormat("     RouteNo: {0}{1}", RouteNo, Environment.NewLine);
            output.AppendFormat("     RunNo: {0}{1}", RunNo, Environment.NewLine);
            output.AppendFormat("     StartDate: {0}{1}", StartDate, Environment.NewLine);
            output.AppendFormat("     PublicTrip: {0:d}{1}", PublicTrip, Environment.NewLine);
            return output.ToString();
        }

        /// <summary>
        /// There is no consistent definition for Route inside TramTRACKER. Sometimes it is analogous to HAVM2 Route, sometime to HAVM2 Headboard.
        /// This routine assumes Route is analogous to Route in HAVM2.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public short GetRouteNumberUsingRoute(HavmTrip trip)
        {
            short route;
            if (short.TryParse(trip.Route, out route))
            {
                return route;
            }
            else
            {
                throw new FormatException(String.Format("Unexpected format for route on trip with HASTUS Id {0}. Expecting a number but got \"{1}\".", trip.HastusTripId, (trip.Headboard ?? "")));
            }
        }

        /// <summary>
        /// Runs (also known as Blocks) are a series of contiguous trips performed by a vehicle, usually this series begins and ends at a depot.
        /// Run Numbers can be in long form () and short form (). This routine returns it in long form.
        /// In long form we use the depot identifer followed by the depot-sepcific sequence for the block/run. This happens to be how HAVM2 sends the data across anyway.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public string GetRunNumberLongForm(HavmTrip trip)
        {
            return trip.Block.ToLower();
        }
    }
}
