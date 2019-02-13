using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.SideBySideTests.Models
{
    internal class T_Temp_SchedulesDetailsComparer : TramTrackerComparer
    {
        public T_Temp_SchedulesDetailsComparer()
        {
            base.TableName = "T_Temp_SchedulesDetails";
        }

        /// <summary>
        /// Returns a DataTable listing SchedulesDetails that are present in Existing but not in New.
        /// </summary>
        public override DataTable GetExistingRowsMissingFromNew()
        {
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDetailsDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDetailsDataTable newRows = CastRows(base.NewData);

            var excludedIDs = new HashSet<int>(newRows.Select(n => (n.TripID.Trim() + "." + n.StopID.Trim()).GetHashCode()));
            var existingSchedulesDetailsMissingFromNew = existingRows.Where(e => !excludedIDs.Contains((e.TripID.Trim() + "." + e.StopID.Trim()).GetHashCode()));

            if (existingSchedulesDetailsMissingFromNew.Count() > 0)
            {
                return existingSchedulesDetailsMissingFromNew.CopyToDataTable();
            }
            else
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// Returns a DataTable listing SchedulesDetails that are present in New but are not present in Existing.
        /// </summary>
        public override DataTable GetNewRowsNotInExisting()
        {
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDetailsDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDetailsDataTable newRows = CastRows(base.NewData);

            var excludedIDs = new HashSet<int>(existingRows.Select(e => (e.TripID.Trim() + "." + e.StopID.Trim()).GetHashCode()));
            var newSchedulesDetailsNotInExisting = newRows.Where(n => !excludedIDs.Contains((n.TripID.Trim() + "." + n.StopID.Trim()).GetHashCode()));

            if (newSchedulesDetailsNotInExisting.Count() > 0)
            {
                return newSchedulesDetailsNotInExisting.CopyToDataTable();
            }
            else
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// Returns a list of matching SchedulesDetails that differ in some way.
        /// </summary>
        public override List<RowPair> GetDifferingRows()
        {
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDetailsDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDetailsDataTable newRows = CastRows(base.NewData);

            var existingSchedulesDetailsThatDifferFromNew = from existingSchedulesDetail in existingRows
                                                    join newSchedulesDetail in newRows on new { TripID = existingSchedulesDetail.TripID.Trim(), StopID = existingSchedulesDetail.StopID.Trim() } equals new { TripID = newSchedulesDetail.TripID.Trim(), StopID = newSchedulesDetail.StopID.Trim() }
                                                    where !(existingSchedulesDetail.ArrivalTime == newSchedulesDetail.ArrivalTime
                                                    && existingSchedulesDetail.StopID == newSchedulesDetail.StopID
                                                    && existingSchedulesDetail.TripID == newSchedulesDetail.TripID
                                                    && existingSchedulesDetail.RunNo == newSchedulesDetail.RunNo
                                                    && existingSchedulesDetail.OPRTimePoint == newSchedulesDetail.OPRTimePoint)
                                                    select new RowPair { ExistingRow = existingSchedulesDetail, NewRow = newSchedulesDetail };

            return existingSchedulesDetailsThatDifferFromNew.ToList<RowPair>();
        }

        /// <summary>
        /// Converts a generic DataTable in to a (typed) T_Temp_SchedulesDetailsDataTable.
        /// </summary>
        private Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDetailsDataTable CastRows(DataTable rows)
        {
            var castedRows = new Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesDetailsDataTable();
            castedRows.Merge(rows);

            return castedRows;
        }
    }
}
