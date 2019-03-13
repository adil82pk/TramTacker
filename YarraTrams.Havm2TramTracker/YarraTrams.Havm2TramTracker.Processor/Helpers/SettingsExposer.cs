using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Processor.Helpers
{
    public static class SettingsExposer
    {
        public static string DbTableSuffix()
        {
            return Properties.Settings.Default.DbTableSuffix;
        }
    }
}
