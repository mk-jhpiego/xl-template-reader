using System;
using System.Windows.Forms;
using template_reader.model;

namespace template_reader.views
{
    public partial class ProgramSelector : Form
    {
        public ProjectName SelectedProject { get; set; }

        public ProgramSelector()
        {
            InitializeComponent();
        }

        private void btnDod_Click(object sender, EventArgs e)
        {
            SelectedProject = ProjectName.DOD;
            DialogResult = DialogResult.OK;
        }

        private void btnVmmc_Click(object sender, EventArgs e)
        {
            SelectedProject = ProjectName.IHP_VMMC;
            DialogResult = DialogResult.OK;
        }

        private void btnCapacityBuilding_Click(object sender, EventArgs e)
        {
            SelectedProject = ProjectName.IHP_Capacity_Building_and_Training;
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
