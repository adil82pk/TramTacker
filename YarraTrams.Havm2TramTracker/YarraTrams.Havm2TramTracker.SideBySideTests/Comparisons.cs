using SpreadsheetLight;
using System;
using System.Data;
using System.Drawing;

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
            DataTable existingRowsMissingFromNewT_Temp_Trips;
            DataTable newRowsNotInExistingT_Temp_Trips;
            DataTable existingRowsThatDifferFromNewT_TempTrips;
            tripsComparer.RunComparison(out existingRowsMissingFromNewT_Temp_Trips, out newRowsNotInExistingT_Temp_Trips, out existingRowsThatDifferFromNewT_TempTrips);
            this.OutputToFile("T_Temp_Trips", existingRowsMissingFromNewT_Temp_Trips, newRowsNotInExistingT_Temp_Trips, existingRowsThatDifferFromNewT_TempTrips);

            //T_Temp_Schedules
            var schedulesComparer = new Models.T_Temp_SchedulesComparer();
            DataTable existingRowsMissingFromNewT_Temp_Schedules;
            DataTable newRowsNotInExistingT_Temp_Schedules;
            DataTable existingRowsThatDifferFromNewT_TempSchedules;
            schedulesComparer.RunComparison(out existingRowsMissingFromNewT_Temp_Schedules, out newRowsNotInExistingT_Temp_Schedules, out existingRowsThatDifferFromNewT_TempSchedules);
            this.OutputToFile("T_Temp_Schedules", existingRowsMissingFromNewT_Temp_Schedules, newRowsNotInExistingT_Temp_Schedules, existingRowsThatDifferFromNewT_TempSchedules);

            //T_Temp_SchedulesMaster
            var schedulesMasterComparer = new Models.T_Temp_SchedulesMasterComparer();
            DataTable existingRowsMissingFromNewT_Temp_SchedulesMaster;
            DataTable newRowsNotInExistingT_Temp_SchedulesMaster;
            DataTable existingRowsThatDifferFromNewT_Temp_SchedulesMaster;
            schedulesMasterComparer.RunComparison(out existingRowsMissingFromNewT_Temp_SchedulesMaster, out newRowsNotInExistingT_Temp_SchedulesMaster, out existingRowsThatDifferFromNewT_Temp_SchedulesMaster);
            this.OutputToFile("T_Temp_SchedulesMaster", existingRowsMissingFromNewT_Temp_SchedulesMaster, newRowsNotInExistingT_Temp_SchedulesMaster, existingRowsThatDifferFromNewT_Temp_SchedulesMaster);

            //T_Temp_schedulesDetails
            var schedulesDetailsComparer = new Models.T_Temp_SchedulesDetailsComparer();
            DataTable existingRowsMissingFromNewT_Temp_SchedulesDetails;
            DataTable newRowsNotInExistingT_Temp_SchedulesDetails;
            DataTable existingRowsThatDifferFromNewT_Temp_SchedulesDetails;
            schedulesDetailsComparer.RunComparison(out existingRowsMissingFromNewT_Temp_SchedulesDetails, out newRowsNotInExistingT_Temp_SchedulesDetails, out existingRowsThatDifferFromNewT_Temp_SchedulesDetails);
            this.OutputToFile("T_Temp_SchedulesDetails", existingRowsMissingFromNewT_Temp_SchedulesDetails, newRowsNotInExistingT_Temp_SchedulesDetails, existingRowsThatDifferFromNewT_Temp_SchedulesDetails);
        }

        private void OutputToFile(string tableName, DataTable existingRowsMissingFromNew, DataTable newRowsNotInExisting, DataTable existingRowsThatDifferFromNew)
        {
            SLDocument sl = new  SLDocument();

            SLPageSettings psGood = new SLPageSettings() { TabColor = System.Drawing.Color.LightGreen };
            SLPageSettings psBad = new SLPageSettings() { TabColor = System.Drawing.Color.OrangeRed };

            sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, "existingRowsMissingFromNew");
            sl.ImportDataTable(1, 1, existingRowsMissingFromNew, true);
            sl.InsertTable(sl.CreateTable(1, 1, existingRowsMissingFromNew.Rows.Count + 1, existingRowsMissingFromNew.Columns.Count));
            sl.SetPageSettings(existingRowsMissingFromNew.Rows.Count==0? psGood : psBad);

            sl.AddWorksheet("newRowsNotInExisting");
            sl.ImportDataTable(1, 1, newRowsNotInExisting, true);
            sl.InsertTable(sl.CreateTable(1, 1, newRowsNotInExisting.Rows.Count + 1, newRowsNotInExisting.Columns.Count));
            sl.SetPageSettings(newRowsNotInExisting.Rows.Count == 0 ? psGood : psBad);

            sl.AddWorksheet("existingRowsThatDifferFromNew");
            sl.ImportDataTable(1, 1, existingRowsThatDifferFromNew, true);
            sl.InsertTable(sl.CreateTable(1, 1, existingRowsThatDifferFromNew.Rows.Count + 1, existingRowsThatDifferFromNew.Columns.Count));
            sl.SetPageSettings(existingRowsThatDifferFromNew.Rows.Count == 0 ? psGood : psBad);

            string filePath = Properties.Settings.Default.FilePathForResults;
            if (filePath.Substring(filePath.Length-1,1) != "\\")
            {
                filePath = filePath + "\\";
            }
            sl.SaveAs(string.Format("{0}{1}{2}.xlsx", filePath, tableName, DateTime.Now.ToString("yyyyMMddThhmmss")));
        }
    }
}
