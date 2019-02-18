using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models
{
    public class TramTrackerBase
    {
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
                throw new FormatException(string.Format("Unexpected format for headboard on trip with HASTUS Id {0}. Expecting a number but got \"{1}\".", trip.HastusTripId, (trip.Headboard ?? "")));
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
                throw new FormatException(string.Format("Unknown vehicle \"{0}\".", trip.VehicleType)); ;
            }
        }
    }
}
