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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class IOSettings : MKY.Utilities.Settings.Settings
	{
		/// <summary></summary>
		public const Domain.IOType IOTypeDefault = Domain.IOType.SerialPort;

		/// <summary></summary>
		public const string SerialParityErrorReplacementDefault = @"\h(00)";

		/// <summary></summary>
		public const Endianess EndianessDefault = Endianess.BigEndian;

		private Domain.IOType ioType;
		private MKY.IO.Serial.SerialPortSettings serialPort;
		private string serialParityErrorReplacement;
		private MKY.IO.Serial.SocketSettings socket;
		private MKY.IO.Serial.UsbHidDeviceSettings usbHidDevice;
		private Endianess endianess;

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
			IOType                       = rhs.IOType;
			SerialPort                   = new MKY.IO.Serial.SerialPortSettings(rhs.SerialPort);
			SerialParityErrorReplacement = rhs.SerialParityErrorReplacement;
			Socket                       = new MKY.IO.Serial.SocketSettings(rhs.Socket);
			UsbHidDevice                 = new MKY.IO.Serial.UsbHidDeviceSettings(rhs.UsbHidDevice);
			Endianess                    = rhs.Endianess;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			IOType                       = IOTypeDefault;
			SerialParityErrorReplacement = SerialParityErrorReplacementDefault;
			Endianess                    = EndianessDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("IOType")]
		public virtual Domain.IOType IOType
		{
			get { return (this.ioType); }
			set
			{
				if (value != this.ioType)
				{
					this.ioType = value;
					SetChanged();

					// Always set socket settings host type as well.
					if (this.socket != null)
						this.socket.HostType = (Domain.XIOType)value;
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SerialPort")]
		public virtual MKY.IO.Serial.SerialPortSettings SerialPort
		{
			get { return (this.serialPort); }
			set
			{
				if (value == null)
				{
					this.serialPort = value;
					DetachNode(this.serialPort);
				}
				else if (this.serialPort == null)
				{
					this.serialPort = value;
					AttachNode(this.serialPort);
				}
				else if (value != this.serialPort)
				{
					MKY.IO.Serial.SerialPortSettings old = this.serialPort;
					this.serialPort = value;
					ReplaceNode(old, this.serialPort);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SerialParityErrorReplacement")]
		public virtual string SerialParityErrorReplacement
		{
			get { return (this.serialParityErrorReplacement); }
			set
			{
				if (value != this.serialParityErrorReplacement)
				{
					this.serialParityErrorReplacement = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Socket")]
		public virtual MKY.IO.Serial.SocketSettings Socket
		{
			get { return (this.socket); }
			set
			{
				if (value == null)
				{
					this.socket = value;
					DetachNode(this.socket);
				}
				else if (this.socket == null)
				{
					this.socket = value;
					AttachNode(this.socket);
				}
				else if (value != this.socket)
				{
					MKY.IO.Serial.SocketSettings old = this.socket;
					this.socket = value;
					ReplaceNode(old, this.socket);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UsbHidDevice")]
		public virtual MKY.IO.Serial.UsbHidDeviceSettings UsbHidDevice
		{
			get { return (this.usbHidDevice); }
			set
			{
				if (value == null)
				{
					this.usbHidDevice = value;
					DetachNode(this.usbHidDevice);
				}
				else if (this.usbHidDevice == null)
				{
					this.usbHidDevice = value;
					AttachNode(this.usbHidDevice);
				}
				else if (value != this.usbHidDevice)
				{
					MKY.IO.Serial.UsbHidDeviceSettings old = this.usbHidDevice;
					this.usbHidDevice = value;
					ReplaceNode(old, this.usbHidDevice);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Endianess")]
		public virtual Endianess Endianess
		{
			get { return (this.endianess); }
			set
			{
				if (value != this.endianess)
				{
					this.endianess = value;
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
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			IOSettings other = (IOSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(this.ioType                       == other.ioType) &&
				(this.serialParityErrorReplacement == other.serialParityErrorReplacement) &&
				(this.endianess                    == other.endianess)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.ioType                      .GetHashCode() ^
				this.serialParityErrorReplacement.GetHashCode() ^
				this.endianess                   .GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Utilities.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
