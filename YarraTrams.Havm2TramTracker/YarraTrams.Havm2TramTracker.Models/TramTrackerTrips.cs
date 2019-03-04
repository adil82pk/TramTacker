using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models
{
    public class TramTrackerTrips : TramTrackerBase
    {
        public int TripID { get; set; }
        public string RunNo{ get; set; }
        public short RouteNo{ get; set; }
        public string FirstTP{ get; set; }
        public int FirstTime{ get; set; }
        public string EndTP{ get; set; }
        public int EndTime{ get; set; }
        public short AtLayoverTime{ get; set; }
        public short NextRouteNo{ get; set; }
        public bool UpDirection{ get; set; }
        public bool LowFloor{ get; set; }
        public decimal TripDistance{ get; set; }
        public bool PublicTrip{ get; set; }
        public byte DayOfWeek{ get; set; }

        /// <summary>
        /// Populate data from HavmTrip object
        /// </summary>
        public void FromHavmTrip(HavmTrip havmTrip)
        {
            this.TripID = havmTrip.HastusTripId;
            this.RunNo = this.GetRunNumberShortForm(havmTrip);
            this.RouteNo = this.GetRouteNumberUsingHeadboard(havmTrip);
            this.FirstTP = havmTrip.StartTimepoint;
            this.FirstTime = havmTrip.StartTimeSam;
            this.EndTP = havmTrip.EndTimepoint;
            this.EndTime = havmTrip.EndTimeSam - 1;
            this.AtLayoverTime = this.GetAtLayovertime(havmTrip);
            this.NextRouteNo = this.GetNextRouteNo(havmTrip);
            this.UpDirection = this.GetUpDirection(havmTrip);
            this.LowFloor = this.GetLowFloor(havmTrip);
            this.TripDistance = this.GetTripDistance(havmTrip);
            this.PublicTrip = havmTrip.IsPublic;
            this.DayOfWeek = this.GetDayOfWeek(havmTrip);
        }

        /// <summary>
        /// Return DataRow
        /// </summary>
        /// <returns></returns>
        public TramTrackerDataSet.T_Temp_TripsRow ToDataRow()
        {
            TramTrackerDataSet.T_Temp_TripsDataTable dataTable = new TramTrackerDataSet.T_Temp_TripsDataTable();
            TramTrackerDataSet.T_Temp_TripsRow row = dataTable.NewT_Temp_TripsRow();
            row.TripID = this.TripID;
            row.RunNo = this.RunNo;
            row.RouteNo = this.RouteNo;
            row.FirstTP = this.FirstTP;
            row.FirstTime = this.FirstTime;
            row.EndTP = this.EndTP;
            row.EndTime = this.EndTime;
            row.AtLayoverTime = this.AtLayoverTime;
            row.NextRouteNo = this.NextRouteNo;
            row.UpDirection = this.UpDirection;
            row.LowFloor = this.LowFloor;
            row.TripDistance = this.TripDistance;
            row.PublicTrip = this.PublicTrip;
            row.DayOfWeek = this.DayOfWeek;

            return row;
        }

        /// <summary>
        /// Returns contents of the class as a string
        /// </summary>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.AppendFormat("Trip TripID: {0}{1}", TripID, Environment.NewLine);
            output.AppendFormat("     RunNo: {0}{1}", RunNo, Environment.NewLine);
            output.AppendFormat("     RouteNo: {0}{1}", RouteNo,  Environment.NewLine);
            output.AppendFormat("     FirstTP: {0}{1}", FirstTP, Environment.NewLine);
            output.AppendFormat("     FirstTime: {0}{1}", FirstTime, Environment.NewLine);
            output.AppendFormat("     EndTP: {0}{1}", EndTP, Environment.NewLine);
            output.AppendFormat("     EndTime: {0}{1}", EndTime, Environment.NewLine);
            output.AppendFormat("     AtLayoverTime: {0}{1}", AtLayoverTime, Environment.NewLine);
            output.AppendFormat("     NextRouteNo: {0}{1}", NextRouteNo, Environment.NewLine);
            output.AppendFormat("     UpDirection: {0}{1}", UpDirection, Environment.NewLine);
            output.AppendFormat("     LowFloor: {0}{1}", LowFloor, Environment.NewLine);
            output.AppendFormat("     TripDistance: {0}{1}", TripDistance, Environment.NewLine);
            output.AppendFormat("     PublicTrip: {0}{1}", PublicTrip, Environment.NewLine);
            output.AppendFormat("     DayOfWeek: {0}{1}", DayOfWeek, Environment.NewLine);
            return output.ToString();
        }

        /// <summary>
        /// Layover time (the time a vehicle spends at its final timpoint prior to embarking on its next trip) is measured in seconds by HAVM2.
        /// In TramTRACKER the layover time is measured in minutes.
        /// This routine converts the seconds to minutes and does some validation as it goes.
        /// This routine also rounds to the nearest minute, if required, however at the moment all HAVM2 values are recorded as a round minute (always a multiple of 60).
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public short GetAtLayovertime(HavmTrip trip)
        {
            //We merely convert the seconds to minutes
            decimal AtLayovertimeDec = ((decimal)trip.HeadwayNextSeconds / 60);

            short AtLayovertimeShort;

            if (AtLayovertimeDec >= short.MaxValue)
            {
#if !DEBUG
                //Todo: Log warning, but not when unit testing
#endif
                AtLayovertimeShort = short.MaxValue;
            }
            else if (AtLayovertimeDec <= 0)
            {
#if !DEBUG
                //Todo: Log warning, but not when unit testing
#endif
                AtLayovertimeShort = 0;
            }
            else
            {
                AtLayovertimeShort = (short)Math.Round(AtLayovertimeDec, MidpointRounding.AwayFromZero);
            }

            return AtLayovertimeShort;
        }

        /// <summary>
        /// Route numbers arrive from HAVM2 as text but get saved to TramTRACKER as a number.
        /// This routine does the conversion and throws a friendly error if the conversion fails.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public short GetNextRouteNo(HavmTrip trip)
        {
            short nextRoute;
            if (short.TryParse(trip.NextRoute, out nextRoute))
            {
                return nextRoute;
            }
            else
            {
                throw new FormatException(String.Format("Unexpected format for next route number on trip with HASTUS Id {0}. Expecting a number but got \"{1}\".", trip.HastusTripId, (trip.NextRoute ?? "")));
            }
        }

        /// <summary>
        /// The designation for up/down direction comes from HAVM2 as a string (either "UP" or "DOWN").
        /// TramTRACKER expects the up/down direction to be defined as true/false (up = true, down = false).
        /// THis routine converts the string designation to a boolean.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public bool GetUpDirection(HavmTrip trip)
        {
            if (!(trip.Direction == null))
            {
                if (trip.Direction.Trim().ToUpper() == "UP")
                {
                    return true;
                }
                else if (trip.Direction.Trim().ToUpper() == "DOWN")
                {
                    return false;
                }
                else
                {
                    throw new FormatException(String.Format("Unexpected trip direction on trip with HASTUS Id {0}. Expecting \"UP\" or \"DOWN\" but got \"{1}\".", trip.HastusTripId, trip.Direction));
                }
            }
            else
            {
                throw new FormatException(String.Format("Unexpected trip direction on trip with HASTUS Id {0}. Expecting \"UP\" or \"DOWN\" but got null.", trip.HastusTripId));
            }
        }

        /// <summary>
        /// Trip distance in HAVM is defined in metres.
        /// This routine converts the metres in to kilometres.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public decimal GetTripDistance(HavmTrip trip)
        {
            return (decimal)trip.DistanceMetres / (decimal)1000m;
        }
    }
}
