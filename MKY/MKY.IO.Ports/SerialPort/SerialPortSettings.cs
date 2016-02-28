//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO.Ports;
using System.Reflection;
using System.Xml.Serialization;

#endregion

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

		private BaudRate baudRate;
		private DataBits dataBits;
		private Parity parity;
		private StopBits stopBits;
		private Handshake handshake;

		/// <summary>
		/// Creates new port settings with defaults.
		/// </summary>
		public SerialPortSettings()
		{
			SetDefaults();
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		public SerialPortSettings(BaudRate baudRate, DataBits dataBits, Parity parity, StopBits stopBits, Handshake handshake)
		{
			BaudRate  = baudRate;
			DataBits  = dataBits;
			Parity    = parity;
			StopBits  = stopBits;
			Handshake = handshake;
		}

		/// <summary>
		/// Creates new port settings from <paramref name="rhs"/>.
		/// </summary>
		public SerialPortSettings(SerialPortSettings rhs)
		{
			BaudRate  = rhs.BaudRate;
			DataBits  = rhs.DataBits;
			Parity    = rhs.Parity;
			StopBits  = rhs.StopBits;
			Handshake = rhs.Handshake;
		}

		/// <summary>
		/// Sets default port settings.
		/// </summary>
		protected void SetDefaults()
		{
			BaudRate  = BaudRate.Baud009600;
			DataBits  = DataBits.Eight;
			Parity    = Parity.None;
			StopBits  = StopBits.One;
			Handshake = Handshake.None;
		}

		#region Properties

		/// <summary></summary>
		[XmlElement("BaudRate")]
		public BaudRate BaudRate
		{
			get { return (this.baudRate); }
			set { this.baudRate = value; }
		}

		/// <summary></summary>
		[XmlElement("DataBits")]
		public DataBits DataBits
		{
			get { return (this.dataBits); }
			set { this.dataBits = value; }
		}

		/// <summary></summary>
		[XmlElement("Parity")]
		public Parity Parity
		{
			get { return (this.parity); }
			set { this.parity = value; }
		}

		/// <summary></summary>
		[XmlElement("StopBits")]
		public StopBits StopBits
		{
			get { return (this.stopBits); }
			set { this.stopBits = value; }
		}

		/// <summary></summary>
		[XmlElement("Handshake")]
		public Handshake Handshake
		{
			get { return (this.handshake); }
			set { this.handshake = value; }
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SerialPortSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SerialPortSettings other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			return
			(
				(BaudRate  == other.BaudRate) &&
				(DataBits  == other.DataBits) &&
				(Parity    == other.Parity)   &&
				(StopBits  == other.StopBits) &&
				(Handshake == other.Handshake)
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			return
			(
				BaudRate .GetHashCode() ^
				DataBits .GetHashCode() ^
				Parity   .GetHashCode() ^
				StopBits .GetHashCode() ^
				Handshake.GetHashCode()
			);
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return
			(
				((BaudRateEx) BaudRate) + ", " +
				((DataBitsEx) DataBits) + ", " +
				((ParityEx)   Parity)   + ", " +
				((StopBitsEx) StopBits) + ", " +
				((HandshakeEx)Handshake).ToShortString()
			);
		}

		/// <summary>
		/// Returns port settings as a single string. The string is limited to the basic settings.
		/// </summary>
		public virtual string ToShortString()
		{
			return
			(
				((BaudRateEx)this.baudRate).ToString() + ", " +
				((DataBitsEx)this.dataBits).ToString() + ", " +
				((ParityEx)  this.parity)  .ToShortString()
			);
		}

		/// <summary>
		/// Parses <paramref name="s"/> for serial port settings and returns a corresponding settings object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static SerialPortSettings Parse(string s)
		{
			SerialPortSettings result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" does not specify valid serial port settings."));
		}

		/// <summary>
		/// Tries to parse <paramref name="s"/> for serial port settings and returns a corresponding settings object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SerialPortSettings settings)
		{
			string delimiters = "/,;";
			string[] sa = s.Trim().Split(delimiters.ToCharArray());
			if (sa.Length == 5)
			{
				BaudRate baudRate;
				if (BaudRateEx.TryParse(sa[0], out baudRate))
				{
					DataBits dataBits;
					if (DataBitsEx.TryParse(sa[1], out dataBits))
					{
						Parity parity;
						if (ParityEx.TryParse(sa[2], out parity))
						{
							StopBits stopBits;
							if (StopBitsEx.TryParse(sa[3], out stopBits))
							{
								Handshake handshake;
								if (HandshakeEx.TryParse(sa[4], out handshake))
								{
									settings = new SerialPortSettings(baudRate, dataBits, parity, stopBits, handshake);
									return (true);
								}
							}
						}
					}
				}
			}

			settings = null;
			return (false);
		}

		#endregion

		/// <summary>
		/// Returns bits size of an interface packet according to the current
		/// interface settings (StartBit + DataBits + StopBits).
		/// </summary>
		public double PacketSize
		{
			get { return (1 + (double)this.dataBits + (double)this.stopBits); }
		}

		/// <summary>
		/// Returns duration of an interface packet according to the current
		/// interface settings (PacketSize * (1 / BaudRate)) in milliseconds.
		/// </summary>
		public long PacketDuration
		{
			get { return ((long)(1000 * PacketSize * (1 / (int)this.baudRate))); }
		}

		/// <summary>
		/// Returns whether the given byte is an XOn or XOff byte.
		/// </summary>
		public static bool IsXOnXOffByte(byte b)
		{
			return ((b == XOnByte) || (b == XOffByte));
		}

		#region Comparison Operators

		/// <summary></summary>
		public static bool operator ==(SerialPortSettings lhs, SerialPortSettings rhs)
		{
			// Base reference type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			// Ensure that potiential <Derived>.Equals() is called.
			// Thus, ensure that object.Equals() is called.
			object obj = (object)lhs;
			return (obj.Equals(rhs));
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
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Performance is not an issue here, readability is...")]
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
		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Performance is not an issue here, readability is...")]
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
