using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace template_reader.database
{
    public class DbHelper
    {
        static string connString = @"Data Source = D-5932S32\SQLEXPRESS; Initial Catalog = JhpiegoDb; Integrated Security = true";
        public SqlConnection GetConnection()
        {
            return new SqlConnection(connString);
        }

        internal void ExecSql(string res)
        {
            using(var conn = new SqlConnection(connString))
            using(var cmd = new SqlCommand(res) {Connection = conn })
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        internal void WriteTableToDb(string targetTable, DataTable table)
        {
            //use SqlBulkCopy to write to the server
            using (var conn = new SqlConnection(connString))
            using (var bcp = new SqlBulkCopy(conn) { DestinationTableName = targetTable })
            {
                foreach (DataColumn c in table.Columns)
                {
                    bcp.ColumnMappings.Add(c.ColumnName, c.ColumnName);
                }
                conn.Open();
                table.AcceptChanges();
                bcp.WriteToServer(table);
                bcp.Close();
            }
        }
    }
}
