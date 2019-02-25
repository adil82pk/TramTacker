using SpreadsheetLight;
using System;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Mail;

namespace YarraTrams.Havm2TramTracker.SideBySideTests
{
    public class Comparisons
    {
        /// <summary>
        /// Compares data in the existing DataSet with data on the new DataSet.
        /// </summary>
        public void RunComparisons()
        {
            // We set up an email here that we add to as we go.
            MailMessage mail = new MailMessage(Properties.Settings.Default.ComparisonSummaryEmailFrom, Properties.Settings.Default.ComparisonSummaryEmailsTo.Replace(";", ","));
            mail.Subject = "Havm2TramTracker Data Comparisons";
            mail.IsBodyHtml = true;
            mail.Body = string.Format("<h1>Data Comparisons Summary for comparisons started at {0}</h1>", DateTime.Now);

            string fileName;

            // T_Temp_Trips
            var tripsComparer = new Models.T_Temp_TripsComparer();
            DataTable existingRowsMissingFromNewT_Temp_Trips;
            DataTable newRowsNotInExistingT_Temp_Trips;
            DataTable existingRowsThatDifferFromNewT_TempTrips;
            tripsComparer.RunComparison(out existingRowsMissingFromNewT_Temp_Trips, out newRowsNotInExistingT_Temp_Trips, out existingRowsThatDifferFromNewT_TempTrips);
            fileName = this.OutputToFile("T_Temp_Trips", existingRowsMissingFromNewT_Temp_Trips, newRowsNotInExistingT_Temp_Trips, existingRowsThatDifferFromNewT_TempTrips);
            mail.Body += GetTableComparisonHtmlSummary("T_Temp_Trips", tripsComparer.ExistingData.Rows.Count, tripsComparer.NewData.Rows.Count, existingRowsMissingFromNewT_Temp_Trips.Rows.Count,newRowsNotInExistingT_Temp_Trips.Rows.Count,existingRowsThatDifferFromNewT_TempTrips.Rows.Count,fileName);
            mail.Attachments.Add(new Attachment(fileName));

            // T_Temp_Schedules
            var schedulesComparer = new Models.T_Temp_SchedulesComparer();
            DataTable existingRowsMissingFromNewT_Temp_Schedules;
            DataTable newRowsNotInExistingT_Temp_Schedules;
            DataTable existingRowsThatDifferFromNewT_TempSchedules;
            schedulesComparer.RunComparison(out existingRowsMissingFromNewT_Temp_Schedules, out newRowsNotInExistingT_Temp_Schedules, out existingRowsThatDifferFromNewT_TempSchedules);
            fileName = this.OutputToFile("T_Temp_Schedules", existingRowsMissingFromNewT_Temp_Schedules, newRowsNotInExistingT_Temp_Schedules, existingRowsThatDifferFromNewT_TempSchedules);
            mail.Body += GetTableComparisonHtmlSummary("T_Temp_Schedules", tripsComparer.ExistingData.Rows.Count, tripsComparer.NewData.Rows.Count, existingRowsMissingFromNewT_Temp_Schedules.Rows.Count, newRowsNotInExistingT_Temp_Schedules.Rows.Count, existingRowsThatDifferFromNewT_TempSchedules.Rows.Count, fileName);
            mail.Attachments.Add(new Attachment(fileName));

            // T_Temp_SchedulesMaster
            //var schedulesMasterComparer = new Models.T_Temp_SchedulesMasterComparer();
            //DataTable existingRowsMissingFromNewT_Temp_SchedulesMaster;
            //DataTable newRowsNotInExistingT_Temp_SchedulesMaster;
            //DataTable existingRowsThatDifferFromNewT_Temp_SchedulesMaster;
            //schedulesMasterComparer.RunComparison(out existingRowsMissingFromNewT_Temp_SchedulesMaster, out newRowsNotInExistingT_Temp_SchedulesMaster, out existingRowsThatDifferFromNewT_Temp_SchedulesMaster);
            //this.OutputToFile("T_Temp_SchedulesMaster", existingRowsMissingFromNewT_Temp_SchedulesMaster, newRowsNotInExistingT_Temp_SchedulesMaster, existingRowsThatDifferFromNewT_Temp_SchedulesMaster);

            // T_Temp_schedulesDetails
            //var schedulesDetailsComparer = new Models.T_Temp_SchedulesDetailsComparer();
            //DataTable existingRowsMissingFromNewT_Temp_SchedulesDetails;
            //DataTable newRowsNotInExistingT_Temp_SchedulesDetails;
            //DataTable existingRowsThatDifferFromNewT_Temp_SchedulesDetails;
            //schedulesDetailsComparer.RunComparison(out existingRowsMissingFromNewT_Temp_SchedulesDetails, out newRowsNotInExistingT_Temp_SchedulesDetails, out existingRowsThatDifferFromNewT_Temp_SchedulesDetails);
            //this.OutputToFile("T_Temp_SchedulesDetails", existingRowsMissingFromNewT_Temp_SchedulesDetails, newRowsNotInExistingT_Temp_SchedulesDetails, existingRowsThatDifferFromNewT_Temp_SchedulesDetails);

            SmtpClient client = new SmtpClient();
            NetworkCredential basicCredential = new NetworkCredential(Properties.Settings.Default.SmtpUsername, Properties.Settings.Default.SmtpPassword);
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = Properties.Settings.Default.SmtpHost;
            client.Credentials = basicCredential;
            client.Send(mail);

            System.Console.WriteLine(string.Format("Summary email sent to {0}.", Properties.Settings.Default.ComparisonSummaryEmailsTo));
            System.Console.WriteLine("Comparisons complete.");
        }

