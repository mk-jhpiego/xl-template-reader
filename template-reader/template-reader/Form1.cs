using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using template_reader.Commands;
using template_reader.database;
using template_reader.excelProcessing;
using template_reader.model;
using template_reader.views;

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
            var dataImporter = new GetValuesFromReport() { fileName = lblSelectedFile.Text, SelectedProject = CurrentProjectName }.DoDataImport(CurrentProjectName);
        }

        delegate void refreshDisplay(DataSet ds);
        CodeRunner<List<DataValue>> _runner;

        volatile List<DataValue> valuesList = null;
        volatile DataSet valuesDataset = null;

        public ProjectName CurrentProjectName;

        DbHelper _dbHelper = null;

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
                    CodeToExcute = new GetValuesFromReport() { fileName = lblSelectedFile.Text, SelectedProject = CurrentProjectName },
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
            //Output written to the file ProgramAreaDefinitions.json in the same folder where app will run from
            //copy contents into the file staticdata/ProgramAreaDefinitions.json and restart app
            var res = new GetProgramAreaIndicators().UpdateProgramAreaIndicators();
            splitContainer1.Panel2.Controls.Clear();
            var rtb = new RichTextBox() { Dock = DockStyle.Fill };
            splitContainer1.Panel2.Controls.Add(rtb);
            rtb.Text = res;
        }

        static List<string> fields = new List<string>() { "FacilityName", "ReportYear", "ReportMonth", "ProgramArea", "IndicatorId", "AgeGroup", "Sex", "IndicatorValue" };
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
            //we show where data will be saved to, prompt if there's need to chaneg the connection
            var serverConfig = new frmServerConfig() { StartPosition = FormStartPosition.CenterParent };
            if (serverConfig.ShowDialog() == DialogResult.OK)
            {
                //we get valuesDataset
                var ds = valuesDataset;
                if (ds.Tables.Count == 0)
                {
                    MessageBox.Show("Nothing to export");
                    return;
                }
                var tempTableName = new RandomTableNameGenerator().Execute();
                valuesDataset.Tables[0].TableName = tempTableName;

                var dataImporter = new SaveTableToDbCommand() { TargetDataset = valuesDataset };
                dataImporter.Execute();

                //we start the merge
                var dataMerge = new DataMergeCommand()
                {
                    TempTableName = tempTableName,
                    DestinationTable = "FacilityData"
                };

                // we save, 
                dataMerge.Execute();

                EnableSaveButtons(false);
                //and show the confirmatin that the file has been saved
                //perhaps show a tick
                //MessageBox.Show("Merge completed");
            }
            else
            {
                btnSaveToCsv.EnableControl(true);
                btnSaveToServer.EnableControl(false);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            var projectSelector = new ProgramSelector() { StartPosition = FormStartPosition.CenterParent };
            if (projectSelector.ShowDialog() == DialogResult.OK)
            {
                var connBuilder = DbFactory.Instance.SetProjectDatabase(projectSelector.SelectedProject);
                _dbHelper = DbFactory.Instance.GetDbHelper();
                CurrentProjectName = projectSelector.SelectedProject;

                //we set the appropriate settings to show this project
                Text += string.Format("  ({0})", projectSelector.SelectedProject.ToString().Replace('_', ' '));
                this.Visible = true;
            }
            else
            {
                Application.Exit();
            }
        }
    }
}
