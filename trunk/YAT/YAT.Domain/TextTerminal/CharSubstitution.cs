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
// YAT Version 2.1.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in the YAT.Domain\TextTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum CharSubstitution

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum CharSubstitution
	{
		None,

		ToUpper,
		ToLower
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum CharSubstitutionEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class CharSubstitutionEx : EnumEx
	{
		#region String Definitions

		private const string None_string = "None (mixed case)";
		private const string ToUpper_string = "Upper case";
		private const string ToLower_string = "Lower case";

		#endregion

		/// <summary>Default is <see cref="CharSubstitution.None"/>.</summary>
		public const CharSubstitution Default = CharSubstitution.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public CharSubstitutionEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public CharSubstitutionEx(CharSubstitution substitution)
			: base(substitution)
		{
		}

		#region ToString
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public override string ToString()
		{
			switch ((CharSubstitution)UnderlyingEnum)
			{
				case CharSubstitution.None:    return (None_string);
				case CharSubstitution.ToUpper: return (ToUpper_string);
				case CharSubstitution.ToLower: return (ToLower_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static CharSubstitutionEx[] GetItems()
		{
			var a = new List<CharSubstitutionEx>(3); // Preset the required capacity to improve memory management.

			a.Add(new CharSubstitutionEx(CharSubstitution.None));
			a.Add(new CharSubstitutionEx(CharSubstitution.ToUpper));
			a.Add(new CharSubstitutionEx(CharSubstitution.ToLower));

			return (a.ToArray());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static CharSubstitutionEx Parse(string s)
		{
			CharSubstitutionEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid char substitution string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out CharSubstitutionEx result)
		{
			CharSubstitution enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new CharSubstitutionEx(enumResult);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out CharSubstitution result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = CharSubstitution.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_string))
			{
				result = CharSubstitution.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ToUpper_string))
			{
				result = CharSubstitution.ToUpper;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, ToLower_string))
			{
				result = CharSubstitution.ToLower;
				return (true);
			}
			else // Invalid string!
			{
				result = new CharSubstitutionEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

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
