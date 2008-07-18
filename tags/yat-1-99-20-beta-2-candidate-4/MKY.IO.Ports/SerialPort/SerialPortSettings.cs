using System;
using System.Xml.Serialization;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.IO.Ports;

namespace MKY.IO.Ports
{
	/// <summary>
	/// Serial port settings.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(SerialPortSettingsConverter))]
	public class SerialPortSettings : IEquatable<SerialPortSettings>
	{
		/// <summary></summary>
		public const byte XOnByte  = 0x11;
		/// <summary></summary>
		public const byte XOffByte = 0x13;

		/// <summary></summary>
		public const string XOnDescription = "XOn = 11h (DC1)";
		/// <summary></summary>
		public const string XOffDescription = "XOff = 13h (DC3)";

		private BaudRate _baudRate;
		private DataBits _dataBits;
		private Parity _parity;
		private StopBits _stopBits;
		private Handshake _handshake;

		/// <summary>
		/// Creates new port settings with defaults.
		/// </summary>
		public SerialPortSettings()
		{
			SetDefaults();
		}

		/// <summary>
		/// Creates new port settings with specified argument.
		/// </summary>
		public SerialPortSettings(BaudRate baudRate, DataBits dataBits, Parity parity, StopBits stopBits, Handshake handshake)
		{
			BaudRate = baudRate;
			DataBits = dataBits;
			Parity = parity;
			StopBits = stopBits;
			Handshake = handshake;
		}

		/// <summary>
		/// Creates new port settings from "rhs".
		/// </summary>
		public SerialPortSettings(SerialPortSettings rhs)
		{
			BaudRate = rhs.BaudRate;
			DataBits = rhs.DataBits;
			Parity = rhs.Parity;
			StopBits = rhs.StopBits;
			Handshake = rhs.Handshake;
		}

		/// <summary>
		/// Sets default port settings.
		/// </summary>
		protected void SetDefaults()
		{
			BaudRate = BaudRate.Baud009600;
			DataBits = DataBits.Eight;
			Parity = Parity.None;
			StopBits = StopBits.One;
			Handshake = Handshake.None;
		}

		#region Properties

		/// <summary>
		/// BaudRate.
		/// </summary>
		[XmlElement("BaudRate")]
		public BaudRate BaudRate
		{
			get { return (_baudRate); }
			set { _baudRate = value; }
		}

		/// <summary>
		/// DataBits.
		/// </summary>
		[XmlElement("DataBits")]
		public DataBits DataBits
		{
			get { return (_dataBits); }
			set { _dataBits = value; }
		}

		/// <summary>
		/// Parity.
		/// </summary>
		[XmlElement("Parity")]
		public Parity Parity
		{
			get { return (_parity); }
			set { _parity = value; }
		}

		/// <summary>
		/// StopBits.
		/// </summary>
		[XmlElement("StopBits")]
		public StopBits StopBits
		{
			get { return (_stopBits); }
			set { _stopBits = value; }
		}

