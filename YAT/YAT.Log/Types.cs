//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 3 Development Version 1.99.31
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
	[Serializable]
	public class FileNameSeparator : IEquatable<FileNameSeparator>
	{
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
		public const string UnderscoreToString = "Underscore (_)";

		/// <summary></summary>
		public const string DashToString = "Dash (-)";

		/// <summary></summary>
		public const string DashWithSpacesToString = "Dash with spaces ( - )";

		/// <summary></summary>
		public const string BallToString = "Ball (°)";

		/// <summary></summary>
		public const string BallWithSpacesToString = "Ball with spaces ( ° )";

		/// <summary></summary>
		public const string NoneToString = "None";

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "OK for the moment, should be replaced by an XEnum anyway.")]
		public static readonly FileNameSeparator Underscore = new FileNameSeparator("_", UnderscoreToString);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "OK for the moment, should be replaced by an XEnum anyway.")]
		public static readonly FileNameSeparator Dash = new FileNameSeparator("-", DashToString);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "OK for the moment, should be replaced by an XEnum anyway.")]
		public static readonly FileNameSeparator DashWithSpaces = new FileNameSeparator(" - ", DashWithSpacesToString);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "OK for the moment, should be replaced by an XEnum anyway.")]
		public static readonly FileNameSeparator Ball = new FileNameSeparator("°", BallToString);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "OK for the moment, should be replaced by an XEnum anyway.")]
		public static readonly FileNameSeparator BallWithSpaces = new FileNameSeparator(" ° ", BallWithSpacesToString);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "OK for the moment, should be replaced by an XEnum anyway.")]
		public static readonly FileNameSeparator None = new FileNameSeparator("", NoneToString);

		/// <summary></summary>
		public static ReadOnlyCollection<FileNameSeparator> Items
		{
			get
			{
				List<FileNameSeparator> items = new List<FileNameSeparator>();
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
				case UnderscoreToString:     return (Underscore);
				case DashToString:           return (Dash);
				case DashWithSpacesToString: return (DashWithSpaces);
				case BallToString:           return (Ball);
				case BallWithSpacesToString: return (BallWithSpaces);
				case NoneToString:           return (None);
				default:                     return (new FileNameSeparator(description, description));
			}
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework,
		/// whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static FileNameSeparator Parse(string s)
		{
			s = s.Trim();

			switch (s)
			{
				case UnderscoreToString:     return (Underscore);
				case DashToString:           return (Dash);
				case DashWithSpacesToString: return (DashWithSpaces);
				case BallToString:           return (Ball);
				case BallWithSpacesToString: return (BallWithSpaces);
				case NoneToString:           return (None);
				default: return (new FileNameSeparator(s));
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
