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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
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
		/// Request To Send.
		/// </summary>
		[XmlElement("RTS")]
		public bool Rts;

		/// <summary>
		/// Clear To Send.
		/// </summary>
		[XmlElement("CTS")]
		public bool Cts;

		/// <summary>
		/// Data Terminal Ready.
		/// </summary>
		[XmlElement("DTR")]
		public bool Dtr;

		/// <summary>
		/// Data Set Ready.
		/// </summary>
		[XmlElement("DSR")]
		public bool Dsr;

		/// <summary>
		/// Data Carrier Detect.
		/// </summary>
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
				(Rts == other.Rts) &&
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
				Rts.GetHashCode() ^
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
				"RTS=" + Rts.ToString() + ", " +
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
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
