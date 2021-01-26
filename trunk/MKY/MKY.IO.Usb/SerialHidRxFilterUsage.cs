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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace MKY.IO.Usb
{
	/// <summary>
	/// Serial HID Rx ID setting.
	/// </summary>
	[Serializable]
	public struct SerialHidRxFilterUsage : IEquatable<SerialHidRxFilterUsage>
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const bool SeparateRxIdDefault = false;

		/// <summary></summary>
		public const bool AnyRxIdDefault = false;

		/// <summary></summary>
		public const byte RxIdDefault = SerialHidReportFormat.IdDefault;

		#endregion

		/// <summary></summary>
		[XmlElement("SeparateRxId")]
		public bool SeparateRxId { get; set; }

		/// <summary></summary>
		[XmlElement("AnyRxId")]
		public bool AnyRxId { get; set; }

		/// <summary></summary>
		[XmlElement("RxId")]
		public byte RxId { get; set; }

		/// <summary></summary>
		public SerialHidRxFilterUsage(bool separateRxId, bool anyRxId, byte rxId)
		{
			SeparateRxId = separateRxId;
			AnyRxId      = anyRxId;
			RxId         = rxId;
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
				SeparateRxId + ", " +
				AnyRxId      + ", " +
				RxId
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

				hashCode =                    SeparateRxId.GetHashCode();
				hashCode = (hashCode * 397) ^ AnyRxId     .GetHashCode();
				hashCode = (hashCode * 397) ^ RxId        .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is SerialHidRxFilterUsage)
				return (Equals((SerialHidRxFilterUsage)obj));
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
		public bool Equals(SerialHidRxFilterUsage other)
		{
			return
			(
				SeparateRxId.Equals(other.SeparateRxId) &&
				AnyRxId     .Equals(other.AnyRxId     ) &&
				RxId        .Equals(other.RxId        )
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(SerialHidRxFilterUsage lhs, SerialHidRxFilterUsage rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(SerialHidRxFilterUsage lhs, SerialHidRxFilterUsage rhs)
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
