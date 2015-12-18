using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using template_reader.Commands;
using template_reader.database;
using template_reader.excelProcessing;
using template_reader.model;
using System.Linq;
using System.Text;

namespace template_reader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void importDataAction()
        {
            var dataImporter = new GetValuesFromReport() { fileName = lblSelectedFile.Text }.DoDataImport();
        }

        delegate void refreshDisplay(DataSet ds);
        CodeRunner<List<DataValue>> _runner;

        volatile List<DataValue> valuesList = null;
        volatile DataSet valuesDataset = null;

        private void btnSelectFileToImport_Click(object sender, EventArgs e)
        {            
            var fileName = string.Empty;
            using (var x = new OpenFileDialog() { Filter = "*.xlsx|*.xlsx|*.xls|*.xls", CheckFileExists = true })
            {
                if (x.ShowDialog() == DialogResult.OK)
                {
                    fileName = x.FileName;
                    lblSelectedFile.Text = fileName;

                    EnableSaveButtons(false);
                    valuesList = null;
                    valuesDataset = null;
                }
                else
                {
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                _runner = new CodeRunner<List<DataValue>>()
                {
                    ShowSplash = true,
                    CodeToExcute = new GetValuesFromReport() { fileName = lblSelectedFile.Text },
                    AsyncCallBack = (q) =>
                    {
                        if (q == null)
                            return;

                        EnableSaveButtons(true);

                        valuesList = q;
                        valuesDataset = q.ToDataset();

                        if (dataGridView1.InvokeRequired)
                        {
                            dataGridView1.Invoke(
                                new refreshDisplay((s) => { dataGridView1.DataSource = s.Tables[0]; }),
                                valuesDataset);
                            return;
                        }
                        dataGridView1.DataSource = q.ToDataset().Tables[0];
                    }
                };
                _runner.Execute();
            }
        }

        private void EnableSaveButtons(bool controlState)
        {
            btnSaveToCsv.EnableControl(controlState);
            btnSaveToServer.EnableControl(controlState);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            btnSelectFileToImport_Click(sender, e);
        }

        private void btnUpdateProgramIndicatorsList_Click(object sender, EventArgs e)
        {
            //we update the file ProgramAreaIndicators.txt
            var res = new GetProgramAreaIndicators().UpdateProgramAreaIndicators();
            splitContainer1.Panel2.Controls.Clear();
            var rtb = new RichTextBox() { Dock = DockStyle.Fill };
            splitContainer1.Panel2.Controls.Add(rtb);
            rtb.Text = res;
        }

        private void btnSaveToCsv_Click(object sender, EventArgs e)
        {
            var fields = new List<string>() { "FacilityName", "ReportYear", "ReportMonth", "ProgramArea", "IndicatorId", "AgeGroup", "Sex", "IndicatorValue" };
            if (valuesList != null)
            {
                using (var writer = new StreamWriter("csvoutput.csv", false))
                {
                    writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", "FacilityName", "ReportYear", "ReportMonth", "ProgramArea", "IndicatorId", "AgeGroup", "Sex", "IndicatorValue"));
                    valuesList.ForEach(t =>
                    {
                        writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", t.FacilityName.csvDelim(), t.ReportYear, t.ReportMonth.csvDelim(), t.ProgramArea.csvDelim(), t.IndicatorId.csvDelim(), t.AgeGroup.csvDelim(true), t.Sex.csvDelim(), t.IndicatorValue));
                    });
                }
            }
        }

        private void btnSaveToServer_Click(object sender, EventArgs e)
        {
            //we get valuesDataset
            var ds = valuesDataset;
            if (ds.Tables.Count == 0)
            {
                MessageBox.Show("Nothing to export");
                return;
            }

            //initialise db
            var db = new DbHelper();

            //we copy to server
            var table = ds.Tables[0];

            var targetTable = "etl_" + Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            table.TableName = targetTable;

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

            db.ExecSql(res);

            //use bulkcopy to write table to db
            db.WriteTableToDb(targetTable, table);


            //we save to main table
            //var copyToPrimaryProc = "";
        }
    }
}
