using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Logger;

namespace YarraTrams.Havm2TramTracker.SideBySideTests.Helpers
{
    class DBHelper
    {
        /// <summary>
        /// Populates a DataTable using data from the database specified in the passed-in connection string.
        /// </summary>
        public static void ExecuteSQL(string sql)
        {
            var clock = new Stopwatch();

            clock.Start();
            
            using (SqlConnection connection =
                    new SqlConnection(Properties.Settings.Default.TramTrackerDB))
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 300;
                cmd.ExecuteNonQuery();

                connection.Close();
            }

            clock.Stop();
            LogWriter.Instance.LogWithoutDelay(EventLogCodes.SIDE_BY_SIDE_INFO
                     , String.Format("Execution of SQL ({0}...) took {1} seconds.", sql.Substring(0, 20).Replace(Environment.NewLine," "), clock.Elapsed));
        }

        /// <summary>
        /// THIS MUST BE WRAPPED IN A USING STATEMENT.
        /// Execute supplied SQL,
        /// Return a DataReader with the results
        /// </summary>
        private static SqlDataReader executeSQLReader(string sql)
        {
            SqlConnection conn = new SqlConnection(Properties.Settings.Default.TramTrackerDB);

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();
                // When using CommandBehavior.CloseConnection, the connection will be closed when the IDataReader is closed.
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                return reader;
            }
        }

        /// <summary>
        /// Execute supplied SQL
        /// Return and integer.
        /// </summary>
        public static int ExecuteSQLReturnInt(string sql)
        {
            using (SqlDataReader reader = DBHelper.executeSQLReader(sql))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    int ret;
                    int.TryParse(reader[0].ToString(), out ret);
                    return ret;
                }
                else
                {
                    throw new Exception("No result returned");
                }
            }
        }

        /// <summary>
        /// Populates a DataTable using data from the database specified in the passed-in connection string.
        /// </summary>
        public static DataTable GetData(string tableName)
        {
            DataTable dt = new DataTable();
            using (var da = new SqlDataAdapter(string.Format("SELECT * FROM {0} WITH (NOLOCK)", tableName), Properties.Settings.Default.TramTrackerDB))
            {
                da.Fill(dt);
            }
            return dt;
        }
    }
}
