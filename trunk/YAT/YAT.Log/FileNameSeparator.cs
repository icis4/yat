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
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

using MKY;

namespace YAT.Log
{
	#region Enum FileNameSeparator

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum FileNameSeparator
	{
		None,

		Underscore,
		UnderscoreWithSpaces,
		Dash,
		DashWithSpaces,
		Ball,
		BallWithSpaces,

		Explicit
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum FileNameSeparatorEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class FileNameSeparatorEx : EnumEx
	{
		#region String Definitions

		private const string None_stringSeparator                   = "";
		private const string None_stringDescription                 = "[None]";

		private const string Underscore_stringSeparator             =             "_";
		private const string Underscore_stringDescription           = "Underscore |_|";
		private const string UnderscoreWithSpaces_stringSeparator   =                         " _ ";
		private const string UnderscoreWithSpaces_stringDescription = "Underscore with spaces | _ |";

		private const string Dash_stringSeparator                   =         "-";
		private const string Dash_stringDescription                 = "Dash |-|";
		private const string DashWithSpaces_stringSeparator         =                   " - ";
		private const string DashWithSpaces_stringDescription       = "Dash with spaces | - |";

		private const string Ball_stringSeparator                   =       "°";
		private const string Ball_stringDescription                 = "Ball |°|";
		private const string BallWithSpaces_stringSeparator         =                   " ° ";
		private const string BallWithSpaces_stringDescription       = "Ball with spaces | ° |";

		#endregion

		private string explicitSeparator; // = null;

		/// <summary>Default is <see cref="FileNameSeparator.Dash"/>.</summary>
		public const FileNameSeparator Default = FileNameSeparator.Dash;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public FileNameSeparatorEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public FileNameSeparatorEx(FileNameSeparator separator)
			: base(separator)
		{
			if (separator == FileNameSeparator.Explicit)
				throw (new InvalidOperationException("'FileNameSeparator.Explicit' requires a separator string, use FileNameSeparatorEx(string) instead!"));
		}

		/// <summary></summary>
		public FileNameSeparatorEx(string separator)
			: this(FileNameSeparator.Explicit)
		{
			this.explicitSeparator = separator;
		}

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as FileNameSeparatorEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public virtual bool Equals(FileNameSeparatorEx other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			if ((FileNameSeparator)UnderlyingEnum == FileNameSeparator.Explicit)
			{
				return
				(
					base.Equals(other) &&
					(this.explicitSeparator == other.explicitSeparator)
				);
			}
			else
			{
				return (base.Equals(other));
			}
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();

				if ((FileNameSeparator)UnderlyingEnum == FileNameSeparator.Explicit)
					hashCode = (hashCode * 397) ^ (this.explicitSeparator != null ? this.explicitSeparator.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary></summary>
		public override string ToString()
		{
			return (ToDescription());
		}

		/// <summary></summary>
		public virtual string ToSeparator()
		{
			switch ((FileNameSeparator)UnderlyingEnum)
			{
				case FileNameSeparator.None:                 return (None_stringSeparator);

				case FileNameSeparator.Underscore:           return (Underscore_stringSeparator);
				case FileNameSeparator.UnderscoreWithSpaces: return (UnderscoreWithSpaces_stringSeparator);

				case FileNameSeparator.Dash:                 return (Dash_stringSeparator);
				case FileNameSeparator.DashWithSpaces:       return (DashWithSpaces_stringSeparator);

				case FileNameSeparator.Ball:                 return (Ball_stringSeparator);
				case FileNameSeparator.BallWithSpaces:       return (BallWithSpaces_stringSeparator);

				case FileNameSeparator.Explicit:             return (this.explicitSeparator);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public virtual string ToDescription()
		{
			switch ((FileNameSeparator)UnderlyingEnum)
			{
				case FileNameSeparator.None:                 return (None_stringDescription);

				case FileNameSeparator.Underscore:           return (Underscore_stringDescription);
				case FileNameSeparator.UnderscoreWithSpaces: return (UnderscoreWithSpaces_stringDescription);

				case FileNameSeparator.Dash:                 return (Dash_stringDescription);
				case FileNameSeparator.DashWithSpaces:       return (DashWithSpaces_stringDescription);

				case FileNameSeparator.Ball:                 return (Ball_stringDescription);
				case FileNameSeparator.BallWithSpaces:       return (BallWithSpaces_stringDescription);

				case FileNameSeparator.Explicit:             return (this.explicitSeparator);
			}
			throw (new NotSupportedException("Program execution should never get here,'" + UnderlyingEnum.ToString() + "' is an unknown item." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		#endregion

		#region GetItems

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static FileNameSeparatorEx[] GetItems()
		{
			List<FileNameSeparatorEx> a = new List<FileNameSeparatorEx>(7); // Preset the required capacity to improve memory management.
			a.Add(new FileNameSeparatorEx(FileNameSeparator.None));
			a.Add(new FileNameSeparatorEx(FileNameSeparator.Underscore));
			a.Add(new FileNameSeparatorEx(FileNameSeparator.UnderscoreWithSpaces));
			a.Add(new FileNameSeparatorEx(FileNameSeparator.Dash));
			a.Add(new FileNameSeparatorEx(FileNameSeparator.DashWithSpaces));
			a.Add(new FileNameSeparatorEx(FileNameSeparator.Ball));
			a.Add(new FileNameSeparatorEx(FileNameSeparator.BallWithSpaces));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as certain separators contain spaces.
		/// </remarks>
		public static FileNameSeparatorEx Parse(string s)
		{
			FileNameSeparatorEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid file name separator! String must be a valid separator, or one of the predefined separators."));
		}

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as certain separators contain spaces.
		/// </remarks>
		public static bool TryParse(string s, out FileNameSeparatorEx result)
		{
			FileNameSeparator enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = enumResult;
				return (true);
			}
			else
			{
				char[] invalid = Path.GetInvalidFileNameChars();
				if (!StringEx.ContainsAny(s, invalid)) // Valid explicit?
				{
					result = new FileNameSeparatorEx(s);
					return (true);
				}
				else // Invalid string!
				{
					result = null;
					return (false);
				}
			}
		}

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as certain separators contain spaces.
		/// </remarks>
		public static bool TryParse(string s, out FileNameSeparator result)
		{
			// Do not s = s.Trim(); due to reason described above.

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = FileNameSeparator.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_stringDescription))
			{
				result = FileNameSeparator.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Underscore_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Underscore_stringDescription))
			{
				result = FileNameSeparator.Underscore;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, UnderscoreWithSpaces_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, UnderscoreWithSpaces_stringDescription))
			{
				result = FileNameSeparator.UnderscoreWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Dash_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Dash_stringDescription))
			{
				result = FileNameSeparator.Dash;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_stringDescription))
			{
				result = FileNameSeparator.DashWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Ball_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Ball_stringDescription))
			{
				result = FileNameSeparator.Ball;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_stringDescription))
			{
				result = FileNameSeparator.BallWithSpaces;
				return (true);
			}
			else // Invalid string!
			{
				result = new FileNameSeparatorEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator FileNameSeparator(FileNameSeparatorEx separator)
		{
			return ((FileNameSeparator)separator.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator FileNameSeparatorEx(FileNameSeparator separator)
		{
			return (new FileNameSeparatorEx(separator));
		}

		/// <summary></summary>
		public static implicit operator string(FileNameSeparatorEx separator)
		{
			return (separator.ToString());
		}

		/// <summary></summary>
		public static implicit operator FileNameSeparatorEx(string separator)
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
