using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Processor.Helpers
{
    class SettingsRefresher
    {
        /// <summary>
        /// Reload settings from config file
        /// </summary>
        public static void RefreshSettings()
        {
            YarraTrams.Havm2TramTracker.Processor.Properties.Settings.Default.Reload();
        }
    }
}
