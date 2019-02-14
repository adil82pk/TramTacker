using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.SideBySideTests.Models
{
    internal class T_Temp_SchedulesMasterComparer : TramTrackerComparer
    {
        public T_Temp_SchedulesMasterComparer()
        {
            base.TableName = "T_Temp_SchedulesMaster";
        }

        /// <summary>
        /// Returns a DataTable listing SchedulesMaster records that are present in Existing but not in New.
        /// </summary>
        public override DataTable GetExistingRowsMissingFromNew()
        {
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesMasterDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesMasterDataTable newRows = CastRows(base.NewData);

            var excludedIDs = new HashSet<string>(newRows.Select(n => n.TripNo));
            var existingMasterShedulesRecordsMissingFromNew = existingRows.Where(e => !excludedIDs.Contains(e.TripNo));

            if (existingMasterShedulesRecordsMissingFromNew.Count() > 0)
            {
                return existingMasterShedulesRecordsMissingFromNew.CopyToDataTable(); 
            }
            else
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// Returns a DataTable listing SchedulesMaster records that are present in New but are not present in Existing.
        /// </summary>
        public override DataTable GetNewRowsNotInExisting()
        {
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesMasterDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesMasterDataTable newRows = CastRows(base.NewData);

            var excludedIDs = new HashSet<string>(existingRows.Select(e => e.TripNo));
            var newSchedulesMasterRecordsNotInExisting = newRows.Where(n => !excludedIDs.Contains(n.TripNo));

            if (newSchedulesMasterRecordsNotInExisting.Count() > 0)
            {
                return newSchedulesMasterRecordsNotInExisting.CopyToDataTable();
            }
            else
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// Returns a list of matching SchedulesMaster records that differ in some way.
        /// </summary>
        public override List<RowPair> GetDifferingRows()
        {
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesMasterDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesMasterDataTable newRows = CastRows(base.NewData);

            var existingMasterRecordsThatDifferFromNew = from existingRecord in existingRows
                                                 from newRecord in newRows.Where(mapping => mapping.TripNo.Trim() == existingRecord.TripNo.Trim())
                                                 where !(existingRecord.TramClass == newRecord.TramClass
                                                        && existingRecord.HeadboardNo == newRecord.HeadboardNo
                                                        && existingRecord.RouteNo == newRecord.RouteNo
                                                        && existingRecord.RunNo == newRecord.RunNo
                                                        && existingRecord.StartDate == newRecord.StartDate
                                                        && existingRecord.TripNo == newRecord.TripNo
                                                        && existingRecord.PublicTrip == newRecord.PublicTrip)
                                                 select new RowPair { ExistingRow = existingRecord, NewRow = newRecord };
            
            return existingMasterRecordsThatDifferFromNew.ToList<RowPair>();
        }

        /// <summary>
        /// Converts a generic DataTable in to a (typed) T_Temp_SchedulesMasterDataTable.
        /// </summary>
        private Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesMasterDataTable CastRows(DataTable rows)
        {
            var castedRows = new Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_SchedulesMasterDataTable();
            castedRows.Merge(rows);

            return castedRows;
        }
    }
}
