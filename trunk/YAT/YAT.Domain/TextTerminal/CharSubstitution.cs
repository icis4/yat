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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

using MKY.System.Types;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum CharSubstitution

	/// <summary></summary>
	public enum CharSubstitution
	{
		/// <summary></summary>
		None,

		/// <summary></summary>
		ToUpper,

		/// <summary></summary>
		ToLower,
	}

	#endregion

	/// <summary>
	/// Extended enum XCharSubstitution.
	/// </summary>
	public class XCharSubstitution : XEnum
	{
		#region String Definitions

		private const string None_string = "None (mixed case)";
		private const string ToUpper_string = "Upper case";
		private const string ToLower_string = "Lower case";

		#endregion

		/// <summary>Default is <see cref="CharSubstitution.None"/>.</summary>
		public XCharSubstitution()
			: base(CharSubstitution.None)
		{
		}

		/// <summary></summary>
		protected XCharSubstitution(CharSubstitution substitution)
			: base(substitution)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			switch ((CharSubstitution)UnderlyingEnum)
			{
				case CharSubstitution.None:    return (None_string);
				case CharSubstitution.ToUpper: return (ToUpper_string);
				case CharSubstitution.ToLower: return (ToLower_string);
			}
			throw (new NotImplementedException(UnderlyingEnum.ToString()));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static XCharSubstitution[] GetItems()
		{
			List<XCharSubstitution> a = new List<XCharSubstitution>();
			a.Add(new XCharSubstitution(CharSubstitution.None));
			a.Add(new XCharSubstitution(CharSubstitution.ToUpper));
			a.Add(new XCharSubstitution(CharSubstitution.ToLower));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XCharSubstitution Parse(string substitution)
		{
			XCharSubstitution result;

			if (TryParse(substitution, out result))
				return (result);
			else
				throw (new ArgumentOutOfRangeException("substitution", substitution, "Invalid substitution."));
		}

		/// <summary></summary>
		public static bool TryParse(string substitution, out XCharSubstitution result)
		{
			if      (string.Compare(substitution, None_string, StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = new XCharSubstitution(CharSubstitution.None);
				return (true);
			}
			else if (string.Compare(substitution, ToUpper_string, StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = new XCharSubstitution(CharSubstitution.ToUpper);
				return (true);
			}
			else if (string.Compare(substitution, ToLower_string, StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = new XCharSubstitution(CharSubstitution.ToLower);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator CharSubstitution(XCharSubstitution substitution)
		{
			return ((CharSubstitution)substitution.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XCharSubstitution(CharSubstitution substitution)
		{
			return (new XCharSubstitution(substitution));
		}

		/// <summary></summary>
		public static implicit operator int(XCharSubstitution substitution)
		{
			return (substitution.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XCharSubstitution(int substitution)
		{
			return (new XCharSubstitution((CharSubstitution)substitution));
		}

		/// <summary></summary>
		public static implicit operator string(XCharSubstitution substitution)
		{
			return (substitution.ToString());
		}

		/// <summary></summary>
		public static implicit operator XCharSubstitution(string substitution)
		{
			return (Parse(substitution));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
