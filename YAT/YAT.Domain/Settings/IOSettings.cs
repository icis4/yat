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
// YAT 2.0 Beta 4 Candidate 1 Version 1.99.28
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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
	public class IOSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const Domain.IOType IOTypeDefault = Domain.IOType.SerialPort;

		/// <summary></summary>
		public const Endianess EndianessDefault = Endianess.BigEndian;

		/// <summary></summary>
		public const bool IndicateSerialPortBreakStatesDefault = false;

		/// <summary></summary>
		public const bool SerialPortOutputBreakIsModifiableDefault = false;

		private Domain.IOType ioType;
		private MKY.IO.Serial.SerialPortSettings serialPort;
		private MKY.IO.Serial.SocketSettings socket;
		private MKY.IO.Serial.UsbSerialHidDeviceSettings usbSerialHidDevice;
		private Endianess endianess;
		private bool indicateSerialPortBreakStates;
		private bool serialPortOutputBreakIsModifiable;

		/// <summary></summary>
		public IOSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		/// <summary></summary>
		public IOSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		private void InitializeNodes()
		{
			SerialPort         = new MKY.IO.Serial.SerialPortSettings(SettingsType);
			Socket             = new MKY.IO.Serial.SocketSettings(SettingsType);
			UsbSerialHidDevice = new MKY.IO.Serial.UsbSerialHidDeviceSettings(SettingsType);
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public IOSettings(IOSettings rhs)
			: base(rhs)
		{
			IOType             = rhs.IOType;
			SerialPort         = new MKY.IO.Serial.SerialPortSettings(rhs.SerialPort);
			Socket             = new MKY.IO.Serial.SocketSettings(rhs.Socket);
			UsbSerialHidDevice = new MKY.IO.Serial.UsbSerialHidDeviceSettings(rhs.UsbSerialHidDevice);
			Endianess          = rhs.Endianess;

			IndicateSerialPortBreakStates = rhs.IndicateSerialPortBreakStates;
			SerialPortOutputBreakIsModifiable = rhs.SerialPortOutputBreakIsModifiable;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			IOType    = IOTypeDefault;
			Endianess = EndianessDefault;

			IndicateSerialPortBreakStates = IndicateSerialPortBreakStatesDefault;
			SerialPortOutputBreakIsModifiable = SerialPortOutputBreakIsModifiableDefault;
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
						this.socket.HostType = (Domain.IOTypeEx)value;
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
		[XmlElement("UsbSerialHidDevice")]
		public virtual MKY.IO.Serial.UsbSerialHidDeviceSettings UsbSerialHidDevice
		{
			get { return (this.usbSerialHidDevice); }
			set
			{
				if (value == null)
				{
					this.usbSerialHidDevice = value;
					DetachNode(this.usbSerialHidDevice);
				}
				else if (this.usbSerialHidDevice == null)
				{
					this.usbSerialHidDevice = value;
					AttachNode(this.usbSerialHidDevice);
				}
				else if (value != this.usbSerialHidDevice)
				{
					MKY.IO.Serial.UsbSerialHidDeviceSettings old = this.usbSerialHidDevice;
					this.usbSerialHidDevice = value;
					ReplaceNode(old, this.usbSerialHidDevice);
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

		/// <summary></summary>
		[XmlElement("IndicateSerialPortBreakStates")]
		public virtual bool IndicateSerialPortBreakStates
		{
			get { return (this.indicateSerialPortBreakStates); }
			set
			{
				if (value != this.indicateSerialPortBreakStates)
				{
					this.indicateSerialPortBreakStates = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SerialPortOutputBreakIsModifiable")]
		public virtual bool SerialPortOutputBreakIsModifiable
		{
			get { return (this.serialPortOutputBreakIsModifiable); }
			set
			{
				if (value != this.serialPortOutputBreakIsModifiable)
				{
					this.serialPortOutputBreakIsModifiable = value;
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

				(this.ioType    == other.ioType) &&
				(this.endianess == other.endianess) &&

				(this.indicateSerialPortBreakStates == other.indicateSerialPortBreakStates) &&
				(this.serialPortOutputBreakIsModifiable == other.serialPortOutputBreakIsModifiable)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.ioType   .GetHashCode() ^
				this.endianess.GetHashCode() ^

				this.indicateSerialPortBreakStates.GetHashCode() ^
				this.serialPortOutputBreakIsModifiable.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
