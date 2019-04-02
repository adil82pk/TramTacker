using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models.Helpers
{
    public static class SettingsExposer
    {
        public static System.Collections.Specialized.StringCollection VehicleGroupsWithLowFloor()
        {
            return Properties.Settings.Default.VehicleGroupsWithLowFloor;
        }
        public static System.Collections.Specialized.StringCollection VehicleGroupsWithoutLowFloor()
        {
            return Properties.Settings.Default.VehicleGroupsWithoutLowFloor;
        }
        public static System.Collections.Specialized.StringCollection DepotFirstChacterMapping()
        {
            return Properties.Settings.Default.DepotFirstChacterMapping;
        }
    }
}
