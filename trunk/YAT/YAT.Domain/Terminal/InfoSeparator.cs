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
// YAT Version 2.3.90 Development
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
using System.Diagnostics;
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

		private const string None_separator                   = "";
		private const string None_description                 = "[None]";

		private const string Space_separator                  = " ";
		private const string Space_description                = "[Space]";

		private const string Underscore_separator             =             "_";
		private const string Underscore_description           = "Underscore |_|";
		private const string UnderscoreWithSpaces_separator   =                         " _ ";
		private const string UnderscoreWithSpaces_description = "Underscore with spaces | _ |";

		private const string Dash_separator                   =       "-";
		private const string Dash_description                 = "Dash |-|";
		private const string DashWithSpaces_separator         =                   " - ";
		private const string DashWithSpaces_description       = "Dash with spaces | - |";

		private const string Ball_separator                   =       "°";
		private const string Ball_description                 = "Ball |°|";
		private const string BallWithSpaces_separator         =                   " ° ";
		private const string BallWithSpaces_description       = "Ball with spaces | ° |";

		private const string Comma_separator                  =        ",";
		private const string Comma_description                = "Comma |,|";
		private const string CommaWithSpace_separator         =                   ", ";
		private const string CommaWithSpace_description       = "Comma with space |, |";

		private const string Semicolon_separator              =            ";";
		private const string Semicolon_description            = "Semicolon |;|";
		private const string SemicolonWithSpace_separator     =                       "; ";
		private const string SemicolonWithSpace_description   = "Semicolon with space |; |";

		#endregion

		private string explicitSeparator; // = null;

		/// <summary>Default is <see cref="InfoSeparator.None"/>.</summary>
		public const InfoSeparator Default = InfoSeparator.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public InfoSeparatorEx()
			: this(Default)
		{
		}

		/// <remarks>
		/// Do not use with <see cref="InfoSeparator.Explicit"/> because that selection requires
		/// a separator string. Use <see cref="InfoSeparatorEx(string)"/> instead.
		/// </remarks>
		public InfoSeparatorEx(InfoSeparator separator)
			: base(separator)
		{
			Debug.Assert((separator != InfoSeparator.Explicit), "'InfoSeparator.Explicit' requires a separator string, use 'InfoSeparatorEx(string)' instead!");
		}

		/// <summary></summary>
		public InfoSeparatorEx(string separator)
			: base(InfoSeparator.Explicit) // Do not call this(...) above since that would result in exception above!
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
				case InfoSeparator.None:                 return (None_separator);
				case InfoSeparator.Space:                return (Space_separator);

				case InfoSeparator.Underscore:           return (Underscore_separator);
				case InfoSeparator.UnderscoreWithSpaces: return (UnderscoreWithSpaces_separator);

				case InfoSeparator.Dash:                 return (Dash_separator);
				case InfoSeparator.DashWithSpaces:       return (DashWithSpaces_separator);

				case InfoSeparator.Ball:                 return (Ball_separator);
				case InfoSeparator.BallWithSpaces:       return (BallWithSpaces_separator);

				case InfoSeparator.Comma:                return (Comma_separator);
				case InfoSeparator.CommaWithSpace:       return (CommaWithSpace_separator);

				case InfoSeparator.Semicolon:            return (Semicolon_separator);
				case InfoSeparator.SemicolonWithSpace:   return (SemicolonWithSpace_separator);

				case InfoSeparator.Explicit:             return (this.explicitSeparator);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToDescription()
		{
			switch ((InfoSeparator)UnderlyingEnum)
			{
				case InfoSeparator.None:                 return (None_description);
				case InfoSeparator.Space:                return (Space_description);

				case InfoSeparator.Underscore:           return (Underscore_description);
				case InfoSeparator.UnderscoreWithSpaces: return (UnderscoreWithSpaces_description);

				case InfoSeparator.Dash:                 return (Dash_description);
				case InfoSeparator.DashWithSpaces:       return (DashWithSpaces_description);

				case InfoSeparator.Ball:                 return (Ball_description);
				case InfoSeparator.BallWithSpaces:       return (BallWithSpaces_description);

				case InfoSeparator.Comma:                return (Comma_description);
				case InfoSeparator.CommaWithSpace:       return (CommaWithSpace_description);

				case InfoSeparator.Semicolon:            return (Semicolon_description);
				case InfoSeparator.SemicolonWithSpace:   return (SemicolonWithSpace_description);

				case InfoSeparator.Explicit:             return (this.explicitSeparator);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
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
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as InfoSeparatorEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
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
			var a = new List<InfoSeparatorEx>(12); // Preset the required capacity to improve memory management.

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

			// This method shall only return the fixed items, 'Explicit' is not added therefore.

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
				result = new InfoSeparatorEx(enumResult);
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
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_description))
			{
				result = InfoSeparator.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Space_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Space_description))
			{
				result = InfoSeparator.Space;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Underscore_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Underscore_description))
			{
				result = InfoSeparator.Underscore;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, UnderscoreWithSpaces_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, UnderscoreWithSpaces_description))
			{
				result = InfoSeparator.UnderscoreWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Dash_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Dash_description))
			{
				result = InfoSeparator.Dash;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_description))
			{
				result = InfoSeparator.DashWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Ball_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Ball_description))
			{
				result = InfoSeparator.Ball;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_description))
			{
				result = InfoSeparator.BallWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Comma_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Comma_description))
			{
				result = InfoSeparator.Comma;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, CommaWithSpace_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, CommaWithSpace_description))
			{
				result = InfoSeparator.CommaWithSpace;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Semicolon_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Semicolon_description))
			{
				result = InfoSeparator.Semicolon;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, SemicolonWithSpace_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, SemicolonWithSpace_description))
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
