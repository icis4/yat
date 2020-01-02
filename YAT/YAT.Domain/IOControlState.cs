//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2020 Matthias Kläy.
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
using System.Globalization;
using System.Xml.Serialization;

using MKY.IO.Ports;

#endregion

namespace YAT.Domain
{
	/// <summary></summary>
	[Serializable]
	public struct IOControlState : IEquatable<IOControlState>
	{
		/// <summary>
		/// Serial port control pins.
		/// </summary>
		[XmlElement("SerialPortControlPins")]
		public SerialPortControlPins SerialPortControlPins { get; set; }

		/// <summary>
		/// Serial port control pin count.
		/// </summary>
		[XmlElement("SerialPortControlPinCount")]
		public SerialPortControlPinCount SerialPortControlPinCount { get; set; }

		/// <summary>
		/// Input XOn/XOff reflects the XOn/XOff state of this serial port itself, i.e. this computer.
		/// </summary>
		[XmlElement("InputIsXOn")]
		public bool InputIsXOn { get; set; }

		/// <summary>
		/// Output XOn/XOff reflects the XOn/XOff state of the communication counterpart, i.e. a device.
		/// </summary>
		[XmlElement("OutputIsXOn")]
		public bool OutputIsXOn { get; set; }

		/// <summary>
		/// The number of sent XOn bytes, i.e. the count of input XOn/XOff signaling.
		/// </summary>
		[XmlElement("SentXOnCount")]
		public int SentXOnCount { get; set; }

		/// <summary>
		/// The number of sent XOff bytes, i.e. the count of input XOn/XOff signaling.
		/// </summary>
		[XmlElement("SentXOffCount")]
		public int SentXOffCount { get; set; }

		/// <summary>
		/// The number of received XOn bytes, i.e. the count of output XOn/XOff signaling.
		/// </summary>
		[XmlElement("ReceivedXOnCount")]
		public int ReceivedXOnCount { get; set; }

		/// <summary>
		/// The number of received XOff bytes, i.e. the count of output XOn/XOff signaling.
		/// </summary>
		[XmlElement("ReceivedXOffCount")]
		public int ReceivedXOffCount { get; set; }

		/// <summary>
		/// The input break state.
		/// </summary>
		[XmlElement("InputBreak")]
		public bool InputBreak { get; set; }

		/// <summary>
		/// The output break state.
		/// </summary>
		[XmlElement("OutputBreak")]
		public bool OutputBreak { get; set; }

		/// <summary>
		/// Returns the number of input breaks.
		/// </summary>
		[XmlElement("InputBreakCount")]
		public int InputBreakCount { get; set; }

		/// <summary>
		/// Returns the number of output breaks.
		/// </summary>
		[XmlElement("OutputBreakCount")]
		public int OutputBreakCount { get; set; }

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
				SerialPortControlPins    .ToString() + ", " +
				SerialPortControlPinCount.ToString() + ", " +

				"IXS=" + InputIsXOn .ToString() + "|" + SentXOnCount    .ToString(CultureInfo.CurrentCulture) + "|" + SentXOffCount    .ToString(CultureInfo.CurrentCulture) + ", " +
				"OXS=" + OutputIsXOn.ToString() + "|" + ReceivedXOnCount.ToString(CultureInfo.CurrentCulture) + "|" + ReceivedXOffCount.ToString(CultureInfo.CurrentCulture) + ", " +

				"IBS=" + InputBreak .ToString() + "|" + InputBreakCount .ToString(CultureInfo.CurrentCulture) + ", " +
				"OBS=" + OutputBreak.ToString() + "|" + OutputBreakCount.ToString(CultureInfo.CurrentCulture)
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

				hashCode =                SerialPortControlPins.GetHashCode();
				hashCode =            SerialPortControlPinCount.GetHashCode();

				hashCode = (hashCode * 397) ^ InputIsXOn       .GetHashCode();
				hashCode = (hashCode * 397) ^ OutputIsXOn      .GetHashCode();
				hashCode = (hashCode * 397) ^ SentXOnCount     .GetHashCode();
				hashCode = (hashCode * 397) ^ SentXOffCount    .GetHashCode();
				hashCode = (hashCode * 397) ^ ReceivedXOnCount .GetHashCode();
				hashCode = (hashCode * 397) ^ ReceivedXOffCount.GetHashCode();

				hashCode = (hashCode * 397) ^ InputBreak       .GetHashCode();
				hashCode = (hashCode * 397) ^ OutputBreak      .GetHashCode();
				hashCode = (hashCode * 397) ^ InputBreakCount  .GetHashCode();
				hashCode = (hashCode * 397) ^ OutputBreakCount .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is IOControlState)
				return (Equals((IOControlState)obj));
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
		public bool Equals(IOControlState other)
		{
			return
			(
				SerialPortControlPins    .Equals(other.SerialPortControlPins)     &&
				SerialPortControlPinCount.Equals(other.SerialPortControlPinCount) &&

				InputIsXOn       .Equals(other.InputIsXOn)  &&
				OutputIsXOn      .Equals(other.OutputIsXOn) &&
				SentXOnCount     .Equals(other.OutputIsXOn) &&
				SentXOffCount    .Equals(other.OutputIsXOn) &&
				ReceivedXOnCount .Equals(other.OutputIsXOn) &&
				ReceivedXOffCount.Equals(other.OutputIsXOn) &&

				InputBreak       .Equals(other.InputBreak)  &&
				OutputBreak      .Equals(other.OutputBreak) &&
				InputBreakCount  .Equals(other.InputBreak)  &&
				OutputBreakCount .Equals(other.InputBreak)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(IOControlState lhs, IOControlState rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(IOControlState lhs, IOControlState rhs)
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
