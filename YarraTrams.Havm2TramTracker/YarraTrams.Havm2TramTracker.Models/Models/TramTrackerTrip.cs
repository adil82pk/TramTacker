using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models
{
    class TramTrackerTrip
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
            this.FirstTime = (int)havmTrip.StartTime.TotalSeconds;
            this.EndTP = havmTrip.EndTimepoint;
            this.EndTime = (int)havmTrip.EndTime.TotalSeconds - 1;//Todo: Investigate this. Why is TT making all trips end 1 second early?
            this.AtLayoverTime = this.GetAtLayovertime(havmTrip);
            this.NextRouteNo = this.GetNextRouteNo(havmTrip);
            this.UpDirection = this.GetUpDirection(havmTrip);
            this.LowFloor = this.GetLowFloor(havmTrip);
            this.TripDistance = this.GetTripDistance(havmTrip);
            this.PublicTrip = havmTrip.IsPublic; //Todo: Confirm whether we bother filtering non public trips or we trust HAVM2.
            this.DayOfWeek = this.GetDayOfWeek(havmTrip);
        }

        /// <summary>
        /// Runs (also known as Blocks) are a series of contiguous trips performed by a vehicle, usually this series begins and ends at a depot.
        /// Run Numbers can be in long form () and short form (). This routine returns it in short form.
        /// In short form we use the first letter of the depot identifier followed by the depot-sepcific sequence for the block/run.
        /// Yarra Trams have defined a mapping for depots that have an identical first letter to another depot. This mapping must be defined in the [insert setting here] config setting.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        private string GetRunNumberShortForm(HavmTrip trip)
        {
            string block = trip.Block.Trim().ToUpper();


            var blockMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            StringCollection depotFirstChacterMapping = Properties.Settings.Default.DepotFirstChacterMapping;
            foreach (string s in depotFirstChacterMapping)
            {
                string[] pair = s.Split(new char[] { ',' });
                blockMapping.Add(pair[0].ToLower(), pair[1].ToLower());
            }

            string firstChar;
            string firstTwoCharsOfBlock = block.Substring(0, 2);
            if (blockMapping.ContainsKey(firstTwoCharsOfBlock.ToLower()))
            {
                firstChar = blockMapping[firstTwoCharsOfBlock.ToLower()];
            }
            else
            {
                firstChar = firstTwoCharsOfBlock.Substring(0, 1);
            }

            string trailingChars = block.Substring(block.Length - 3, 3).Trim();

            return firstChar.ToUpper() + "-" + trailingChars;
        }

        /// <summary>
        /// There is no consistent definition for Route inside TramTRACKER. Sometimes it is analogous to HAVM2 Route, sometime to HAVM2 Headboard.
        /// This routine assumes Route is analogous to Headboard in HAVM2.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        private short GetRouteNumberUsingHeadboard(HavmTrip trip)
        {
            if (short.TryParse(trip.Headboard, out short route))
            {
                return route;
            }
            else
            {
                throw new FormatException($"Unexpected format for headboard on trip with HASTUS Id {trip.HastusTripId}. Expecting a number but got \"{(trip.Headboard ?? "")}\".");
            }
        }

        /// <summary>
        /// Layover time (the time a vehicle spends at its final timpoint prior to embarking on its next trip) is measured in seconds by HAVM2.
        /// In TramTRACKER the layover time is measured in minutes.
        /// This routine converts the seconds to minutes and does some validation as it goes.
        /// This routine also rounds to the nearest minute, if required, however at the moment all HAVM2 values are recorded as a round minute (always a multiple of 60).
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        private short GetAtLayovertime(HavmTrip trip)
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
        private short GetNextRouteNo(HavmTrip trip)
        {
            if (short.TryParse(trip.NextRoute, out short nextRoute))
            {
                return nextRoute;
            }
            else
            {
                throw new FormatException($"Unexpected format for next route number on trip with HASTUS Id {trip.HastusTripId}. Expecting a number but got \"{(trip.NextRoute ?? "")}\".");
            }
        }

        /// <summary>
        /// The designation for up/down direction comes from HAVM2 as a string (either "UP" or "DOWN").
        /// TramTRACKER expects the up/down direction to be defined as true/false (up = true, down = false).
        /// THis routine converts the string designation to a boolean.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        private bool GetUpDirection(HavmTrip trip)
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
                    throw new FormatException($"Unexpected trip direction on trip with HASTUS Id {trip.HastusTripId}. Expecting \"UP\" or \"DOWN\" but got \"{trip.Direction}\".");
                }
            }
            else
            {
                throw new FormatException($"Unexpected trip direction on trip with HASTUS Id {trip.HastusTripId}. Expecting \"UP\" or \"DOWN\" but got null.");
            }
        }

        /// <summary>
        /// This routine checks if the passed-in vehicle has a low floor (i.e. it is an accessible tram).
        /// The routine is reliant on the vehicle group being present in the application config (in either the VehicleGroupsWithLowFloor setting or the VehicleGroupsWithoutLowFloor setting).
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        private bool GetLowFloor(HavmTrip trip)
        {
            var vehicleGroupsWithLowFloor = Properties.Settings.Default.VehicleGroupsWithLowFloor;
            var vehicleGroupsWithoutLowFloor = Properties.Settings.Default.VehicleGroupsWithoutLowFloor;
            //Todo: Change vehicle type to vehicle group on Trip.

            // There is no case insensitive comparer option for a StringCollection, therefore we must assume that all the values listed in the config file are lower case.
            if (vehicleGroupsWithLowFloor.Contains(trip.VehicleType.ToLower()))
            {
                return true;
            }
            else if (vehicleGroupsWithoutLowFloor.Contains(trip.VehicleType.ToLower()))
            {
                return false;
            }
            else //We have an vehicle group that we're not aware of!
            {
                throw new FormatException($"Unknown vehicle \"{trip.VehicleType}\"."); ;
            }
        }

        /// <summary>
        /// Trip distance in HAVM is defined in metres.
        /// This routine converts the metres in to kilometres.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        private decimal GetTripDistance(HavmTrip trip)
        {
            return (decimal)trip.DistanceMetres / (decimal)1000m;
        }

        /// <summary>
        /// TramTracker links every trip to a day of the week.
        /// Sunday is defined as day 0, Monday as day 1...., Saturday as day 6.
        /// This routine returns the day of the week for the passed-in HAVM2 operational day.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        private byte GetDayOfWeek(HavmTrip trip)
        {
            switch (trip.OperationalDay.DayOfWeek)
            {
                //Todo: Check this
                //A Melbourne tram week begins on a Sunday
                case System.DayOfWeek.Saturday: return 6;
                case System.DayOfWeek.Sunday: return 0;
                case System.DayOfWeek.Monday: return 1;
                case System.DayOfWeek.Tuesday: return 2;
                case System.DayOfWeek.Wednesday: return 3;
                case System.DayOfWeek.Thursday: return 4;
                case System.DayOfWeek.Friday: return 5;
                default: throw new IndexOutOfRangeException("Unknown day of the week.");
            }
        }
    }
}
