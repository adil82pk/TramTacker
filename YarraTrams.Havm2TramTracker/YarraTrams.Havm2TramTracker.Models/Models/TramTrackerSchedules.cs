﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models.Models
{
    class TramTrackerSchedules
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
            this.StopID = this.GetStopId(havmStop);
            this.Time = (int)havmStop.PassingTime.TotalSeconds;
        }

        /// <summary>
        /// Runs (also known as Blocks) are a series of contiguous trips performed by a vehicle, usually this series begins and ends at a depot.
        /// Run Numbers can be in long form () and short form (). This routine returns it in short form.
        /// In short form we use the first letter of the depot identifier followed by the depot-sepcific sequence for the block/run.
        /// Yarra Trams have defined a mapping for depots that have an identical first letter to another depot. This mapping must be defined in the [insert setting here] config setting.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public string GetRunNumberShortForm(HavmTrip trip)
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
        public short GetRouteNumberUsingHeadboard(HavmTrip trip)
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
        /// TramTracker links every trip to a day of the week.
        /// Sunday is defined as day 0, Monday as day 1...., Saturday as day 6.
        /// This routine returns the day of the week for the passed-in HAVM2 operational day.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public byte GetDayOfWeek(HavmTrip trip)
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
        /// <summary>
        /// This routine checks if the passed-in vehicle has a low floor (i.e. it is an accessible tram).
        /// The routine is reliant on the vehicle group being present in the application config (in either the VehicleGroupsWithLowFloor setting or the VehicleGroupsWithoutLowFloor setting).
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public bool GetLowFloor(HavmTrip trip)
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
        /// A StopId is a textual identifer for a defined tram stop. e.g. DD16Coll, U080Glen
        /// A StopNo is a numeric identifer for a stop, as defined by HASTUS.
        /// This routine converts a StopNo in to a StopId.
        /// It relies on mapping data defined in the TramTracker database.
        /// This mapping data must be loaded in to memory by calling HastusStopMapper.Populate() prior to calling this routine.
        /// </summary>
        /// <param name="tripStop"></param>
        /// <returns></returns>
        public string GetStopId(HavmTripStop tripStop)
        {
            if (int.TryParse(tripStop.HastusStopId, out var stopID))
            {
                if (HastusStopMapper.stops.ContainsKey(stopID))
                {
                    return HastusStopMapper.stops[(stopID)];
                }
            }
            throw new Exception($"Unable to find mapping for stop with Hastus Id of {tripStop.HastusStopId}. Has HastusStopMapper.Populate() been run? Is the DB table empty? Is this a new or invalid stop?");
        }
    }
}
