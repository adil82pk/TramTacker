using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Models;

namespace YarraTrams.Havm2TramTracker.SideBySideTests.Models
{
    internal class T_Temp_SchedulesComparer : TramTrackerComparer
    {
        public T_Temp_SchedulesComparer()
        {
            base.TableName = "T_Temp_Schedules";
        }

        /// <summary>
        /// Returns a DataTable listing Schedules that are present in Existing but not in New.
        /// </summary>
        public override DataTable GetExistingRowsMissingFromNew()
        {
            TramTrackerDataSet.T_Temp_SchedulesDataTable existingRows = CastRows(base.ExistingData);
            TramTrackerDataSet.T_Temp_SchedulesDataTable newRows = CastRows(base.NewData);

            var excludedIDs = new HashSet<int>(newRows.Select(n => (n.TripID.ToString() + "." + n.StopID.Trim()).GetHashCode()));
            var existingSchedulesMissingFromNew = existingRows.Where(e => !excludedIDs.Contains((e.TripID.ToString() + "." + e.StopID.Trim()).GetHashCode()));

            if (existingSchedulesMissingFromNew.Count() > 0)
            {
                return existingSchedulesMissingFromNew.CopyToDataTable();
            }
            else
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// Returns a DataTable listing Schedules that are present in New but are not present in Existing.
        /// </summary>
        public override DataTable GetNewRowsNotInExisting()
        {
            TramTrackerDataSet.T_Temp_SchedulesDataTable existingRows = CastRows(base.ExistingData);
            TramTrackerDataSet.T_Temp_SchedulesDataTable newRows = CastRows(base.NewData);

            var excludedIDs = new HashSet<int>(existingRows.Select(e => (e.TripID.ToString() + "." + e.StopID.Trim()).GetHashCode()));
            var newSchedulesNotInExisting = newRows.Where(n => !excludedIDs.Contains((n.TripID.ToString() + "." + n.StopID.Trim()).GetHashCode()));

            if (newSchedulesNotInExisting.Count() > 0)
            {
                return newSchedulesNotInExisting.CopyToDataTable();
            }
            else
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// Returns a list of matching Schedules that differ in some way.
        /// </summary>
        public override List<RowPair> GetDifferingRows()
        {
            TramTrackerDataSet.T_Temp_SchedulesDataTable existingRows = CastRows(base.ExistingData);
            TramTrackerDataSet.T_Temp_SchedulesDataTable newRows = CastRows(base.NewData);

            List<RowPair> existingSchedulesThatDifferFromNew = new List<RowPair>();

            for (int dayOfWeek = 0; dayOfWeek < 7; dayOfWeek++)
            {
                string select = string.Format("DayOfWeek = {0}", dayOfWeek);

                // Copy subset of rows to working tables
                TramTrackerDataSet.T_Temp_SchedulesDataTable existingRowsSubset;
                if (existingRows.Select(select).Length > 0)
                {
                    existingRowsSubset = CastRows(existingRows.Select(select).CopyToDataTable());
                }
                else
                {
                    existingRowsSubset = new TramTrackerDataSet.T_Temp_SchedulesDataTable();
                }

                TramTrackerDataSet.T_Temp_SchedulesDataTable newRowsSubset;
                if (newRows.Select(select).Length > 0)
                {
                    newRowsSubset = CastRows(newRows.Select(select).CopyToDataTable());
                }
                else
                {
                    newRowsSubset = new TramTrackerDataSet.T_Temp_SchedulesDataTable();
                }

                // Remove working rows from base tables
                existingRows.Select(select).ToList<DataRow>().ForEach(x => existingRows.Rows.Remove(x));
                existingRows.AcceptChanges();
                newRows.Select(select).ToList<DataRow>().ForEach(x => newRows.Rows.Remove(x));
                newRows.AcceptChanges();

                var subsetOfExistingSchedulesThatDifferFromNew = from existingSchedules in existingRowsSubset
                                                                 join newSchedules in newRowsSubset on new { existingSchedules.TripID, StopID = existingSchedules.StopID.Trim() } equals new { newSchedules.TripID, StopID = newSchedules.StopID.Trim() }
                                                                 where !(existingSchedules.OPRTimePoint == newSchedules.OPRTimePoint
                                                                 && existingSchedules.RunNo == newSchedules.RunNo
                                                                 && existingSchedules.StopID == newSchedules.StopID
                                                                 && existingSchedules.RouteNo == newSchedules.RouteNo
                                                                 && existingSchedules.OPRTimePoint == newSchedules.OPRTimePoint
                                                                 && existingSchedules.Time == newSchedules.Time
                                                                 && existingSchedules.DayOfWeek == newSchedules.DayOfWeek
                                                                 && existingSchedules.LowFloor == newSchedules.LowFloor
                                                                 && existingSchedules.PublicTrip == newSchedules.PublicTrip)
                                                                 select new RowPair { ExistingRow = existingSchedules, NewRow = newSchedules };

                existingSchedulesThatDifferFromNew.AddRange(subsetOfExistingSchedulesThatDifferFromNew);
            }

            return existingSchedulesThatDifferFromNew;
        }

        /// <summary>
        /// Converts a generic DataTable in to a (typed) T_Temp_SchedulesDataTable.
        /// </summary>
        private TramTrackerDataSet.T_Temp_SchedulesDataTable CastRows(DataTable rows)
        {
            var castedRows = new TramTrackerDataSet.T_Temp_SchedulesDataTable();
            castedRows.Merge(rows);

            return castedRows;
        }
    }
}
