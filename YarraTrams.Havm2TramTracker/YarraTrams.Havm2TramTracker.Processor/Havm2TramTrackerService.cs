using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using YarraTrams.Havm2TramTracker.Logger;
using YarraTrams.Havm2TramTracker.Processor.Helpers;

namespace YarraTrams.Havm2TramTracker.Processor
{
    public partial class Havm2TramTrackerService : ServiceBase
    {
        private System.Threading.Timer processingTimer;
        private TimerState stateObj;

        private bool stopConfigFileChangeWatcher = false;
        private FileSystemWatcher fileSystemWatcher;

        public Havm2TramTrackerService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Service starts
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            LogWriter.Instance.Log(EventLogCodes.SERVICE_STARTED, "Havm2TramTracker has been started");

            if (IsConfigurationValid())
            {
                stopConfigFileChangeWatcher = true;

                /// start config file change watcher
                Thread myWorkerThread = new Thread(WatchConfigFileChange) { Name = "Havm2TramTracker Service Config File Change Watcher Thread" };
                myWorkerThread.Start();

                try
                {
                    this.RunTimer();
                }
                catch (Exception ex)
                {
                    LogWriter.Instance.Log(EventLogCodes.FATAL_ERROR, String.Format("An Havm2TramTracker error has occured and wasn't caught by the core Processor\n\nMessage: {0}\n\nStacktrace:{1}", ex.Message, ex.StackTrace));
                }
            }
            else
            {
                this.Stop();
            }
        }

        /// <summary>
        /// Service stops
        /// </summary>
        protected override void OnStop()
        {
            this.StopTimer();
            LogWriter.Instance.Log(EventLogCodes.SERVICE_STOPPED, "Havm2TramTracker has been stopped");
        }

