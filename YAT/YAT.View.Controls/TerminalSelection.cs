﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.Windows.Forms;

using MKY;
using MKY.Windows.Forms;

#endregion

namespace YAT.View.Controls
{
	/// <summary></summary>
	[DefaultEvent("TerminalTypeChanged")]
	public partial class TerminalSelection : UserControl
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <remarks>\fixme: Replace this help text with a real help.</remarks>
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
			"   random wait times ensure proper operation even when two AutoSockets are interconnected to each other." + Environment.NewLine +
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

		private const Domain.TerminalType TerminalTypeDefault = Domain.Settings.TerminalSettings.TerminalTypeDefault;
		private const Domain.IOType       IOTypeDefault       = Domain.Settings.IOSettings.IOTypeDefault;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SettingControlsHelper isSettingControls;

		private Domain.TerminalType terminalType = TerminalTypeDefault;
		private Domain.IOType       ioType       = IOTypeDefault;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the TerminalType property is changed.")]
		public event EventHandler TerminalTypeChanged;

		/// <summary></summary>
		[Category("Property Changed")]
		[Description("Event raised when the IOType property is changed.")]
		public event EventHandler IOTypeChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TerminalSelection()
		{
			InitializeComponent();

			comboBox_TerminalType.Items.AddRange(Domain.TerminalTypeEx.GetItems());
			comboBox_IOType.Items.AddRange(Domain.IOTypeEx.GetItems());

			// SetControls() is initially called in the 'Paint' event handler.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[Category("Terminal")]
		[Description("The terminal type.")]
		[DefaultValue(TerminalTypeDefault)]
		public Domain.TerminalType TerminalType
		{
			get { return (this.terminalType); }
			set
			{
				if (this.terminalType != value)
				{
					this.terminalType = value;
					SetControls();
					OnTerminalTypeChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary></summary>
		[Category("Terminal")]
		[Description("The port type.")]
		[DefaultValue(IOTypeDefault)]
		public Domain.IOType IOType
		{
			get { return (this.ioType); }
			set
			{
				if (this.ioType != value)
				{
					this.ioType = value;
					SetControls();
					OnIOTypeChanged(EventArgs.Empty);
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
		/// <remarks>
		/// Use paint event to ensure that message boxes in case of errors (e.g. validation errors)
		/// are shown on top of a properly painted control or form.
		/// </remarks>
		private void TerminalSelection_Paint(object sender, PaintEventArgs e)
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
		private void TerminalSelection_EnabledChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				SetControls();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void comboBox_TerminalType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				TerminalType = (Domain.TerminalTypeEx)comboBox_TerminalType.SelectedItem;
		}

		private void comboBox_IOType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				IOType = (Domain.IOTypeEx)comboBox_IOType.SelectedItem;
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		private void SetControls()
		{
			this.isSettingControls.Enter();

			if (Enabled)
			{
				comboBox_TerminalType.SelectedItem = (Domain.TerminalTypeEx)this.terminalType;
				comboBox_IOType.SelectedItem       = (Domain.IOTypeEx)this.ioType;
			}
			else
			{
				comboBox_TerminalType.SelectedIndex = ControlEx.InvalidIndex;
				comboBox_IOType.SelectedIndex       = ControlEx.InvalidIndex;
			}

			this.isSettingControls.Leave();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnTerminalTypeChanged(EventArgs e)
		{
			EventHelper.FireSync(TerminalTypeChanged, this, e);
		}

		/// <summary></summary>
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
