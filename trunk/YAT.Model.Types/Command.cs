using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.Utilities.Types;

namespace YAT.Model.Types
{
	/// <summary>
	/// Stores information about a single line, multi line or file command.
	/// </summary>
	/// <remarks>
	/// This class intentionally combines all three command types in a single class to allow
	/// co-existence of a line and a file command.
	/// </remarks>
	public class Command : IEquatable<Command>, IComparable
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const string EmptyCommandText = "<Enter a command...>";
		/// <summary></summary>
		public const string UndefinedCommandText = "<Define...>";
		/// <summary></summary>
		public const string MultiLineCommandText = "<Multi line...>";
		/// <summary></summary>
		public const string UndefinedFilePathText = "<Set a file...>";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private string _description;
		private string[] _commandLines;
		private Domain.Radix _defaultRadix;
		private bool _isFilePath;
		private string _filePath;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Command()
		{
			Initialize();
		}

		/// <summary></summary>
		public Command(string commandLine)
		{
			Initialize(commandLine, false, new string[] { commandLine }, Domain.Radix.String, "");
		}

		/// <summary></summary>
		public Command(string description, string commandLine)
		{
			Initialize(description, false, new string[] { commandLine }, Domain.Radix.String, "");
		}

		/// <summary></summary>
		public Command(string description, string[] commandLines)
		{
			Initialize(description, false, commandLines, Domain.Radix.String, "");
		}

		/// <summary></summary>
		public Command(string description, string commandLine, Domain.Radix defaultRadix)
		{
			Initialize(description, false, new string[] { commandLine }, defaultRadix, "");
		}

		/// <summary></summary>
		public Command(string description, string[] commandLines, Domain.Radix defaultRadix)
		{
			Initialize(description, false, commandLines, defaultRadix, "");
		}

		/// <summary></summary>
		public Command(string description, bool isFilePath, string filePath)
		{
			Initialize(description, isFilePath, new string[] { "" }, Domain.Radix.String, filePath);
		}

		private void Initialize()
		{
			Initialize("", false, new string[] { "" }, Domain.Radix.String, "");
		}

		private void Initialize(string description, bool isFilePath, string[] commandLines, Domain.Radix defaultRadix, string filePath)
		{
			_description = description;
			_commandLines = commandLines;
			_defaultRadix = defaultRadix;
			_isFilePath = isFilePath;
			_filePath = filePath;
		}

		/// <summary></summary>
		public Command(Command rhs)
		{
			if (rhs != null)
			{
				_description = rhs._description;
				_commandLines = rhs._commandLines;
				_defaultRadix = rhs._defaultRadix;
				_isFilePath = rhs._isFilePath;
				_filePath = rhs._filePath;
			}
			else
			{
				Initialize();
			}
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("Description")]
		public string Description
		{
			get
			{
				if (!string.IsNullOrEmpty(_description))
					return (_description);
				else if (IsCommand)
					return (SingleLineCommand);
				else if (IsFilePath)
					return (System.IO.Path.GetFileNameWithoutExtension(FilePath));
				else
					return ("");
			}
			set
			{
				_description = value;
			}
		}

		/// <summary></summary>
		[XmlElement("CommandLines")]
		public string[] CommandLines
		{
			get { return (_commandLines); }
			set { _commandLines = value;  }
		}

		/// <summary></summary>
		[XmlElement("DefaultRadix")]
		public Domain.Radix DefaultRadix
		{
			get { return (_defaultRadix); }
			set { _defaultRadix = value; }
		}

		/// <summary></summary>
		[XmlElement("IsFilePath")]
		public bool IsFilePath
		{
			get
			{
				return
					(
					_isFilePath &&
					!string.IsNullOrEmpty(_filePath)
					);
			}
			set
			{
				_isFilePath = value;
			}
		}

		/// <summary></summary>
		[XmlElement("FilePath")]
		public string FilePath
		{
			get
			{
				if (IsFilePath)
					return (_filePath);
				else
					return ("");
			}
			set
			{
				_filePath = value;
			}
		}

		#endregion

