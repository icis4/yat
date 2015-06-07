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
// MKY Development Version 1.0.13
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

using System.Diagnostics.CodeAnalysis;

namespace MKY.Xml
{
	/// <summary></summary>
	public struct AlternateXmlElement
	{
		private string[] xmlPath;
		private string   localName;
		private string[] alternateLocalNames;

		/// <summary></summary>
		public AlternateXmlElement(string[] xmlPath, string localName, string[] alternateLocalNames)
		{
			this.xmlPath = xmlPath;
			this.localName = localName;
			this.alternateLocalNames = alternateLocalNames;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Performance is not an issue here, flexibility and ease of use is...")]
		public string[] XmlPath
		{
			get { return (this.xmlPath); }
		}

		/// <summary></summary>
		public string LocalName
		{
			get { return (this.localName); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Performance is not an issue here, flexibility and ease of use is...")]
		public string[] AlternateLocalNames
		{
			get { return (this.alternateLocalNames); }
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

			AlternateXmlElement other = (AlternateXmlElement)obj;
			return
			(
				(StringEx.EqualsOrdinalIgnoreCase(XmlPath,             other.XmlPath)) &&
				(StringEx.EqualsOrdinalIgnoreCase(LocalName,           other.LocalName)) &&
				(StringEx.EqualsOrdinalIgnoreCase(AlternateLocalNames, other.AlternateLocalNames))
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
				XmlPath.GetHashCode() ^
				LocalName.GetHashCode() ^
				AlternateLocalNames.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(AlternateXmlElement lhs, AlternateXmlElement rhs)
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
		public static bool operator !=(AlternateXmlElement lhs, AlternateXmlElement rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	/// <summary>
	/// Interface for XML contents that have alternate XML elements, e.g. when an XML element
	/// has changed its name.
	/// </summary>
	public interface IAlternateXmlElementProvider
	{
		/// <summary>
		/// Alternate XML elements. Applies to any kind of XML nodes.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Performance is not an issue here, flexibility and ease of use is...")]
		AlternateXmlElement[] AlternateXmlElements { get; }
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
