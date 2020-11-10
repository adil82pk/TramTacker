using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
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
        }

        protected override void OnStop()
        {
            LogWriter.Instance.Log(EventLogCodes.SERVICE_STOPPED, "AvmRevisionCheckService has been stopped");
        }
    }
}
