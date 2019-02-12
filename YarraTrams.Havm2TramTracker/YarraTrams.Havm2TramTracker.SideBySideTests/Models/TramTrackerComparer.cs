using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.SideBySideTests.Models
{
    abstract class TramTrackerComparer
    {
        protected string TableName;

        protected DataTable ExistingData;
        protected DataTable NewData;

        private const int fieldLength = 9;

        public abstract DataTable GetExistingRowsMissingFromNew();

        public abstract DataTable GetNewRowsNotInExisting();

        public abstract List<RowPair> GetDifferingRows();

        /// <summary>
        /// Compare data between a TramTRACKER table populated via Existing means and a TramTRACKER table populated via New means.
        /// Looks for:
        /// - Rows in Existing that are missing from New
        /// - Rows in New that are not present in Existing
        /// - Rows with matching key data between Existing and New but with differing detail
        /// 
        /// Prints results to console (for the time being).
        /// </summary>
        public void RunComparison()
        {
            ExistingData = GetData(Properties.Settings.Default.TramTrackerExisting, TableName);
            NewData = GetData(Properties.Settings.Default.TramTrackerNew, TableName);

            // Existing rows missing from New
            var existingRowsMissingFromNew = GetExistingRowsMissingFromNew();

            System.Console.WriteLine($"Missing rows from new data: {existingRowsMissingFromNew.Rows.Count}");
            System.Console.Write(outputRawDataRows(existingRowsMissingFromNew));

            // New rows not in Existing
            var newRowsNotInExisting = GetNewRowsNotInExisting();

            System.Console.WriteLine($"Extra rows in new data: {newRowsNotInExisting.Rows.Count}");
            System.Console.Write(outputRawDataRows(newRowsNotInExisting));

            // Rows in both Existing and New that differ
            var existingRowsThatDifferFromNew = GetDifferingRows();

            System.Console.WriteLine($"Matching rows that differ in some way: {existingRowsThatDifferFromNew.Count()}");
            System.Console.Write(outputComparisonRows(existingRowsThatDifferFromNew));
        }

        /// <summary>
        /// Populates a DataTable using data from the database specified in the passed-in connection string.
        /// </summary>
        private DataTable GetData(string conn, string tableName)
        {
            System.Console.WriteLine($"Using connection{conn}:");
            System.Console.WriteLine($"...populating {tableName}");
            DataTable dt = new DataTable();
            using (var da = new SqlDataAdapter($"SELECT TOp 50 * FROM {tableName} WITH (NOLOCK)", conn))
            {
                da.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// Converts the passed-in DataTable to the a string.
        /// (This routine is to be replaced by something a little easier for a human to process - likely a spreadsheet.)
        /// </summary>
        private string outputRawDataRows(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return "";
            }
            else
            {
                var output = new StringBuilder();

                foreach (DataColumn col in dt.Columns)
                {
                    output.Append(col.ColumnName.PadRight(fieldLength));
                }
                output.Append("\n");

                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        string value = row[col.ColumnName].ToString();
                        output.Append(value.PadRight(fieldLength));
                    }
                    output.Append("\n");
                }
                return output.ToString();
            }
        }

        /// <summary>
        /// Converts the passed-in row pairs to a string.
        /// (This routine is to be replaced by something a little easier for a human to process - likely a spreadsheet.)
        /// </summary>
        private string outputComparisonRows(IList<RowPair> rowPairsForComparison)
        {
            if (rowPairsForComparison.Count == 0)
            {
                return "";
            }
            else
            {
                var output = new StringBuilder();

                // Create a data table definition that matches the data rows, so we can iterate the columns.
                List<DataRow> tmpRows = new List<DataRow> { rowPairsForComparison[0].ExistingRow };
                DataTable dt = tmpRows.CopyToDataTable();
                foreach (DataColumn col in dt.Columns)
                {
                    output.Append(col.ColumnName.PadRight(fieldLength * 2));
                }
                output.Append("\n");
                foreach (DataColumn col in dt.Columns)
                {
                    output.Append("Existing".PadRight(fieldLength));
                    output.Append("New".PadRight(fieldLength));
                }
                output.Append("\n");

                foreach (RowPair rowPair in rowPairsForComparison)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        //Emit the differing fields, not the identical fields
                        if (rowPair.ExistingRow[col.ColumnName].GetHashCode() != rowPair.NewRow[col.ColumnName].GetHashCode())
                        {
                            string existingValue = rowPair.ExistingRow[col.ColumnName].ToString();
                            string newValue = rowPair.ExistingRow[col.ColumnName].ToString();
                            output.Append(existingValue.PadRight(fieldLength));
                            output.Append(newValue.PadRight(fieldLength));
                        }
                        else
                        {
                            output.Append(new String(' ', fieldLength * 2));
                        }
                    }
                    output.Append("\n");
                }
                return output.ToString();
            }
        }
    }

    public struct RowPair
    {
        public DataRow ExistingRow { get; set; }
        public DataRow NewRow { get; set; }
    }
}
