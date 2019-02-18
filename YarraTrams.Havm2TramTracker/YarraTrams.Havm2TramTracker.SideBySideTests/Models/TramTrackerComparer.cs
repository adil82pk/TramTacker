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
        public void RunComparison(out DataTable existingRowsMissingFromNew, out DataTable newRowsNotInExisting, out DataTable existingRowsThatDifferFromNew)
        {
            ExistingData = GetData(Properties.Settings.Default.TramTrackerExisting, TableName);
            NewData = GetData(Properties.Settings.Default.TramTrackerNew, TableName);

            System.Console.WriteLine(string.Format("Total existing rows: {0}", ExistingData.Rows.Count));
            System.Console.WriteLine(string.Format("Total new rows: {0}", NewData.Rows.Count));

            // Existing rows missing from New
            existingRowsMissingFromNew = GetExistingRowsMissingFromNew();

            System.Console.WriteLine(string.Format("Missing rows from new data: {0}", existingRowsMissingFromNew.Rows.Count));
            System.Console.Write(outputRawDataRows(existingRowsMissingFromNew));

            // New rows not in Existing
            newRowsNotInExisting = GetNewRowsNotInExisting();

            System.Console.WriteLine(string.Format("Extra rows in new data: {0}", newRowsNotInExisting.Rows.Count));
            System.Console.Write(outputRawDataRows(newRowsNotInExisting));

            // Rows in both Existing and New that differ
            List<RowPair> existingRowsThatDifferFromNewAsRowPairs = GetDifferingRows();
            existingRowsThatDifferFromNew = this.Convert(existingRowsThatDifferFromNewAsRowPairs);

            System.Console.WriteLine(string.Format("Matching rows that differ in some way: {0}", existingRowsThatDifferFromNewAsRowPairs.Count()));
            System.Console.Write(outputComparisonRows(existingRowsThatDifferFromNewAsRowPairs));
        }

        /// <summary>
        /// Populates a DataTable using data from the database specified in the passed-in connection string.
        /// </summary>
        private DataTable GetData(string conn, string tableName)
        {
            System.Console.WriteLine(string.Format("Using connection {0}:", conn));
            System.Console.WriteLine(string.Format("...populating {0}", tableName));
            DataTable dt = new DataTable();
            using (var da = new SqlDataAdapter(string.Format("SELECT * FROM {0} WITH (NOLOCK)", tableName), conn))
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
                    if(col.ColumnName.Length >= fieldLength)
                    {
                        output.Append(col.ColumnName.Substring(0,fieldLength-3)+"...");
                    }
                    else
                    {
                        output.Append(col.ColumnName.PadRight(fieldLength));
                    }
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
                            string newValue = rowPair.NewRow[col.ColumnName].ToString();
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

        private DataTable Convert(List<RowPair> input)
        {
            DataTable output = new DataTable();
            // Create a data table definition that matches the data rows, so we can iterate the columns.
            List<DataRow> tmpRows = new List<DataRow> { input[0].ExistingRow };
            DataTable dt = tmpRows.CopyToDataTable();
            
            foreach (DataColumn col in dt.Columns)
            {
                output.Columns.Add(string.Format("{0} Existing", col.ColumnName), col.DataType);
                output.Columns.Add(string.Format("{0} New", col.ColumnName), col.DataType);
            }

            foreach(RowPair rowPair in input)
            {
                DataRow dr = output.NewRow();
                int ii = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    dr[ii] = rowPair.ExistingRow[col.Ordinal];
                    ii++;
                    dr[ii] = rowPair.NewRow[col.Ordinal];
                    ii++;
                }
                output.Rows.Add(dr);
            }

            return output;
        }
    }

    public struct RowPair
    {
        public DataRow ExistingRow { get; set; }
        public DataRow NewRow { get; set; }
    }
}
