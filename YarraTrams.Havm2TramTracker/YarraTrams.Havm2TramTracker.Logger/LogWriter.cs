using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Threading;

namespace YarraTrams.Havm2TramTracker.Logger
{
    public class LogWriter
    {
        //Singleton pattern
        private static LogWriter instance = null;
        private static readonly object syncRoot = new Object();
        private Dictionary<int, LogEntry> majorEscalationEvents;
        private string eventLogName;
        private string eventLogSource;
        private string[] majorEscalationCodes;
        private int majorEscalationThresholdMinutes;
        private int majorEscalationEventBaseCode;

        public string EventLogName {
            get {
                return this.eventLogName;
            }
        }

        public static LogWriter Instance
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new LogWriter();
                    }
                }

                return instance;
            }
        }

        public void InitializeSettings()
        {
            this.eventLogName = EventLogConfig.EVENT_LOG_NAME;
            this.eventLogSource = EventLogConfig.EVENT_LOG_SOURCE;

            string majorEscalationCodesCsv = YarraTrams.Havm2TramTracker.Logger.Properties.Settings.Default.MajorEscalationCodesCsv;
            this.majorEscalationCodes = majorEscalationCodesCsv.Split(',');
            this.majorEscalationThresholdMinutes = (int)YarraTrams.Havm2TramTracker.Logger.Properties.Settings.Default.MajorEscalationThresholdMinutes;
            this.majorEscalationEventBaseCode = (int)YarraTrams.Havm2TramTracker.Logger.Properties.Settings.Default.MajorEscalationEventBaseCode;
        }

        public void setEventLogName(string logName)
        {
            this.eventLogName = logName;
        }

        public void setEventLogSource(string logSource)
        {
            this.eventLogSource = logSource;
        }

        public void setEscalationThresholdMinutes(int escalationThresholdMinutes)
        {
            this.majorEscalationThresholdMinutes = escalationThresholdMinutes;
        }

        public void setEscalationEventBaseCode(int escalationBaseCode)
        {
            this.majorEscalationEventBaseCode = escalationBaseCode;
        }

        public void setEscalationEventCodesCsv(string escalationCodesCsv)
        {
            this.majorEscalationCodes = escalationCodesCsv.Split(',');
        }

        public LogWriter()
        {
            this.InitializeSettings();
            this.majorEscalationEvents = new Dictionary<int, LogEntry>();
        }

        /// <summary>
        /// Log message to source, Event log, File, or Console (for testing) 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="detail"></param>
        /// <param name="loggingType"></param>
        /// <param name="fileOutputPath"></param>
        public void Log(int code, string message, string detail = null, int loggingType = LoggingType.EVENT_LOG, string fileOutputPath = null, bool shouldDelayAfterLog = true)
        {
            //if in interactive env (Console or Test) and loging to Windows Event
            if (Environment.UserInteractive && loggingType == LoggingType.EVENT_LOG)
            {
                //also log to console for debugging
                this.LogToConsole(code, message, detail);
            }

            switch (loggingType)
            {
                case LoggingType.EVENT_LOG:
                    this.LogToWindowsEvent(code, message, detail, shouldDelayAfterLog);
                    break;
                case LoggingType.FILE:
                    this.LogToFile(fileOutputPath, code, message, detail);
                    break;
                case LoggingType.CONSOLE:
                    this.LogToConsole(code, message, detail);
                    break;
            }
        }

        public void LogWithoutDelay(int code, string message, string detail = null, int loggingType = LoggingType.EVENT_LOG, string fileOutputPath = null)
        {
            this.Log(code, message, detail, loggingType, fileOutputPath, false);
        }

        /// <summary>
        /// Log to Windows Event Log, also logs Major Escalation Events when appropriate.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="detail"></param>
        public void LogToWindowsEvent(int code, string message, string detail = null, bool shouldDelayAfterLog = true)
        {
            DateTime now = DateTime.Now;

            LogEntry logEntry = new LogEntry
            {
                Code = code,
                Type = this.GetLogType(code),
                Message = message,
                MinutesOccurred = 0,
                NumberOfOccurrence = 1,
                CreatedAt = now,
                LastOccurredAt = now,
                Detail = detail,
            };

            this.logEntry(logEntry, shouldDelayAfterLog);

            // Check if the event code should be monitored for creating a major escalation event.
            if (this.majorEscalationCodes.Contains(code.ToString()))
            {
                lock (this.majorEscalationEvents)
                {
                    // If this event code has not occurred in the last 2 minutes, start tracking this event code.
                    if (!this.majorEscalationEvents.ContainsKey(code))
                    {
                        this.majorEscalationEvents.Add(code, logEntry);
                    }
                    // Otherwise this event has already occurred in the past minute, check if its time to log a major escalation event.
                    else
                    {
                        LogEntry escalatedlogEntry = this.majorEscalationEvents[code];
                        escalatedlogEntry.NumberOfOccurrence++;
                        double minutesBetweenLastOccurrence = (now - escalatedlogEntry.CreatedAt).TotalMinutes - escalatedlogEntry.MinutesOccurred;

                        // If two minutes have passed since the last time this event occured,
                        // update the last event with this event to restart the count for this event code.
                        if (minutesBetweenLastOccurrence >= 2)
                        {
                            this.majorEscalationEvents[code] = logEntry;
                        }
                        // Or if only a minute has passed (but less than two minutes) since the last time this event occurred; 
                        else if (minutesBetweenLastOccurrence >= 1)
                        {
                            // increase the minutes occured by 1 (used for determining the minutes between the last time we saw this event)
                            escalatedlogEntry.MinutesOccurred++;
                            // Check if this event has occured at least once a minute, for the past X (this.majorEscalationThresholdMinutes) minutes
                            // If it has, set the major escalation version of this event,
                            // then log and remove this event restarting the tracking process of this event code.
                            if (escalatedlogEntry.MinutesOccurred >= this.majorEscalationThresholdMinutes)
                            {
                                escalatedlogEntry.MajorEscalationEventBaseCode = this.majorEscalationEventBaseCode;
                                this.logEntry(escalatedlogEntry, shouldDelayAfterLog);
                                this.majorEscalationEvents.Remove(code);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Log to text file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="detail"></param>
        public void LogToFile(string filePath, int code, string message, string detail = null)
        {
            if (!filePath.EndsWith("/"))
            {
                filePath += "/";
            }

            using (StreamWriter writer = new StreamWriter(filePath + "log.txt", true))
            {
                writer.WriteLine(getFormattedMessage(code, message, detail));
            }
        }

        /// <summary>
        /// Log to Trace Console (for testing)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="detail"></param>
        public void LogToConsole(int code, string message, string detail = null)
        {
            Console.WriteLine(getFormattedMessage(code, message, detail) + Environment.NewLine);
        }

        /// <summary>
        /// Format message for display
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        private string getFormattedMessage(int code, string message, string detail = null)
        {
            return String.Format("[{0}] - {1}, {2}, {3}\n\n{4}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF"), GetLogType(code), code, message, detail);
        }

        /// <summary>
        /// Return LogEntryType (Info, Warning, Error) based on code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>EventLogEntryType Enum</returns>
        public EventLogEntryType GetLogType(int code)
        {
            if (code >= EventLogTypeCode.ERROR && code < EventLogTypeCode.WARNING)
            {
                return EventLogEntryType.Error;
            }
            else if (code >= EventLogTypeCode.WARNING && code < EventLogTypeCode.INFO)
            {
                return EventLogEntryType.Warning;
            }

            return EventLogEntryType.Information;
        }

        private void logEntry(LogEntry logEntry, bool shouldDelayAfterLog)
        {
            if (!EventLog.SourceExists(this.eventLogSource))
            {
                EventLog.CreateEventSource(this.eventLogSource, this.eventLogName);
            }

            EventLog eventLog = new EventLog();
            eventLog.Source = this.eventLogSource;
            eventLog.Log = this.eventLogName;

            eventLog.WriteEntry(
                logEntry.GetFormattedMessage(),
                logEntry.Type,
                logEntry.ActualCode,
                EventLogCategoryCode.HAVM2TRAMTRACKER,
                null
            );

            if (shouldDelayAfterLog)
            {
                //sleeping for 1 second after logging a Windows Event, as Event Log doesn't order correctly (only down to second)
                //slightly weird but helps show event logs in the correct sequence which helps in debugging.
                Thread.Sleep(1000);
            }
        }
    }
}