        /// <summary>
        /// Core processing orchestration
        /// </summary>
        private void RunProcessing(Enums.Processes process)
        {
            this.StopTimer();
            LogWriter.Instance.Log(EventLogCodes.TIMER_TRIGGERED, String.Format("Havm2TramTracker scheduled execution has been triggered, running the {0} process", process.ToString()));
            try
            {
                switch (process)
                {
                    case Enums.Processes.CopyToLive:
                        Processor.CopyToLive();
                        break;
                    case Enums.Processes.RefreshTemp:
                        Processor.RefreshTemp();
                        break;
                    default:
                        throw new NotImplementedException(string.Format("Process {0} not implemented in Havm2TramTracker.", process.ToString()));
                }
                        
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.FATAL_ERROR, String.Format("A Fatal Error has occured in Havm2TramTracker\n\nMessage: {0}\n\nStacktrace:{1}", ex.Message, ex.StackTrace));
            }
            this.RunTimer();
        }

        /// <summary>
        /// Schedules the next process execution, according to the config settings.
        /// </summary>
        public void RunTimer()
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan triggerTimeSpan;
            Enums.Processes triggerProcess;

            DetermineNextTrigger(currentTime, Properties.Settings.Default.RefreshTempWithTomorrowsDataDueTime, Properties.Settings.Default.CopyTodaysDataToLiveDueTime, out triggerTimeSpan, out triggerProcess);

            int dueTimeMilliseconds = this.GetTriggerTime(currentTime, triggerTimeSpan);

            stateObj = new TimerState();
            stateObj.TimerCanceled = false;
            stateObj.process = triggerProcess;

            System.Threading.TimerCallback TimerDelegate = new System.Threading.TimerCallback(TimerTask);

            int interval = (60 * 60 * 24) * 1000; //get seconds in the day then convert to ms

            processingTimer = new System.Threading.Timer(TimerDelegate, stateObj, dueTimeMilliseconds, interval);

            LogWriter.Instance.Log(EventLogCodes.TIMER_SET,
                                    string.Format("Havm2TramTracker scheduled to wake up again in {0} seconds to run {1}.\n\nCopyToLive setting = {2}\nRefreshTemp setting = {3}",
                                        dueTimeMilliseconds/1000,
                                        stateObj.process.ToString(),
                                        Properties.Settings.Default.CopyTodaysDataToLiveDueTime,
                                        Properties.Settings.Default.RefreshTempWithTomorrowsDataDueTime
                                    )
                                  );

            // Save a reference for Dispose.
            stateObj.TimerReference = processingTimer;
        }

        /// <summary>
        /// Gets trigger time in milliseconds, taking into account daylight savings start & ends
        /// </summary>
        /// <param name="currentDateTime">The current date + time of day</param>
        /// <param name="triggerTime">The time of the next trigger event, not adjusted for Daylight savings</param>
        /// <returns></returns>
        public int GetTriggerTime(DateTime currentDateTime, TimeSpan triggerTime)
        {
            int triggerInMilliseconds = this.ConvertDueTimeToMilliseconds(currentDateTime.TimeOfDay, triggerTime);
            TimeZone localTimezone = TimeZone.CurrentTimeZone;
            TimeSpan currentOffset = localTimezone.GetUtcOffset(currentDateTime);
            DateTime triggerDateTime = currentDateTime.AddMilliseconds(triggerInMilliseconds);
            TimeSpan tomorrowsOffet = localTimezone.GetUtcOffset(currentDateTime.AddDays(1));

            // if the trigger is happening tomorrow
            if (currentDateTime.Date != triggerDateTime.Date)
            {
                // if tomorrows UTC offset is different from todays...
                if(currentOffset != tomorrowsOffet)
                {
                    // get difference in milliseconds (e.g. -1 hour in ms for DST start, +1 hour in ms for DST end)
                    int offsetDifferenceMilliseconds = (int)(currentOffset - tomorrowsOffet).TotalMilliseconds;

                    // add difference to the total to correctly realign the trigger time
                    triggerInMilliseconds += offsetDifferenceMilliseconds;
                }
            }
            else
            {
                // if the trigger is happening today
                DaylightTime daylightSavingsInfo = TimeZone.CurrentTimeZone.GetDaylightChanges(currentDateTime.Year);

                // if today is a DST change over day
                if (currentDateTime.Date == daylightSavingsInfo.Start.Date || currentDateTime.Date == daylightSavingsInfo.End.Date)
                {
                    // and current time is > 12 and < 2am
                    if (currentDateTime.Hour >= 0 && currentDateTime.Hour <= 1)
                    {
                        TimeSpan yesterdayOffset = localTimezone.GetUtcOffset(currentDateTime.AddDays(-1));

                        // get difference in milliseconds from yesterday to tommorow
                        int offsetDifferenceMilliseconds = (int)(yesterdayOffset - tomorrowsOffet).TotalMilliseconds;

                        // add difference to the total to correctly realign the trigger time
                        // making sure the trigger later today adds or removes the hour correctly
                        triggerInMilliseconds += offsetDifferenceMilliseconds;
                    }
                    // else if its in the DST "window", we don't support this currently (between 2 and 3)
                    else if (currentDateTime.Hour >= 2 && currentDateTime.Hour < 3)
                    {
                        // this is within daylight savings switch over time which is not supported (where there could be weirdness)
                        // log an event, and do no adjustment
                        LogWriter.Instance.Log(EventLogCodes.DST_TRIGGER_IN_ADJUSTMENT_TIME_NOT_SUPPORTED, 
                            String.Format("We do not support adjustments for a Havm2TramTracker timer when inside the DST changeover period, triggering in {0}", TimeSpan.FromMilliseconds(triggerInMilliseconds)));
                    }
                }
            }

            return triggerInMilliseconds;
        }

        public void StopTimer()
        {
            if (stateObj != null)
            {
                stateObj.TimerCanceled = true;
            }

        }

        private void TimerTask(object StateObj)
        {
            TimerState State = (TimerState)StateObj;
            
            //System.Diagnostics.Debug.WriteLine("Launched new thread  " + DateTime.Now.ToString());
            if (State.TimerCanceled) // Dispose Requested.            
            {
                State.TimerReference.Dispose();
            }
            else
            {
                this.RunProcessing(State.process);
            }
        }

        /// <summary>
        /// Uses the (passed-in) current time and the passed-in trigger times to determine:
        ///  - the next trigger time; and
        ///  - the next triggered operation
        /// </summary>
        public void DetermineNextTrigger(DateTime currentDateTime,TimeSpan refreshTempWithTomorrowsDataDueTime, TimeSpan copyTodaysDataToLiveDueTime, out TimeSpan triggerTime, out Enums.Processes process)
        {
            // This logic depends on the validation inside the TriggerTimesAreValid method.
            // If the current time is either prior to the two triggers or subsequent to the two triggers...
            if ((currentDateTime.TimeOfDay < copyTodaysDataToLiveDueTime) || (currentDateTime.TimeOfDay > refreshTempWithTomorrowsDataDueTime))
            {
                // ...then the next trigger time is the earlier of the two triggers, which is always CopyToLive.
                triggerTime = copyTodaysDataToLiveDueTime;
                process = Enums.Processes.CopyToLive;
            }
            else
            {
                // ...otherwise we're in between the triggers so the we want the later of the two, which is always RefreshTemp.
                triggerTime = refreshTempWithTomorrowsDataDueTime;
                process = Enums.Processes.RefreshTemp;
            }
        }

        /// <summary>
        /// Returns an int representing the number of milliseconds between the passed-in currentTime and the dueTime.
        /// Assumes both times are less than 24hrs.
        /// </summary>
        public int ConvertDueTimeToMilliseconds(TimeSpan currentTime, TimeSpan dueTime)
        {
            int dueTimeSeconds;

            if (currentTime < dueTime)
            {
                //If trigger time hasn't yet happened today then find the number of seconds between now and then
                dueTimeSeconds = (int)dueTime.Subtract(currentTime).TotalSeconds;
            }
            else
            {
                //If trigger time has already happened today then take 24 hours and minus the time elapsed since 3am
                dueTimeSeconds = (60 * 60 * 24) - (int)currentTime.Subtract(dueTime).TotalSeconds;
            }

            return dueTimeSeconds * 1000;
        }

        /// <summary>
        /// Watch the config file change
        /// </summary>
        private void WatchConfigFileChange()
        {
            string assemblyDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            NotifyFilters notifyFilters = NotifyFilters.LastWrite;

            fileSystemWatcher = new FileSystemWatcher()
            {
                Path = assemblyDirectory,
                NotifyFilter = notifyFilters,
                Filter = "*.config"
            };
            fileSystemWatcher.Changed += OnConfigChanged;
            fileSystemWatcher.EnableRaisingEvents = true;

            while (!stopConfigFileChangeWatcher)
            {
                Thread.Sleep(5 * 1000);
            }
        }

        /// <summary>
        /// Reload the config settings for all projects when the file is updated
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnConfigChanged(object source, FileSystemEventArgs e)
        {
            LogWriter.Instance.Log(EventLogCodes.SERVICE_CONFIG_CHANGED, "Configuration file has changed for Havm2TramTracker");

            fileSystemWatcher.EnableRaisingEvents = false;
            YarraTrams.Havm2TramTracker.Processor.Helpers.SettingsRefresher.RefreshSettings();
            YarraTrams.Havm2TramTracker.Models.Helpers.SettingsRefresher.RefreshSettings();
            LogWriter.Instance.InitializeSettings();
            fileSystemWatcher.EnableRaisingEvents = true;

            if (!IsConfigurationValid())
            {
                this.Stop();
            }
            else
            {
                // If config updated whilst a process is executing then weird stuff might happen. We raise an alert but leave the current processing alone.
                if (stateObj != null && stateObj.TimerCanceled)
                {
                    LogWriter.Instance.Log(EventLogCodes.CONFIGURATION_UPDATED_WHILST_INPROCESS, "Configuration updated whilst Havm2TramTracker process being actively executed. This can cause unusual behaviour.");
                }
                else
                {
                    // Stop currently running timer then trigger new timer.
                    LogWriter.Instance.Log(EventLogCodes.TIMER_SET, "Resetting the Havm2TramTracker timer");
                    this.StopTimer();
                    this.RunTimer();
                }
            }
        }

        /// <summary>
        /// Checks that the entries in the configuration file are valid.
        /// Can't identify all issues though.
        /// 
        /// Writes event log entries when it finds something wrong.
        /// </summary>
        protected bool IsConfigurationValid()
        {
            if (AllRequiredConfigEntriesPresent())
            {
                if (AllStringsAreLowerCase(Models.Helpers.SettingsExposer.VehicleGroupsWithLowFloor()) && AllStringsAreLowerCase(Models.Helpers.SettingsExposer.VehicleGroupsWithoutLowFloor()))
                {
                    if (TriggerTimesAreValid(Properties.Settings.Default.RefreshTempWithTomorrowsDataDueTime, Properties.Settings.Default.CopyTodaysDataToLiveDueTime))
                    {
                        if (Properties.Settings.Default.NumberDailyTimetablesToRetrieve >= 1)
                        {
                            return true;
                        }
                        else
                        {
                            LogWriter.Instance.Log(
                                EventLogCodes.INVALID_CONFIGURATION,
                                "Fatal error - NumberDailyTimetablesToRetrieve must be set to a number exceeding zero.");
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    LogWriter.Instance.Log(
                        EventLogCodes.INVALID_CONFIGURATION,
                        "Fatal error - VehicleGroupsWithLowFloor and VehicleGroupsWithLowFloor config entry lists must only contain lower-case items.");
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if all required config settings are set
        /// 
        /// Works with string fields and timespan fields, not with bools though.
        /// </summary>
        private bool AllRequiredConfigEntriesPresent()
        {
            try
            {
                List<string> requiredConfig = new List<string> {
                "TramTrackerDB",
                "Havm2TramTrackerAPI",
                "CopyTodaysDataToLiveDueTime",
                "RefreshTempWithTomorrowsDataDueTime"
            };

                // filter items that are set
                List<string> configNotSet = requiredConfig.Select(conf => (Properties.Settings.Default[conf] == null) || string.IsNullOrEmpty(Properties.Settings.Default[conf].ToString()) ? conf : null)
                    .Where(conf => conf != null)
                    .ToList<string>();

                if (configNotSet.Count > 0)
                {
                    configNotSet.ForEach(conf => LogWriter.Instance.Log(
                        EventLogCodes.INVALID_CONFIGURATION,
                        String.Format("Fatal Havm2TramTracker error - Please set Config property: {0}", conf)));

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(
                        EventLogCodes.INVALID_CONFIGURATION,
                        String.Format("Fatal Havm2TramTracker error: {0}", ex.Message));

                return false;
            }
        }

        /// <summary>
        /// Returns true if all strings in a list (from the config file) are lower case
        /// </summary>
        public bool AllStringsAreLowerCase(System.Collections.Specialized.StringCollection col)
        {
            return col.Cast<string>()
                    .ToList()
                        .All(str => !string.IsNullOrEmpty(str) && !str.Any(c => char.IsUpper(c)));
        }

        /// <summary>
        /// Returns true if:
        /// - The trigger times are both less than 24 hours; and
        /// - They differ by more than 30 mins; and
        /// - The refreshTempWithTomorrowsDataDueTime follows the copyTodaysDataToLiveDueTime.
        /// Logs to the event log and returns false if the above isn't true.
        /// </summary>
        public bool TriggerTimesAreValid(TimeSpan refreshTempWithTomorrowsDataDueTime, TimeSpan copyTodaysDataToLiveDueTime)
        {
            if (refreshTempWithTomorrowsDataDueTime.TotalHours >= 24 || copyTodaysDataToLiveDueTime.TotalHours >= 24)
            {
                LogWriter.Instance.Log(
                        EventLogCodes.INVALID_CONFIGURATION,
                        string.Format("Fatal error - the values for RefreshTempWithTomorrowsDataDueTime ({0}) and copyTodaysDataToLiveDueTime ({1}) must be between 00:00:00 and 23:59:59."
                                , refreshTempWithTomorrowsDataDueTime
                                , copyTodaysDataToLiveDueTime
                                )
                        );
                return false;
            }

            if (refreshTempWithTomorrowsDataDueTime.TotalHours <= copyTodaysDataToLiveDueTime.TotalHours)
            {
                LogWriter.Instance.Log(
                        EventLogCodes.INVALID_CONFIGURATION,
                        string.Format("Fatal error - the RefreshTempWithTomorrowsDataDueTime ({0}) must be set to a time that occurs after the copyTodaysDataToLiveDueTime ({1}). This is because the RefreshTemp process will not include data for the current day in the refresh, therefore the CopyToLive process would have no data on which to base current day predictions."
                                , refreshTempWithTomorrowsDataDueTime
                                , copyTodaysDataToLiveDueTime
                                )
                        );
                return false;
            }

            const double minDiff = 30; // Minutes.
            double diff = Math.Abs(refreshTempWithTomorrowsDataDueTime.TotalMinutes - copyTodaysDataToLiveDueTime.TotalMinutes);
            if (diff < minDiff || diff > (1440 - minDiff)) // There are 1440 minutes in a day.
            {
                LogWriter.Instance.Log(
                        EventLogCodes.INVALID_CONFIGURATION,
                        string.Format("Fatal error - the values for RefreshTempWithTomorrowsDataDueTime ({0}) and copyTodaysDataToLiveDueTime ({1}) must be more than {2} minutes apart, currently they're {3} minutes apart."
                                , refreshTempWithTomorrowsDataDueTime
                                , copyTodaysDataToLiveDueTime
                                , minDiff
                                , diff < minDiff ? diff : (1440 - diff)
                                )
                        );
                return false;
            }

            return true;
        }
    }
}
