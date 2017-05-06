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
// MKY Development Version 1.0.18
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

namespace MKY.Xml
{
	/// <summary></summary>
	public struct AlternateXmlElement : IEquatable<AlternateXmlElement>
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
		//==========================================================================================
		// Object Members
		//==========================================================================================

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

				hashCode =                     XmlPath                      .GetHashCode();
				hashCode = (hashCode * 397) ^ (LocalName != null ? LocalName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  AlternateLocalNames          .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is AlternateXmlElement)
				return (Equals((AlternateXmlElement)obj));
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
		public bool Equals(AlternateXmlElement other)
		{
			return
			(	// XML is case insensitive!
				(StringEx.EqualsOrdinalIgnoreCase(XmlPath,             other.XmlPath)) &&
				(StringEx.EqualsOrdinalIgnoreCase(LocalName,           other.LocalName)) &&
				(StringEx.EqualsOrdinalIgnoreCase(AlternateLocalNames, other.AlternateLocalNames))
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(AlternateXmlElement lhs, AlternateXmlElement rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
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
