using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.SideBySideTests.Models
{
    internal class T_Temp_TripsComparer : TramTrackerComparer
    {
        public T_Temp_TripsComparer()
        {
            base.TableName = "T_Temp_Trips";
        }

        /// <summary>
        /// Returns a DataTable listing Trips that are present in Existing but not in New.
        /// </summary>
        public override DataTable GetExistingRowsMissingFromNew()
        {
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_TripsDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_TripsDataTable newRows = CastRows(base.NewData);

            var excludedIDs = new HashSet<int>(newRows.Select(n => n.TripID));
            var existingTripsMissingFromNew = existingRows.Where(e => !excludedIDs.Contains(e.TripID));

            if (existingTripsMissingFromNew.Count() > 0)
            {
                return existingTripsMissingFromNew.CopyToDataTable(); 
            }
            else
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// Returns a DataTable listing Trips that are present in New but are not present in Existing.
        /// </summary>
        public override DataTable GetNewRowsNotInExisting()
        {
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_TripsDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_TripsDataTable newRows = CastRows(base.NewData);

            var excludedIDs = new HashSet<int>(existingRows.Select(e => e.TripID));
            var newTripsNotInExisting = newRows.Where(n => !excludedIDs.Contains(n.TripID));

            if (newTripsNotInExisting.Count() > 0)
            {
                return newTripsNotInExisting.CopyToDataTable();
            }
            else
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// Returns a list of matching Trips that differ in some way.
        /// </summary>
        public override List<RowPair> GetDifferingRows()
        {
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_TripsDataTable existingRows = CastRows(base.ExistingData);
            Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_TripsDataTable newRows = CastRows(base.NewData);

            var existingTripsThatDifferFromNew = from existingTrips in existingRows
                                                 from newTrips in newRows.Where(mapping => mapping.TripID == existingTrips.TripID)
                                                 where !(existingTrips.RunNo == newTrips.RunNo
                                                        && existingTrips.RouteNo == newTrips.RouteNo
                                                        && existingTrips.FirstTP == newTrips.FirstTP
                                                        && existingTrips.FirstTime == newTrips.FirstTime
                                                        && existingTrips.EndTP == newTrips.EndTP
                                                        && existingTrips.EndTime == newTrips.EndTime
                                                        && existingTrips.AtLayoverTime == newTrips.AtLayoverTime
                                                        && existingTrips.NextRouteNo == newTrips.NextRouteNo
                                                        && existingTrips.UpDirection == newTrips.UpDirection
                                                        && existingTrips.LowFloor == newTrips.LowFloor
                                                        && existingTrips.TripDistance == newTrips.TripDistance
                                                        && existingTrips.PublicTrip == newTrips.PublicTrip
                                                        && existingTrips.DayOfWeek == newTrips.DayOfWeek)
                                                 select new RowPair { ExistingRow = existingTrips, NewRow = newTrips };

            return existingTripsThatDifferFromNew.ToList<RowPair>();
        }

        /// <summary>
        /// Converts a generic DataTable in to a (typed) T_Temp_TripsDataTable.
        /// </summary>
        private Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_TripsDataTable CastRows(DataTable rows)
        {
            var castedRows = new Havm2TramTracker.Models.TramTrackerDataSet.T_Temp_TripsDataTable();
            castedRows.Merge(rows);

            return castedRows;
        }
    }
}
