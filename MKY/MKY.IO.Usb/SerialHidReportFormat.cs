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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace MKY.IO.Usb
{
	/// <summary>
	/// Serial HID report format.
	/// </summary>
	[Serializable]
	public struct SerialHidReportFormat : IEquatable<SerialHidReportFormat>
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const bool UseIdDefault = true;

		/// <summary></summary>
		public const byte IdDefault = 0;

		/// <summary></summary>
		public const bool PrependPayloadByteLengthDefault = false;

		/// <summary></summary>
		public const bool AppendTerminatingZeroDefault = false;

		/// <remarks>
		/// It is a requirement for most systems to fill each report to the advertised byte length.
		/// Also, Windows HID.dll requires that output reports are always filled!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "A DLL file is just '.dll'...")]
		public const bool FillLastReportDefault = true;

		#endregion

		/// <summary></summary>
		[XmlElement("UseId")]
		public bool UseId { get; set; }

		/// <summary></summary>
		[XmlElement("Id")]
		public byte Id { get; set; }

		/// <summary></summary>
		[XmlElement("PrependPayloadByteLength")]
		public bool PrependPayloadByteLength { get; set; }

		/// <summary></summary>
		[XmlElement("AppendTerminatingZero")]
		public bool AppendTerminatingZero { get; set; }

		/// <summary></summary>
		[XmlElement("FillLastReport")]
		public bool FillLastReport { get; set; }

		/// <summary>
		/// Creates new format with specified arguments.
		/// </summary>
		public SerialHidReportFormat(bool useId, bool prependPayloadByteLength, bool appendTerminatingZero, bool fillLastReport)
			: this(useId, IdDefault, prependPayloadByteLength, appendTerminatingZero, fillLastReport)
		{
		}

		/// <summary>
		/// Creates new format with specified arguments.
		/// </summary>
		public SerialHidReportFormat(bool useId, byte id, bool prependPayloadByteLength, bool appendTerminatingZero, bool fillLastReport)
		{
			UseId                    = useId;
			Id                       = id;
			PrependPayloadByteLength = prependPayloadByteLength;
			AppendTerminatingZero    = appendTerminatingZero;
			FillLastReport           = fillLastReport;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Returns the byte length of the report header, depending on the given settings.
		/// </summary>
		/// <remarks>
		/// The length also indicates the location of the first payload byte.
		/// </remarks>
		public int HeaderByteLength
		{
			get
			{
				int length = 0;

				if (UseId)
					length++;

				if (PrependPayloadByteLength)
					length++;

				return (length);
			}
		}

		#endregion

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
				UseId                    + ", " +
				Id                       + ", " +
				PrependPayloadByteLength + ", " +
				AppendTerminatingZero    + ", " +
				FillLastReport
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

				hashCode =                    UseId                   .GetHashCode();
				hashCode = (hashCode * 397) ^ Id                      .GetHashCode();
				hashCode = (hashCode * 397) ^ PrependPayloadByteLength.GetHashCode();
				hashCode = (hashCode * 397) ^ AppendTerminatingZero   .GetHashCode();
				hashCode = (hashCode * 397) ^ FillLastReport          .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is SerialHidReportFormat)
				return (Equals((SerialHidReportFormat)obj));
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
		public bool Equals(SerialHidReportFormat other)
		{
			return
			(
				UseId                   .Equals(other.UseId                   ) &&
				Id                      .Equals(other.Id                      ) &&
				PrependPayloadByteLength.Equals(other.PrependPayloadByteLength) &&
				AppendTerminatingZero   .Equals(other.AppendTerminatingZero   ) &&
				FillLastReport          .Equals(other.FillLastReport          )
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(SerialHidReportFormat lhs, SerialHidReportFormat rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(SerialHidReportFormat lhs, SerialHidReportFormat rhs)
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
