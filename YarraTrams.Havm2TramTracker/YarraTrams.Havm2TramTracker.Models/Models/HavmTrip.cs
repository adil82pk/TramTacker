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
    }
}
