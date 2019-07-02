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
        public int HavmTripId { get; set; }
        public int HavmTimetableId { get; set; }
        public int HastusPermanentTripNumber { get; set; }
        public string RunNo { get; set; }
        public int RunSequenceNumber { get; set; }
        public short RouteNo { get; set; }
        public string FirstTP { get; set; }
        public int FirstTime { get; set; }
        public string EndTP { get; set; }
        public int EndTime { get; set; }
        public int AtLayoverTimePrevious { get; set; }
        public short AtLayoverTime { get; set; }
        public short NextRouteNo { get; set; }
        public bool UpDirection { get; set; }
        public bool LowFloor { get; set; }
        public decimal TripDistance { get; set; }
        public bool PublicTrip { get; set; }
        public byte DayOfWeek { get; set; }
        public DateTime OperationalDay { get; set; }

        /// <summary>
        /// Populate data from HavmTrip object
        /// </summary>
        public void FromHavmTrip(HavmTrip havmTrip)
        {
            this.TripID = havmTrip.HastusTripId;
            this.HavmTripId = havmTrip.HavmTripId;
            this.HavmTimetableId = havmTrip.HavmTimetableId;
            this.HastusPermanentTripNumber = havmTrip.HastusPermanentTripNumber;
            this.RunNo = this.GetRunNumberShortForm(havmTrip);
            this.RunSequenceNumber = havmTrip.RunSequenceNumber;
            this.RouteNo = this.GetRouteNumberUsingHeadboard(havmTrip);
            this.FirstTP = havmTrip.StartTimepoint;
            this.FirstTime = havmTrip.StartTimeSam;
            this.EndTP = havmTrip.EndTimepoint;
            this.EndTime = havmTrip.EndTimeSam - 1;
            this.AtLayoverTimePrevious = this.GetAtLayoverTimePrevious(havmTrip);
            this.AtLayoverTime = this.GetAtLayovertime(havmTrip);
            this.NextRouteNo = this.GetNextRouteNo(havmTrip);
            this.UpDirection = this.GetUpDirection(havmTrip);
            this.LowFloor = this.GetLowFloor(havmTrip);
            this.TripDistance = this.GetTripDistance(havmTrip);
            this.PublicTrip = havmTrip.IsPublic;
            this.DayOfWeek = this.GetDayOfWeek(havmTrip);
            this.OperationalDay = havmTrip.OperationalDay;
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
            row.HavmTripId = this.HavmTripId;
            row.HavmTimetableId = this.HavmTimetableId;
            row.HastusPermanentTripNumber = this.HastusPermanentTripNumber;
            row.RunNo = this.RunNo;
            row.RunSequenceNumber = this.RunSequenceNumber;
            row.RouteNo = this.RouteNo;
            row.FirstTP = this.FirstTP;
            row.FirstTime = this.FirstTime;
            row.EndTP = this.EndTP;
            row.EndTime = this.EndTime;
            row.AtLayoverTimePrevious = this.AtLayoverTimePrevious;
            row.AtLayoverTime = this.AtLayoverTime;
            row.NextRouteNo = this.NextRouteNo;
            row.UpDirection = this.UpDirection;
            row.LowFloor = this.LowFloor;
            row.TripDistance = this.TripDistance;
            row.PublicTrip = this.PublicTrip;
            row.DayOfWeek = this.DayOfWeek;
            row.OperationalDay = this.OperationalDay;

            return row;
        }

        /// <summary>
        /// Returns contents of the class as a string
        /// </summary>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.AppendFormat("Trip TripID: {0}{1}", TripID, Environment.NewLine);
            output.AppendFormat("     HavmTripId: {0}{1}", HavmTripId, Environment.NewLine);
            output.AppendFormat("     HavmTimetableId: {0}{1}", HavmTimetableId, Environment.NewLine);
            output.AppendFormat("     HastusPermanentTripNumber: {0}{1}", HastusPermanentTripNumber, Environment.NewLine);
            output.AppendFormat("     RunNo: {0}{1}", RunNo, Environment.NewLine);
            output.AppendFormat("     RunSequenceNumber: {0}{1}", RunSequenceNumber, Environment.NewLine);
            output.AppendFormat("     RouteNo: {0}{1}", RouteNo,  Environment.NewLine);
            output.AppendFormat("     FirstTP: {0}{1}", FirstTP, Environment.NewLine);
            output.AppendFormat("     FirstTime: {0}{1}", FirstTime, Environment.NewLine);
            output.AppendFormat("     EndTP: {0}{1}", EndTP, Environment.NewLine);
            output.AppendFormat("     EndTime: {0}{1}", EndTime, Environment.NewLine);
            output.AppendFormat("     AtLayoverTimePrevious: {0}{1}", AtLayoverTimePrevious, Environment.NewLine);
            output.AppendFormat("     AtLayoverTime: {0}{1}", AtLayoverTime, Environment.NewLine);
            output.AppendFormat("     NextRouteNo: {0}{1}", NextRouteNo, Environment.NewLine);
            output.AppendFormat("     UpDirection: {0}{1}", UpDirection, Environment.NewLine);
            output.AppendFormat("     LowFloor: {0}{1}", LowFloor, Environment.NewLine);
            output.AppendFormat("     TripDistance: {0}{1}", TripDistance, Environment.NewLine);
            output.AppendFormat("     PublicTrip: {0}{1}", PublicTrip, Environment.NewLine);
            output.AppendFormat("     DayOfWeek: {0}{1}", DayOfWeek, Environment.NewLine);
            output.AppendFormat("     OperationalDay: {0}{1}", OperationalDay.ToShortDateString(), Environment.NewLine);
            return output.ToString();
        }

        /// <summary>
        /// Gets the AtLayover value for this trip - the time it plans to wait once it completes it.
        /// </summary>
        /// <param name="trip"></param>
        public short GetAtLayovertime(HavmTrip trip)
        {
            return GetLayovertime(trip.HeadwayNextSeconds);
        }

        /// <summary>
        /// Gets the AtLayoverPrevious value for this trip - the time it plans to wait prior to starting it.
        /// </summary>
        /// <param name="trip"></param>
        public short GetAtLayoverTimePrevious(HavmTrip trip)
        {
            return GetLayovertime(trip.HeadwayPreviousSeconds);
        }

        /// <summary>
        /// Layover time (the time a vehicle spends at its final timpoint prior to embarking on its next trip) is measured in seconds by HAVM2.
        /// In TramTRACKER the layover time is measured in minutes.
        /// This routine converts the seconds to minutes and does some validation as it goes.
        /// This routine also rounds to the nearest minute, if required, however at the moment all HAVM2 values are recorded as a round minute (always a multiple of 60).
        /// </summary>
        public short GetLayovertime(int havmLayoverValue)
        {
            //We merely convert the seconds to minutes
            decimal layoverTimeDec = ((decimal)havmLayoverValue / 60);

            short layoverTimeShort;

            if (layoverTimeDec >= short.MaxValue)
            {
                layoverTimeShort = short.MaxValue;
            }
            else if (layoverTimeDec <= 0)
            {
                layoverTimeShort = 0;
            }
            else
            {
                layoverTimeShort = (short)Math.Round(layoverTimeDec, MidpointRounding.AwayFromZero);
            }

            return layoverTimeShort;
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