        /// <summary>
        /// Writes all results to a file on disk and returns reference to the new file.
        /// File location is determined by config entry FilePathForResults.
        /// </summary>
        /// <returns>The full path and filename</returns>
        private string OutputToFile(string tableName, DataTable existingRowsMissingFromNew, DataTable newRowsNotInExisting, DataTable existingRowsThatDifferFromNew)
        {
            int maxRowsToExport = 10000;

            if (existingRowsMissingFromNew.Rows.Count > maxRowsToExport)
            {
                existingRowsMissingFromNew = existingRowsMissingFromNew.AsEnumerable().Skip(existingRowsMissingFromNew.Rows.Count - maxRowsToExport).CopyToDataTable<DataRow>();
            }

            if (newRowsNotInExisting.Rows.Count > maxRowsToExport)
            {
                newRowsNotInExisting = newRowsNotInExisting.AsEnumerable().Skip(newRowsNotInExisting.Rows.Count - maxRowsToExport).CopyToDataTable<DataRow>();
            }

            if (existingRowsThatDifferFromNew.Rows.Count > maxRowsToExport)
            {
                existingRowsThatDifferFromNew = existingRowsThatDifferFromNew.AsEnumerable().Skip(existingRowsThatDifferFromNew.Rows.Count - maxRowsToExport).CopyToDataTable<DataRow>();
            }

            SLDocument sl = new  SLDocument();

            SLPageSettings psGood = new SLPageSettings() { TabColor = System.Drawing.Color.LightGreen };
            SLPageSettings psBad = new SLPageSettings() { TabColor = System.Drawing.Color.OrangeRed };

            // First worksheet
            sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, "existingRowsMissingFromNew");
            sl.ImportDataTable(1, 1, existingRowsMissingFromNew, true);
            sl.InsertTable(sl.CreateTable(1, 1, existingRowsMissingFromNew.Rows.Count + 1, existingRowsMissingFromNew.Columns.Count));
            sl.SetPageSettings(existingRowsMissingFromNew.Rows.Count==0? psGood : psBad);

            // Second worksheet
            sl.AddWorksheet("newRowsNotInExisting");
            sl.ImportDataTable(1, 1, newRowsNotInExisting, true);
            sl.InsertTable(sl.CreateTable(1, 1, newRowsNotInExisting.Rows.Count + 1, newRowsNotInExisting.Columns.Count));
            sl.SetPageSettings(newRowsNotInExisting.Rows.Count == 0 ? psGood : psBad);

            // Third worksheet
            sl.AddWorksheet("existingRowsThatDifferFromNew");
            sl.ImportDataTable(1, 1, existingRowsThatDifferFromNew, true);
            sl.InsertTable(sl.CreateTable(1, 1, existingRowsThatDifferFromNew.Rows.Count + 1, existingRowsThatDifferFromNew.Columns.Count));

            int rowCounter = 1;
            foreach (DataRow row in existingRowsThatDifferFromNew.Rows)
            {
                rowCounter++;
                for (int colCounter = 0; colCounter < (existingRowsThatDifferFromNew.Columns.Count-1); colCounter = colCounter + 2)
                {
                    if(!row[colCounter].Equals(row[colCounter+1]))
                    {
                        // This data is different so we highlight it.
                        sl.ApplyNamedCellStyle(rowCounter, (colCounter+2), SLNamedCellStyleValues.Bad);
                    }
                }
            }
            sl.SetPageSettings(existingRowsThatDifferFromNew.Rows.Count == 0 ? psGood : psBad);

            // Save to disk
            string filePath = Properties.Settings.Default.FilePathForResults;
            if (filePath.Substring(filePath.Length-1,1) != "\\")
            {
                filePath = filePath + "\\";
            }
            string fileNameWithPath = string.Format("{0}{1}{2}.xlsx", filePath, tableName, DateTime.Now.ToString("yyyyMMddThhmmss"));
            sl.SaveAs(fileNameWithPath);

            return fileNameWithPath;
        }

        /// <summary>
        /// Formats a table comparison result summary in HTML format.
        /// </summary>
        /// <returns>A HTML strring.</returns>
        string GetTableComparisonHtmlSummary(string tableName, int rowsExisting, int rowsNew, int rowsExistingExtra, int rowsNewExtra, int rowsDiffering, string fileName)
        {
            string output = "<br />";
            output += "<h2>T_Temp_Trips:</h2>";
            output += string.Format("<br />{0} rows in existing, {1} rows in new.", rowsExisting, rowsNew);
            output += string.Format("<br />{0} rows that match exactly between new and existing.", Math.Max(0, rowsExisting - rowsExistingExtra - rowsDiffering));
            output += string.Format("<br />{0} rows in existing that are not present in new.", rowsExistingExtra);
            output += string.Format("<br />{0} rows in new that are not present in existing.", rowsNewExtra);
            output += string.Format("<br />{0} rows in both new and existing that differ in some way.", rowsDiffering);
            output += string.Format("<br />See file {0} on the comparison server for more information, or see attached.", fileName);
            return output;
        }
    }
}