		#region Convenience Properties
		//==========================================================================================
		// Convenience Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlIgnore]
		public bool IsCommand
		{
			get
			{
				return
					(
					!IsFilePath &&
					(_commandLines.Length >= 1) &&
					(_commandLines[0] != null)
					);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public bool IsSingleLineCommand
		{
			get
			{
				if (IsCommand)
					return ((_commandLines.Length == 1));
				else
					return (false);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public bool IsMultiLineCommand
		{
			get
			{
				if (IsCommand)
					return ((_commandLines.Length > 1));
				else
					return (false);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public bool IsValidCommand
		{
			get
			{
				if (!IsCommand)
					return (false);

				Domain.Parser.Parser p = new Domain.Parser.Parser(_defaultRadix);
				foreach (string commandLine in _commandLines)
				{
					if (!p.TryParse(commandLine))
						return (false);
				}
				return (true);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public string SingleLineCommand
		{
			get
			{
				if (IsSingleLineCommand)
				{
					return (_commandLines[0]);
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("<");
					sb.Append(MultiLineCommand.Length.ToString());
					sb.Append(" lines...>");
					for (int i = 0; i < MultiLineCommand.Length; i++)
					{
						sb.Append(" [");
						sb.Append(MultiLineCommand[i]);
						sb.Append("]");
					}
					return (sb.ToString());
				}
			}
			set
			{
				_commandLines = new string[] { value };
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public string[] MultiLineCommand
		{
			get
			{
				if (IsMultiLineCommand)
					return (_commandLines);
				else if (IsSingleLineCommand)
					return (new string[] { SingleLineCommand });
				else
					return (new string[] { "" });
			}
			set
			{
				_commandLines = value;
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public bool IsValidFilePath
		{
			get
			{
				if (!IsFilePath)
					return (false);

				return (System.IO.File.Exists(_filePath));
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public bool IsValid
		{
			get
			{
				if (IsCommand)
					return (IsValidCommand);
				else
					return (IsValidFilePath);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public bool IsEmpty
		{
			get
			{
				return (!(IsCommand || IsFilePath));
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public override string ToString()
		{
			return (Description);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is Command)
				return (Equals((Command)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(Command value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_description.Equals (value._description) &&
					XArray.ValueEquals(_commandLines, value._commandLines) &&
					_defaultRadix.Equals(value._defaultRadix) &&
					_isFilePath.Equals  (value._isFilePath) &&
					_filePath.Equals    (value._filePath)
					);
			}
			return (false);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region IComparable Members
		//==========================================================================================
		// IComparable Members
		//==========================================================================================

		/// <summary></summary>
		public int CompareTo(object obj)
		{
			if (obj is Command)
			{
				Command c = (Command)obj;
				return (_description.CompareTo(c._description));
			}
			throw (new ArgumentException("Object is not a Command entry"));
		}

		#endregion

		#region Comparison Methods
		//==========================================================================================
		// Comparision Methods
		//==========================================================================================

		/// <summary></summary>
		public static int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB)) return (0);
			if (objA is Command)
			{
				Command casted = (Command)objA;
				return (casted.CompareTo(objB));
			}
			return (-1);
		}

		#endregion

		#region Comparison Operators
		//==========================================================================================
		// Comparison Operators
		//==========================================================================================

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(Command lhs, Command rhs)
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
		public static bool operator !=(Command lhs, Command rhs)
		{
			return (!(lhs == rhs));
		}

		/// <summary></summary>
		public static bool operator <(Command lhs, Command rhs)
		{
			return (Compare(lhs, rhs) < 0);
		}

		/// <summary></summary>
		public static bool operator >(Command lhs, Command rhs)
		{
			return (Compare(lhs, rhs) > 0);
		}

		/// <summary></summary>
		public static bool operator <=(Command lhs, Command rhs)
		{
			return (Compare(lhs, rhs) <= 0);
		}

		/// <summary></summary>
		public static bool operator >=(Command lhs, Command rhs)
		{
			return (Compare(lhs, rhs) >= 0);
		}

		#endregion
	}
}
