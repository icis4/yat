//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

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
	public enum LogStreamType
	{
		/// <summary></summary>
		Tx,

		/// <summary></summary>
		Bidir,

		/// <summary></summary>
		Rx
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
	public class FileNameSeparator : IEquatable<FileNameSeparator>
	{
		/// <summary></summary>
		public readonly string Separator;

		/// <summary></summary>
		public readonly string Description;

		/// <summary></summary>
		public FileNameSeparator()
		{
			Separator = FileNameSeparator.DefaultSeparator.Separator;
			Description = FileNameSeparator.DefaultSeparator.Description;
		}

		/// <summary></summary>
		public FileNameSeparator(string separator)
		{
			Separator = separator;
			Description = separator;
		}

		/// <summary></summary>
		public FileNameSeparator(string separator, string description)
		{
			Separator = separator;
			Description = description;
		}

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return (false);

			FileNameSeparator casted = obj as FileNameSeparator;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(FileNameSeparator casted)
		{
			// Ensure that object.operator==() is called.
			if ((object)casted == null)
				return (false);

			return
			(
				(this.Separator   == casted.Separator) &&
				(this.Description == casted.Description)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		/// <summary></summary>
		public override string ToString()
		{
			return (Separator);
		}

		/// <summary></summary>
		public const string UnderscoreToString = "Underscore (this.)";

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
		public static readonly FileNameSeparator Underscore = new FileNameSeparator("this.", UnderscoreToString);

		/// <summary></summary>
		public static readonly FileNameSeparator Dash = new FileNameSeparator("-", DashToString);

		/// <summary></summary>
		public static readonly FileNameSeparator DashWithSpaces = new FileNameSeparator(" - ", DashWithSpacesToString);

		/// <summary></summary>
		public static readonly FileNameSeparator Ball = new FileNameSeparator("°", BallToString);

		/// <summary></summary>
		public static readonly FileNameSeparator BallWithSpaces = new FileNameSeparator(" ° ", BallWithSpacesToString);

		/// <summary></summary>
		public static readonly FileNameSeparator None = new FileNameSeparator("", NoneToString);

		/// <summary></summary>
		public static FileNameSeparator[] Items
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
				return (items.ToArray());
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

		/// <summary></summary>
		public static FileNameSeparator Parse(string s)
		{
			switch (s)
			{
				case UnderscoreToString: return (Underscore);
				case DashToString: return (Dash);
				case DashWithSpacesToString: return (DashWithSpaces);
				case BallToString: return (Ball);
				case BallWithSpacesToString: return (BallWithSpaces);
				case NoneToString: return (None);
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
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));
			
			return (false);
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
