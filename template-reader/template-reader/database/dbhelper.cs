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
        private List<object> GetList(string sqlStatement)
        {
            var res = new List<object>();
            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand(sqlStatement) { Connection = conn })
            {
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var item = reader[0];
                    if (item == DBNull.Value)
                        continue;

                    res.Add(reader[0]);
                }
                conn.Close();
            }
            return res;
        }
        //
        internal List<string> GetListText(string sqlStatement)
        {
            var results = GetList(sqlStatement);
            return (from item in results
                    select Convert.ToString(item)).ToList();
        }

        internal List<int> GetIntList(string sqlStatement)
        {
            var results = GetList(sqlStatement);
            return (from item in results
                    select Convert.ToInt32(item)).ToList();
        }

        //internal List<int> GetIntList(string sqlStatement)
        //{
        //    var results = GetList(sqlStatement);
        //    return (from item in results
        //            select Convert.ToInt32(item)).ToList();
        //}

        internal int GetScalar(string sqlString)
        {
            var res = -1;
            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand(sqlString) { Connection = conn })
            {
                conn.Open();
                var dbRes = cmd.ExecuteScalar();
                res = Convert.ToInt32(dbRes);
                conn.Close();
            }
            return res;
        }
    }
}
