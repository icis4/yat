﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using MKY;
using MKY.IO;

#endregion

namespace YAT.Model.Types
{
	/// <summary>
	/// Stores information about a single line, multi-line or file command.
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
		public const Domain.Radix DefaultRadixDefault = Domain.Parser.Parser.DefaultRadixDefault;

		/// <summary></summary>
		public const string DefineCommandText = "<Define...>";

		/// <remarks>'TextText' as it is the text of a text command.</remarks>
		public const string EnterTextText = "<Enter text...>";

		/// <remarks>'TextText' as it is the text of a multi-line text command.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiLine", Justification = "What's wrong with 'MultiLine'?")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiLine'?")]
		public const string MultiLineTextText = "<Multi-line...>";

		/// <summary></summary>
		public const string UndefinedFilePathText = "<Set a file...>";

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string[] UndefinedCommandLines = new string[] { "" };

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <remarks>Required to allow commands with an empty but non-null string.</remarks>
		private bool         isDefined;

		private string       description;
		private Domain.Radix defaultRadix;
		private string[]     commandLines;
		private bool         isPartialText;
		private bool         isPartialTextEol;
		private bool         isFilePath;
		private string       filePath;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <remarks>Parameter-less constructor is required to XML serialization.</remarks>
		public Command()
		{
			Initialize();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public Command(Domain.Radix defaultRadix = DefaultRadixDefault)
		{
			Initialize(defaultRadix);
		}

		/// <remarks>Note that command is initialized as 'not defined'.</remarks>
		private void Initialize(Domain.Radix defaultRadix = DefaultRadixDefault)
		{
			Initialize(false, "", UndefinedCommandLines, defaultRadix, false, false, false, "");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public Command(string commandLine, bool isPartialText = false, Domain.Radix defaultRadix = DefaultRadixDefault)
		{
			Initialize(true, "", new string[] { commandLine }, defaultRadix, isPartialText, false, false, "");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public Command(bool isPartialTextEol, Domain.Radix defaultRadix = DefaultRadixDefault)
		{
			Initialize(true, "", UndefinedCommandLines, defaultRadix, false, isPartialTextEol, false, "");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public Command(string[] commandLines, Domain.Radix defaultRadix = DefaultRadixDefault)
			: this("", commandLines, defaultRadix)
		{
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public Command(string description, string[] commandLines, Domain.Radix defaultRadix = DefaultRadixDefault)
		{
			Initialize(true, description, commandLines, defaultRadix, false, false, false, "");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behavior.")]
		public Command(string description, bool isFilePath, string filePath, Domain.Radix defaultRadix = DefaultRadixDefault)
		{
			Initialize(true, description, UndefinedCommandLines, defaultRadix, false, false, isFilePath, filePath);
		}

		private void Initialize(bool isDefined, string description, string[] commandLines, Domain.Radix defaultRadix, bool isPartialText, bool isPartialTextEol, bool isFilePath, string filePath)
		{
			this.isDefined        = isDefined;
			this.description      = description;
			this.defaultRadix     = defaultRadix;
			this.commandLines     = commandLines;
			this.isPartialText    = isPartialText;
			this.isPartialTextEol = isPartialTextEol;
			this.isFilePath       = isFilePath;
			this.filePath         = filePath;
		}

		/// <summary></summary>
		public Command(Command rhs)
		{
			this.isDefined        = rhs.isDefined;
			this.description      = rhs.description;
			this.defaultRadix     = rhs.defaultRadix;
			this.commandLines     = rhs.commandLines;
			this.isPartialText    = rhs.isPartialText;
			this.isPartialTextEol = rhs.isPartialTextEol;
			this.isFilePath       = rhs.isFilePath;
			this.filePath         = rhs.filePath;
		}

#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~Command()
		{
			MKY.Diagnostics.DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

#endif // DEBUG

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
						return (Path.GetFileName(FilePath)); // Only use file name for better readability!
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
				// Ensure that description never becomes 'null'!

				if (!string.IsNullOrEmpty(value))
					this.description = value;
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
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "The command lines shall be serialized/deserialized as an array.")]
		[XmlElement("CommandLines")]
		public virtual string[] CommandLines
		{
			get
			{
				if (IsDefined)
					return (this.commandLines);
				else
					return (UndefinedCommandLines);
			}
			set
			{
				// Ensure that commandLines never become 'null'!

				if ((value != null) && (value.Length >= 1) && (value[0] != null))
				{
					if (!string.IsNullOrEmpty(value[0]))
					{
						this.isDefined = true;
						this.commandLines = value;
					}
					else // Empty string! Either explicitly defined by the user, or result of XML deserialization!
					{
						// Do not set isDefined = true since it could be the result of the of XML deserialization!
						this.commandLines = value;
					}

					// Reset the other fields:
					if (!string.IsNullOrEmpty(value[0]))
					{
						this.isPartialTextEol = false;
						this.isFilePath = false;
						this.filePath = "";
					}

					// Attention, don't modify the description as that can be defined separately!
				}
				else
				{
					// Reset the own fields:
					this.commandLines = UndefinedCommandLines;
					this.isPartialText = false;
				}
			}
		}

		/// <summary></summary>
		[XmlElement("IsFilePath")]
		public virtual bool IsFilePath
		{
			get
			{
				return (IsDefined && this.isFilePath && (!string.IsNullOrEmpty(this.filePath)));
			}
			set
			{
				this.isFilePath = value;

				if (value)
				{
					// Reset the other fields:
					this.commandLines = UndefinedCommandLines;
					this.isPartialText = false;
					this.isPartialTextEol = false;

					// Attention, don't modify the description as that can be defined separately!
				}
				else
				{
					// Reset the own field:
					this.filePath = "";
				}
			}
		}

		/// <summary>
		/// The absolute or relative path to the file related to this command.
		/// </summary>
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
				// Ensure that only non-empty strings define a file command!

				if (!string.IsNullOrEmpty(value))
				{
					this.isDefined = true;
					this.isFilePath = true;
					this.filePath = value;

					// Reset the other fields:
					this.commandLines = UndefinedCommandLines;
					this.isPartialText = false;
					this.isPartialTextEol = false;

					// Attention, don't modify the description as that can be defined separately!
				}
				else
				{
					// Reset the own field:
					this.isFilePath = false;
				}
			}
		}

		#endregion

		#region Convenience Properties
		//==========================================================================================
		// Convenience Properties
		//==========================================================================================

		/// <remarks>
		/// Similar to <see cref="Description"/>, but not taking the user defined description string
		/// into account. <see cref="DefineCommandText"/> is considered though.
		/// </remarks>
		[XmlIgnore]
		public virtual string Caption
		{
			get
			{
				if (IsDefined)
				{
					if (IsText)
						return (SingleLineText);
					else if (IsFilePath)
						return (Path.GetFileName(FilePath)); // Only use file name as a workaround until PathComboBox has been fixed. See #308 "Minor issues with commands".
					else
						return (DefineCommandText);
				}
				else
				{
					return (DefineCommandText);
				}
			}
		}

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
		public virtual bool IsPartialText
		{
			get
			{
				return (IsDefined && this.isPartialText);
			}

			// { set } makes no sense without text string, use PartialText { set } instead.
		}

		/// <remarks>Required to compose a completed text command after sending the EOL.</remarks>
		[XmlIgnore]
		public virtual bool IsPartialTextEol
		{
			get
			{
				return (IsDefined && this.isPartialTextEol);
			}
			set
			{
				if (value)
				{
					this.isDefined = true;
					this.isPartialTextEol = true;

					// Reset the other fields:
					this.commandLines = UndefinedCommandLines;
					this.isPartialText = false;
					this.isFilePath = true;
					this.filePath = "";

					// Attention, don't modify the description as that can be defined separately!
				}
				else
				{
					// Reset the own field:
					this.isPartialTextEol = false;
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsSingleLineText
		{
			get
			{
				if (IsText && !IsPartialText && !IsPartialTextEol)
					return ((this.commandLines.Length == 1));
				else
					return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiLine", Justification = "What's wrong with 'MultiLine'?")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiLine'?")]
		[XmlIgnore]
		public virtual bool IsMultiLineText
		{
			get
			{
				if (IsText && !IsPartialText && !IsPartialTextEol)
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

				using (Domain.Parser.Parser p = new Domain.Parser.Parser())
				{
					foreach (string commandLine in this.commandLines)
					{
						if (!p.TryParse(commandLine, this.defaultRadix))
							return (false);
					}

					return (true);
				}
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
					var sb = new StringBuilder();
					sb.Append("<");
					sb.Append(MultiLineText.Length.ToString(CultureInfo.CurrentCulture));
					sb.Append(" lines...>");
					for (int i = 0; i < MultiLineText.Length; i++)
					{
						sb.Append(" [");
						sb.Append(MultiLineText[i]);
						sb.Append("]");
					}
					return (sb.ToString());
				}
				else // includes IsPartialTextEol
				{
					return ("");
				}
			}
			set
			{
				if (value != null)
				{
					this.isDefined = true;
					this.commandLines = new string[] { value };

					// Reset the other fields:
					this.isPartialText = false;
					this.isPartialTextEol = false;
					this.isFilePath = true;
					this.filePath = "";

					// Attention, don't modify the description as that can be defined separately!
				}
				else
				{
					// Reset the own fields:
					this.commandLines = UndefinedCommandLines;
				}
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
				if (value != null)
				{
					this.isDefined = true;
					this.commandLines = new string[] { value };
					this.isPartialText = true;

					// Reset the other fields:
					this.isPartialTextEol = false;
					this.isFilePath = true;
					this.filePath = "";

					// Attention, don't modify the description as that can be defined separately!
				}
				else
				{
					// Reset the own fields:
					this.commandLines = UndefinedCommandLines;
					this.isPartialText = false;
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiLine", Justification = "What's wrong with 'MultiLine'?")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiLine'?")]
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "The multi-line text shall be of the same type as the command lines, thus an array.")]
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
					return (CommandLines);
				else // includes IsPartialTextEol
					return (UndefinedCommandLines);
			}
			set
			{
				if ((value != null) && (value.Length >= 1) && (value[0] != null))
				{
					this.isDefined = true;
					this.commandLines = value;

					// Reset the other fields:
					this.isPartialText = false;
					this.isPartialTextEol = false;
					this.isFilePath = true;
					this.filePath = "";

					// Attention, don't modify the description as that can be defined separately!
				}
				else
				{
					// Reset the own fields:
					this.commandLines = UndefinedCommandLines;
				}
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

				return (File.Exists(Environment.ExpandEnvironmentVariables(this.filePath))); // May be absolute or relative to current directory.
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
			Initialize(this.defaultRadix); // Clear all except default radix.
		}

		/// <summary></summary>
		public virtual void ClearDescription()
		{
			this.description = "";
		}

		/// <summary></summary>
		public virtual void DescriptionFromSingleLineText()
		{
			this.description = SingleLineText;
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Do not redirect to <see cref="Description"/> in order to ensure that user defined
		/// description string is not shown in a combo box or similar. Instead, use the dedicated
		/// <see cref="Caption"/> property.
		/// </remarks>
		public override string ToString()
		{
			return (Caption);
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
			unchecked
			{
				int hashCode;

				hashCode =                     IsDefined                      .GetHashCode();
				hashCode = (hashCode * 397) ^  CommandLines                   .GetHashCode();
				hashCode = (hashCode * 397) ^  DefaultRadix                   .GetHashCode();
				hashCode = (hashCode * 397) ^  IsFilePath                     .GetHashCode();
				hashCode = (hashCode * 397) ^ (FilePath     != null ? FilePath.GetHashCode() : 0);

				// Do not consider 'Description' as it does not define the command.

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as Command));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(Command other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				IsDefined                   .Equals(other.IsDefined) &&
				ArrayEx.ElementsEqual(CommandLines, other.CommandLines) &&
				DefaultRadix                .Equals(other.DefaultRadix) &&
				IsFilePath                  .Equals(other.IsFilePath) &&
				PathEx.Equals(FilePath,             other.FilePath)
			);

			// Do not consider 'Description' as it does not define the command.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(Command lhs, Command rhs)
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
		public static bool operator !=(Command lhs, Command rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region IComparable Members / Comparison Methods and Operators
		//==========================================================================================
		// IComparable Members / Comparison Methods and Operators
		//==========================================================================================

		/// <summary></summary>
		public virtual int CompareTo(object obj)
		{
			var other = (obj as Command);
			if (other != null) // Using 'Description' as this is visible to the user.
				return (string.Compare(Description, other.Description, StringComparison.CurrentCulture));
			else
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'" + obj.ToString() + "' does not specify a 'Command'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, "obj"));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "'obj' is commonly used throughout the .NET framework.")]
		public static int Compare(object objA, object objB)
		{
			if (ReferenceEquals(objA, objB))
				return (0);

			var casted = (objA as Command);
			if (casted != null)
				return (casted.CompareTo(objB));

			return (ObjectEx.InvalidComparisonResult);
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
