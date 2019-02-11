using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Logger;
using YarraTrams.Havm2TramTracker.Models;

namespace YarraTrams.Havm2TramTracker.Models
{
    public static class Transformations
    {
        /// <summary>
        /// Runs (also known as Blocks) are a series of contiguous trips performed by a vehicle, usually this series begins and ends at a depot.
        /// Run Numbers can be in long form () and short form (). This routine returns it in short form.
        /// In short form we use the first letter of the depot identifier followed by the depot-sepcific sequence for the block/run.
        /// Yarra Trams have defined a mapping for depots that have an identical first letter to another depot. This mapping must be defined in the [insert setting here] config setting.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public static string GetRunNumberShortForm(HavmTrip trip)
        {
            string block = trip.Block.Trim().ToUpper();

            
            var blockMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            StringCollection vehicleGroupsWithLowFloor = Properties.Settings.Default.DepotFirstChacterMapping;
            foreach(string s in vehicleGroupsWithLowFloor)
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
        /// Runs (also known as Blocks) are a series of contiguous trips performed by a vehicle, usually this series begins and ends at a depot.
        /// Run Numbers can be in long form () and short form (). This routine returns it in long form.
        /// In long form we use the depot identifer followed by the depot-sepcific sequence for the block/run. This happens to be how HAVM2 sends the data across anyway.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public static string GetRunNumberLongForm(HavmTrip trip)
        {
            return trip.Block.ToLower();
        }
        
        /// <summary>
        /// There is no consistent definition for Route inside TramTRACKER. Sometimes it is analogous to HAVM2 Route, sometime to HAVM2 Headboard.
        /// This routine assumes Route is analogous to Headboard in HAVM2.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public static short GetRouteNoUsingHeadboard(HavmTrip trip)
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
        /// There is no consistent definition for Route inside TramTRACKER. Sometimes it is analogous to HAVM2 Route, sometime to HAVM2 Headboard.
        /// This routine assumes Route is analogous to Route in HAVM2.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        public static short GetRouteNoUsingRoute(HavmTrip trip)
        {
            if (short.TryParse(trip.Route, out short route))
            {
                return route;
            }
            else
            {
                throw new FormatException($"Unexpected format for route on trip with HASTUS Id {trip.HastusTripId}. Expecting a number but got \"{(trip.Headboard ?? "")}\".");
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
        public static short GetAtLayovertime(HavmTrip trip)
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
        public static short GetNextRouteNo(HavmTrip trip)
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
        public static bool GetUpDirection(HavmTrip trip)
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
        public static bool GetLowFloor(HavmTrip trip)
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
        public static decimal GetTripDistance(HavmTrip trip)
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
        public static byte GetDayOfWeek(HavmTrip trip)
        {
            switch (trip.OperationalDay.DayOfWeek)
            {
                //Todo: Check this
                //A Melbourne tram week begins on a Sunday
                case DayOfWeek.Saturday: return 6;
                case DayOfWeek.Sunday: return 0;
                case DayOfWeek.Monday: return 1;
                case DayOfWeek.Tuesday: return 2;
                case DayOfWeek.Wednesday: return 3;
                case DayOfWeek.Thursday: return 4;
                case DayOfWeek.Friday: return 5;
                default: throw new IndexOutOfRangeException("Unknown day of the week.");
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
        public static string GetStopId(HavmTripStop tripStop)
        {
            if (int.TryParse(tripStop.HastusStopId, out var stopID))
            {
                if (Models.HastusStopMapper.stops.ContainsKey(stopID))
                {
                    return Models.HastusStopMapper.stops[(stopID)];
                }
            }
            throw new Exception($"Unable to find mapping for stop with Hastus Id of {tripStop.HastusStopId}. Has HastusStopMapper.Populate() been run? Is the DB table empty? Is this a new or invalid stop?");
        }

        /// <summary>
        /// ArrivalTime is a left-aligned fixed-length string of 8 characters.
        /// The hh:mm portion MUST be five characters long, even when we have a single-digit hour - a single digit hour gets padded with a space on the left.
        /// </summary>
        /// <param name="tripStop"></param>
        /// <returns></returns>
        public static string GetArrivalTime(HavmTripStop tripStop)
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
