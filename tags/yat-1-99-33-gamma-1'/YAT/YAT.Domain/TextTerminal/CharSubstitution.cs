﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 1' Version 1.99.33
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

using MKY;

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
	/// Extended enum CharSubstitutionEx.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class CharSubstitutionEx : EnumEx
	{
		#region String Definitions

		private const string None_string = "None (mixed case)";
		private const string ToUpper_string = "Upper case";
		private const string ToLower_string = "Lower case";

		#endregion

		/// <summary>Default is <see cref="CharSubstitution.None"/>.</summary>
		public CharSubstitutionEx()
			: base(CharSubstitution.None)
		{
		}

		/// <summary></summary>
		protected CharSubstitutionEx(CharSubstitution substitution)
			: base(substitution)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((CharSubstitution)UnderlyingEnum)
			{
				case CharSubstitution.None:    return (None_string);
				case CharSubstitution.ToUpper: return (ToUpper_string);
				case CharSubstitution.ToLower: return (ToLower_string);
			}
			throw (new InvalidOperationException("Program execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug!"));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static CharSubstitutionEx[] GetItems()
		{
			List<CharSubstitutionEx> a = new List<CharSubstitutionEx>();
			a.Add(new CharSubstitutionEx(CharSubstitution.None));
			a.Add(new CharSubstitutionEx(CharSubstitution.ToUpper));
			a.Add(new CharSubstitutionEx(CharSubstitution.ToLower));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static CharSubstitutionEx Parse(string s)
		{
			CharSubstitutionEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid char substitution string."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out CharSubstitutionEx result)
		{
			s = s.Trim();

			if      (StringEx.EqualsOrdinalIgnoreCase(s, None_string))
			{
				result = new CharSubstitutionEx(CharSubstitution.None);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ToUpper_string))
			{
				result = new CharSubstitutionEx(CharSubstitution.ToUpper);
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ToLower_string))
			{
				result = new CharSubstitutionEx(CharSubstitution.ToLower);
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
		public static implicit operator CharSubstitution(CharSubstitutionEx substitution)
		{
			return ((CharSubstitution)substitution.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator CharSubstitutionEx(CharSubstitution substitution)
		{
			return (new CharSubstitutionEx(substitution));
		}

		/// <summary></summary>
		public static implicit operator int(CharSubstitutionEx substitution)
		{
			return (substitution.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator CharSubstitutionEx(int substitution)
		{
			return (new CharSubstitutionEx((CharSubstitution)substitution));
		}

		/// <summary></summary>
		public static implicit operator string(CharSubstitutionEx substitution)
		{
			return (substitution.ToString());
		}

		/// <summary></summary>
		public static implicit operator CharSubstitutionEx(string substitution)
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