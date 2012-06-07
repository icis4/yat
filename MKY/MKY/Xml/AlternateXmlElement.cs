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

namespace MKY.Xml
{
	/// <summary></summary>
	public struct AlternateXmlElement
	{
		/// <summary></summary>
		public string[] XmlPath;

		/// <summary></summary>
		public string LocalName;

		/// <summary></summary>
		public string[] AlternateLocalNames;

		/// <summary></summary>
		public AlternateXmlElement(string[] xmlPath, string localName, string[] alternateLocalNames)
		{
			XmlPath = xmlPath;
			LocalName = localName;
			AlternateLocalNames = alternateLocalNames;
		}

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

			AlternateXmlElement other = (AlternateXmlElement)obj;
			return
			(
				(StringEx.EqualsOrdinalIgnoreCase(XmlPath,   other.XmlPath)) &&
				(StringEx.EqualsOrdinalIgnoreCase(LocalName, other.LocalName)) &&
				(StringEx.EqualsOrdinalIgnoreCase(AlternateLocalNames, other.AlternateLocalNames))
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				XmlPath.GetHashCode()   ^
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
		AlternateXmlElement[] AlternateXmlElements { get; }
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
