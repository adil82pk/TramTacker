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
        public const int TIMER_SET = 3003;
        public const int TIMER_TRIGGERED = 3004;
        public const int TRIP_TRANSFORMATION_SUCCESS = 3005;
        public const int SCHEDULE_TRANSFORMATION_SUCCESS = 3006;
        public const int SCHEDULEMASTER_TRANSFORMATION_SUCCESS = 3007;
        public const int SCHEDULEDETAILS_TRANSFORMATION_SUCCESS = 3008;
        public const int PRE_CALL_TO_HAVM = 3009;
        public const int POST_CALL_TO_HAVM = 3010;
        public const int COPY_TO_LIVE_SUCCESS = 3011;
        public const int COPY_TO_LIVE_SUBSEQUENT_PROC_SUCCESS = 3012;
        public const int SAVE_TO_DATABASE_SUCCESS = 3013;

        public const int SIDE_BY_SIDE_INFO = 3100;
        #endregion

        #region warning
        public const int SERVICE_CONFIG_CHANGED = 2001;
        #endregion

        #region error
        public const int FATAL_ERROR = 1000;
        public const int DB_CONNECTION_ERROR = 1001;
        public const int DB_EXECUTE_ERROR = 1002;
        public const int LOG_FILE_WRITE_ERROR = 1003;
        public const int TRIP_TRANSFORMATION_ERROR = 1004;
        public const int SCHEDULE_TRANSFORMATION_ERROR = 1005;
        public const int SCHEDULEMASTER_TRANSFORMATION_ERROR = 1006;
        public const int SCHEDULEDETAILS_TRANSFORMATION_ERROR = 1007;
        public const int COPY_TO_LIVE_FAILED = 1008;
        public const int INVALID_CONFIGURATION = 1009;
        public const int CONFIGURATION_UPDATED_WHILST_INPROCESS = 1011;

        public const int SIDE_BY_SIDE_ERROR = 1100;
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
