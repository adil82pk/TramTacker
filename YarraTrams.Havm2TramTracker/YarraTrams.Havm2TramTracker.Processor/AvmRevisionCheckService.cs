using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using YarraTrams.Havm2TramTracker.Logger;
using YarraTrams.Havm2TramTracker.Processor.Helpers;

namespace YarraTrams.Havm2TramTracker.Processor
{
    partial class AvmRevisionCheckService : ServiceBase
    {
        private System.Threading.Timer processingTimer;
        private TimerState stateObj;

        private bool stopConfigFileChangeWatcher = false;
        private FileSystemWatcher fileSystemWatcher;

        public AvmRevisionCheckService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LogWriter.Instance.Log(EventLogCodes.SERVICE_STARTED, "AvmRevisionCheckService has been started");

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

        protected override void OnStop()
        {
            LogWriter.Instance.Log(EventLogCodes.SERVICE_STOPPED, "AvmRevisionCheckService has been stopped");
        }

        /// <summary>
        /// Core processing orchestration
        /// </summary>
        private void RunProcessing(Enums.Processes process)
        {
            this.StopTimer();
            LogWriter.Instance.Log(EventLogCodes.TIMER_TRIGGERED, String.Format("Havm2TramTracker.AvmRevisionCheckService scheduled execution has been triggered, running the {0} process", process.ToString()));
            try
            {
                switch (process)
                {
                    case Enums.Processes.CheckAvmTimetableRevision:
                        // do something
                        break;
                    default:
                        throw new NotImplementedException(string.Format("Process {0} not implemented inside Havm2TramTracker.AvmRevisionCheckService.", process.ToString()));
                }

            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.FATAL_ERROR, String.Format("A Fatal Error has occured inside Havm2TramTracker.AvmRevisionCheckService\n\nMessage: {0}\n\nStacktrace:{1}", ex.Message, ex.StackTrace));
            }
            this.RunTimer();
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
        /// Schedules the next process execution, according to the config settings.
        /// </summary>
        public void RunTimer()
        {
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            TimeSpan triggerTime = Properties.Settings.Default.CheckTomorrowsAvmTimetableRevisionDueTime;
            const Enums.Processes triggerProcess = Enums.Processes.CheckAvmTimetableRevision;
            int dayInMilliseconds = (60 * 60 * 24) * 1000; //get seconds in the day then convert to ms

            int baseTimeDiff = (int)triggerTime.Subtract(currentTime).TotalMilliseconds;
            int dueTimeMilliseconds = (currentTime >= triggerTime) ? ((24 * 60 * 60 * 1000) - baseTimeDiff) : baseTimeDiff; // TODO: allow for DST

            stateObj = new TimerState();
            stateObj.TimerCanceled = false;
            stateObj.process = triggerProcess;

            System.Threading.TimerCallback TimerDelegate = new System.Threading.TimerCallback(TimerTask);


            processingTimer = new System.Threading.Timer(TimerDelegate, stateObj, dueTimeMilliseconds, dayInMilliseconds);

            LogWriter.Instance.Log(EventLogCodes.TIMER_SET,
                                    string.Format("Havm2TramTracker.AvmRevisionCheckService scheduled to wake up again in {0} seconds to run {1}.\n\nCheckTomorrowsAvmTimetableRevisionDueTime setting = {2}",
                                        dueTimeMilliseconds / 1000,
                                        stateObj.process.ToString(),
                                        Properties.Settings.Default.CheckTomorrowsAvmTimetableRevisionDueTime
                                    )
                                  );

            // Save a reference for Dispose.
            stateObj.TimerReference = processingTimer;
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
            LogWriter.Instance.Log(EventLogCodes.SERVICE_CONFIG_CHANGED, "Configuration file has changed for Havm2TramTracker.AvmRevisionCheckService");

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
                    LogWriter.Instance.Log(EventLogCodes.CONFIGURATION_UPDATED_WHILST_INPROCESS, "Configuration updated whilst Havm2TramTracker.AvmRevisionCheckService process being actively executed. This can cause unusual behaviour.");
                }
                else
                {
                    // Stop currently running timer then trigger new timer.
                    LogWriter.Instance.Log(EventLogCodes.TIMER_SET, "Resetting the Havm2TramTracker.AvmRevisionCheckService timer");
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
                return true;
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
        /// Writes event log entries when it finds something wrong.
        /// </summary>
        private bool AllRequiredConfigEntriesPresent()
        {
            try
            {
                List<string> requiredConfig = new List<string> {
                "Havm2TramTrackerAPI"
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
    }
}
