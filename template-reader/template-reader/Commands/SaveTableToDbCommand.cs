using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using template_reader.database;
using template_reader.model;

namespace template_reader.Commands
{
    public class SaveTableToDbCommand : IQueryHelper<IEnumerable<string>>
    {
        //public SaveTableToDbCommand()
        //{
        //}
        public IDisplayProgress progressDisplayHelper { get; set; }
        public DataSet TargetDataset { get; internal set; }

        public IEnumerable<string> Execute()
        {
            //we copy to server
            var table = TargetDataset.Tables[0];
            var targetTable = table.TableName;
            //we create the table
            var builder = new StringBuilder();
            foreach (DataColumn dc in table.Columns)
            {
                builder.AppendFormat("");
            }

            var res =
                string.Format("create table {0} ({1})", targetTable,
                string.Join(",",
                (
            from DataColumn dc in table.Columns
            select dc.ColumnName + " varchar(250)")));

            //initialise db
            var db = new DbHelper();
            int recordsImported = -5;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(2, 0, 0)))
            {
                //and create the temp table
                db.ExecSql(res);

                //use bulkcopy to write table to db
                db.WriteTableToDb(targetTable, table);

                //we check how many records in the table
                recordsImported = db.GetScalar("select count(*) from " + targetTable);

                transaction.Complete();
            }
            MessageBox.Show("Records imported " + recordsImported);
            return new List<string>();
        }
    }
}
