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
// MKY Version 1.0.17
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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

#endregion

namespace MKY.IO.Usb
{
	/// <summary>
	/// Serial HID Rx ID setting.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Rx", Justification = "'Rx' is a common term in serial communication.")]
	[Serializable]
	public class SerialHidRxIdUsage : IEquatable<SerialHidRxIdUsage>
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Rx", Justification = "'Rx' is a common term in serial communication.")]
		public const bool SeparateRxIdDefault = false;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Rx", Justification = "'Rx' is a common term in serial communication.")]
		public const bool AnyRxIdDefault = false;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Rx", Justification = "'Rx' is a common term in serial communication.")]
		public const byte RxIdDefault = SerialHidReportFormat.IdDefault;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool separateRxId;
		private bool anyRxId;
		private byte rxId;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Creates new setting with defaults.
		/// </summary>
		public SerialHidRxIdUsage()
		{
			SetDefaults();
		}

		/// <summary>
		/// Creates new setting with specified arguments.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Rx", Justification = "'Rx' is a common term in serial communication.")]
		public SerialHidRxIdUsage(bool separateRxId, bool anyRxId, byte rxId)
		{
			SeparateRxId = separateRxId;
			AnyRxId      = anyRxId;
			RxId         = rxId;
		}

		/// <summary>
		/// Creates new setting from <paramref name="rhs"/>.
		/// </summary>
		public SerialHidRxIdUsage(SerialHidRxIdUsage rhs)
		{
			SeparateRxId = rhs.SeparateRxId;
			AnyRxId      = rhs.AnyRxId;
			RxId         = rhs.RxId;
		}

		/// <summary>
		/// Sets default setting.
		/// </summary>
		protected void SetDefaults()
		{
			SeparateRxId = SeparateRxIdDefault;
			AnyRxId      = AnyRxIdDefault;
			RxId         = RxIdDefault;
		}

		#endregion

		#region Properties

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Rx", Justification = "'Rx' is a common term in serial communication.")]
		[XmlElement("SeparateRxId")]
		public bool SeparateRxId
		{
			get { return (this.separateRxId); }
			set { this.separateRxId = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Rx", Justification = "'Rx' is a common term in serial communication.")]
		[XmlElement("AnyRxId")]
		public bool AnyRxId
		{
			get { return (this.anyRxId); }
			set { this.anyRxId = value;  }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Rx", Justification = "'Rx' is a common term in serial communication.")]
		[XmlElement("RxId")]
		public byte RxId
		{
			get { return (this.rxId); }
			set { this.rxId = value;  }
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SerialHidRxIdUsage));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public virtual bool Equals(SerialHidRxIdUsage other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			return
			(
				(SeparateRxId == other.SeparateRxId) &&
				(AnyRxId      == other.AnyRxId     ) &&
				(RxId         == other.RxId        )
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

		#endregion

		#region Comparison Operators

		/// <summary></summary>
		public static bool operator ==(SerialHidRxIdUsage lhs, SerialHidRxIdUsage rhs)
		{
			// Base reference type implementation of operator ==.
			// See MKY.Test.EqualityAnalysis for details.

			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			// Ensure that potiential <Derived>.Equals() is called.
			// Thus, ensure that object.Equals() is called.
			object obj = (object)lhs;
			return (obj.Equals(rhs));
		}

		/// <summary></summary>
		public static bool operator !=(SerialHidRxIdUsage lhs, SerialHidRxIdUsage rhs)
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
