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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	public class FileNameSeparatorEx : EnumEx, IEquatable<FileNameSeparatorEx>
	{
		#region String Definitions

		private const string None_separator                   = "";
		private const string None_description                 = "[None]";

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

		#endregion

		private string explicitSeparator; // = null;

		/// <summary>Default is <see cref="FileNameSeparator.Dash"/>.</summary>
		public const FileNameSeparator Default = FileNameSeparator.Dash;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public FileNameSeparatorEx()
			: this(Default)
		{
		}

		/// <remarks>
		/// Do not use with <see cref="FileNameSeparator.Explicit"/> because that selection requires
		/// a separator string. Use <see cref="FileNameSeparatorEx(string)"/> instead.
		/// </remarks>
		public FileNameSeparatorEx(FileNameSeparator separator)
			: base(separator)
		{
			Debug.Assert((separator != FileNameSeparator.Explicit), "'FileNameSeparator.Explicit' requires a separator string, use 'FileNameSeparatorEx(string)' instead!");
		}

		/// <summary></summary>
		public FileNameSeparatorEx(string separator)
			: base(FileNameSeparator.Explicit) // Do not call this(...) above since that would result in exception above!
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
			switch ((FileNameSeparator)UnderlyingEnum)
			{
				case FileNameSeparator.None:                 return (None_separator);

				case FileNameSeparator.Underscore:           return (Underscore_separator);
				case FileNameSeparator.UnderscoreWithSpaces: return (UnderscoreWithSpaces_separator);

				case FileNameSeparator.Dash:                 return (Dash_separator);
				case FileNameSeparator.DashWithSpaces:       return (DashWithSpaces_separator);

				case FileNameSeparator.Ball:                 return (Ball_separator);
				case FileNameSeparator.BallWithSpaces:       return (BallWithSpaces_separator);

				case FileNameSeparator.Explicit:             return (this.explicitSeparator);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual string ToDescription()
		{
			switch ((FileNameSeparator)UnderlyingEnum)
			{
				case FileNameSeparator.None:                 return (None_description);

				case FileNameSeparator.Underscore:           return (Underscore_description);
				case FileNameSeparator.UnderscoreWithSpaces: return (UnderscoreWithSpaces_description);

				case FileNameSeparator.Dash:                 return (Dash_description);
				case FileNameSeparator.DashWithSpaces:       return (DashWithSpaces_description);

				case FileNameSeparator.Ball:                 return (Ball_description);
				case FileNameSeparator.BallWithSpaces:       return (BallWithSpaces_description);

				case FileNameSeparator.Explicit:             return (this.explicitSeparator);

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

				if ((FileNameSeparator)UnderlyingEnum == FileNameSeparator.Explicit)
					hashCode = (hashCode * 397) ^ (this.explicitSeparator != null ? this.explicitSeparator.GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as FileNameSeparatorEx));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(FileNameSeparatorEx other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			if ((FileNameSeparator)UnderlyingEnum == FileNameSeparator.Explicit)
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
		public static bool operator ==(FileNameSeparatorEx lhs, FileNameSeparatorEx rhs)
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
		public static bool operator !=(FileNameSeparatorEx lhs, FileNameSeparatorEx rhs)
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
		public static FileNameSeparatorEx[] GetItems()
		{
			var a = new List<FileNameSeparatorEx>(7); // Preset the required capacity to improve memory management.

			a.Add(new FileNameSeparatorEx(FileNameSeparator.None));
			a.Add(new FileNameSeparatorEx(FileNameSeparator.Underscore));
			a.Add(new FileNameSeparatorEx(FileNameSeparator.UnderscoreWithSpaces));
			a.Add(new FileNameSeparatorEx(FileNameSeparator.Dash));
			a.Add(new FileNameSeparatorEx(FileNameSeparator.DashWithSpaces));
			a.Add(new FileNameSeparatorEx(FileNameSeparator.Ball));
			a.Add(new FileNameSeparatorEx(FileNameSeparator.BallWithSpaces));

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
				result = new FileNameSeparatorEx(enumResult);
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
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_description))
			{
				result = FileNameSeparator.None;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Underscore_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Underscore_description))
			{
				result = FileNameSeparator.Underscore;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, UnderscoreWithSpaces_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, UnderscoreWithSpaces_description))
			{
				result = FileNameSeparator.UnderscoreWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Dash_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Dash_description))
			{
				result = FileNameSeparator.Dash;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_description))
			{
				result = FileNameSeparator.DashWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Ball_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Ball_description))
			{
				result = FileNameSeparator.Ball;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_separator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_description))
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
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

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
