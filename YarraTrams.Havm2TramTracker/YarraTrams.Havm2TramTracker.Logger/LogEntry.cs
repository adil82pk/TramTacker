using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace YarraTrams.Havm2TramTracker.Logger
{
    public class LogEntry
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Detail { get; set; }
        public EventLogEntryType Type { get; set; }
        public int MinutesOccurred { get; set; }
        public int NumberOfOccurrence { get; set; }
        public DateTime LastOccurredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MajorEscalationEventBaseCode { get; set; }

        public int ActualCode
        {
            get
            {
                if (this.MajorEscalationEventBaseCode > 0)
                {
                    return this.Code + this.MajorEscalationEventBaseCode;
                }
                return this.Code;
            }
        }

        public string GetFormattedMessage()
        {
            string formattedMessage = String.Format("{0}, {1}, {2}{3}{4}"
                , this.Type
                , this.Code
                , this.Message
                , Environment.NewLine
                , this.Detail
            );

            DateTime logTime = this.CreatedAt;

            if (this.NumberOfOccurrence > 1 && this.LastOccurredAt != null)
            {
                formattedMessage = String.Format(
                    "{0}{1}Number of occurrence since start of counting cycle at {2}: {3}."
                    , formattedMessage
                    , Environment.NewLine
                    , this.LastOccurredAt.ToString("yyyy-MM-dd HH:mm:ss.FFF")
                    , this.NumberOfOccurrence
                );
                logTime = this.LastOccurredAt;
            }

            formattedMessage = String.Format(
                "[{0}] - {1}{2}System Version: {3}"
                , logTime.ToString("yyyy-MM-dd HH:mm:ss.FFF")
                , formattedMessage
                , Environment.NewLine
                , SystemConstants.SystemVersion);

            return formattedMessage;
        }
    }
}
