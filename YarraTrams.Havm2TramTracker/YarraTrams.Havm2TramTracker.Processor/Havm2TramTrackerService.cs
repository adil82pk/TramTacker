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
using YarraTrams.Havm2TramTracker.Logger;

namespace YarraTrams.Havm2TramTracker.Processor
{
    public partial class Havm2TramTrackerService : ServiceBase
    {
        private System.Threading.Timer processingTimer;
        private TimerStateClass stateObj;

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
                    LogWriter.Instance.Log(EventLogCodes.FATAL_ERROR, String.Format("An error has occured and wasn't caught by the core Processor\n\nMessage: {0}\n\nStacktrace:{1}", ex.Message, ex.StackTrace));
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
        private void RunProcessing(Processes process)
        {
            this.StopTimer();
            LogWriter.Instance.Log(EventLogCodes.TIMER_TRIGGERED, String.Format("Havm2TramTracker scheduled execution has been triggered, running the {0} process", process.ToString()));
            try
            {
                switch (process)
                {
                    case Processes.CopyToLive:
                        Processor.CopyToLive();
                        break;
                    case Processes.RefreshTemp:
                        Processor.RefreshTemp();
                        break;
                    default:
                        throw new NotImplementedException(string.Format("Process {0} not implemented.", process.ToString()));
                }
                        
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.FATAL_ERROR, String.Format("A Fatal Error has Occured\n\nMessage: {0}\n\nStacktrace:{1}", ex.Message, ex.StackTrace));
            }
            this.RunTimer();
        }

        /// <summary>
        /// Schedules the next process execution, according to the config settings.
        /// </summary>
        public void RunTimer()
        {
            stateObj = new TimerStateClass();
            stateObj.TimerCanceled = false;

            TimeSpan refreshTempDueTime = Properties.Settings.Default.RefreshTempDueTime;
            TimeSpan copyToLiveDueTime = Properties.Settings.Default.CopyToLiveDueTime;
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            TimeSpan dueTime;

            // This logic depends on the validation inside the TriggerTimesAreValid method.
            // If the current time is either prior to the two triggers or subsequent to the two triggers...
            if ((currentTime < copyToLiveDueTime) || (currentTime > refreshTempDueTime))
            {
                // ...then the next trigger time is the earlier of the two triggers, which is always CopyToLive.
                dueTime = copyToLiveDueTime;
                stateObj.process = Processes.CopyToLive;
            }
            else
            {
                // ...otherwise we're in between the triggers so the we want the later of the two, which is always RefreshTemp.
                dueTime = refreshTempDueTime;
                stateObj.process = Processes.RefreshTemp;
            }

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
            
            System.Threading.TimerCallback TimerDelegate = new System.Threading.TimerCallback(TimerTask);

            int interval = (60 * 60 * 24) * 1000; //get seconds in the day then convert to ms

            processingTimer = new System.Threading.Timer(TimerDelegate, stateObj, (int)dueTimeSeconds * 1000, interval); //Convert from seconds to ms

            LogWriter.Instance.Log(EventLogCodes.TIMER_SET,
                                    string.Format("Havm2TramTracker scheduled to wake up again in {0} seconds to run {1}.\n\nCopyToLive setting = {2}\nRefreshTemp setting = {3}",
                                        (int)dueTimeSeconds,
                                        stateObj.process.ToString(),
                                        copyToLiveDueTime,
                                        refreshTempDueTime
                                    )
                                  );

            // Save a reference for Dispose.
            stateObj.TimerReference = processingTimer;
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
            TimerStateClass State = (TimerStateClass)StateObj;
            // Use the interlocked class to increment the counter variable.
            //System.Threading.Interlocked.Increment(ref State.SomeValue);
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

        private enum Processes
        {
            CopyToLive,
            RefreshTemp
        }

        private class TimerStateClass
        {
            public System.Threading.Timer TimerReference;
            public bool TimerCanceled;
            public Processes process;
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
                    if (TriggerTimesAreValid())
                    {
                        return true;
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
                "CopyToLiveDueTime",
                "RefreshTempDueTime"
            };

                // filter items that are set
                List<string> configNotSet = requiredConfig.Select(conf => (Properties.Settings.Default[conf] == null) || string.IsNullOrEmpty(Properties.Settings.Default[conf].ToString()) ? conf : null)
                    .Where(conf => conf != null)
                    .ToList<string>();

                if (configNotSet.Count > 0)
                {
                    configNotSet.ForEach(conf => LogWriter.Instance.Log(
                        EventLogCodes.INVALID_CONFIGURATION,
                        String.Format("Fatal error - Please set Config property: {0}", conf)));

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(
                        EventLogCodes.INVALID_CONFIGURATION,
                        String.Format("Fatal error: {0}", ex.Message));

                return false;
            }
        }

        /// <summary>
        /// Returns true if all strings in a list (from the config file) are lower case
        /// </summary>
        private bool AllStringsAreLowerCase(System.Collections.Specialized.StringCollection col)
        {
            return col.Cast<string>()
                    .ToList()
                        .All(str => !string.IsNullOrEmpty(str) && !str.Any(c => char.IsUpper(c)));
        }

        /// <summary>
        /// Returns true if the trigger times are both less than 24 hours and differ by more than 30 mins.
        /// Logs to the event log and returns false if the above isn't true.
        /// </summary>
        private bool TriggerTimesAreValid()
        {
            TimeSpan refreshTempDueTime = Properties.Settings.Default.RefreshTempDueTime;
            TimeSpan copyToLiveDueTime = Properties.Settings.Default.CopyToLiveDueTime;

            if (refreshTempDueTime.TotalHours >= 24 || copyToLiveDueTime.TotalHours >= 24)
            {
                LogWriter.Instance.Log(
                        EventLogCodes.INVALID_CONFIGURATION,
                        string.Format("Fatal error - the values for RefreshTempDueTime ({0}) and CopyToLiveDueTime ({1}) must be between 00:00:00 and 23:59:59."
                                , refreshTempDueTime
                                , copyToLiveDueTime
                                )
                        );
                return false;
            }

            const double minDiff = 30; // Minutes.
            double diff = Math.Abs(refreshTempDueTime.TotalMinutes - copyToLiveDueTime.TotalMinutes);
            if (diff < minDiff || diff > (1440 - minDiff)) // There are 1440 minutes in a day.
            {
                LogWriter.Instance.Log(
                        EventLogCodes.INVALID_CONFIGURATION,
                        string.Format("Fatal error - the values for RefreshTempDueTime ({0}) and CopyToLiveDueTime ({1}) must be more than {2} minutes apart, currently they're {3} minutes apart."
                                , refreshTempDueTime
                                , copyToLiveDueTime
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
