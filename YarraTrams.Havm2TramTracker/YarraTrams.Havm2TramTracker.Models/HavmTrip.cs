using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YarraTrams.Havm2TramTracker.Models
{
    public class HavmTrip
    {
        [JsonProperty(Required = Required.Always)]
        public int HastusTripId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int HavmTripId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int HavmTimetableId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int HastusPermanentTripNumber { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Block { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int RunSequenceNumber { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Headboard { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Route { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string StartTimepoint { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int StartTimeSam { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string EndTimepoint { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int EndTimeSam { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int HeadwayPreviousSeconds { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int HeadwayNextSeconds { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string NextRoute { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Direction { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string VehicleType { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int DistanceMetres { get; set; }

        [JsonProperty(Required = Required.Always)]
        public bool IsPublic { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DateTime OperationalDay { get; set; }

        public List<HavmTripStop> Stops { get; set; }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.AppendFormat("Trip HastusTripId: {0}{1}", HastusTripId, Environment.NewLine);
            output.AppendFormat("     HavmTripId: {0}{1}", HavmTripId, Environment.NewLine);
            output.AppendFormat("     HavmTimetableId: {0}{1}", HavmTimetableId, Environment.NewLine);
            output.AppendFormat("     HastusPermanentTripNumber: {0}{1}", HastusPermanentTripNumber, Environment.NewLine);
            output.AppendFormat("     Day: {0}{1}", OperationalDay, Environment.NewLine);
            output.AppendFormat("     Block: {0}{1}", Block, Environment.NewLine);
            output.AppendFormat("     RunSequenceNumber: {0}{1}", RunSequenceNumber, Environment.NewLine);
            output.AppendFormat("     Direction: {0}{1}", Direction, Environment.NewLine);
            output.AppendFormat("     Headboard: {0}{1}", Headboard, Environment.NewLine);
            output.AppendFormat("     Route: {0}{1}", Route, Environment.NewLine);
            output.AppendFormat("     HeadwayPreviousSeconds: {0}{1}", Route, Environment.NewLine);
            output.AppendFormat("     HeadwayNextSeconds: {0}{1}", Route, Environment.NewLine);
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
                    TimeSpan passingTime = new TimeSpan(0, 0, stop.PassingTimeSam);
                    output.AppendFormat("        Stop {0:d}: {1} arriving at {2:c} ({3} SaM){4}", stopNum, stop.HastusStopId, passingTime, stop.PassingTimeSam, Environment.NewLine);
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
