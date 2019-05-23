﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.9
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
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
		/// Creates new port settings with specified argument.
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
		/// Creates new port settings from "rhs".
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
		/// Parses s for the first integer number and returns the corresponding port.
		/// </summary>
		public static SerialPortSettings Parse(string s)
		{
			SerialPortSettings ps = new SerialPortSettings();

			string delimiters = "/,;";
			string[] sa = s.Split(delimiters.ToCharArray());
			ps.baudRate  = BaudRateEx .Parse(sa[0]);
			ps.dataBits  = DataBitsEx .Parse(sa[1]);
			ps.parity    = ParityEx   .Parse(sa[2]);
			ps.stopBits  = StopBitsEx .Parse(sa[3]);
			ps.handshake = HandshakeEx.Parse(sa[4]);

			return (ps);
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
		/// Returns port settings summary.
		/// </summary>
		public virtual string ToShortString()
		{
			return
			(
				((BaudRateEx)this.baudRate).ToString() + ", " +
				((DataBitsEx)this.dataBits).ToString() + ", " +
				((ParityEx)this.parity).ToShortString()
			);
		}

		/// <summary>
		/// Returns port settings summary.
		/// </summary>
		public virtual string ToLongString()
		{
			return
			(
				((BaudRateEx)this.baudRate).ToString() + ", " +
				((DataBitsEx)this.dataBits).ToString() + ", " +
				((ParityEx)this.parity).ToShortString() + ", " +
				((StopBitsEx)this.stopBits).ToString() + ", " +
				((HandshakeEx)this.handshake).ToShortString()
			);
		}

		#region Comparision Operators

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