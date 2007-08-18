using System;
using System.Collections.Generic;

namespace MKY.YAT.Log
{
	public enum LogFormat
	{
		Raw, Neat
	}

	public enum LogStreamType
	{
		Tx, Bidir, Rx
	}

	public enum LogFileWriteMode
	{
		Create, Append
	}

	public class FileNameSeparator : IEquatable<FileNameSeparator>
	{
		public readonly string Separator;
		public readonly string Description;

		public FileNameSeparator()
		{
			Separator = FileNameSeparator.DefaultSeparator.Separator;
			Description = FileNameSeparator.DefaultSeparator.Description;
		}

		public FileNameSeparator(string separator)
		{
			Separator = separator;
			Description = separator;
		}

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

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		new public string ToString()
		{
			return (Separator);
		}

		public const string UnderscoreToString = "Underscore (_)";
		public const string DashToString = "Dash (-)";
		public const string DashWithSpacesToString = "Dash with spaces ( - )";
		public const string BallToString = "Ball (°)";
		public const string BallWithSpacesToString = "Ball with spaces ( ° )";
		public const string NoneToString = "None";

		public readonly static FileNameSeparator Underscore = new FileNameSeparator("_", UnderscoreToString);
		public readonly static FileNameSeparator Dash = new FileNameSeparator("-", DashToString);
		public readonly static FileNameSeparator DashWithSpaces = new FileNameSeparator(" - ", DashWithSpacesToString);
		public readonly static FileNameSeparator Ball = new FileNameSeparator("°", BallToString);
		public readonly static FileNameSeparator BallWithSpaces = new FileNameSeparator(" ° ", BallWithSpacesToString);
		public readonly static FileNameSeparator None = new FileNameSeparator("", NoneToString);

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

		public static FileNameSeparator DefaultSeparator
		{
			get { return (Dash); }
		}

		public static implicit operator string(FileNameSeparator separator)
		{
			return (separator.Description);
		}

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
		/// Determines whether the two specified objects have reference and value equality.
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
