using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models
{
    public static class Transformations
    {
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
                AtLayovertimeShort = (short)Math.Round(AtLayovertimeDec, MidpointRounding.AwayFromZero);

            return (AtLayovertimeShort);
        }

        public static bool GetUpDirection(HavmTrip trip)
        {
            if (!(trip.Direction == null))
            {
                if (trip.Direction.Trim().ToUpper() == "UP")
                    return true;
                else if (trip.Direction.Trim().ToUpper() == "DOWN")
                    return false;
                else
                    throw new FormatException($"Unexpected trip direction on trip with HASTUS Id {trip.HastusTripId}. Expecting \"UP\" or \"DOWN\" but got \"{trip.Direction}\".");
            }
            else throw new FormatException($"Unexpected trip direction on trip with HASTUS Id {trip.HastusTripId}. Expecting \"UP\" or \"DOWN\" but got null.");
        }
    }
}
