using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data
{
    public static class DataRowExtensions
    {
        //Todo: This becomes unecessary if/when we change to using pure SQL instead of bulk copy.
        public static string ToLogString(this DataRow row)
        {
            var output = new StringBuilder();
            foreach(DataColumn col in row.Table.Columns)
            {
                output.AppendFormat("{0}: {1}{2}", col.ColumnName, row[col].ToString(), Environment.NewLine);
            }
            return output.ToString();
        }
    }
}
