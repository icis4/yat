using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace HSR.YAT.Gui.Controls
{
	[DesignerCategory("Windows Forms")]
	[DefaultEvent("TerminalTypeChanged")]
	public partial class TerminalSelection : UserControl
	{
		//------------------------------------------------------------------------------------------
		// Constants
		//------------------------------------------------------------------------------------------

		// \fixme Replace this help text with a real help
		public static readonly string NewTerminalHelpText =
			"TCP/IP and UDP/IP:" + Environment.NewLine +
			Environment.NewLine +
			"The remote host is the remote computer to connect to or" + Environment.NewLine +
			"   an other program running on this machine" + Environment.NewLine +
			"Examples:" + Environment.NewLine +
			"   '127.0.0.1' is the IP localhost" + Environment.NewLine +
			"   '::1' is the IP version 6 localhost" + Environment.NewLine +
			Environment.NewLine +
			"The local interface is the network interface that on this" + Environment.NewLine +
			"   machine that is used for this connection" + Environment.NewLine +
			"Examples:" + Environment.NewLine +
			"   '127.0.0.1' is the IP loopback interface" + Environment.NewLine +
			"   '::1' is the IP version 6 loopback interface";

		private const Domain.TerminalType _TerminalTypeDefault = Domain.TerminalType.Text;
		private const Domain.IOType       _IOTypeDefault       = Domain.IOType.SerialPort;

		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private bool _isSettingControls = false;

		private Domain.TerminalType _terminalType = _TerminalTypeDefault;
		private Domain.IOType       _ioType       = _IOTypeDefault;

		//------------------------------------------------------------------------------------------
		// Events
		//------------------------------------------------------------------------------------------

		[Category("Property Changed")]
		[Description("Event raised when the TerminalType property is changed.")]
		public event EventHandler TerminalTypeChanged;

		[Category("Property Changed")]
		[Description("Event raised when the IOType property is changed.")]
		public event EventHandler IOTypeChanged;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public TerminalSelection()
		{
			InitializeComponent();

			comboBox_TerminalType.Items.AddRange(Domain.XTerminalType.GetItems());
			comboBox_IOType.Items.AddRange(Domain.XIOType.GetItems());

			SetControls();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

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
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

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
		//------------------------------------------------------------------------------------------
		// Private Methods
		//------------------------------------------------------------------------------------------

		private void SetControls()
		{
			_isSettingControls = true;

			comboBox_TerminalType.SelectedItem = (Domain.XTerminalType)_terminalType;
			comboBox_IOType.SelectedItem = (Domain.XIOType)_ioType;

			_isSettingControls = false;
		}

		#endregion

		#region Event Invoking
		//------------------------------------------------------------------------------------------
		// Event Invoking
		//------------------------------------------------------------------------------------------

		protected virtual void OnTerminalTypeChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(TerminalTypeChanged, this, e);
		}

		protected virtual void OnIOTypeChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(IOTypeChanged, this, e);
		}

		#endregion
	}
}
