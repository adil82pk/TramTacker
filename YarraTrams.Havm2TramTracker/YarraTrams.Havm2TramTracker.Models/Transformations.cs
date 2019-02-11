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
        public static string GetRunNumberShort(HavmTrip trip)
        {
            string block = trip.Block.Trim().ToUpper();

            //Todo: Make this configurable.
            var blockMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "cw", "v" }
            };

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

        public static string GetRunNumberLong(HavmTrip trip)
        {
            return trip.Block.ToLower();
        }

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

        public static bool GetLowFloor(HavmTrip trip)
        {
            var vehicleGroupsWithLowFloor = Properties.Settings.Default.VehicleGroupsWithLowFloor;
            var vehicleGroupsWithoutLowFloor = Properties.Settings.Default.VehicleGroupsWithoutLowFloor;
            //Todo: Change vehicle type to vehicle group on Trip.
            
            // There is no case insensitive comparer option for a StringCollection, therefore we must assume that all the values listed in the config file at lower case.
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

        public static decimal GetTripDistance(HavmTrip trip)
        {
            return (decimal)trip.DistanceMetres / (decimal)1000m;
        }

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

        public static string GetArrivalTime(HavmTripStop tripStop)
        {
            // ArrivalTime is a left-aligned fixed-length string of 8 characters.
            // The hh:mm portion MUST be five characters long, even when we have a single-digit hour - a single digit hour gets padded with a space on the left. 

            string arrivalTime = tripStop.PassingTime.ToString(@"h\:mm");

            if (tripStop.PassingTime.Hours <= 9)
            {
                arrivalTime = " " + arrivalTime;
            }

            return arrivalTime.PadRight(8);
        }
    }
}
