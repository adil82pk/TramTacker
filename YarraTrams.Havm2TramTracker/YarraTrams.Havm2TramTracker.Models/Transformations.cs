using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Logger;

namespace YarraTrams.Havm2TramTracker.Models
{
    public static class Transformations
    {
        public static string GetRunNumber(HavmTrip trip)
        {
            //Todo: refactor this at DB level (split fields)
            string block = trip.Block.Trim().ToUpper();

            //Todo: Make this configurable?
            //Todo: Confirm rule with John.
            string firstChar;
            string firstTwoCharsOfBlock = block.Substring(0, 2);
            if (firstTwoCharsOfBlock == "CW")
            {
                firstChar = "V";
            }
            else
            {
                firstChar = firstTwoCharsOfBlock.Substring(0, 1);
            }

            string trailingChars = block.Substring(block.Length - 3, 3).Trim();

            return firstChar + "-" + trailingChars;
        }

        public static short GetRouteNo(HavmTrip trip)
        {
            if (short.TryParse(trip.DisplayCode, out short route))
            {
                return route;
            }
            else
            {
                throw new FormatException($"Unexpected format for route number on trip with HASTUS Id {trip.HastusTripId}. Expecting a number but got \"{(trip.DisplayCode ?? "")}\".");
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
            if (short.TryParse(trip.NextDisplayCode, out short nextRoute))
            {
                return nextRoute;
            }
            else
            {
                throw new FormatException($"Unexpected format for next route number on trip with HASTUS Id {trip.HastusTripId}. Expecting a number but got \"{(trip.NextDisplayCode ?? "")}\".");
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
            //Todo: Make sure comparison is case-insensitive
            if (vehicleGroupsWithLowFloor.Contains(trip.VehicleType))
            {
                return true;
            }
            else if (vehicleGroupsWithoutLowFloor.Contains(trip.VehicleType))
            {
                return false;
            }
            else //We have an vehicle group that we're not aware of!
            {
//#if !DEBUG
                LogWriter.Instance.LogWithoutDelay(EventLogCodes.UNKNOWN_VEHICLE_ENCOUNTERED
                    , $"Unknown vehicle \"{trip.VehicleType}\"."
                    , trip.ToString());
//#endif
                return false;
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
    }
}
