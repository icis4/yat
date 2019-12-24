using System;
using System.Windows.Forms;

namespace ScottPlot.UserControls
{
    public partial class FormHelp : Form
    {
        public FormHelp()
        {
            InitializeComponent();
            lblVersion.Text = Tools.GetVersionString();
            btnScottPlotGithub.Select();
        }

        private void FormHelp_Load(object sender, EventArgs e)
        {

        }

        private void BtnScottPlotGithub_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/swharden/ScottPlot");
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
