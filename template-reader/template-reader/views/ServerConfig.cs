using System;
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
            var connBuilder = BuildConnection();
            if (connBuilder.IsValid())
            {
                MessageBox.Show("Database connection successful");
            }
            else
            {
                MessageBox.Show("Failed to verify database connection");
            }
        }

        private ConnectionBuilder BuildConnection()
        {
            return new ConnectionBuilder() { DatabaseName = tDatabaseName.Text, InstanceName = tInstanceName.Text, ServerName = tServerName.Text };
        }

        public ProjectName SelectedProject { get; internal set; }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            var connBuilder = BuildConnection();
            if (!connBuilder.IsValid())
            {
                MessageBox.Show("Failed to verify database connection");
                return;
            }
            DbFactory.Instance.OverwriteDefaultConnection(connBuilder);
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void frmServerConfig_Load(object sender, EventArgs e)
        {
            DisplayConnectionSettings(DbFactory.Instance.ConnBuilder);
        }

        private void DisplayConnectionSettings(ConnectionBuilder connBuilder)
        {
            tDatabaseName.Text = connBuilder.DatabaseName;
            tInstanceName.Text = connBuilder.InstanceName;
            tServerName.Text = connBuilder.ServerName;
        }

        private void btnCycleConnection_Click(object sender, EventArgs e)
        {
            DisplayConnectionSettings(DbFactory.Instance.GetAlternateConnection(DbFactory.Instance.ConnBuilder));
        }
    }
}
