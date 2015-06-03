//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.12
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
	public struct SerialPortControlPins
	{
		/// <summary>
		/// RFR (Ready For Receiving) control line. This line was formerly called RTS (Request To Send).
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rfr", Justification = "RFR is a common term for serial ports.")]
		[XmlElement("RFR")]
		public bool Rfr;

		/// <summary>
		/// CTS (Clear To Send) control line.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Cts", Justification = "CTS is a common term for serial ports.")]
		[XmlElement("CTS")]
		public bool Cts;

		/// <summary>
		/// DTR (Data Terminal Ready) control line.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dtr", Justification = "DTR is a common term for serial ports.")]
		[XmlElement("DTR")]
		public bool Dtr;

		/// <summary>
		/// DSR (Data Set Ready) control line.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dsr", Justification = "DSR is a common term for serial ports.")]
		[XmlElement("DSR")]
		public bool Dsr;

		/// <summary>
		/// DCD (Data Carrier Detect) control line.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "This field is public for the ease of the implementation.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dcd", Justification = "DCD is a common term for serial ports.")]
		[XmlElement("DCD")]
		public bool Dcd;

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			SerialPortControlPins other = (SerialPortControlPins)obj;
			return
			(
				(Rfr == other.Rfr) &&
				(Cts == other.Cts) &&
				(Dtr == other.Dtr) &&
				(Dsr == other.Dsr) &&
				(Dcd == other.Dcd)
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
				Rfr.GetHashCode() ^
				Cts.GetHashCode() ^
				Dtr.GetHashCode() ^
				Dsr.GetHashCode() ^
				Dcd.GetHashCode()
			);
		}

		/// <summary></summary>
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

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SerialPortControlPins lhs, SerialPortControlPins rhs)
		{
			// Value type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
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
	public struct SerialPortControlPinCount
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

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			SerialPortControlPinCount other = (SerialPortControlPinCount)obj;
			return
			(
				(RfrDisableCount == other.RfrDisableCount) &&
				(CtsDisableCount == other.CtsDisableCount) &&
				(DtrDisableCount == other.DtrDisableCount) &&
				(DsrDisableCount == other.DsrDisableCount) &&
				(DcdCount        == other.DcdCount)
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
				RfrDisableCount.GetHashCode() ^
				CtsDisableCount.GetHashCode() ^
				DtrDisableCount.GetHashCode() ^
				DsrDisableCount.GetHashCode() ^
				DcdCount       .GetHashCode()
			);
		}

		/// <summary></summary>
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

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SerialPortControlPinCount lhs, SerialPortControlPinCount rhs)
		{
			// Value type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
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
