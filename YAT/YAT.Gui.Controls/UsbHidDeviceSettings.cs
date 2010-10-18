//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.ComponentModel;
using System.Windows.Forms;

using MKY.IO.Serial;
using MKY.System.Event;

namespace YAT.Gui.Controls
{
	/// <summary></summary>
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("AutoReopenChanged")]
	public partial class UsbHidDeviceSettings : UserControl
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isSettingControls = false;

		private AutoRetry autoReopen = MKY.IO.Serial.UsbHidDeviceSettings.AutoReopenDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the AutoReopen property is changed.")]
		public event EventHandler AutoReopenChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public UsbHidDeviceSettings()
		{
			InitializeComponent();
			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Category("Socket")]
		[Description("Sets auto reopen.")]
		public AutoRetry AutoReopen
		{
			get { return (this.autoReopen); }
			set
			{
				if (value != this.autoReopen)
				{
					this.autoReopen = value;
					SetControls();
					OnAutoReopenChanged(new EventArgs());
				}
			}
		}

		#endregion

		#region Control Event Handlers
		//==========================================================================================
		// Control Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void UsbHidPortSettings_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;
				SetControls();
			}
		}

		/// <summary>
		/// Ensure that all controls are cleared when control gets disabled.
		/// </summary>
		private void UsbHidDeviceSettings_EnabledChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_AutoReopen_CheckedChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				MKY.IO.Serial.AutoRetry ar = this.autoReopen;
				ar.Enabled = checkBox_AutoReopen.Checked;
				AutoReopen = ar;
			}
		}

		private void textBox_AutoReopenInterval_Validating(object sender, CancelEventArgs e)
		{
			if (!this.isSettingControls)
			{
				int interval;
				if (int.TryParse(textBox_AutoReopenInterval.Text, out interval) && (interval >= 100))
				{
					MKY.IO.Serial.AutoRetry ar = this.autoReopen;
					ar.Interval = interval;
					AutoReopen = ar;
				}
				else
				{
					MessageBox.Show
						(
						this,
						"Reopen interval must be at least 100 ms!",
						"Invalid Input",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
						);
					e.Cancel = true;
				}
			}
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			this.isSettingControls = true;

			if (Enabled)
			{
				bool autoReopenEnabled = this.autoReopen.Enabled;
				checkBox_AutoReopen.Checked = autoReopenEnabled;
				textBox_AutoReopenInterval.Enabled = autoReopenEnabled;
				textBox_AutoReopenInterval.Text = this.autoReopen.Interval.ToString();
			}
			else
			{
				checkBox_AutoReopen.Checked = false;
				textBox_AutoReopenInterval.Enabled = false;
				textBox_AutoReopenInterval.Text = "";
			}

			this.isSettingControls = false;
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnAutoReopenChanged(EventArgs e)
		{
			EventHelper.FireSync(AutoReopenChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
