using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models.Helpers
{
    public class SettingsRefresher
    {
        /// <summary>
        /// Reload settings from config file
        /// </summary>
        public static void RefreshSettings()
        {
            YarraTrams.Havm2TramTracker.Models.Properties.Settings.Default.Reload();
        }
    }
}
