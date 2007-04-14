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
	[DefaultEvent("BaudRateChanged")]
	public partial class SerialPortSettings : UserControl
	{
		//------------------------------------------------------------------------------------------
		// Constants
		//------------------------------------------------------------------------------------------

		private const IO.Ports.BaudRate        _BaudRateDefault  = IO.Ports.BaudRate.Baud009600;
		private const IO.Ports.DataBits        _DataBitsDefault  = IO.Ports.DataBits.Eight;
		private const System.IO.Ports.Parity   _ParityDefault    = System.IO.Ports.Parity.None;
		private const System.IO.Ports.StopBits _StopBitsDefault  = System.IO.Ports.StopBits.One;
		private const Domain.IO.Handshake      _HandshakeDefault = Domain.IO.Handshake.None;

		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private bool _isSettingControls = false;

		private IO.Ports.BaudRate        _baudRate  = _BaudRateDefault;
		private IO.Ports.DataBits        _dataBits  = _DataBitsDefault;
		private System.IO.Ports.Parity   _parity    = _ParityDefault;
		private System.IO.Ports.StopBits _stopBits  = _StopBitsDefault;
		private Domain.IO.Handshake      _handshake = _HandshakeDefault;

		//------------------------------------------------------------------------------------------
		// Events
		//------------------------------------------------------------------------------------------

		[Category("Property Changed")]
		[Description("Event raised when the BaudRate property is changed.")]
		public event EventHandler BaudRateChanged;

		[Category("Property Changed")]
		[Description("Event raised when the DataBits property is changed.")]
		public event EventHandler DataBitsChanged;

		[Category("Property Changed")]
		[Description("Event raised when the Parity property is changed.")]
		public event EventHandler ParityChanged;

		[Category("Property Changed")]
		[Description("Event raised when the StopBits property is changed.")]
		public event EventHandler StopBitsChanged;

		[Category("Property Changed")]
		[Description("Event raised when the Handshake property is changed.")]
		public event EventHandler HandshakeChanged;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------
	
		public SerialPortSettings()
		{
			InitializeComponent();

			comboBox_BaudRate.Items.AddRange(IO.Ports.XBaudRate.GetItems());
			comboBox_DataBits.Items.AddRange(IO.Ports.XDataBits.GetItems());
			comboBox_Parity.Items.AddRange(IO.Ports.XParity.GetItems());
			comboBox_StopBits.Items.AddRange(IO.Ports.XStopBits.GetItems());
			comboBox_Handshake.Items.AddRange(Domain.IO.XHandshake.GetItems());
			SetControls();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[Category("Serial Port")]
		[Description("The baud rate.")]
		[DefaultValue(_BaudRateDefault)]
		public IO.Ports.BaudRate BaudRate
		{
			get { return (_baudRate); }
			set
			{
				if (_baudRate != value)
				{
					_baudRate = value;
					SetControls();
					OnBaudRateChanged(new EventArgs());
				}
			}
		}

		[Category("Serial Port")]
		[Description("The number of data bits.")]
		[DefaultValue(_DataBitsDefault)]
		public IO.Ports.DataBits DataBits
		{
			get { return (_dataBits); }
			set
			{
				if (_dataBits != value)
				{
					_dataBits = value;
					SetControls();
					OnDataBitsChanged(new EventArgs());
				}
			}
		}

		[Category("Serial Port")]
		[Description("The partiy type.")]
		[DefaultValue(_ParityDefault)]
		public System.IO.Ports.Parity Parity
		{
			get { return (_parity); }
			set
			{
				if (_parity != value)
				{
					_parity = value;
					SetControls();
					OnParityChanged(new EventArgs());
				}
			}
		}

		[Category("Serial Port")]
		[Description("The number of stop bits.")]
		[DefaultValue(_StopBitsDefault)]
		public System.IO.Ports.StopBits StopBits
		{
			get { return (_stopBits); }
			set
			{
				if (_stopBits != value)
				{
					_stopBits = value;
					SetControls();
					OnStopBitsChanged(new EventArgs());
				}
			}
		}

		[Category("Serial Port")]
		[Description("The handshake type.")]
		[DefaultValue(_HandshakeDefault)]
		public Domain.IO.Handshake Handshake
		{
			get { return (_handshake); }
			set
			{
				if (_handshake != value)
				{
					_handshake = value;
					SetControls();
					OnHandshakeChanged(new EventArgs());
				}
			}
		}

		#endregion

		#region Controls Event Handlers
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers
		//------------------------------------------------------------------------------------------

		private void comboBox_BaudRate_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				BaudRate = (IO.Ports.XBaudRate)comboBox_BaudRate.SelectedItem;
		}

		private void comboBox_DataBits_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				DataBits = (IO.Ports.XDataBits)comboBox_DataBits.SelectedItem;
		}

		private void comboBox_Parity_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				Parity = (IO.Ports.XParity)comboBox_Parity.SelectedItem;
		}

		private void comboBox_StopBits_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				StopBits = (IO.Ports.XStopBits)comboBox_StopBits.SelectedItem;
		}

		private void comboBox_Handshake_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				Handshake = (Domain.IO.XHandshake)comboBox_Handshake.SelectedItem;
		}

		#endregion

		#region Private Methods
		//------------------------------------------------------------------------------------------
		// Private Methods
		//------------------------------------------------------------------------------------------

		private void SetControls()
		{
			_isSettingControls = true;

			comboBox_BaudRate.SelectedItem = (IO.Ports.XBaudRate)_baudRate;
			comboBox_DataBits.SelectedItem = (IO.Ports.XDataBits)_dataBits;
			comboBox_Parity.SelectedItem = (IO.Ports.XParity)_parity;
			comboBox_StopBits.SelectedItem = (IO.Ports.XStopBits)_stopBits;
			comboBox_Handshake.SelectedItem = (Domain.IO.XHandshake)_handshake;

			_isSettingControls = false;
		}

		#endregion

		#region Event Invoking
		//------------------------------------------------------------------------------------------
		// Event Invoking
		//------------------------------------------------------------------------------------------

		protected virtual void OnBaudRateChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(BaudRateChanged, this, e);
		}

		protected virtual void OnDataBitsChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(DataBitsChanged, this, e);
		}

		protected virtual void OnParityChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(ParityChanged, this, e);
		}

		protected virtual void OnStopBitsChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(StopBitsChanged, this, e);
		}

		protected virtual void OnHandshakeChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(HandshakeChanged, this, e);
		}

		#endregion
	}
}
