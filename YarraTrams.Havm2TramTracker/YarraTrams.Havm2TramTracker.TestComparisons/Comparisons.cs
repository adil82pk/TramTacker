using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using YarraTrams.Havm2TramTracker.Logger;
using YarraTrams.Havm2TramTracker.TestComparisons.Helpers;

namespace YarraTrams.Havm2TramTracker.TestComparisons
{
    public class Comparisons
    {
        /// <summary>
        /// Entry method for running side-by-side comparisons.
        /// Triggers a comparison of all relevant tables.
        /// </summary>
        public void RunComparisons()
        {
            try
            {
                int runId;

                //Create Comparison Run parent record
                runId = DBHelper.ExecuteSQLReturnInt(string.Format("INSERT Havm2TTComparisonRun VALUES ('{0}'); SELECT scope_identity()", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFF")));

                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, string.Format("Running side-by-side comparisons. Run Id is {0}. Suffix for new tables is '{1}'.", runId, Processor.Helpers.SettingsExposer.DbTableSuffix()));

                // T_Temp_Trips
                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, "Comparing T_Temp_Trips.");
                Models.T_Temp_TripsComparer tripsComparer = new Models.T_Temp_TripsComparer();
                tripsComparer.RunComparison(runId);

                // T_Temp_Schedules
                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, "Comparing T_Temp_Schedules.");
                Models.T_Temp_SchedulesComparer schedulesComparer = new Models.T_Temp_SchedulesComparer();
                schedulesComparer.RunComparison(runId);

                // T_Temp_Schedules
                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, "Comparing T_Temp_SchedulesMaster.");
                Models.T_Temp_SchedulesMasterComparer schedulesMasterComparer = new Models.T_Temp_SchedulesMasterComparer();
                schedulesMasterComparer.RunComparison(runId);


                // Sending summary email
                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_INFO, "Sending summary email.");
                
                List<DataTable> tablesToInclude = new List<DataTable>();
                tablesToInclude.Add(tripsComparer.GetSummaryHistory(14));
                tablesToInclude.Add(schedulesComparer.GetSummaryHistory(14));

                SendSummaryEmail(tablesToInclude);

                System.Console.WriteLine("Comparisons complete.");
            }
            catch (Exception ex)
            {
                LogWriter.Instance.Log(EventLogCodes.SIDE_BY_SIDE_ERROR, string.Format("Error when running side-by-side test:\n{0}\n\n{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// Send an email to say the comparison run is complete. Pass in any relevent tabular data.
        /// </summary>
        private void SendSummaryEmail(List<DataTable> tablesToInclude)
        {
            MailMessage mail = new MailMessage(Properties.Settings.Default.ComparisonSummaryEmailFrom, Properties.Settings.Default.ComparisonSummaryEmailsTo.Replace(";", ","));
            mail.Subject = "Havm2TramTracker Data Comparisons";
            mail.IsBodyHtml = true;
            mail.Body = string.Format("<h1>Data Comparisons Summary for comparisons completed at {0}</h1>", DateTime.Now);

            foreach (DataTable dt in tablesToInclude)
            {
                mail.Body += ExportDatatableToHtml(dt) + "<br />";
            }

            SmtpClient client = new SmtpClient();
            NetworkCredential basicCredential = new NetworkCredential(Properties.Settings.Default.SmtpUsername, Properties.Settings.Default.SmtpPassword);
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = Properties.Settings.Default.SmtpHost;
            client.Credentials = basicCredential;
            client.Send(mail);
        }

        /// <summary>
        /// Convert datatable to HTML string
        /// </summary>
        protected string ExportDatatableToHtml(DataTable dt)
        {
            StringBuilder strHTMLBuilder = new StringBuilder();
            strHTMLBuilder.Append("<table border='1px' cellpadding='1' cellspacing='0' bgcolor='#99FF99' style='font-family:Garamond; font-size:smaller'>");

            strHTMLBuilder.Append("<tr>");
            foreach (DataColumn myColumn in dt.Columns)
            {
                strHTMLBuilder.Append("<th>");
                strHTMLBuilder.Append(ToSentenceCase(myColumn.ColumnName));
                strHTMLBuilder.Append("</th>");

            }
            strHTMLBuilder.Append("</tr>");
            
            bool firstRowAndCol = true;
            foreach (DataRow myRow in dt.Rows)
            {
                strHTMLBuilder.Append("<tr" + (firstRowAndCol ? " bgcolor='#66CC66'" : "") + " > ");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    strHTMLBuilder.Append("<td>");
                    strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString() + (firstRowAndCol ? " (latest)" : ""));
                    strHTMLBuilder.Append("</td>");
                    firstRowAndCol = false;
                }
                strHTMLBuilder.Append("</tr>");
            }
 
            strHTMLBuilder.Append("</table>");

            return strHTMLBuilder.ToString();
        }

        /// <summary>
        /// Convert Pascal (or Camel) notated string to the same notation but with spaces.
        /// e.g. "MyNiceString" becomes "My Nice String"
        /// </summary>
        private string ToSentenceCase(string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + m.Value[1]);
        }
    }
}
