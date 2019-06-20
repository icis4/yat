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
			if (obj is FileNameSeparator)
				return (Equals((FileNameSeparator)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(FileNameSeparator value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					Separator.Equals(value.Separator) &&
					Description.Equals(value.Description)
					);
			}
			return (false);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		/// <summary></summary>
		new public string ToString()
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
		public const string BallToString = "Ball (�)";
		/// <summary></summary>
		public const string BallWithSpacesToString = "Ball with spaces ( � )";
		/// <summary></summary>
		public const string NoneToString = "None";

		/// <summary></summary>
		public readonly static FileNameSeparator Underscore = new FileNameSeparator("_", UnderscoreToString);
		/// <summary></summary>
		public readonly static FileNameSeparator Dash = new FileNameSeparator("-", DashToString);
		/// <summary></summary>
		public readonly static FileNameSeparator DashWithSpaces = new FileNameSeparator(" - ", DashWithSpacesToString);
		/// <summary></summary>
		public readonly static FileNameSeparator Ball = new FileNameSeparator("�", BallToString);
		/// <summary></summary>
		public readonly static FileNameSeparator BallWithSpaces = new FileNameSeparator(" � ", BallWithSpacesToString);
		/// <summary></summary>
		public readonly static FileNameSeparator None = new FileNameSeparator("", NoneToString);

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
				case UnderscoreToString: return (Underscore);
				case DashToString: return (Dash);
				case DashWithSpacesToString: return (DashWithSpaces);
				case BallToString: return (Ball);
				case BallWithSpacesToString: return (BallWithSpaces);
				case NoneToString: return (None);
				default: return (new FileNameSeparator(description, description));
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