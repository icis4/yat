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
		public const string EnterCommandText = "<Enter a command...>";

		/// <summary></summary>
		public const string DefineCommandText = "<Define...>";

		/// <summary></summary>
		public const string MultiLineCommandText = "<Multi line...>";

		/// <summary></summary>
		public const string UndefinedFilePathText = "<Set a file...>";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool         isDefined;
		private string       description;
		private string[]     commandLines;
		private Domain.Radix defaultRadix;
		private bool         isPartial;
		private bool         isPartialEol;
		private bool         isFilePath;
		private string       filePath;

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
			Initialize(true, "", new string[] { commandLine }, Domain.Radix.String, false, false, false, "");
		}

		/// <summary></summary>
		public Command(string commandLine, bool isPartial)
		{
			Initialize(true, "", new string[] { commandLine }, Domain.Radix.String, isPartial, false, false, "");
		}

		/// <summary></summary>
		public Command(bool isPartialEol, string completeCommandLine)
		{
			Initialize(true, "", new string[] { completeCommandLine }, Domain.Radix.String, true, isPartialEol, false, "");
		}

		/// <summary></summary>
		public Command(string description, string commandLine)
		{
			Initialize(true, description, new string[] { commandLine }, Domain.Radix.String, false, false, false, "");
		}

		/// <summary></summary>
		public Command(string[] commandLines)
		{
			Initialize(true, "", commandLines, Domain.Radix.String, false, false, false, "");
		}

		/// <summary></summary>
		public Command(string description, string[] commandLines)
		{
			Initialize(true, description, commandLines, Domain.Radix.String, false, false, false, "");
		}

		/// <summary></summary>
		public Command(string description, string commandLine, Domain.Radix defaultRadix)
		{
			Initialize(true, description, new string[] { commandLine }, defaultRadix, false, false, false, "");
		}

		/// <summary></summary>
		public Command(string description, string[] commandLines, Domain.Radix defaultRadix)
		{
			Initialize(true, description, commandLines, defaultRadix, false, false, false, "");
		}

		/// <summary></summary>
		public Command(string description, bool isFilePath, string filePath)
		{
			Initialize(true, description, new string[] { "" }, Domain.Radix.String, false, false, isFilePath, filePath);
		}

		private void Initialize()
		{
			Initialize(false, "", new string[] { "" }, Domain.Radix.String, false, false, false, "");
		}

		private void Initialize(bool isDefined, string description, string[] commandLines, Domain.Radix defaultRadix, bool isPartial, bool isPartialEol, bool isFilePath, string filePath)
		{
			this.isDefined    = isDefined;
			this.description  = description;
			this.commandLines = commandLines;
			this.defaultRadix = defaultRadix;
			this.isPartial    = isPartial;
			this.isPartialEol = isPartialEol;
			this.isFilePath   = isFilePath;
			this.filePath     = filePath;
		}

		/// <summary></summary>
		public Command(Command rhs)
		{
			if (rhs != null)
			{
				this.isDefined    = rhs.isDefined;
				this.description  = rhs.description;
				this.commandLines = rhs.commandLines;
				this.defaultRadix = rhs.defaultRadix;
				this.isPartial    = rhs.isPartial;
				this.isPartialEol = rhs.isPartialEol;
				this.isFilePath   = rhs.isFilePath;
				this.filePath     = rhs.filePath;
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
		[XmlElement("IsDefined")]
		public virtual bool IsDefined
		{
			get { return (this.isDefined); }
			set { this.isDefined = value;  }
		}

		/// <summary>
		/// Gets and sets description.
		/// </summary>
		/// <remarks>
		/// Description cannot be cleared to "" with setting this property because of XML
		/// deserialization issues. Instead, use <see cref="ClearDescription()"/>.
		/// </remarks>
		[XmlElement("Description")]
		public virtual string Description
		{
			get
			{
				if (IsDefined)
				{
					if (!string.IsNullOrEmpty(this.description))
						return (this.description);
					else if (IsText)
						return (SingleLineText);
					else if (IsFilePath)
						return (System.IO.Path.GetFileNameWithoutExtension(FilePath));
					else
						return ("");
				}
				else
				{
					return ("");
				}
			}
			set
			{
				if (IsDefined)
				{
					this.description = value;
				}
				else if ((value != null) &&
						 (value.Length > 0)) // ensure that XML deserialization keeps command undefined
				{
					this.isDefined = true;
					this.description = value;
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CommandLines")]
		public virtual string[] CommandLines
		{
			get
			{
				if (IsDefined)
					return (this.commandLines);
				else
					return (new string[] { "" });
			}
			set
			{
				if (IsDefined)
				{
					this.commandLines = value;
				}
				else if ((value != null) &&
						 (value.Length >= 1) &&
						 (value[0] != null) &&
						 (value[0].Length > 0)) // ensure that XML deserialization keeps command undefined
				{
					this.isDefined = true;
					this.commandLines = value;
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DefaultRadix")]
		public virtual Domain.Radix DefaultRadix
		{
			get { return (this.defaultRadix); }
			set { this.defaultRadix = value; }
		}

		/// <summary></summary>
		[XmlElement("IsPartialText")]
		public virtual bool IsPartialText
		{
			get
			{
				return (IsDefined && this.isPartial);
			}
			set
			{
				if (IsDefined)
				{
					this.isPartial = value;
				}
				else if (value) // Ensure that XML deserialization keeps command undefined.
				{
					this.isDefined = true;
					this.isPartial = value;
				}
			}
		}

		/// <summary></summary>
		[XmlElement("IsPartialEolText")]
		public virtual bool IsPartialEolText
		{
			get
			{
				return (IsDefined && this.isPartialEol);
			}
			set
			{
				if (IsDefined)
				{
					this.isPartialEol = value;
				}
				else if (value) // Ensure that XML deserialization keeps command undefined.
				{
					this.isDefined = true;
					this.isPartialEol = value;
				}
			}
		}

		/// <summary></summary>
		[XmlElement("IsFilePath")]
		public virtual bool IsFilePath
		{
			get
			{
				return (IsDefined && this.isFilePath && !string.IsNullOrEmpty(this.filePath));
			}
			set
			{
				if (IsDefined)
				{
					this.isFilePath = value;
				}
				else if (value) // Ensure that XML deserialization keeps command undefined.
				{
					this.isDefined = true;
					this.isFilePath = value;
				}
			}
		}

		/// <summary></summary>
		[XmlElement("FilePath")]
		public virtual string FilePath
		{
			get
			{
				if (IsFilePath)
					return (this.filePath);
				else
					return ("");
			}
			set
			{
				if (IsDefined)
				{
					this.filePath = value;
				}
				else if ((value != null) &&
						 (value.Length > 0)) // Ensure that XML deserialization keeps command undefined.
				{
					this.isDefined = true;
					this.filePath = value;
				}
			}
		}

		#endregion

		#region Convenience Properties
		//==========================================================================================
		// Convenience Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsText
		{
			get
			{
				return
					(
					IsDefined &&
					!IsFilePath &&
					(this.commandLines != null) &&
					(this.commandLines.Length >= 1) &&
					(this.commandLines[0] != null)
					);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsSingleLineText
		{
			get
			{
				if (IsText && !IsPartialText)
					return ((this.commandLines.Length == 1));
				else
					return (false);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsMultiLineText
		{
			get
			{
				if (IsText && !IsPartialText)
					return ((this.commandLines.Length > 1));
				else
					return (false);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsValidText
		{
			get
			{
				if (!IsText)
					return (false);

				Domain.Parser.Parser p = new Domain.Parser.Parser(this.defaultRadix);
				foreach (string commandLine in this.commandLines)
				{
					if (!p.TryParse(commandLine))
						return (false);
				}
				return (true);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string SingleLineText
		{
			get
			{
				if      (IsSingleLineText)
				{
					return (this.commandLines[0]);
				}
				else if (IsPartialText)
				{
					return (this.commandLines[0]);
				}
				else if (IsMultiLineText)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("<");
					sb.Append(MultiLineText.Length.ToString());
					sb.Append(" lines...>");
					for (int i = 0; i < MultiLineText.Length; i++)
					{
						sb.Append(" [");
						sb.Append(MultiLineText[i]);
						sb.Append("]");
					}
					return (sb.ToString());
				}
				else
				{
					return ("");
				}
			}
			set
			{
				CommandLines = new string[] { value };
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string PartialText
		{
			get
			{
				return (SingleLineText);
			}
			set
			{
				CommandLines = new string[] { value };
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual string[] MultiLineText
		{
			get
			{
				if      (IsSingleLineText)
					return (new string[] { SingleLineText });
				else if (IsPartialText)
					return (new string[] { SingleLineText });
				else if (IsMultiLineText)
					return (this.commandLines);
				else
					return (new string[] { "" });
			}
			set
			{
				CommandLines = value;
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsValidFilePath
		{
			get
			{
				if (!IsFilePath)
					return (false);

				return (System.IO.File.Exists(this.filePath));
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsValid
		{
			get
			{
				if (IsText)
					return (IsValidText);
				else
					return (IsValidFilePath);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void Clear()
		{
			Initialize();
		}

		/// <summary></summary>
		public virtual void ClearDescription()
		{
			this.description = "";
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
			if (obj == null)
				return (false);

			Command casted = obj as Command;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(Command other)
		{
			// Ensure that object.operator==() is called.
			if ((object)other == null)
				return (false);

			return
			(
				(this.isDefined    == other.isDefined) &&
				(this.description  == other.description) &&
				XArray.ValuesEqual(this.commandLines, other.commandLines) &&
				(this.defaultRadix == other.defaultRadix) &&
				(this.isFilePath   == other.isFilePath) &&
				(this.filePath     == other.filePath)
			);
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
		public virtual int CompareTo(object obj)
		{
			if (obj is Command)
			{
				Command c = (Command)obj;
				return (this.description.CompareTo(c.description));
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
