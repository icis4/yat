using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings.SerialPort
{
	/// <summary></summary>
	[Serializable]
	public class SerialCommunicationSettings : Utilities.Settings.Settings, IEquatable<SerialCommunicationSettings>
	{
		private int _baudRate;
		private MKY.IO.Ports.DataBits _dataBits;
		private System.IO.Ports.Parity _parity;
		private System.IO.Ports.StopBits _stopBits;
		private Domain.IO.Handshake _handshake;

		/// <summary></summary>
		public SerialCommunicationSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public SerialCommunicationSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public SerialCommunicationSettings(SerialCommunicationSettings rhs)
			: base(rhs)
		{
			_baudRate   = rhs.BaudRate;
			_dataBits   = rhs.DataBits;
			_parity     = rhs.Parity;
			_stopBits   = rhs.StopBits;
			_handshake  = rhs.Handshake;
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			BaudRate   = (int)MKY.IO.Ports.BaudRate.Baud009600;
			DataBits   = MKY.IO.Ports.DataBits.Eight;
			Parity     = System.IO.Ports.Parity.None;
			StopBits   = System.IO.Ports.StopBits.One;
			Handshake  = Domain.IO.Handshake.None;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("BaudRate")]
		public int BaudRate
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

		/// <summary></summary>
		[XmlElement("DataBits")]
		public MKY.IO.Ports.DataBits DataBits
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

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		/// <summary></summary>
		public override string ToString()
		{
			return
			  (
			  _baudRate.ToString() + ", " +
			  ((MKY.IO.Ports.XDataBits)_dataBits).ToString() + ", " +
			  ((MKY.IO.Ports.XParity)_parity).ToString() + ", " +
			  ((MKY.IO.Ports.XStopBits)_stopBits).ToString() + ", " +
			  ((Domain.IO.XHandshake)_handshake).ToShortString()
			  );
		}

		#endregion

		/// <summary></summary>
		public string ToShortString()
		{
			return
			  (
			  _baudRate.ToString() + ", " +
			  ((MKY.IO.Ports.XDataBits)_dataBits).ToString() + ", " +
			  ((MKY.IO.Ports.XParity)_parity).ToShortString()
			  );
		}

		/// <summary></summary>
		public string ToLongString()
		{
			return
			  (
			  _baudRate.ToString() + ", " +
			  ((MKY.IO.Ports.XDataBits)_dataBits).ToString() + ", " +
			  ((MKY.IO.Ports.XParity)_parity).ToString() + ", " +
			  ((MKY.IO.Ports.XStopBits)_stopBits).ToString() + ", " +
			  ((Domain.IO.XHandshake)_handshake).ToString()
			  );
		}

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
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
