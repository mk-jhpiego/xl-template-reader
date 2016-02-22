using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using template_reader.model;

namespace template_reader.database
{
    public class ConnectionBuilder
    {
        //static string connString = @"Data Source = D-5932S32\SQLEXPRESS; Initial Catalog = JhpiegoDb; Integrated Security = true";
        static string connStringX = @"Data Source = {0}\{1}; Initial Catalog = {2}; Integrated Security = true";

        public string GetConnectionString()
        {
            return string.Format(connStringX, ServerName, InstanceName, DatabaseName);
        }
        public string ServerName { get; set; }
        public string InstanceName { get; set; }

        internal bool IsValid()
        {
            return true;
        }

        public string DatabaseName { get; set; }
        //public bool IntegratedSecurity { get; set; }
        //public string UserName { get; set; }
        //public string Password { get; set; }
    }

    public class DbHelper
    {
        public DbHelper(bool f)
        {

        }

        static string _connString = string.Empty;
            //@"Data Source = D-5932S32\SQLEXPRESS; Initial Catalog = JhpiegoDb; Integrated Security = true";
        public ConnectionBuilder DefaultConnectionBuilder
        {
            get; private set;
        }
        //public SqlConnection GetConnection()
        //{
        //    var csHelper = new ConnectionBuilder() {DatabaseName= "D-5932S32", InstanceName = "SQLEXPRESS", ServerName = "JhpiegoDb" };
        //    return new SqlConnection(connString);
        //}

        public ConnectionBuilder SetDefaultConnection(ProjectName projectName)
        {
            var defaultServerName = "D-5932S32";
            var defaultSqlExpress = "SQLEXPRESS";
            var defaultDbName = "JhpiegoDb";
            ConnectionBuilder connBuilder = null;
            switch (projectName)
            {
                case ProjectName.DOD:
                    {
                        connBuilder = new ConnectionBuilder() { DatabaseName = "JhpiegoDb_DOD", InstanceName = defaultSqlExpress, ServerName = defaultServerName };
                        break;
                    }
                case ProjectName.IHP_VMMC:
                    {
                        connBuilder = new ConnectionBuilder() { DatabaseName = "JhpiegoDb_IhpVmmc", InstanceName = defaultSqlExpress, ServerName = defaultServerName };
                        break;
                    }
                case ProjectName.IHP_Capacity_Building_and_Training:
                    {
                        connBuilder = new ConnectionBuilder() { DatabaseName = "JhpiegoDb_IhpTraining", InstanceName = defaultSqlExpress, ServerName = defaultServerName };
                        break;
                    }
            }
            DefaultConnectionBuilder = connBuilder;
            _connString = DefaultConnectionBuilder.GetConnectionString();
            return connBuilder;
        }

        internal void ExecSql(string res)
        {
            using(var conn = new SqlConnection(_connString))
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
            using (var conn = new SqlConnection(_connString))
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
            using (var conn = new SqlConnection(_connString))
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
            using (var conn = new SqlConnection(_connString))
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
