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
// MKY Version 1.0.28 Development
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace MKY.IO.Ports
{
	/// <remarks>
	/// 'class' instead of 'struct' to allow a parameterless constructor, i.e. initializing
	/// <see cref="UseId"/> and <see cref="PortName"/> to other value than 0 and "".
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parameterless' is a correct English term.")]
	[Serializable]
	public class InUseInfo : IEquatable<InUseInfo>
	{
		/// <summary></summary>
		public const string ActivePortInUseTextDefault = "(in use by this serial port)";

		/// <summary></summary>
		public const string OtherAppInUseTextDefault = "(in use by another application)";

		/// <summary>A unique ID of the item/client that uses the stated serial port.</summary>
		[XmlElement("UseId")]
		public int UseId { get; set; } // = 0;

		/// <summary>The name of the stated serial port.</summary>
		[XmlElement("PortName")]
		public string PortName { get; set; } = SerialPortId.FirstStandardPortName;

		/// <summary>Indicates whether the serial port is open.</summary>
		[XmlElement("IsOpen")]
		public bool IsOpen { get; set; } // = false

		/// <summary>Contains the in use statement text.</summary>
		[XmlElement("IsOpen")]
		public string InUseText { get; set; } = ActivePortInUseTextDefault;

		/// <summary></summary>
		public InUseInfo(int useId, string portName, bool isOpen, string inUseText)
		{
			UseId     = useId;
			PortName  = portName;
			IsOpen    = isOpen;
			InUseText = inUseText;
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
				UseId    + ", " +
				PortName + ", " +
				IsOpen   + ", " +
				InUseText
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

				hashCode =                     UseId;
				hashCode = (hashCode * 397) ^ (PortName != null ? PortName.GetHashCode() : 0);

				// Do not include 'IsOpen' and 'InUseText'.
				// Hash (as well as equality) shall be defined by IDs only.

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as InUseInfo));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(InUseInfo other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
			////base.Equals(other) is not required when deriving from 'object'.

				UseId   .Equals                 (other.UseId) &&
				StringEx.EqualsOrdinal(PortName, other.PortName)

				// Do not include 'IsOpen' and 'InUseText'.
				// Equality (as well as hash) shall be defined by IDs only.
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(InUseInfo lhs, InUseInfo rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(InUseInfo lhs, InUseInfo rhs)
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
