using System;
using System.Text;
using Newtonsoft.Json;

namespace YarraTrams.Havm2TramTracker.Models
{
    public class HavmTimetable
    {
        [JsonProperty(Required = Required.Always)]
        public DateTime Date { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int ExportTimestamp { get; set; }

        public bool IsPtvApproved { get; set; }

        public DateTime? PtvApprovedDate { get; set; }

        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.AppendFormat("Date: {0:yyyy-MM-dd}{1}", Date, Environment.NewLine);
            output.AppendFormat("ExportTimestamp: {0}{1}", ExportTimestamp, Environment.NewLine);
            output.AppendFormat("IsPtvApproved: {0}{1}", IsPtvApproved, Environment.NewLine);
            output.AppendFormat("PtvApprovedDate: {0:yyyy-MM-dd hh:mm:ss}{1}", PtvApprovedDate, Environment.NewLine);
            output.AppendFormat("Id: {0}{1}", Id, Environment.NewLine);
            output.AppendFormat("CreatedDate: {0:yyyy-MM-dd hh:mm:ss}{1}", CreatedDate, Environment.NewLine);
            output.AppendFormat("UpdatedDate: {0:yyyy-MM-dd hh:mm:ss}{1}", UpdatedDate, Environment.NewLine);

            return output.ToString();
        }
    }
}
