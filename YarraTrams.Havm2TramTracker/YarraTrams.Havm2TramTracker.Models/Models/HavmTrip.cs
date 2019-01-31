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
        public string DisplayCode { get; set; }
        public string StartTimepoint { get; set; }
        public TimeSpan StartTime { get; set; }
        public string EndTimepoint { get; set; }
        public TimeSpan EndTime { get; set; }
        public int HeadwayNextSeconds { get; set; }
        public string NextDisplayCode { get; set; }
        public string Direction { get; set; }
        public string VehicleType { get; set; }
        public int DistanceMetres { get; set; }
        public bool IsPublic { get; set; }
        public DateTime OperationalDay { get; set; }
        public List<Models.HavmTripStop> Stops { get; set; }
        //Todo: Create ToSQL that returns and string (and does transformations).

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine($"Trip HastusTripId: {HastusTripId}");
            output.AppendLine($"     Block: {Block}");
            output.AppendLine($"     Direction: {Direction}");
            output.AppendLine($"     DisplayCode: {DisplayCode}");
            output.AppendLine($"     DistanceMetres: {DistanceMetres:d}");
            output.AppendLine($"     NextDisplayCode: {NextDisplayCode}");
            output.AppendLine($"     StartTime: {StartTime:c}");
            output.AppendLine($"     StartTimepoint: {StartTimepoint}");
            output.AppendLine($"     EndTime: {EndTime:c}");
            output.AppendLine($"     EndTimepoint: {EndTimepoint}");
            output.AppendLine($"     VehicleType: {VehicleType}");
            if (Stops != null)
            {
                output.AppendLine($"     Stops: {Stops.Count:d}");
                int stopNum = 1;
                foreach (var stop in Stops)
                {
                    output.AppendLine($"        Stop {stopNum:d}: {stop.HastusStopId} arriving at {stop.PassingTime:c}");
                    stopNum++;
                }
            } else
            {
                output.AppendLine($"     Stops: 0");
            }
            return output.ToString();
        }
    }

    
}