		/// <summary>
		/// Handshake.
		/// </summary>
		[XmlElement("Handshake")]
		public Handshake Handshake
		{
			get { return (_handshake); }
			set { _handshake = value; }
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is SerialPortSettings)
				return (Equals((SerialPortSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(SerialPortSettings value)
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

		/// <summary>
		/// Returns hash code.
		/// </summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		/// <summary>
		/// ToString.
		/// </summary>
		public override string ToString()
		{
			return
			  (
			  ((XBaudRate)_baudRate).ToString() + ", " +
			  ((XDataBits)_dataBits).ToString() + ", " +
			  ((XParity)_parity).ToString() + ", " +
			  ((XStopBits)_stopBits).ToString() + ", " +
			  ((XHandshake)_handshake).ToShortString()
			  );
		}

		/// <summary>
		/// Parses s for the first integer number and returns the corresponding port.
		/// </summary>
		public static SerialPortSettings Parse(string s)
		{
			SerialPortSettings ps = new SerialPortSettings();

			string delimiters = "/,;";
			string[] sa = s.Split(delimiters.ToCharArray());
			ps._baudRate = XBaudRate.Parse(sa[0]);
			ps._dataBits = XDataBits.Parse(sa[1]);
			ps._parity = XParity.Parse(sa[2]);
			ps._stopBits = XStopBits.Parse(sa[3]);
			ps._handshake = XHandshake.Parse(sa[4]);

			return (ps);
		}

		#endregion

		/// <summary>
		/// Returns bits size of an interface packet according to the current
		/// interface settings (StartBit + DataBits + StopBits).
		/// </summary>
		public double PacketSize
		{
			get { return (1 + (double)_dataBits + (double)_stopBits); }
		}

		/// <summary>
		/// Returns length of an interface packet according to the current
		/// interface settings (PacketSize * (1 / BaudRate)) in milliseconds.
		/// </summary>
		public long PacketLength
		{
			get { return ((long)(1000 * PacketSize * (1 / (int)_baudRate))); }
		}

		/// <summary>
		/// Returns port settings summary.
		/// </summary>
		public string ToShortString()
		{
			return
			  (
			  ((XBaudRate)_baudRate).ToString() + ", " +
			  ((XDataBits)_dataBits).ToString() + ", " +
			  ((XParity)_parity).ToShortString()
			  );
		}

		/// <summary>
		/// Returns port settings summary.
		/// </summary>
		public string ToLongString()
		{
			return
			  (
			  ((XBaudRate)_baudRate).ToString() + ", " +
			  ((XDataBits)_dataBits).ToString() + ", " +
			  ((XParity)_parity).ToShortString() + ", " +
			  ((XStopBits)_stopBits).ToString() + ", " +
			  ((XHandshake)_handshake).ToShortString()
			  );
		}

		#region Comparision Methods

		/// <summary></summary>
		public static new bool Equals(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB)) return (true);
			if (objA is SerialPortSettings)
			{
				SerialPortSettings casted = (SerialPortSettings)objA;
				return (casted.Equals(objB));
			}
			return (false);
		}

		#endregion

		#region Comparision Operators

		/// <summary></summary>
		public static bool operator ==(SerialPortSettings lhs, SerialPortSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));

			return (false);
		}

		/// <summary></summary>
		public static bool operator !=(SerialPortSettings lhs, SerialPortSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator string(SerialPortSettings settings)
		{
			return (settings.ToString());
		}

		/// <summary></summary>
		public static implicit operator SerialPortSettings(string settings)
		{
			return (Parse(settings));
		}

		#endregion
	}

	/// <summary></summary>
	public class SerialPortSettingsConverter : TypeConverter
	{
		/// <summary></summary>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string)) return (true);
			return (base.CanConvertFrom(context, sourceType));
		}

		/// <summary></summary>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string) return (SerialPortSettings.Parse((string)value));
			return (base.ConvertFrom(context, culture, value));
		}

		/// <summary></summary>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string)) return (true);
			if (destinationType == typeof(InstanceDescriptor))
			{
				return (true);
			}
			return (base.CanConvertTo(context, destinationType));
		}

		/// <summary></summary>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string)) return (((SerialPortSettings)value).ToString());
			if (destinationType == typeof(InstanceDescriptor) && value is SerialPortSettings)
			{
				SerialPortSettings settings = (SerialPortSettings)value;
				ConstructorInfo ctor = typeof(SerialPortSettings).GetConstructor(new Type[] { typeof(BaudRate), typeof(DataBits), typeof(Parity), typeof(StopBits), typeof(Handshake) });
				if (ctor != null)
					return (new InstanceDescriptor(ctor, new object[] { settings.BaudRate, settings.DataBits, settings.Parity, settings.StopBits, settings.Handshake }));
			}
			return (base.ConvertTo(context, culture, value, destinationType));
		}
	}
}
