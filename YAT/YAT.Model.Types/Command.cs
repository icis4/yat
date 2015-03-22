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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

using MKY;
using MKY.IO;

#endregion

namespace YAT.Model.Types
{
	/// <summary>
	/// Stores information about a single line, multi line or file command.
	/// </summary>
	/// <remarks>
	/// This class intentionally combines all three command types in a single class to allow
	/// co-existence of a line and a file command.
	/// </remarks>
	[Serializable]
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiLine'?")]
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
		private bool         isPartialText;
		private bool         isPartialTextEol;
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
		public Command(string commandLine, bool isPartialText)
		{
			Initialize(true, "", new string[] { commandLine }, Domain.Radix.String, isPartialText, false, false, "");
		}

		/// <summary></summary>
		public Command(bool isPartialTextEol)
		{
			Initialize(true, "", new string[] { "" }, Domain.Radix.String, isPartialTextEol, isPartialTextEol, false, "");
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

		private void Initialize(bool isDefined, string description, string[] commandLines, Domain.Radix defaultRadix, bool isPartialText, bool isPartialTextEol, bool isFilePath, string filePath)
		{
			this.isDefined        = isDefined;
			this.description      = description;
			this.commandLines     = commandLines;
			this.defaultRadix     = defaultRadix;
			this.isPartialText    = isPartialText;
			this.isPartialTextEol = isPartialTextEol;
			this.isFilePath       = isFilePath;
			this.filePath         = filePath;
		}

		/// <summary></summary>
		public Command(Command rhs)
		{
			if (rhs != null)
			{
				this.isDefined        = rhs.isDefined;
				this.description      = rhs.description;
				this.commandLines     = rhs.commandLines;
				this.defaultRadix     = rhs.defaultRadix;
				this.isPartialText    = rhs.isPartialText;
				this.isPartialTextEol = rhs.isPartialTextEol;
				this.isFilePath       = rhs.isFilePath;
				this.filePath         = rhs.filePath;
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
				else if (!string.IsNullOrEmpty(value)) // Ensure that only non-empty strings define
				{                                      // the command during XML deserialization!
					this.isDefined = true;
					this.description = value;
				}
				else
				{
					// Ensure that XML deserialization keeps command undefined in case of empty strings!
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "The command lines shall be serialized/deserialized as an array.")]
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
						 (value[0].Length > 0)) // Ensure that only non-empty strings define
				{                               // the command during XML deserialization!
					this.isDefined = true;
					this.commandLines = value;
				}
				else
				{
					// Ensure that XML deserialization keeps command undefined in case of empty strings!
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
				return (IsDefined && this.isPartialText);
			}
			set
			{
				if (IsDefined)
				{
					this.isPartialText = value;
				}
				else if (value) // Ensure that only a set flag defines the
				{               // command during XML deserialization!
					this.isDefined = true;
					this.isPartialText = value;
				}
				else
				{
					// Ensure that XML deserialization keeps command undefined in case of cleared flag!
				}
			}
		}

		/// <summary></summary>
		[XmlElement("IsPartialTextEol")]
		public virtual bool IsPartialTextEol
		{
			get
			{
				return (IsDefined && this.isPartialTextEol);
			}
			set
			{
				if (IsDefined)
				{
					this.isPartialTextEol = value;
				}
				else if (value) // Ensure that only a set flag defines the
				{               // command during XML deserialization!
					this.isDefined = true;
					this.isPartialTextEol = value;
				}
				else
				{
					// Ensure that XML deserialization keeps command undefined in case of cleared flag!
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
				else if (value) // Ensure that only a set flag defines the
				{               // command during XML deserialization!
					this.isDefined = true;
					this.isFilePath = value;
				}
				else
				{
					// Ensure that XML deserialization keeps command undefined in case of cleared flag!
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
				else if (!string.IsNullOrEmpty(value)) // Ensure that only non-empty strings define
				{                                      // the command during XML deserialization!
					this.isDefined = true;
					this.filePath = value;
				}
				else
				{
					// Ensure that XML deserialization keeps command undefined in case of empty strings!
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiLine'?")]
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
					sb.Append(MultiLineText.Length.ToString(CultureInfo.InvariantCulture));
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiLine'?")]
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "The multi line text shall be of the same type as the command lines, thus an array.")]
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

		/// <summary></summary>
		public virtual void SetDescriptionFromSingleLineText()
		{
			this.description = SingleLineText;
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
			return (Equals(obj as Command));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(Command other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			return
			(
				(IsDefined                     == other.IsDefined) &&
				(Description                   == other.Description) &&
				ArrayEx.ValuesEqual(CommandLines, other.CommandLines) &&
				(DefaultRadix                  == other.DefaultRadix) &&
				(IsFilePath                    == other.IsFilePath) &&
				PathEx.Equals(FilePath,           other.FilePath)
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
				IsDefined   .GetHashCode() ^
				Description .GetHashCode() ^
				CommandLines.GetHashCode() ^
				DefaultRadix.GetHashCode() ^
				IsFilePath  .GetHashCode() ^
				FilePath    .GetHashCode()
			);
		}

		#endregion

		#region IComparable Members
		//==========================================================================================
		// IComparable Members
		//==========================================================================================

		/// <summary></summary>
		public virtual int CompareTo(object obj)
		{
			Command other = obj as Command;
			if (other != null) // \todo (MKY 2013-05-12): Comparison should be based on 'this.commandLines'.
				return (string.Compare(this.description, other.description, StringComparison.CurrentCulture));
			else
				throw (new ArgumentException("Object is not a Command entry"));
		}

		#endregion

		#region Comparison Methods
		//==========================================================================================
		// Comparison Methods
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB))
				return (0);

			Command casted = objA as Command;
			if (casted != null)
				return (casted.CompareTo(objB));

			return (ObjectEx.InvalidComparisonResult);
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
