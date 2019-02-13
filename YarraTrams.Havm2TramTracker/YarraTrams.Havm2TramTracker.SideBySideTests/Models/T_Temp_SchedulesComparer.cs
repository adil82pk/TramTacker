using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDataTable newRows = CastRows(base.NewData);

            var excludedIDs = new HashSet<int>(newRows.Select(n => (n.TripID.ToString() + "." + n.StopID).GetHashCode()));
            var existingSchedulesMissingFromNew = existingRows.Where(e => !excludedIDs.Contains((e.TripID.ToString() + "." + e.StopID).GetHashCode()));

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
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDataTable newRows = CastRows(base.NewData);

            var excludedIDs = new HashSet<int>(existingRows.Select(e => (e.TripID.ToString() + "." + e.StopID).GetHashCode()));
            var newSchedulesNotInExisting = newRows.Where(n => !excludedIDs.Contains((n.TripID.ToString() + "." + n.StopID).GetHashCode()));

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
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDataTable newRows = CastRows(base.NewData);

            var existingSchedulesThatDifferFromNew = from existingSchedules in existingRows
                                                    join newSchedules in newRows on new { existingSchedules.TripID, existingSchedules.StopID } equals new { newSchedules.TripID, newSchedules.StopID }
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

            return existingSchedulesThatDifferFromNew.ToList<RowPair>();
        }

        /// <summary>
        /// Converts a generic DataTable in to a (typed) T_Temp_SchedulesDataTable.
        /// </summary>
        private Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDataTable CastRows(DataTable rows)
        {
            var castedRows = new Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDataTable();
            castedRows.Merge(rows);

            return castedRows;
        }
    }
}
