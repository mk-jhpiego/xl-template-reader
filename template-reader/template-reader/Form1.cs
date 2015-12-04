using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace template_reader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelectFileToImport_Click(object sender, EventArgs e)
        {
            var fileName = string.Empty;
            using (var x = new OpenFileDialog() { Filter = "*.xlsx|*.xlsx|*.xls|*.xls", CheckFileExists = true })
            {
                if (x.ShowDialog() == DialogResult.OK)
                {
                    fileName = x.FileName;
                    lblSelectedFile.Text = fileName;

                    //if (MessageBox.Show("Do you want to conitnue and import the file " + fileName) == DialogResult.OK)
                    //{
                        //proceed and install
                        var dataImporter = new ReadTemplateValues() { fileName = lblSelectedFile.Text }.DoDataImport();

                        //var ds = new ReadTemplateValues() { fileName = lblSelectedFile.Text }.getValues();
                        //dataGridView1.DataSource = ds.Tables[1];
                    //}
                }
                else
                {
                    return;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            btnSelectFileToImport_Click(sender, e);
        }

        private void btnUpdateProgramIndicatorsList_Click(object sender, EventArgs e)
        {
            //we update the file ProgramAreaIndicators.txt
            new ReadTemplateValues() { fileName = lblSelectedFile.Text }.GetValuesReloaded();
        }

        private void btnUpdateProgramAreasList_Click(object sender, EventArgs e)
        {
            //we update the file requiredTemplateHeaders.json
        }
    }
}
