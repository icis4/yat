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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
// is intentionally placed into the YAT.Domain namespace even though the file is located in
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	#region Enum Separator

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum Separator
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
	public class SeparatorEx : EnumEx, IEquatable<SeparatorEx>
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

		/// <summary>Default is <see cref="Separator.None"/>.</summary>
		public const Separator Default = Separator.None;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public SeparatorEx()
			: this(Default)
		{
		}

		/// <remarks>
		/// Do not use with <see cref="Separator.Explicit"/> because that selection requires
		/// a separator string. Use <see cref="SeparatorEx(string)"/> instead.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="separator"/> is <see cref="Separator.Explicit"/>. Use <see cref="SeparatorEx(string)"/> instead.
		/// </exception>
		public SeparatorEx(Separator separator)
			: base(separator)
		{
			if (separator == Separator.Explicit)
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'Separator.Explicit' requires a separator string, use 'SeparatorEx(string)' instead!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public SeparatorEx(string separator)
			: base(Separator.Explicit) // Do not call this(...) above since that would result in exception above!
		{
			this.explicitSeparator = separator;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public bool IsExplicit
		{
			get { return ((Separator)UnderlyingEnum == Separator.Explicit); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual string ToSeparator()
		{
			switch ((Separator)UnderlyingEnum)
			{
				case Separator.None:                 return (None_separator);
				case Separator.Space:                return (Space_separator);

				case Separator.Underscore:           return (Underscore_separator);
				case Separator.UnderscoreWithSpaces: return (UnderscoreWithSpaces_separator);

				case Separator.Dash:                 return (Dash_separator);
				case Separator.DashWithSpaces:       return (DashWithSpaces_separator);

				case Separator.Ball:                 return (Ball_separator);
				case Separator.BallWithSpaces:       return (BallWithSpaces_separator);

				case Separator.Comma:                return (Comma_separator);
				case Separator.CommaWithSpace:       return (CommaWithSpace_separator);

				case Separator.Semicolon:            return (Semicolon_separator);
				case Separator.SemicolonWithSpace:   return (SemicolonWithSpace_separator);

				case Separator.Explicit:             return (this.explicitSeparator);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToDescription()
		{
			switch ((Separator)UnderlyingEnum)
			{
				case Separator.None:                 return (None_description);
				case Separator.Space:                return (Space_description);

				case Separator.Underscore:           return (Underscore_description);
				case Separator.UnderscoreWithSpaces: return (UnderscoreWithSpaces_description);

				case Separator.Dash:                 return (Dash_description);
				case Separator.DashWithSpaces:       return (DashWithSpaces_description);

				case Separator.Ball:                 return (Ball_description);
				case Separator.BallWithSpaces:       return (BallWithSpaces_description);

				case Separator.Comma:                return (Comma_description);
				case Separator.CommaWithSpace:       return (CommaWithSpace_description);

				case Separator.Semicolon:            return (Semicolon_description);
				case Separator.SemicolonWithSpace:   return (SemicolonWithSpace_description);

				case Separator.Explicit:             return (this.explicitSeparator);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

				if ((Separator)UnderlyingEnum == Separator.Explicit)
					hashCode = (hashCode * 397) ^ (this.explicitSeparator != null ? this.explicitSeparator.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SeparatorEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(SeparatorEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			if ((Separator)UnderlyingEnum == Separator.Explicit)
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
		public static bool operator ==(SeparatorEx lhs, SeparatorEx rhs)
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
		public static bool operator !=(SeparatorEx lhs, SeparatorEx rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static SeparatorEx[] GetItems()
		{
			var a = new List<SeparatorEx>(12); // Preset the required capacity to improve memory management.

			a.Add(new SeparatorEx(Separator.None));
			a.Add(new SeparatorEx(Separator.Space));
			a.Add(new SeparatorEx(Separator.Underscore));
			a.Add(new SeparatorEx(Separator.UnderscoreWithSpaces));
			a.Add(new SeparatorEx(Separator.Dash));
			a.Add(new SeparatorEx(Separator.DashWithSpaces));
			a.Add(new SeparatorEx(Separator.Ball));
			a.Add(new SeparatorEx(Separator.BallWithSpaces));
			a.Add(new SeparatorEx(Separator.Comma));
			a.Add(new SeparatorEx(Separator.CommaWithSpace));
			a.Add(new SeparatorEx(Separator.Semicolon));
			a.Add(new SeparatorEx(Separator.SemicolonWithSpace));

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
		public static SeparatorEx Parse(string s)
		{
			SeparatorEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid info element separator! String must be a valid separator, or one of the predefined separators."));
		}

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as certain separators contain spaces.
		/// </remarks>
		public static bool TryParse(string s, out SeparatorEx result)
		{
			Separator enumResult;
			if (TryParse(s, out enumResult))
			{
				result = new SeparatorEx(enumResult);
				return (true);
			}
			else // Other!
			{
				result = new SeparatorEx(s);
				return (true);
			}
		}

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as certain separators contain spaces.
		/// </remarks>
		public static bool TryParse(string s, out Separator result)
		{
			// Do not s = s.Trim(); due to reason described above.

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = Separator.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_description))
			{
				result = Separator.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Space_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Space_description))
			{
				result = Separator.Space;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Underscore_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Underscore_description))
			{
				result = Separator.Underscore;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, UnderscoreWithSpaces_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, UnderscoreWithSpaces_description))
			{
				result = Separator.UnderscoreWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Dash_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Dash_description))
			{
				result = Separator.Dash;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_description))
			{
				result = Separator.DashWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Ball_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Ball_description))
			{
				result = Separator.Ball;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_description))
			{
				result = Separator.BallWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Comma_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Comma_description))
			{
				result = Separator.Comma;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, CommaWithSpace_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, CommaWithSpace_description))
			{
				result = Separator.CommaWithSpace;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Semicolon_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Semicolon_description))
			{
				result = Separator.Semicolon;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, SemicolonWithSpace_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, SemicolonWithSpace_description))
			{
				result = Separator.SemicolonWithSpace;
				return (true);
			}
			else // Invalid string!
			{
				result = new SeparatorEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator Separator(SeparatorEx separator)
		{
			return ((Separator)separator.UnderlyingEnum);
		}

		/// <remarks>
		/// Explicit because cast doesn't work for <see cref="Separator.Explicit"/>.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="separator"/> is <see cref="Separator.Explicit"/>.
		/// </exception>
		public static explicit operator SeparatorEx(Separator separator)
		{
			return (new SeparatorEx(separator));
		}

		/// <summary></summary>
		public static implicit operator string(SeparatorEx separator)
		{
			return (separator.ToString());
		}

		/// <summary></summary>
		public static implicit operator SeparatorEx(string separator)
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
