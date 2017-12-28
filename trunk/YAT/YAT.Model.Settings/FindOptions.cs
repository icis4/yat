﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author: stroppel-1$
// $Date: Donnerstag, 5. September 2013 11:02:41$
// $Revision: 2$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Epsilon Version 1.99.90
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Model.Settings
{
	/// <remarks>
	/// Initially prepared 'UseRegex' which would override/disable 'CaseSensitive' and 'WholeWord'.
	/// However, the straight-forward implementation of 'WholeWord' (simply surrounding the word
	/// with \b) conflicts with this, since Regex is to be used then anyway. Thus, decided to
	/// remove the 'UseRegex' option and use Regex implicitly.
	/// </remarks>
	[Serializable]
	public struct FindOptions : IEquatable<FindOptions>
	{
		/// <summary></summary>
		[XmlElement("CaseSensitive")]
		public bool CaseSensitive { get; set; }

		/// <summary></summary>
		[XmlElement("WholeWord")]
		public bool WholeWord { get; set; }

		/// <summary></summary>
		public FindOptions(bool caseSensitive, bool wholeWord)
		{
			CaseSensitive = caseSensitive;
			WholeWord     = wholeWord;
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
				CaseSensitive + ", " +
				WholeWord
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

				hashCode =                    CaseSensitive.GetHashCode();
				hashCode = (hashCode * 397) ^ WholeWord    .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is FindOptions)
				return (Equals((FindOptions)obj));
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
		public bool Equals(FindOptions other)
		{
			return
			(
				CaseSensitive.Equals(other.CaseSensitive) &&
				WholeWord    .Equals(other.WholeWord)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have value equality.
		/// </summary>
		public static bool operator ==(FindOptions lhs, FindOptions rhs)
		{
			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have value inequality.
		/// </summary>
		public static bool operator !=(FindOptions lhs, FindOptions rhs)
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
