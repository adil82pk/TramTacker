using System;
using System.Collections.Generic;
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
        private void RunProcessing()
        {
            this.StopTimer();
            LogWriter.Instance.Log(EventLogCodes.TIMER_TRIGGERED, String.Format("Havm2TramTracker scheduled execution has been triggered, as per config setting of {0}", Properties.Settings.Default.DueTime));
            try
            {
                Processor.Process();
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.FATAL_ERROR, String.Format("A Fatal Error has Occured\n\nMessage: {0}\n\nStacktrace:{1}", ex.Message, ex.StackTrace));
            }
            this.RunTimer();
        }

        /// <summary>
        /// If dueTimeSeconds isn't provided, this method schedules the next execution to occur at 3am (configurable)
        /// </summary>
        public void RunTimer(int? dueTimeSeconds = null)
        {
            stateObj = new TimerStateClass();
            stateObj.TimerCanceled = false;
            
            System.Threading.TimerCallback TimerDelegate = new System.Threading.TimerCallback(TimerTask);

            int interval = (60 * 60 * 24) * 1000; //get seconds in the day then convert to ms

            if (dueTimeSeconds != null)
            {
                processingTimer = new System.Threading.Timer(TimerDelegate, stateObj, dueTimeSeconds.Value * 1000, interval);
            }
            else
            {
                TimeSpan dueTime = Properties.Settings.Default.DueTime;
                TimeSpan currentTime = DateTime.Now.TimeOfDay;
                if (currentTime < dueTime)
                {
                    //If 3am (configurable) hasn't yet happened today then find the number of seconds between now and then
                    dueTimeSeconds = (int)dueTime.Subtract(currentTime).TotalSeconds;
                }
                else
                {
                    //If 3am (configurable) has already happened today then take 24 hours and minus the time elapsed since 3am
                    dueTimeSeconds = (60*60*24) - (int)currentTime.Subtract(dueTime).TotalSeconds;
                } 

                processingTimer = new System.Threading.Timer(TimerDelegate, stateObj, (int)dueTimeSeconds * 1000, interval); //Convert from seconds to ms
            }

            LogWriter.Instance.Log(EventLogCodes.TIMER_SET, string.Format("Havm2TramTracker scheduled to wake up again in {0} seconds", (int)dueTimeSeconds));

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
                this.RunProcessing();
            }
        }

        private class TimerStateClass
        {
            public System.Threading.Timer TimerReference;
            public bool TimerCanceled;
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
        }

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
        /// </summary>
        private bool AllRequiredConfigEntriesPresent()
        {
            List<string> requiredConfig = new List<string> {
                "TramTrackerDB",
                "Havm2TramTrackerAPI",
                "DueTime",
                "ExecuteCopyToLiveAsPartOfDailyProcess"
            };

            // filter items that are set
            List<string> configNotSet = requiredConfig.Select(conf => Properties.Settings.Default.Properties[conf] == null ? conf : null)
                .Where(conf => conf != null)
                .ToList<string>();

            if (configNotSet.Count > 0)
            {
                configNotSet.ForEach(conf => LogWriter.Instance.Log(
                    EventLogCodes.FATAL_ERROR,
                    String.Format("Fatal error - Please set Config property: {0}", conf)));

                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if all strings in a list are lower case
        /// </summary>
        private bool AreStringsLowerCase(List<string> list)
        {
            return list.All(str => !string.IsNullOrEmpty(str) && !str.Any(c => char.IsUpper(c)));
        }
    }
}
