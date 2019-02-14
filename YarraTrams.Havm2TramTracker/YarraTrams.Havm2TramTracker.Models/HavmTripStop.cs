using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models
{
    public class HavmTripStop
    {
        public TimeSpan PassingTime { get; set; }
        public string HastusStopId { get; set; }
        public bool IsMonitoredOPRReliability { get; set; }

        public string ToString(int hastusTripId)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine($"Stop HastusStopId: {HastusStopId} on HastusTripId:{hastusTripId}");
            output.AppendLine($"     PassingTime: {PassingTime}");
            output.AppendLine($"     IsMonitoredOPRReliability: {IsMonitoredOPRReliability}");

            return output.ToString();
        }
    }
}
