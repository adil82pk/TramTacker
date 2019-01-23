using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models
{
    public static class Transformations
    {
           public static string GetRunNumber(HavmTrip trip)
        {
            //Todo: Make this configurable?
            string firstTwoCharsOfBlock = trip.Block.Trim().ToUpper().Substring(0, 2);
            if (firstTwoCharsOfBlock == "CW")
            {
                return "V";
            }
            else
            {
                return firstTwoCharsOfBlock.Substring(0, 1);
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
                //Todo: Log unknown vehicle warning
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
                //A Melbourne tram week begins on a Saturday
                case DayOfWeek.Saturday: return 1;
                case DayOfWeek.Sunday: return 2;
                case DayOfWeek.Monday: return 3;
                case DayOfWeek.Tuesday: return 4;
                case DayOfWeek.Wednesday: return 5;
                case DayOfWeek.Thursday: return 6;
                case DayOfWeek.Friday: return 7;
                default: throw new IndexOutOfRangeException("Unknown day of the week.");
            }
        }
    }
}
