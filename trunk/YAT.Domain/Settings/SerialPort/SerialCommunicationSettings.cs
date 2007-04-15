using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Domain.Settings.SerialPort
{
	public class SerialCommunicationSettings : Utilities.Settings.Settings
	{
		private HSR.IO.Ports.BaudRate _baudRate;
		private HSR.IO.Ports.DataBits _dataBits;
		private System.IO.Ports.Parity _parity;
		private System.IO.Ports.StopBits _stopBits;
		private Domain.IO.Handshake _handshake;

		public SerialCommunicationSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public SerialCommunicationSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public SerialCommunicationSettings(SerialCommunicationSettings rhs)
			: base(rhs)
		{
			BaudRate   = rhs.BaudRate;
			DataBits   = rhs.DataBits;
			Parity     = rhs.Parity;
			StopBits   = rhs.StopBits;
			Handshake  = rhs.Handshake;
			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			BaudRate   = HSR.IO.Ports.BaudRate.Baud009600;
			DataBits   = HSR.IO.Ports.DataBits.Eight;
			Parity     = System.IO.Ports.Parity.None;
			StopBits   = System.IO.Ports.StopBits.One;
			Handshake  = Domain.IO.Handshake.None;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("BaudRate")]
		public HSR.IO.Ports.BaudRate BaudRate
		{
			get { return (_baudRate); }
			set
			{
				if (_baudRate != value)
				{
					_baudRate = value;
					SetChanged();
				}
			}
		}

		[XmlElement("DataBits")]
		public HSR.IO.Ports.DataBits DataBits
		{
			get { return (_dataBits); }
			set
			{
				if (_dataBits != value)
				{
					_dataBits = value;
					SetChanged();
				}
			}
		}

		[XmlElement("Parity")]
		public System.IO.Ports.Parity Parity
		{
			get { return (_parity); }
			set
			{
				if (_parity != value)
				{
					_parity = value;
					SetChanged();
				}
			}
		}

		[XmlElement("StopBits")]
		public System.IO.Ports.StopBits StopBits
		{
			get { return (_stopBits); }
			set
			{
				if (_stopBits != value)
				{
					_stopBits = value;
					SetChanged();
				}
			}
		}

		[XmlElement("Handshake")]
		public Domain.IO.Handshake Handshake
		{
			get { return (_handshake); }
			set
			{
				if (_handshake != value)
				{
					_handshake = value;
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
			if (obj is SerialCommunicationSettings)
				return (Equals((SerialCommunicationSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(SerialCommunicationSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_baudRate.Equals(value._baudRate) &&
					_dataBits.Equals(value._dataBits) &&
					_parity.Equals(value._parity) &&
					_stopBits.Equals(value._stopBits) &&
					_handshake.Equals(value._handshake)
					);
			}
			return (false);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		public override string ToString()
		{
			return
			  (
			  ((HSR.IO.Ports.XBaudRate)_baudRate).ToString() + ", " +
			  ((HSR.IO.Ports.XDataBits)_dataBits).ToString() + ", " +
			  ((HSR.IO.Ports.XParity)_parity).ToString() + ", " +
			  ((HSR.IO.Ports.XStopBits)_stopBits).ToString() + ", " +
			  ((Domain.IO.XHandshake)_handshake).ToShortString()
			  );
		}

		#endregion

		public string ToShortString()
		{
			return
			  (
			  ((HSR.IO.Ports.XBaudRate)_baudRate).ToString() + ", " +
			  ((HSR.IO.Ports.XDataBits)_dataBits).ToString() + ", " +
			  ((HSR.IO.Ports.XParity)_parity).ToShortString()
			  );
		}

		public string ToLongString()
		{
			return
			  (
			  ((HSR.IO.Ports.XBaudRate)_baudRate).ToString() + ", " +
			  ((HSR.IO.Ports.XDataBits)_dataBits).ToString() + ", " +
			  ((HSR.IO.Ports.XParity)_parity).ToString() + ", " +
			  ((HSR.IO.Ports.XStopBits)_stopBits).ToString() + ", " +
			  ((Domain.IO.XHandshake)_handshake).ToString()
			  );
		}

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference and value equality.
		/// </summary>
		public static bool operator ==(SerialCommunicationSettings lhs, SerialCommunicationSettings rhs)
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
		public static bool operator !=(SerialCommunicationSettings lhs, SerialCommunicationSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
