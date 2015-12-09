using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using template_reader.model;

namespace template_reader.views
{
    public partial class SplashScreen : Form, IDisplayProgress
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {

        }

        int progressStep = 10;
        int progressStepsRemaining = 100;
        public void PerformProgressStep(string message = "")
        {
            //we reduce time remaining
            progressStepsRemaining -= progressStep;
            lblMainMessage.SetText(message);
            prgMain.SetValue(Convert.ToInt32((100 - progressStepsRemaining) * 1.0));
        }

        public void MarkStartOfMultipleSteps(int stepsToExpect)
        {
            progressStep = Convert.ToInt32(progressStepsRemaining * 1.0 / stepsToExpect);
            prgMain.SetStepValue(progressStep);
        }

        int _totalSubProgressSteps = 0;
        int _totalSubSteps = 0;
        public void ResetSubProgressIndicator(int stepsToExpect)
        {
            _totalSubProgressSteps = stepsToExpect;
            _totalSubSteps = stepsToExpect;
        }

        public void PerformSubProgressStep()
        {
            //we reduce time remaining
            _totalSubProgressSteps--;
            //lblSubMessage.SetText(_totalSubProgressSteps.ToString());
            prgSubSteps.SetValue(Convert.ToInt32((_totalSubSteps - _totalSubProgressSteps) * 100.0 / _totalSubSteps));
        }
    }
}
