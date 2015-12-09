using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using template_reader.Commands;
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
            new GetProgramAreaIndicators().UpdateProgramAreaIndicators();
        }

        private void btnUpdateProgramAreasList_Click(object sender, EventArgs e)
        {
            //we update the file requiredTemplateHeaders.json
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //var programAreaIndicators = new GetProgramAreaIndicators().LoadAllProgramDataElements();
        }

        private void btnSaveToCsv_Click(object sender, EventArgs e)
        {

        }
    }
}
