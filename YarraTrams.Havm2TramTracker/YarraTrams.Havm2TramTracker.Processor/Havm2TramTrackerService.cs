﻿using System;
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

        DateTime lastSuccessfulCopyToLive = DateTime.MinValue;
        DateTime lastSuccessfulRefreshFromHavm = DateTime.MinValue;
        int consecutiveFailures = 0;

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

            stopConfigFileChangeWatcher = true;

            /// start config file change watcher
            Thread myWorkerThread = new Thread(WatchConfigFileChange) { Name = "Havm2TramTracker Service Config File Change Watcher Thread" };
            myWorkerThread.Start();

            try
            {
                this.RunProcessing();
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.FATAL_ERROR, String.Format("A Fatal Error has Occured\n\nMessage: {0}\n\nStacktrace:{1}", ex.Message, ex.StackTrace));
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
            LogWriter.Instance.Log(EventLogCodes.SERVICE_STARTED, "qqqq");
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

            int interval = 0;//Properties.Settings.Default.IntervalBetweenChecksSeconds * 1000; //convert from seconds to ms

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

                }
                else
                {

                }
                processingTimer = new System.Threading.Timer(TimerDelegate, stateObj, interval, interval);
            }


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
    }
}
