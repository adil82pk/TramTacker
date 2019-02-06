using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models
{
    public static class HastusStopMapper
    {
        public static Dictionary<int, string> stops { get; set; }

        public static void Populate()
        {
            // Ensure our stop list is empty.
            stops = null;

            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.TramTrackerConnectionString))
            {
                string query = "SELECT ISNULL(StopID,''), ISNULL(StopNo,0) FROM T_Stops ORDER BY StopID";

                conn.Open();

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 30;

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    stops = new Dictionary<int, string>();

                    while (reader.Read())
                    {
                        string stopId = (string)reader[0];
                        Int16 stopNo = (Int16)reader[1];
                        if (stopNo > 0 && !String.IsNullOrEmpty(stopId))
                        {
                            stops.Add(stopNo, stopId);
                        }
                    }
                }
                else
                {
                    throw new Exception("No stop mapping found in TramTRACKER T_Stops table.");
                }
            }
        }
    }
}
