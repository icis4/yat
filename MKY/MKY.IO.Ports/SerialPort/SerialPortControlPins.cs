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
		[XmlElement("Rts")]
		public bool Rts;

		/// <summary>
		/// Clear To Send.
		/// </summary>
		[XmlElement("Cts")]
		public bool Cts;

		/// <summary>
		/// Data Terminal Ready.
		/// </summary>
		[XmlElement("Dtr")]
		public bool Dtr;

		/// <summary>
		/// Data Set Ready.
		/// </summary>
		[XmlElement("Dsr")]
		public bool Dsr;

		/// <summary>
		/// Carrier Detect.
		/// </summary>
		[XmlElement("Cd")]
		public bool Cd;

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

			SerialPortControlPins other = (SerialPortControlPins)obj;
			return
			(
				(Rts == other.Rts) &&
				(Cts == other.Cts) &&
				(Dtr == other.Dtr) &&
				(Dsr == other.Dsr) &&
				(Cd  == other.Cd)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				Rts.GetHashCode() ^
				Cts.GetHashCode() ^
				Dtr.GetHashCode() ^
				Dsr.GetHashCode() ^
				Cd .GetHashCode()
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
			// See MKY.Utilities.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs)) return (true);
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
