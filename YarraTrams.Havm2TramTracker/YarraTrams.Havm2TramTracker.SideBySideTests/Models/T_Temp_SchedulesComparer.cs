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

            var existingSchedulesMissingFromNew = from existingSchedules in existingRows
                                                  from newSchedules in newRows.Where(mapping => mapping.TripID == existingSchedules.TripID && mapping.StopID == existingSchedules.StopID).DefaultIfEmpty()
                                                  where newSchedules.ScheduleID == 0
                                                  select (DataRow)existingSchedules;

            return existingSchedulesMissingFromNew.ToList().CopyToDataTable();
        }

        /// <summary>
        /// Returns a DataTable listing Schedules that are present in New but are not present in Existing.
        /// </summary>
        public override DataTable GetNewRowsNotInExisting()
        {
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDataTable newRows = CastRows(base.NewData);

            var newSchedulesNotInExisting = from newSchedules in newRows
                                            from existingSchedules in existingRows.Where(mapping => mapping.TripID == newSchedules.TripID && mapping.StopID == newSchedules.StopID).DefaultIfEmpty()
                                            where existingSchedules.ScheduleID == 0
                                            select (DataRow)newSchedules;

            return newSchedulesNotInExisting.ToList().CopyToDataTable();
        }

        /// <summary>
        /// Returns a list of matching Schedules that differ in some way.
        /// </summary>
        public override List<RowPair> GetDifferingRows()
        {
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDataTable newRows = CastRows(base.NewData);

            var existingSchedulesThatDifferFromNew = from existingSchedules in existingRows
                                                     from newSchedules in newRows.Where(mapping => mapping.TripID == existingSchedules.TripID && mapping.StopID == existingSchedules.StopID)
                                                     where !(existingSchedules.TripID == newSchedules.TripID
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
            castedRows.Merge(base.ExistingData);

            return castedRows;
        }
    }
}
