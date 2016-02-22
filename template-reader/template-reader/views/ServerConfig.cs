using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using template_reader.database;
using template_reader.model;

namespace template_reader.views
{
    public partial class frmServerConfig : Form
    {
        public frmServerConfig()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            var connBuilder = new ConnectionBuilder() {  DatabaseName= tDatabaseName.Text, InstanceName = tInstanceName.Text, ServerName = tServerName.Text };
            if (connBuilder.IsValid())
            {
                MessageBox.Show("Database connection successful");
            }
            else
            {
                DefaultConnectionBuilder = null;
            }
        }

        public ConnectionBuilder DefaultConnectionBuilder = null;

        public ProjectName SelectedProject { get; internal set; }
        //public object DefaultConnectionBuilder { get; internal set; }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            var connBuilder = new ConnectionBuilder() { DatabaseName = tDatabaseName.Text, InstanceName = tInstanceName.Text, ServerName = tServerName.Text };
            if (connBuilder.IsValid())
            {
                DefaultConnectionBuilder = connBuilder;
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                DefaultConnectionBuilder = null;
                MessageBox.Show("Connection attempt unsuccessful");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DefaultConnectionBuilder = null;
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
