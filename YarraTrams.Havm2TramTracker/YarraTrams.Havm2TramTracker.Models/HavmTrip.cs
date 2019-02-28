using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models
{
    public class HavmTrip
    {
        public int HastusTripId { get; set; }
        public string Block { get; set; }
        public string Headboard { get; set; }
        public string Route { get; set; }
        public string StartTimepoint { get; set; }
        public int StartTimeSam { get; set; }
        public string EndTimepoint { get; set; }
        public int EndTimeSam { get; set; }
        public int HeadwayNextSeconds { get; set; }
        public string NextRoute { get; set; }
        public string Direction { get; set; }
        public string VehicleType { get; set; }
        public int DistanceMetres { get; set; }
        public bool IsPublic { get; set; }
        public DateTime OperationalDay { get; set; }
        public List<HavmTripStop> Stops { get; set; }
        //Todo: Create ToSQL that returns and string (and does transformations).

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.AppendFormat("Trip HastusTripId: {0}{1}", HastusTripId, Environment.NewLine);
            output.AppendFormat("     Day: {0}{1}", OperationalDay, Environment.NewLine);
            output.AppendFormat("     Block: {0}{1}", Block, Environment.NewLine);
            output.AppendFormat("     Direction: {0}{1}", Direction, Environment.NewLine);
            output.AppendFormat("     Headboard: {0}{1}", Headboard, Environment.NewLine);
            output.AppendFormat("     Route: {0}{1}", Route, Environment.NewLine);
            output.AppendFormat("     DistanceMetres: {0:d}{1}", DistanceMetres, Environment.NewLine);
            output.AppendFormat("     NextRoute: {0}{1}", NextRoute, Environment.NewLine);
            output.AppendFormat("     StartTime (SAM): {0}{1}", StartTimeSam, Environment.NewLine);
            output.AppendFormat("     StartTimepoint: {0}{1}", StartTimepoint, Environment.NewLine);
            output.AppendFormat("     EndTime (SAM): {0}{1}", EndTimeSam, Environment.NewLine);
            output.AppendFormat("     EndTimepoint: {0}{1}", EndTimepoint, Environment.NewLine);
            output.AppendFormat("     VehicleType: {0}{1}", VehicleType, Environment.NewLine);
            if (Stops != null)
            {
                output.AppendFormat("     Stops: {0:d}{1}", Stops.Count, Environment.NewLine);
                int stopNum = 1;
                foreach (var stop in Stops)
                {
                    output.AppendFormat("        Stop {0:d}: {1} arriving at {2:c}{3}", stopNum, stop.HastusStopId, stop.PassingTime, Environment.NewLine);
                    stopNum++;
                }
            } else
            {
                output.AppendLine("     Stops: 0");
            }
            return output.ToString();
        }
    }

    
}
