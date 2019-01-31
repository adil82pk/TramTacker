using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Logger
{
    public static class EventLogConfig
    {
        public const string EVENT_LOG_SOURCE = "Havm2TramTracker";
        public const string EVENT_LOG_NAME = "Havm2TramTracker.Events";
    }

    public static class LoggingType
    {
        public const int EVENT_LOG = 1;
        public const int FILE = 2;
        public const int CONSOLE = 3;
    }

    public static class EventLogCategoryCode
    {
        public const short HAVM2TRAMTRACKER = 1;
    }

    public static class EventLogTypeCode
    {
        public const int ERROR = 1000;
        public const int WARNING = 2000;
        public const int INFO = 3000;
    }

    public static class EventLogCodes
    {
        //Todo: Configure Escalations in Settings
        #region info
        public const int SERVICE_STARTED = 3001;
        public const int SERVICE_STOPPED = 3002;
        #endregion

        #region warning
        public const int SERVICE_CONFIG_CHANGED = 2001;
        public const int UNKNOWN_VEHICLE_ENCOUNTERED = 2101;
        #endregion

        #region error
        public const int FATAL_ERROR = 1000;
        public const int DB_CONNECTION_ERROR = 1001;
        public const int DB_EXECUTE_ERROR = 1002;
        #endregion
    }

    public static class SystemConstants
    {
        public static int VersionMajor = 1;
        public static int VersionMinor = 0;
        public static int VersionPatch = 0;

        public static string SystemVersion
        {
            get
            {
                return VersionMajor.ToString() + "." + VersionMinor.ToString() + "." + VersionPatch.ToString();
            }
        }
    }
}
