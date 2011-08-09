//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY/MKY.IO.Serial/Structs.cs $
// $Author: maettu_this $
// $Date: 2011-04-23 01:49:13 +0200 (Sa, 23 Apr 2011) $
// $Revision: 407 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.IO
{
	/// <summary></summary>
	[Serializable]
	public struct AutoRetry
	{
		/// <summary></summary>
		[XmlElement("Enabled")]
		public bool Enabled;

		/// <summary>Interval of reconnect in ms.</summary>
		[XmlElement("Interval")]
		public int Interval;

		/// <summary></summary>
		public AutoRetry(bool enabled, int interval)
		{
			Enabled = enabled;
			Interval = interval;
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

			AutoRetry other = (AutoRetry)obj;
			return
			(
				(Enabled == other.Enabled) &&
				(Interval == other.Interval)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				Enabled.GetHashCode() ^
				Interval.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(AutoRetry lhs, AutoRetry rhs)
		{
			// Value type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs)) return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(AutoRetry lhs, AutoRetry rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY/MKY.IO.Serial/Structs.cs $
//==================================================================================================
