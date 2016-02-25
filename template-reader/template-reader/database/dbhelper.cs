﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using template_reader.model;

namespace template_reader.database
{
    public class DbFactory
    {
        static DbFactory _factoryInstance;
        public static DbFactory Instance
        {
            get
            {
                if (_factoryInstance == null)
                {
                    _factoryInstance = new DbFactory();
                }
                return _factoryInstance;
            }
        }

        public ConnectionBuilder ConnBuilder { get; private set; }
        private ConnectionBuilder GetDefaultConnection(ProjectName projectName, bool getAlternate = false)
        {
            var defaultServerName = "ZM-VLUS56";
            var defaultSqlExpress = string.Empty;
            if (getAlternate)
            {
                defaultServerName = "D-5932S32";
                defaultSqlExpress = "SQL2014";
            }

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
            return connBuilder;
        }

        internal void OverwriteDefaultConnection(ConnectionBuilder connBuilder)
        {
            ConnBuilder = connBuilder;
        }

        internal ConnectionBuilder GetAlternateConnection(ConnectionBuilder connBuilder)
        {
            return GetDefaultConnection(_currentProjectName, true);
        }

        ProjectName _currentProjectName;
        internal ConnectionBuilder SetProjectDatabase(ProjectName selectedProject)
        {
            _currentProjectName = selectedProject;
            return ConnBuilder = GetDefaultConnection(_currentProjectName);
        }

        public DbHelper GetDbHelper()
        {
            return new DbHelper(ConnBuilder);
        }
    }

    public class ConnectionBuilder
    {
        //static string connString = @"Data Source = D-5932S32\SQLEXPRESS; Initial Catalog = JhpiegoDb; Integrated Security = true";
        const string connStringX = @"Data Source = {0}\{1}; Initial Catalog = {2}; Integrated Security = true";
        const string connStringForDefaultInstance = @"Data Source = {0}; Initial Catalog = {1}; Integrated Security = true";

        public string GetConnectionString()
        {
            if (string.IsNullOrWhiteSpace(InstanceName))
            {
                return string.Format(connStringForDefaultInstance, ServerName, DatabaseName);
            }
            return string.Format(connStringX, ServerName, InstanceName, DatabaseName);
        }
        public string ServerName { get; set; }
        public string InstanceName { get; set; }

        internal bool IsValid()
        {
            return true;
        }

        public string DatabaseName { get; set; }
        public string ConnectionString { get; internal set; }
        //public static string DefaultInstanceName = "default";
        //public bool IntegratedSecurity { get; set; }
        //public string UserName { get; set; }
        //public string Password { get; set; }
    }

    public class DbHelper
    {
        ConnectionBuilder _connBuilder = null;
        //ProjectName _projectName;
        string _connString;
        public DbHelper(ConnectionBuilder builder)
        {
            _connBuilder = builder;
            _connString = _connBuilder.GetConnectionString();
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
