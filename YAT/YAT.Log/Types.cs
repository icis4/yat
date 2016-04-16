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
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using MKY;

namespace YAT.Log
{
	/// <summary></summary>
	public enum LogFormat
	{
		/// <summary></summary>
		Raw,

		/// <summary></summary>
		Neat
	}

	/// <summary></summary>
	public enum LogChannelType
	{
		/// <summary></summary>
		Tx,

		/// <summary></summary>
		Bidir,

		/// <summary></summary>
		Rx
	}

	/// <summary></summary>
	public enum LogChannel
	{
		/// <summary></summary>
		RawTx = 0,

		/// <summary></summary>
		RawBidir = 1,

		/// <summary></summary>
		RawRx = 2,

		/// <summary></summary>
		NeatTx = 3,

		/// <summary></summary>
		NeatBidir = 4,

		/// <summary></summary>
		NeatRx = 5
	}

	/// <summary></summary>
	public enum LogFileWriteMode
	{
		/// <summary></summary>
		Create,

		/// <summary></summary>
		Append
	}

	/// <summary></summary>
	public enum LogFileEncoding
	{
		/// <summary></summary>
		UTF8,

		/// <summary></summary>
		Terminal
	}

	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	public class FileNameSeparator : IEquatable<FileNameSeparator>
	{
		#region String Definitions

		private const string Underscore_stringSeparator       = "_";
		private const string Underscore_stringDescription     = "Underscore (_)";

		private const string Dash_stringSeparator             = "-";
		private const string Dash_stringDescription           = "Dash (-)";
		private const string DashWithSpaces_stringSeparator   = " - ";
		private const string DashWithSpaces_stringDescription = "Dash with spaces ( - )";

		private const string Ball_stringSeparator             = "°";
		private const string Ball_stringDescription           = "Ball (°)";
		private const string BallWithSpaces_stringSeparator   = " ° ";
		private const string BallWithSpaces_stringDescription = "Ball with spaces ( ° )";

		private const string None_stringSeparator             = "";
		private const string None_stringDescription           = "[None]";

		#endregion

		private string separator;
		private string description;

		/// <summary></summary>
		public FileNameSeparator()
			: this(FileNameSeparator.DefaultSeparator.Separator, FileNameSeparator.DefaultSeparator.Description)
		{
		}

		/// <summary></summary>
		public FileNameSeparator(string separator)
			: this(separator, separator)
		{
		}

		/// <summary></summary>
		public FileNameSeparator(string separator, string description)
		{
			this.separator = separator;
			this.description = description;
		}

		/// <summary></summary>
		public string Separator
		{
			get { return (this.separator); }
		}

		/// <summary></summary>
		public string Description
		{
			get { return (this.description); }
		}

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as FileNameSeparator));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(FileNameSeparator other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			return
			(
				StringEx.EqualsOrdinalIgnoreCase(Separator,   other.Separator) &&
				StringEx.EqualsOrdinalIgnoreCase(Description, other.Description)
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			return
			(
				Separator  .GetHashCode() ^
				Description.GetHashCode()
			);
		}

		/// <summary></summary>
		public override string ToString()
		{
			return (Separator);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "OK for the moment, should be replaced by an XEnum anyway.")]
		public static readonly FileNameSeparator Underscore = new FileNameSeparator(Underscore_stringSeparator, Underscore_stringDescription);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "OK for the moment, should be replaced by an XEnum anyway.")]
		public static readonly FileNameSeparator Dash = new FileNameSeparator(Dash_stringSeparator, Dash_stringDescription);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "OK for the moment, should be replaced by an XEnum anyway.")]
		public static readonly FileNameSeparator DashWithSpaces = new FileNameSeparator(DashWithSpaces_stringSeparator, DashWithSpaces_stringDescription);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "OK for the moment, should be replaced by an XEnum anyway.")]
		public static readonly FileNameSeparator Ball = new FileNameSeparator(Ball_stringSeparator, Ball_stringDescription);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "OK for the moment, should be replaced by an XEnum anyway.")]
		public static readonly FileNameSeparator BallWithSpaces = new FileNameSeparator(BallWithSpaces_stringSeparator, BallWithSpaces_stringDescription);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "OK for the moment, should be replaced by an XEnum anyway.")]
		public static readonly FileNameSeparator None = new FileNameSeparator(None_stringSeparator, None_stringDescription);

		/// <summary></summary>
		public static ReadOnlyCollection<FileNameSeparator> Items
		{
			get
			{
				List<FileNameSeparator> items = new List<FileNameSeparator>(6); // Preset the required capactiy to improve memory management.
				items.Add(Underscore);
				items.Add(Dash);
				items.Add(DashWithSpaces);
				items.Add(Ball);
				items.Add(BallWithSpaces);
				items.Add(None);
				return (items.AsReadOnly());
			}
		}

		/// <summary></summary>
		public static FileNameSeparator DefaultSeparator
		{
			get { return (Dash); }
		}

		/// <summary></summary>
		public static implicit operator string(FileNameSeparator separator)
		{
			return (separator.Description);
		}

		/// <summary></summary>
		public static explicit operator FileNameSeparator(string description)
		{
			switch (description)
			{
				case     Underscore_stringDescription: return (Underscore);
				case           Dash_stringDescription: return (Dash);
				case DashWithSpaces_stringDescription: return (DashWithSpaces);
				case           Ball_stringDescription: return (Ball);
				case BallWithSpaces_stringDescription: return (BallWithSpaces);
				case           None_stringDescription: return (None);
				default:                               return (new FileNameSeparator(description, description));
			}
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as certain separators contain spaces.
		/// </remarks>
		public static FileNameSeparator Parse(string s)
		{
			FileNameSeparator result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid file name separator string."));
		}

		/// <remarks>
		/// Opposed to the convention of the .NET framework, whitespace is NOT
		/// trimmed from <paramref name="s"/> as certain separators contain spaces.
		/// </remarks>
		public static bool TryParse(string s, out FileNameSeparator result)
		{
			// Do not s = s.Trim(); due to reason described above.

			if      (StringEx.EqualsOrdinalIgnoreCase(s, Underscore_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Underscore_stringDescription))
			{
				result = Underscore;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Dash_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Dash_stringDescription))
			{
				result = Dash;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, DashWithSpaces_stringDescription))
			{
				result = DashWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, Ball_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, Ball_stringDescription))
			{
				result = Ball;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, BallWithSpaces_stringDescription))
			{
				result = BallWithSpaces;
				return (true);
			}
			else if (StringEx.EqualsOrdinalIgnoreCase(s, None_stringSeparator) ||
			         StringEx.EqualsOrdinalIgnoreCase(s, None_stringDescription) ||
			         string.IsNullOrEmpty(s)) // Default!
			{
				result = None;
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(FileNameSeparator lhs, FileNameSeparator rhs)
		{
			// Base reference type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			// Ensure that potiential <Derived>.Equals() is called.
			// Thus, ensure that object.Equals() is called.
			object obj = (object)lhs;
			return (obj.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(FileNameSeparator lhs, FileNameSeparator rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
