using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using HSR.YAT.Settings.Application;

namespace HSR.YAT.Gui.Forms
{
	public partial class WelcomeScreen : Form
	{
		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		// settings
		private bool _applicationSettingsLoaded = false;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public WelcomeScreen()
		{
			InitializeComponent();

			label_Name.Text = Application.ProductName;
			if (VersionInfo.HasProductNamePostFix)
				label_Name.Text += VersionInfo.ProductNamePostFix;

			label_Version.Text = "Version " + Application.ProductVersion;
			label_Status.Text = "Loading settings...";
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		public bool ApplicationSettingsLoaded
		{
			get { return (_applicationSettingsLoaded); }
		}

		#endregion

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		private void timer_Startup_Tick(object sender, EventArgs e)
		{
			if (Opacity < 1.00)
			{
				Opacity += 0.05;
				Refresh();
			}
			else
			{
				timer_Startup.Stop();

				try
				{
					_applicationSettingsLoaded = ApplicationSettings.Load();
				}
				catch
				{
				}

				DialogResult = DialogResult.OK;
				Close();
			}
		}

		#endregion
	}
}
