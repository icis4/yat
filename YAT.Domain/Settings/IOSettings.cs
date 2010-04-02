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
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class IOSettings : MKY.Utilities.Settings.Settings, IEquatable<IOSettings>
	{
		/// <summary></summary>
		public const Domain.IOType IOTypeDefault = Domain.IOType.SerialPort;
		/// <summary></summary>
		public const string SerialParityErrorReplacementDefault = @"\h(00)";
		/// <summary></summary>
		public const Endianess EndianessDefault = Endianess.BigEndian;

		private Domain.IOType _ioType;
		private MKY.IO.Serial.SerialPortSettings _serialPort;
		private string _serialParityErrorReplacement;
		private MKY.IO.Serial.SocketSettings _socket;
		private MKY.IO.Serial.UsbHidDeviceSettings _usbHidDevice;
		private Endianess _endianess;

		/// <summary></summary>
		public IOSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		/// <summary></summary>
		public IOSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		private void InitializeNodes()
		{
			SerialPort   = new MKY.IO.Serial.SerialPortSettings(SettingsType);
			Socket       = new MKY.IO.Serial.SocketSettings(SettingsType);
			UsbHidDevice = new MKY.IO.Serial.UsbHidDeviceSettings(SettingsType);
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public IOSettings(IOSettings rhs)
			: base(rhs)
		{
			_ioType      = rhs.IOType;
			SerialPort   = new MKY.IO.Serial.SerialPortSettings(rhs.SerialPort);
			_serialParityErrorReplacement = rhs.SerialParityErrorReplacement;
			Socket       = new MKY.IO.Serial.SocketSettings(rhs.Socket);
			UsbHidDevice = new MKY.IO.Serial.UsbHidDeviceSettings(rhs.UsbHidDevice);
			_endianess   = rhs.Endianess;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			IOType = IOTypeDefault;
			SerialParityErrorReplacement = SerialParityErrorReplacementDefault;
			Endianess = EndianessDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("IOType")]
		public virtual Domain.IOType IOType
		{
			get { return (_ioType); }
			set
			{
				if (value != _ioType)
				{
					_ioType = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SerialPort")]
		public virtual MKY.IO.Serial.SerialPortSettings SerialPort
		{
			get { return (_serialPort); }
			set
			{
				if (_serialPort == null)
				{
					_serialPort = value;
					AttachNode(_serialPort);
				}
				else if (value != _serialPort)
				{
					MKY.IO.Serial.SerialPortSettings old = _serialPort;
					_serialPort = value;
					ReplaceNode(old, _serialPort);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SerialParityErrorReplacement")]
		public virtual string SerialParityErrorReplacement
		{
			get { return (_serialParityErrorReplacement); }
			set
			{
				if (value != _serialParityErrorReplacement)
				{
					_serialParityErrorReplacement = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Socket")]
		public virtual MKY.IO.Serial.SocketSettings Socket
		{
			get { return (_socket); }
			set
			{
				if (_socket == null)
				{
					_socket = value;
					AttachNode(_socket);
				}
				else if (value != _socket)
				{
					MKY.IO.Serial.SocketSettings old = _socket;
					_socket = value;
					ReplaceNode(old, _socket);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UsbHidDevice")]
		public virtual MKY.IO.Serial.UsbHidDeviceSettings UsbHidDevice
		{
			get { return (_usbHidDevice); }
			set
			{
				if (_usbHidDevice == null)
				{
					_usbHidDevice = value;
					AttachNode(_usbHidDevice);
				}
				else if (value != _usbHidDevice)
				{
					MKY.IO.Serial.UsbHidDeviceSettings old = _usbHidDevice;
					_usbHidDevice = value;
					ReplaceNode(old, _usbHidDevice);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Endianess")]
		public virtual Endianess Endianess
		{
			get { return (_endianess); }
			set
			{
				if (value != _endianess)
				{
					_endianess = value;
					SetChanged();
				}
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is IOSettings)
				return (Equals((IOSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(IOSettings value)
		{
			// Ensure that object.operator!=() is called.
			if ((object)value != null)
			{
				return 
					(
					(_ioType                       == value._ioType) &&
					(_serialParityErrorReplacement == value._serialParityErrorReplacement) &&
					(_endianess                    == value._endianess) &&
					base.Equals((MKY.Utilities.Settings.Settings)value) // Compare all settings nodes.
					);
			}
			return (false);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(IOSettings lhs, IOSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));
			
			return (false);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(IOSettings lhs, IOSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
