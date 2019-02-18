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
            output.AppendFormat("Stop HastusStopId: {0} on HastusTripId:{1}{2}", HastusStopId, hastusTripId, Environment.NewLine);
            output.AppendFormat("     PassingTime: {0}{1}", PassingTime, Environment.NewLine);
            output.AppendFormat("     IsMonitoredOPRReliability: {0}{1}", IsMonitoredOPRReliability, Environment.NewLine);

            return output.ToString();
        }
    }
}
