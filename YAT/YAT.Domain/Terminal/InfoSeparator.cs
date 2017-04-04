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
// YAT 2.0 Gamma 3 Development Version 1.99.53
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY;

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum InfoSeparator

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum InfoSeparator
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
	/// Extended enum InfoSeparatorEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class InfoSeparatorEx : EnumEx, IEquatable<InfoSeparatorEx>
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

		/// <summary>Default is <see cref="InfoSeparator.None"/>.</summary>
		public const InfoSeparator Default = InfoSeparator.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public InfoSeparatorEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public InfoSeparatorEx(InfoSeparator separator)
			: base(separator)
		{
			if (separator == InfoSeparator.Explicit)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'InfoSeparator.Explicit' requires a separator string, use InfoElementSeparatorEx(string) instead!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public InfoSeparatorEx(string separator)
			: this(InfoSeparator.Explicit)
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
			switch ((InfoSeparator)UnderlyingEnum)
			{
				case InfoSeparator.None:                 return (None_stringSeparator);
				case InfoSeparator.Space:                return (Space_stringSeparator);

				case InfoSeparator.Underscore:           return (Underscore_stringSeparator);
				case InfoSeparator.UnderscoreWithSpaces: return (UnderscoreWithSpaces_stringSeparator);

				case InfoSeparator.Dash:                 return (Dash_stringSeparator);
				case InfoSeparator.DashWithSpaces:       return (DashWithSpaces_stringSeparator);

				case InfoSeparator.Ball:                 return (Ball_stringSeparator);
				case InfoSeparator.BallWithSpaces:       return (BallWithSpaces_stringSeparator);

				case InfoSeparator.Comma:                return (Comma_stringSeparator);
				case InfoSeparator.CommaWithSpace:       return (CommaWithSpace_stringSeparator);

				case InfoSeparator.Semicolon:            return (Semicolon_stringSeparator);
				case InfoSeparator.SemicolonWithSpace:   return (SemicolonWithSpace_stringSeparator);

				case InfoSeparator.Explicit:             return (this.explicitSeparator);
			}
			throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an unknown item!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public virtual string ToDescription()
		{
			switch ((InfoSeparator)UnderlyingEnum)
			{
				case InfoSeparator.None:                 return (None_stringDescription);
				case InfoSeparator.Space:                return (Space_stringDescription);

				case InfoSeparator.Underscore:           return (Underscore_stringDescription);
				case InfoSeparator.UnderscoreWithSpaces: return (UnderscoreWithSpaces_stringDescription);

				case InfoSeparator.Dash:                 return (Dash_stringDescription);
				case InfoSeparator.DashWithSpaces:       return (DashWithSpaces_stringDescription);

				case InfoSeparator.Ball:                 return (Ball_stringDescription);
				case InfoSeparator.BallWithSpaces:       return (BallWithSpaces_stringDescription);

				case InfoSeparator.Comma:                return (Comma_stringDescription);
				case InfoSeparator.CommaWithSpace:       return (CommaWithSpace_stringDescription);

				case InfoSeparator.Semicolon:            return (Semicolon_stringDescription);
				case InfoSeparator.SemicolonWithSpace:   return (SemicolonWithSpace_stringDescription);

				case InfoSeparator.Explicit:             return (this.explicitSeparator);
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

				if ((InfoSeparator)UnderlyingEnum == InfoSeparator.Explicit)
					hashCode = (hashCode * 397) ^ (this.explicitSeparator != null ? this.explicitSeparator.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as InfoSeparatorEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(InfoSeparatorEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			if ((InfoSeparator)UnderlyingEnum == InfoSeparator.Explicit)
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
		public static bool operator ==(InfoSeparatorEx lhs, InfoSeparatorEx rhs)
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
		public static bool operator !=(InfoSeparatorEx lhs, InfoSeparatorEx rhs)
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
		public static InfoSeparatorEx[] GetItems()
		{
			List<InfoSeparatorEx> a = new List<InfoSeparatorEx>(12); // Preset the required capacity to improve memory management.
			a.Add(new InfoSeparatorEx(InfoSeparator.None));
			a.Add(new InfoSeparatorEx(InfoSeparator.Space));
			a.Add(new InfoSeparatorEx(InfoSeparator.Underscore));
			a.Add(new InfoSeparatorEx(InfoSeparator.UnderscoreWithSpaces));
			a.Add(new InfoSeparatorEx(InfoSeparator.Dash));
			a.Add(new InfoSeparatorEx(InfoSeparator.DashWithSpaces));
			a.Add(new InfoSeparatorEx(InfoSeparator.Ball));
			a.Add(new InfoSeparatorEx(InfoSeparator.BallWithSpaces));
			a.Add(new InfoSeparatorEx(InfoSeparator.Comma));
			a.Add(new InfoSeparatorEx(InfoSeparator.CommaWithSpace));
			a.Add(new InfoSeparatorEx(InfoSeparator.Semicolon));
			a.Add(new InfoSeparatorEx(InfoSeparator.SemicolonWithSpace));
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
		public static InfoSeparatorEx Parse(string s)
		{
			InfoSeparatorEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid info element separator! String must be a valid separator, or one of the predefined separators."));
		}

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as certain separators contain spaces.
		/// </remarks>
		public static bool TryParse(string s, out InfoSeparatorEx result)
		{
			InfoSeparator enumResult;
			if (TryParse(s, out enumResult))
			{
				result = enumResult;
				return (true);
			}
			else // Other!
			{
				result = new InfoSeparatorEx(s);
				return (true);
			}
		}

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as certain separators contain spaces.
		/// </remarks>
		public static bool TryParse(string s, out InfoSeparator result)
		{
			// Do not s = s.Trim(); due to reason described above.

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = InfoSeparator.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_stringDescription))
			{
				result = InfoSeparator.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Space_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Space_stringDescription))
			{
				result = InfoSeparator.Space;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Underscore_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Underscore_stringDescription))
			{
				result = InfoSeparator.Underscore;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, UnderscoreWithSpaces_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, UnderscoreWithSpaces_stringDescription))
			{
				result = InfoSeparator.UnderscoreWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Dash_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Dash_stringDescription))
			{
				result = InfoSeparator.Dash;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_stringDescription))
			{
				result = InfoSeparator.DashWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Ball_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Ball_stringDescription))
			{
				result = InfoSeparator.Ball;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_stringDescription))
			{
				result = InfoSeparator.BallWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Comma_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Comma_stringDescription))
			{
				result = InfoSeparator.Comma;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, CommaWithSpace_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, CommaWithSpace_stringDescription))
			{
				result = InfoSeparator.CommaWithSpace;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Semicolon_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Semicolon_stringDescription))
			{
				result = InfoSeparator.Semicolon;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, SemicolonWithSpace_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, SemicolonWithSpace_stringDescription))
			{
				result = InfoSeparator.SemicolonWithSpace;
				return (true);
			}
			else // Invalid string!
			{
				result = new InfoSeparatorEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator InfoSeparator(InfoSeparatorEx separator)
		{
			return ((InfoSeparator)separator.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator InfoSeparatorEx(InfoSeparator separator)
		{
			return (new InfoSeparatorEx(separator));
		}

		/// <summary></summary>
		public static implicit operator string(InfoSeparatorEx separator)
		{
			return (separator.ToString());
		}

		/// <summary></summary>
		public static implicit operator InfoSeparatorEx(string separator)
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
