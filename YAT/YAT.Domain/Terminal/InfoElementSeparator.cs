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
// YAT 2.0 Gamma 2'' Version 1.99.52
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum InfoElementSeparator

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum InfoElementSeparator
	{
		None,

		Space,
		Underscore,
		UnderscoreWithSpaces,
		Dash,
		DashWithSpaces,
		Ball,
		BallWithSpaces,
		Comma,
		CommaWithSpace,
		Semicolon,
		SemicolonWithSpace,

		Explicit
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum InfoElementSeparatorEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class InfoElementSeparatorEx : EnumEx, IEquatable<InfoElementSeparatorEx>
	{
		#region String Definitions

		private const string None_stringSeparator                   = "";
		private const string None_stringDescription                 = "[None]";

		private const string Space_stringSeparator                  = " ";
		private const string Space_stringDescription                = "[Space]";

		private const string Underscore_stringSeparator             =             "_";
		private const string Underscore_stringDescription           = "Underscore |_|";
		private const string UnderscoreWithSpaces_stringSeparator   =                         " _ ";
		private const string UnderscoreWithSpaces_stringDescription = "Underscore with spaces | _ |";

		private const string Dash_stringSeparator                   =       "-";
		private const string Dash_stringDescription                 = "Dash |-|";
		private const string DashWithSpaces_stringSeparator         =                   " - ";
		private const string DashWithSpaces_stringDescription       = "Dash with spaces | - |";

		private const string Ball_stringSeparator                   =       "°";
		private const string Ball_stringDescription                 = "Ball |°|";
		private const string BallWithSpaces_stringSeparator         =                   " ° ";
		private const string BallWithSpaces_stringDescription       = "Ball with spaces | ° |";

		private const string Comma_stringSeparator                  =        ",";
		private const string Comma_stringDescription                = "Comma |,|";
		private const string CommaWithSpace_stringSeparator         =                   ", ";
		private const string CommaWithSpace_stringDescription       = "Comma with space |, |";

		private const string Semicolon_stringSeparator              =            ";";
		private const string Semicolon_stringDescription            = "Semicolon |;|";
		private const string SemicolonWithSpace_stringSeparator     =                       "; ";
		private const string SemicolonWithSpace_stringDescription   = "Semicolon with space |; |";

		#endregion

		private string explicitSeparator; // = null;

		/// <summary>Default is <see cref="InfoElementSeparator.None"/>.</summary>
		public const InfoElementSeparator Default = InfoElementSeparator.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public InfoElementSeparatorEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public InfoElementSeparatorEx(InfoElementSeparator separator)
			: base(separator)
		{
			if (separator == InfoElementSeparator.Explicit)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'InfoElementSeparator.Explicit' requires a separator string, use InfoElementSeparatorEx(string) instead!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public InfoElementSeparatorEx(string separator)
			: this(InfoElementSeparator.Explicit)
		{
			this.explicitSeparator = separator;
		}

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual string ToSeparator()
		{
			switch ((InfoElementSeparator)UnderlyingEnum)
			{
				case InfoElementSeparator.None:                 return (None_stringSeparator);
				case InfoElementSeparator.Space:                return (Space_stringSeparator);

				case InfoElementSeparator.Underscore:           return (Underscore_stringSeparator);
				case InfoElementSeparator.UnderscoreWithSpaces: return (UnderscoreWithSpaces_stringSeparator);

				case InfoElementSeparator.Dash:                 return (Dash_stringSeparator);
				case InfoElementSeparator.DashWithSpaces:       return (DashWithSpaces_stringSeparator);

				case InfoElementSeparator.Ball:                 return (Ball_stringSeparator);
				case InfoElementSeparator.BallWithSpaces:       return (BallWithSpaces_stringSeparator);

				case InfoElementSeparator.Comma:                return (Comma_stringSeparator);
				case InfoElementSeparator.CommaWithSpace:       return (CommaWithSpace_stringSeparator);

				case InfoElementSeparator.Semicolon:            return (Semicolon_stringSeparator);
				case InfoElementSeparator.SemicolonWithSpace:   return (SemicolonWithSpace_stringSeparator);

				case InfoElementSeparator.Explicit:             return (this.explicitSeparator);
			}
			throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public virtual string ToDescription()
		{
			switch ((InfoElementSeparator)UnderlyingEnum)
			{
				case InfoElementSeparator.None:                 return (None_stringDescription);
				case InfoElementSeparator.Space:                return (Space_stringDescription);

				case InfoElementSeparator.Underscore:           return (Underscore_stringDescription);
				case InfoElementSeparator.UnderscoreWithSpaces: return (UnderscoreWithSpaces_stringDescription);

				case InfoElementSeparator.Dash:                 return (Dash_stringDescription);
				case InfoElementSeparator.DashWithSpaces:       return (DashWithSpaces_stringDescription);

				case InfoElementSeparator.Ball:                 return (Ball_stringDescription);
				case InfoElementSeparator.BallWithSpaces:       return (BallWithSpaces_stringDescription);

				case InfoElementSeparator.Comma:                return (Comma_stringDescription);
				case InfoElementSeparator.CommaWithSpace:       return (CommaWithSpace_stringDescription);

				case InfoElementSeparator.Semicolon:            return (Semicolon_stringDescription);
				case InfoElementSeparator.SemicolonWithSpace:   return (SemicolonWithSpace_stringDescription);

				case InfoElementSeparator.Explicit:             return (this.explicitSeparator);
			}
			throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return (ToDescription());
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();

				if ((InfoElementSeparator)UnderlyingEnum == InfoElementSeparator.Explicit)
					hashCode = (hashCode * 397) ^ (this.explicitSeparator != null ? this.explicitSeparator.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as InfoElementSeparatorEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public virtual bool Equals(InfoElementSeparatorEx other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (ReferenceEquals(this, other))
				return (true);

			if (GetType() != other.GetType())
				return (false);

			if ((InfoElementSeparator)UnderlyingEnum == InfoElementSeparator.Explicit)
			{
				return
				(
					base.Equals(other) &&
					StringEx.EqualsOrdinal(this.explicitSeparator, other.explicitSeparator)
				);
			}
			else
			{
				return (base.Equals(other));
			}
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(InfoElementSeparatorEx lhs, InfoElementSeparatorEx rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(InfoElementSeparatorEx lhs, InfoElementSeparatorEx rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static InfoElementSeparatorEx[] GetItems()
		{
			List<InfoElementSeparatorEx> a = new List<InfoElementSeparatorEx>(12); // Preset the required capacity to improve memory management.
			a.Add(new InfoElementSeparatorEx(InfoElementSeparator.None));
			a.Add(new InfoElementSeparatorEx(InfoElementSeparator.Space));
			a.Add(new InfoElementSeparatorEx(InfoElementSeparator.Underscore));
			a.Add(new InfoElementSeparatorEx(InfoElementSeparator.UnderscoreWithSpaces));
			a.Add(new InfoElementSeparatorEx(InfoElementSeparator.Dash));
			a.Add(new InfoElementSeparatorEx(InfoElementSeparator.DashWithSpaces));
			a.Add(new InfoElementSeparatorEx(InfoElementSeparator.Ball));
			a.Add(new InfoElementSeparatorEx(InfoElementSeparator.BallWithSpaces));
			a.Add(new InfoElementSeparatorEx(InfoElementSeparator.Comma));
			a.Add(new InfoElementSeparatorEx(InfoElementSeparator.CommaWithSpace));
			a.Add(new InfoElementSeparatorEx(InfoElementSeparator.Semicolon));
			a.Add(new InfoElementSeparatorEx(InfoElementSeparator.SemicolonWithSpace));
			return (a.ToArray());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as certain separators contain spaces.
		/// </remarks>
		public static InfoElementSeparatorEx Parse(string s)
		{
			InfoElementSeparatorEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid info element separator! String must be a valid separator, or one of the predefined separators."));
		}

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as certain separators contain spaces.
		/// </remarks>
		public static bool TryParse(string s, out InfoElementSeparatorEx result)
		{
			InfoElementSeparator enumResult;
			if (TryParse(s, out enumResult))
			{
				result = enumResult;
				return (true);
			}
			else // Other!
			{
				result = new InfoElementSeparatorEx(s);
				return (true);
			}
		}

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as certain separators contain spaces.
		/// </remarks>
		public static bool TryParse(string s, out InfoElementSeparator result)
		{
			// Do not s = s.Trim(); due to reason described above.

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = InfoElementSeparator.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_stringDescription))
			{
				result = InfoElementSeparator.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Space_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Space_stringDescription))
			{
				result = InfoElementSeparator.Space;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Underscore_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Underscore_stringDescription))
			{
				result = InfoElementSeparator.Underscore;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, UnderscoreWithSpaces_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, UnderscoreWithSpaces_stringDescription))
			{
				result = InfoElementSeparator.UnderscoreWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Dash_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Dash_stringDescription))
			{
				result = InfoElementSeparator.Dash;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_stringDescription))
			{
				result = InfoElementSeparator.DashWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Ball_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Ball_stringDescription))
			{
				result = InfoElementSeparator.Ball;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_stringDescription))
			{
				result = InfoElementSeparator.BallWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Comma_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Comma_stringDescription))
			{
				result = InfoElementSeparator.Comma;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, CommaWithSpace_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, CommaWithSpace_stringDescription))
			{
				result = InfoElementSeparator.CommaWithSpace;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Semicolon_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Semicolon_stringDescription))
			{
				result = InfoElementSeparator.Semicolon;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, SemicolonWithSpace_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, SemicolonWithSpace_stringDescription))
			{
				result = InfoElementSeparator.SemicolonWithSpace;
				return (true);
			}
			else // Invalid string!
			{
				result = new InfoElementSeparatorEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator InfoElementSeparator(InfoElementSeparatorEx separator)
		{
			return ((InfoElementSeparator)separator.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator InfoElementSeparatorEx(InfoElementSeparator separator)
		{
			return (new InfoElementSeparatorEx(separator));
		}

		/// <summary></summary>
		public static implicit operator string(InfoElementSeparatorEx separator)
		{
			return (separator.ToString());
		}

		/// <summary></summary>
		public static implicit operator InfoElementSeparatorEx(string separator)
		{
			return (Parse(separator));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
