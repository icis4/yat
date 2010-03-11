//==================================================================================================
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MKY.Utilities.Event;
using MKY.IO.Serial;

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("AutoReconnectChanged")]
	public partial class UsbHidDeviceSettings : UserControl
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isSettingControls = false;

		private AutoRetry _autoReconnect = MKY.IO.Serial.UsbHidDeviceSettings.AutoReconnectDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
		[Description("Event raised when the AutoReconnect property is changed.")]
		public event EventHandler AutoReconnectChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

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

		[Category("Socket")]
		[Description("Sets auto reconnect.")]
		public AutoRetry AutoReconnect
		{
			get { return (_autoReconnect); }
			set
			{
				if (_autoReconnect != value)
				{
					_autoReconnect = value;
					SetControls();
					OnAutoReconnectChanged(new EventArgs());
				}
			}
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Startup flag only used in the following event handler.
		/// </summary>
		private bool _isStartingUp = true;

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		private void UsbHidPortSettings_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;
				SetControls();
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void checkBox_AutoReconnect_CheckedChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
			{
				MKY.IO.Serial.AutoRetry ar = _autoReconnect;
				ar.Enabled = checkBox_AutoReconnect.Checked;
				AutoReconnect = ar;
			}
		}

		private void textBox_AutoReconnectInterval_Validating(object sender, CancelEventArgs e)
		{
			if (!_isSettingControls)
			{
				int interval;
				if (int.TryParse(textBox_AutoReconnectInterval.Text, out interval) && (interval >= 100))
				{
					MKY.IO.Serial.AutoRetry ar = _autoReconnect;
					ar.Interval = interval;
					AutoReconnect = ar;
				}
				else
				{
					MessageBox.Show
						(
						this,
						"Reconnect interval must be at least 100 ms!",
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
			_isSettingControls = true;

			checkBox_AutoReconnect.Checked = _autoReconnect.Enabled;
			textBox_AutoReconnectInterval.Enabled = _autoReconnect.Enabled;
			textBox_AutoReconnectInterval.Text = _autoReconnect.Interval.ToString();

			_isSettingControls = false;
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnAutoReconnectChanged(EventArgs e)
		{
			EventHelper.FireSync(AutoReconnectChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
