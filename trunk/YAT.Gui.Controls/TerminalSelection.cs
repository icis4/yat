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

namespace YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("TerminalTypeChanged")]
	public partial class TerminalSelection : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		// \fixme Replace this help text with a real help
		public static readonly string NewTerminalHelpText =
			"Text vs. Binary:" + Environment.NewLine +
			Environment.NewLine +
			"Use a text terminal for text based protocols:" + Environment.NewLine +
			"   - Characters 0x00..0x1F are treated as control characters" + Environment.NewLine +
			"   - Line break is done using end of line sequence (EOL), e.g. <CR><LF>" + Environment.NewLine +
			Environment.NewLine +
			"Use a binary terminal for binary protocols:" + Environment.NewLine +
			"   - Characters 0x00..0x1F are treated as normal data" + Environment.NewLine +
			"   - Line break is done based on various binary settings" + Environment.NewLine +
			Environment.NewLine +
			Environment.NewLine +
			"TCP/IP AutoSocket:" + Environment.NewLine +
			Environment.NewLine +
			"TCP/IP AutoSocket automatically determines whether to run as client or server. On start, it tries to" + Environment.NewLine +
			"   connect to a remote server and run as client. If this fails, it tries to run as server. Retry cycles and" + Environment.NewLine +
			"   random wait times always ensure proper operation." + Environment.NewLine +
			Environment.NewLine +
			Environment.NewLine +
			"TCP/IP and UDP/IP:" + Environment.NewLine +
			Environment.NewLine +
			"The remote host is the remote computer to connect to or an other program running on this machine." + Environment.NewLine +
			"Examples:" + Environment.NewLine +
			"   '127.0.0.1' is the IP v4 localhost" + Environment.NewLine +
			"   '::1' is the IP v6 localhost" + Environment.NewLine +
			Environment.NewLine +
			"The local interface is the network interface that on this machine that is used for this connection." + Environment.NewLine +
			"Examples:" + Environment.NewLine +
			"   '127.0.0.1' is the IP v4 loopback interface" + Environment.NewLine +
			"   '::1' is the IP v6 loopback interface" +
			Environment.NewLine +
			Environment.NewLine +
			"USB Ser/HID:" + Environment.NewLine +
			Environment.NewLine +
			"USB Ser/HID is a serial port using the standard USB HID profile. Ser/HID is no USB standard but used" + Environment.NewLine +
			"   by several device manufacturers which simply need to replace an 'old fashioned' serial interfcae." + Environment.NewLine +
			"   USB Ser/HID requires no additional driver to be installed on the computer.";

		private const Domain.TerminalType _TerminalTypeDefault = Domain.TerminalType.Text;
		private const Domain.IOType       _IOTypeDefault       = Domain.IOType.SerialPort;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isSettingControls = false;

		private Domain.TerminalType _terminalType = _TerminalTypeDefault;
		private Domain.IOType       _ioType       = _IOTypeDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Property Changed")]
		[Description("Event raised when the TerminalType property is changed.")]
		public event EventHandler TerminalTypeChanged;

		[Category("Property Changed")]
		[Description("Event raised when the IOType property is changed.")]
		public event EventHandler IOTypeChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public TerminalSelection()
		{
			InitializeComponent();

			comboBox_TerminalType.Items.AddRange(Domain.XTerminalType.GetItems());
			comboBox_IOType.Items.AddRange(Domain.XIOType.GetItems());

			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		[Category("Terminal")]
		[Description("The terminal type.")]
		[DefaultValue(_TerminalTypeDefault)]
		public Domain.TerminalType TerminalType
		{
			get { return (_terminalType); }
			set
			{
				if (_terminalType != value)
				{
					_terminalType = value;
					SetControls();
					OnTerminalTypeChanged(new EventArgs());
				}
			}
		}

		[Category("Terminal")]
		[Description("The port type.")]
		[DefaultValue(_IOTypeDefault)]
		public Domain.IOType IOType
		{
			get { return (_ioType); }
			set
			{
				if (_ioType != value)
				{
					_ioType = value;
					SetControls();
					OnIOTypeChanged(new EventArgs());
				}
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_TerminalType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				TerminalType = (Domain.XTerminalType)comboBox_TerminalType.SelectedItem;
		}

		private void comboBox_IOType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				IOType = (Domain.XIOType)comboBox_IOType.SelectedItem;
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			_isSettingControls = true;

			comboBox_TerminalType.SelectedItem = (Domain.XTerminalType)_terminalType;
			comboBox_IOType.SelectedItem = (Domain.XIOType)_ioType;

			_isSettingControls = false;
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnTerminalTypeChanged(EventArgs e)
		{
			EventHelper.FireSync(TerminalTypeChanged, this, e);
		}

		protected virtual void OnIOTypeChanged(EventArgs e)
		{
			EventHelper.FireSync(IOTypeChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
