using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.SideBySideTests
{
    public class Comparisons
    {
        /// <summary>
        /// Compares data in the existing DataSet with data on the new DataSet.
        /// </summary>
        public void RunComparisons()
        {
            //T_Temp_Trips
            var tripsComparer = new Models.T_Temp_TripsComparer();
            tripsComparer.RunComparison();

            //T_Temp_Schedules
            var schedulesComparer = new Models.T_Temp_SchedulesComparer();
            schedulesComparer.RunComparison();

            System.Console.ReadLine();
        }
    }
}
