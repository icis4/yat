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
// MKY Development Version 1.0.14
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

#endregion

namespace MKY.IO.Usb
{
	/// <summary>
	/// Serial HID report format.
	/// </summary>
	[Serializable]
	public class SerialHidReportFormat : IEquatable<SerialHidReportFormat>
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
		/// Also, Windows HID.dll requires that outgoing reports are always filled!
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "A DLL file is just '.dll'...")]
		public const bool FillLastReportDefault = true;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool useId;
		private byte id;
		private bool prependPayloadByteLength;
		private bool appendTerminatingZero;
		private bool fillLastReport;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Creates new format with defaults.
		/// </summary>
		public SerialHidReportFormat()
		{
			SetDefaults();
		}

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

		/// <summary>
		/// Creates new format from <paramref name="rhs"/>.
		/// </summary>
		public SerialHidReportFormat(SerialHidReportFormat rhs)
		{
			UseId                    = rhs.UseId;
			Id                       = rhs.Id;
			PrependPayloadByteLength = rhs.PrependPayloadByteLength;
			AppendTerminatingZero    = rhs.AppendTerminatingZero;
			FillLastReport           = rhs.FillLastReport;
		}

		/// <summary>
		/// Sets default format.
		/// </summary>
		protected void SetDefaults()
		{
			UseId                    = UseIdDefault;
			Id                       = IdDefault;
			PrependPayloadByteLength = PrependPayloadByteLengthDefault;
			AppendTerminatingZero    = AppendTerminatingZeroDefault;
			FillLastReport           = FillLastReportDefault;
		}

		#endregion

		#region Properties

		/// <summary></summary>
		[XmlElement("UseId")]
		public bool UseId
		{
			get { return (this.useId); }
			set { this.useId = value;  }
		}

		/// <summary></summary>
		[XmlElement("Id")]
		public byte Id
		{
			get { return (this.id); }
			set { this.id = value;  }
		}

		/// <summary></summary>
		[XmlElement("PrependPayloadByteLength")]
		public bool PrependPayloadByteLength
		{
			get { return (this.prependPayloadByteLength); }
			set { this.prependPayloadByteLength = value;  }
		}

		/// <summary></summary>
		[XmlElement("AppendTerminatingZero")]
		public bool AppendTerminatingZero
		{
			get { return (this.appendTerminatingZero); }
			set { this.appendTerminatingZero = value;  }
		}

		/// <summary></summary>
		[XmlElement("FillLastReport")]
		public bool FillLastReport
		{
			get { return (this.fillLastReport); }
			set { this.fillLastReport = value;  }
		}

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

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SerialHidReportFormat));
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
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			return
			(
				(UseId                    == other.UseId                   ) &&
				(Id                       == other.Id                      ) &&
				(PrependPayloadByteLength == other.PrependPayloadByteLength) &&
				(AppendTerminatingZero    == other.AppendTerminatingZero   ) &&
				(FillLastReport           == other.FillLastReport          )
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
				UseId                   .GetHashCode() ^
				Id                      .GetHashCode() ^
				PrependPayloadByteLength.GetHashCode() ^
				AppendTerminatingZero   .GetHashCode() ^
				FillLastReport          .GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		/// <summary></summary>
		public static bool operator ==(SerialHidReportFormat lhs, SerialHidReportFormat rhs)
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
