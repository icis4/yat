﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.18
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Xml.Serialization;

namespace MKY.IO.Ports
{
	/// <summary>
	/// Serial port control pins.
	/// </summary>
	[Serializable]
	public struct SerialPortControlPins : IEquatable<SerialPortControlPins>
	{
		/// <summary>
		/// RFR (Ready For Receiving) control line. This line was formerly called RTS (Request To Send).
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rfr", Justification = "RFR is a common term for serial ports.")]
		[XmlElement("RFR")]
		public bool Rfr { get; set; }

		/// <summary>
		/// CTS (Clear To Send) control line.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cts", Justification = "CTS is a common term for serial ports.")]
		[XmlElement("CTS")]
		public bool Cts { get; set; }

		/// <summary>
		/// DTR (Data Terminal Ready) control line.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "DTR is a common term for serial ports.")]
		[XmlElement("DTR")]
		public bool Dtr { get; set; }

		/// <summary>
		/// DSR (Data Set Ready) control line.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dsr", Justification = "DSR is a common term for serial ports.")]
		[XmlElement("DSR")]
		public bool Dsr { get; set; }

		/// <summary>
		/// DCD (Data Carrier Detect) control line.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dcd", Justification = "DCD is a common term for serial ports.")]
		[XmlElement("DCD")]
		public bool Dcd { get; set; }

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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
				"RFR=" + Rfr.ToString() + ", " +
				"CTS=" + Cts.ToString() + ", " +
				"DTR=" + Dtr.ToString() + ", " +
				"DSR=" + Dsr.ToString() + ", " +
				"DCD=" + Dcd.ToString()
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
			unchecked
			{
				int hashCode;

				hashCode =                    Rfr.GetHashCode();
				hashCode = (hashCode * 397) ^ Cts.GetHashCode();
				hashCode = (hashCode * 397) ^ Dtr.GetHashCode();
				hashCode = (hashCode * 397) ^ Dsr.GetHashCode();
				hashCode = (hashCode * 397) ^ Dcd.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is SerialPortControlPins)
				return (Equals((SerialPortControlPins)obj));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SerialPortControlPins other)
		{
			return
			(
				Rfr.Equals(other.Rfr) &&
				Cts.Equals(other.Cts) &&
				Dtr.Equals(other.Dtr) &&
				Dsr.Equals(other.Dsr) &&
				Dcd.Equals(other.Dcd)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(SerialPortControlPins lhs, SerialPortControlPins rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(SerialPortControlPins lhs, SerialPortControlPins rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	/// <summary>
	/// Serial port control pin count.
	/// </summary>
	[Serializable]
	public struct SerialPortControlPinCount : IEquatable<SerialPortControlPinCount>
	{
		/// <summary>
		/// RFR (Ready For Receiving) control line. This line was formerly called RTS (Request To Send).
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rfr", Justification = "RFR is a common term for serial ports.")]
		[XmlElement("RFR")]
		public int RfrDisableCount;

		/// <summary>
		/// CTS (Clear To Send) control line.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cts", Justification = "CTS is a common term for serial ports.")]
		[XmlElement("CTS")]
		public int CtsDisableCount;

		/// <summary>
		/// DTR (Data Terminal Ready) control line.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "DTR is a common term for serial ports.")]
		[XmlElement("DTR")]
		public int DtrDisableCount;

		/// <summary>
		/// DSR (Data Set Ready) control line.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dsr", Justification = "DSR is a common term for serial ports.")]
		[XmlElement("DSR")]
		public int DsrDisableCount;

		/// <summary>
		/// DCD (Data Carrier Detect) control line.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dcd", Justification = "DCD is a common term for serial ports.")]
		[XmlElement("DCD")]
		public int DcdCount;

		/// <summary>
		/// Resets the control pin counts.
		/// </summary>
		public void Reset()
		{
			Interlocked.Exchange(ref RfrDisableCount, 0);
			Interlocked.Exchange(ref CtsDisableCount, 0);
			Interlocked.Exchange(ref DtrDisableCount, 0);
			Interlocked.Exchange(ref DsrDisableCount, 0);
			Interlocked.Exchange(ref DcdCount, 0);
		}

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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
				"RFR=" + RfrDisableCount.ToString(CultureInfo.InvariantCulture) + ", " +
				"CTS=" + CtsDisableCount.ToString(CultureInfo.InvariantCulture) + ", " +
				"DTR=" + DtrDisableCount.ToString(CultureInfo.InvariantCulture) + ", " +
				"DSR=" + DsrDisableCount.ToString(CultureInfo.InvariantCulture) + ", " +
				"DCD=" + DcdCount       .ToString(CultureInfo.InvariantCulture)
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
			unchecked
			{
				int hashCode;

				hashCode =                    RfrDisableCount;
				hashCode = (hashCode * 397) ^ CtsDisableCount;
				hashCode = (hashCode * 397) ^ DtrDisableCount;
				hashCode = (hashCode * 397) ^ DsrDisableCount;
				hashCode = (hashCode * 397) ^ DcdCount;

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is SerialPortControlPinCount)
				return (Equals((SerialPortControlPinCount)obj));
			else
				return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SerialPortControlPinCount other)
		{
			return
			(
				RfrDisableCount.Equals(other.RfrDisableCount) &&
				CtsDisableCount.Equals(other.CtsDisableCount) &&
				DtrDisableCount.Equals(other.DtrDisableCount) &&
				DsrDisableCount.Equals(other.DsrDisableCount) &&
				DcdCount       .Equals(other.DcdCount)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(SerialPortControlPinCount lhs, SerialPortControlPinCount rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(SerialPortControlPinCount lhs, SerialPortControlPinCount rhs)
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
