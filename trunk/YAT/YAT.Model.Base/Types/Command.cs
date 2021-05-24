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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
	/// <remarks>
	/// This class cannot be implemented immutable [ImmutableObject(true)] because it is directly
	/// serialized to .yat files (.xml). Default serialization requires { set } fields/properties.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Semantic of readonly fields is constant.")]
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",     Justification = "What's wrong with 'MultiLine'?")]
		public const string MultiLineTextText = "<Multi-line...>";

		/// <summary></summary>
		public const string UndefinedFilePathText = "<Set a file...>";

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "This is a 'readonly', thus meant to be constant.")]
		private readonly string[] UndefinedTextLines = new string[] { "" };

		/// <remarks>Explicitly using "[empty]" instead of "[Empty]" same as e.g. "[any]" or "[localhost]".</remarks>
		private const string EmptyDescription = "[empty]";

		/// <remarks>Explicitly using "lines..." instead of "Lines..." same as "empty above".</remarks>
		private const string LinesText = "lines...";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		/// <remarks>Required to allow commands with an empty but non-null string.</remarks>
		private bool         isDefined;

		private string       description;
		private Domain.Radix defaultRadix;
		private string[]     textLines;
		private bool         isFilePath;
		private string       filePath;

		// Transitory properties:
		private bool         isPartialText;
		private bool         isPartialTextEol;

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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public Command(Domain.Radix defaultRadix = DefaultRadixDefault)
		{
			Initialize(defaultRadix);
		}

		/// <remarks>Note that command is initialized as 'not defined'.</remarks>
		private void Initialize(Domain.Radix defaultRadix = DefaultRadixDefault)
		{
			Initialize(false, "", UndefinedTextLines, defaultRadix, false, false, false, "");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public Command(string textLine, bool isPartialText = false, Domain.Radix defaultRadix = DefaultRadixDefault)
			: this("", textLine, isPartialText, defaultRadix)
		{
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public Command(string description, string textLine, bool isPartialText = false, Domain.Radix defaultRadix = DefaultRadixDefault)
		{
			Initialize(true, description, new string[] { textLine }, defaultRadix, isPartialText, false, false, "");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public Command(bool isPartialTextEol, Domain.Radix defaultRadix = DefaultRadixDefault)
		{
			Initialize(true, "", UndefinedTextLines, defaultRadix, false, isPartialTextEol, false, "");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public Command(string[] textLines, Domain.Radix defaultRadix = DefaultRadixDefault)
			: this("", textLines, defaultRadix)
		{
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public Command(string description, string[] textLines, Domain.Radix defaultRadix = DefaultRadixDefault)
		{
			Initialize(true, description, textLines, defaultRadix, false, false, false, "");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public Command(string description, bool isFilePath, string filePath, Domain.Radix defaultRadix = DefaultRadixDefault)
		{
			Initialize(true, description, UndefinedTextLines, defaultRadix, false, false, isFilePath, filePath);
		}

		private void Initialize(bool isDefined, string description, string[] textLines, Domain.Radix defaultRadix, bool isPartialText, bool isPartialTextEol, bool isFilePath, string filePath)
		{
			this.isDefined        = isDefined;
			this.description      = description;
			this.defaultRadix     = defaultRadix;
			this.textLines        = textLines;
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
			this.textLines        = rhs.textLines;
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
		/// This property shall not be serialized, because a default description text shall not be
		/// serialized, thus, the helper below is used for serialization.
		/// </remarks>
		[XmlIgnore]
		public virtual string Description
		{
			get
			{
				if (!string.IsNullOrEmpty(this.description))
				{
					return (this.description);
				}
				else if (IsText)
				{
					var slt = SingleLineText;
					if (!string.IsNullOrEmpty(slt))
						return (slt);
					else
						return (EmptyDescription);
				}
				else if (IsFilePath)
				{
					return (Path.GetFileName(FilePath)); // Only use file name for better readability? Or better use PathEx.Limit(FilePath, <Length>) when fixing #308 "Minor issues with commands"? But what length?
				}
				else
				{
					return ("");
				}
			}
			set
			{
				// Ensure that description never becomes 'null' because that would lead to XML serialization issues!
				this.description = ((value != null) ? (value) : (""));
			}
		}

		/// <remarks>
		/// Required to limit XML serialization to <see cref="description"/> field rather than <see cref="Description"/> property.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Emphasize the purpose.")]
		[XmlElement("Description")]
		public virtual string Description_ForSerialization
		{
			get { return (this.description); }
			set { this.description = value;  }
		}

		/// <summary></summary>
		[XmlElement("DefaultRadix")]
		public virtual Domain.Radix DefaultRadix
		{
			get { return (this.defaultRadix); }
			set { this.defaultRadix = value;  }
		}

		/// <remarks>
		/// XML tag is "CommandLines" for backward compatibility.
		/// May be changed to "TextLines" once bugs #232 "Issues with TolerantXmlSerializer"
		/// and #246 "Issues with AlternateTolerantXmlSerializer" have been fixed.
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "The text lines shall be serialized/deserialized as an array.")]
		[XmlElement("CommandLines")]
		public virtual string[] TextLines
		{
			get
			{
				if (IsDefined)
					return (this.textLines);
				else
					return (UndefinedTextLines);
			}
			set
			{
				// Ensure that 'textLines' never becomes 'null'!

				if ((value != null) && (value.Length >= 1) && (value[0] != null))
				{
					if (!string.IsNullOrEmpty(value[0]))
					{
						this.isDefined = true;
						this.textLines = value;
					}
					else // Empty string, either explicitly defined by the user, or result of XML deserialization.
					{
						// Do not set isDefined = true since it could be the result of the of XML deserialization.
						this.textLines = value;
					}

					// Reset the other fields:
					if (!string.IsNullOrEmpty(value[0]))
					{
						this.isPartialTextEol = false;
						this.isFilePath = false;
						this.filePath = "";
					}

					// Attention, don't modify the description as that can be defined separately!
					// An empty explicit description will result in the default description.
				}
				else
				{
					// Reset the "own" fields:
					this.textLines = UndefinedTextLines;
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
					this.textLines = UndefinedTextLines;
					this.isPartialText = false;
					this.isPartialTextEol = false;

					// Attention, do not modify the description as that can be defined separately!
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
					this.textLines = UndefinedTextLines;
					this.isPartialText = false;
					this.isPartialTextEol = false;

					// Attention, don't modify the description as that can be defined separately!
					// An empty explicit description will result in the default description.
				}
				else
				{
					// Reset the "own" field:
					this.isFilePath = false;
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
		public virtual bool IsDefinedOrHasDescription
		{
			get { return (IsDefined || HasDescription); }
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool HasDescription
		{
			get { return (!string.IsNullOrWhiteSpace(this.description)); }
		}

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
					(this.textLines != null) &&
					(this.textLines.Length >= 1) &&
					(this.textLines[0] != null)
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
					this.textLines = UndefinedTextLines;
					this.isPartialText = false;
					this.isFilePath = true;
					this.filePath = "";

					// Attention, don't modify the description as that can be defined separately!
					// An empty explicit description will result in the default description.
				}
				else
				{
					// Reset the "own" field:
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
					return ((this.textLines.Length == 1));
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
					return ((this.textLines.Length > 1));
				else
					return (false);
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
					return (this.textLines[0]);
				}
				else if (IsPartialText)
				{
					return (this.textLines[0]);
				}
				else if (IsMultiLineText)
				{
					var sb = new StringBuilder();
					sb.Append('<');
					sb.Append(MultiLineText.Length.ToString(CultureInfo.CurrentCulture));
					sb.Append(" " + LinesText + ">");
					for (int i = 0; i < MultiLineText.Length; i++)
					{
						sb.Append(" [");
						sb.Append(MultiLineText[i]);
						sb.Append(']');
					}
					return (sb.ToString());
				}
				else // incl. IsPartialTextEol
				{
					return ("");
				}
			}
			set
			{
				if (value != null)
				{
					this.isDefined = true;
					this.textLines = new string[] { value };

					// Reset the other fields:
					this.isPartialText = false;
					this.isPartialTextEol = false;
					this.isFilePath = true;
					this.filePath = "";

					// Attention, don't modify the description as that can be defined separately!
					// An empty explicit description will result in the default description.
				}
				else
				{
					// Reset the "own" field:
					this.textLines = UndefinedTextLines;
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
					this.textLines = new string[] { value };
					this.isPartialText = true;

					// Reset the other fields:
					this.isPartialTextEol = false;
					this.isFilePath = true;
					this.filePath = "";

					// Attention, don't modify the description as that can be defined separately!
					// An empty explicit description will result in the default description.
				}
				else
				{
					// Reset the "own" fields:
					this.textLines = UndefinedTextLines;
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
					return (TextLines);
				else // incl. IsPartialTextEol
					return (UndefinedTextLines);
			}
			set
			{
				if ((value != null) && (value.Length >= 1) && (value[0] != null))
				{
					this.isDefined = true;
					this.textLines = value;

					// Reset the other fields:
					this.isPartialText = false;
					this.isPartialTextEol = false;
					this.isFilePath = true;
					this.filePath = "";

					// Attention, don't modify the description as that can be defined separately!
					// An empty explicit description will result in the default description.
				}
				else
				{
					// Reset the "own" field:
					this.textLines = UndefinedTextLines;
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool TextLinesAreNullOrEmpty
		{
			get { return ((TextLines == null) || (TextLines.Length < 1) || (string.IsNullOrEmpty(TextLines[0]))); }
		}

		/// <remarks>
		/// Validation is dependent on <see cref="Domain.Parser.Mode"/>! Thus, validation must be
		/// done during runtime when the mode is given; i.e. it cannot be done once and then kept.
		/// </remarks>
		public virtual bool IsValidText(Domain.Parser.Mode parseModesForText)
		{
			if (!IsText)
				return (false);

			foreach (string commandLine in this.textLines)
			{
				if (!Domain.Utilities.ValidationHelper.ValidateText("text to send", commandLine, parseModesForText, this.defaultRadix))
					return (false);
			}

			return (true);
		}

		/// <remarks>
		/// Method instead of property for consistency with the <see cref="IsValidFilePath(string)"/> method below.
		/// </remarks>
		public virtual bool IsValidFilePath()
		{
			return (IsValidFilePath(Environment.CurrentDirectory));
		}

		/// <summary></summary>
		public virtual bool IsValidFilePath(string rootDirectory)
		{
			if (!IsFilePath)
				return (false);

			return (File.Exists(PathEx.GetNormalizedRootedExpandingEnvironmentVariables(rootDirectory, this.filePath))); // May be absolute or relative to given root path.
		}

		/// <remarks>
		/// Method instead of property for consistency with the <see cref="IsValid(Domain.Parser.Mode, string)"/> method below.
		/// </remarks>
		public virtual bool IsValid(Domain.Parser.Mode parseModesForText)
		{
			if (IsText)
				return (IsValidText(parseModesForText));
			else
				return (IsValidFilePath(Environment.CurrentDirectory));
		}

		/// <summary></summary>
		public virtual bool IsValid(Domain.Parser.Mode parseModesForText, string rootDirectoryForFile)
		{
			if (IsText)
				return (IsValidText(parseModesForText));
			else
				return (IsValidFilePath(rootDirectoryForFile));
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
		public virtual Command ToCommandWithoutDefaultRadix()
		{
			var c = new Command(this);
			c.DefaultRadix = DefaultRadixDefault;
			return (c);
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
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		/// <remarks>
		/// Limited to a single line to keep debug output compact, same as <see cref="ToString()"/>.
		/// </remarks>
		public virtual string ToDiagnosticsString()
		{
			return (ToDiagnosticsString(""));
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		/// <remarks>
		/// Limited to a single line to keep debug output compact, same as <see cref="ToString()"/>.
		/// </remarks>
		public virtual string ToDiagnosticsString(string indent)
		{
			var sb = new StringBuilder(indent);

			sb.Append(Caption);
			sb.Append(" | IsDefined = ");
			sb.Append(IsDefined.ToString(CultureInfo.CurrentCulture));
			sb.Append(" | Description = ");
			sb.Append((Description != null) ? Description : "(none)");
			sb.Append(" | DefaultRadix = ");
			sb.Append(DefaultRadix.ToString());
			sb.Append(" | ");
			sb.Append((TextLines != null) ? TextLines.Length.ToString(CultureInfo.CurrentCulture) : "0");
			sb.Append(" TextLines | IsFilePath = ");
			sb.Append(IsFilePath.ToString(CultureInfo.CurrentCulture));
			sb.Append(" | FilePath = ");
			sb.Append((FilePath != null) ? FilePath : "(none)");

			return (sb.ToString());
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

				hashCode =                     IsDefined                        .GetHashCode();
				hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  DefaultRadix                     .GetHashCode();
				hashCode = (hashCode * 397) ^ (TextLines   != null ?   TextLines.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  IsFilePath                       .GetHashCode();
				hashCode = (hashCode * 397) ^ (FilePath    != null ?    FilePath.GetHashCode() : 0);

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
			////base.Equals(other) is not required when deriving from 'object'.

				IsDefined.Equals(                   other.IsDefined)    &&
				StringEx.EqualsOrdinal(Description, other.Description)  &&
				DefaultRadix.Equals(                other.DefaultRadix) &&
				ArrayEx.ValuesEqual(TextLines,      other.TextLines)    &&
				IsFilePath.Equals(                  other.IsFilePath)   &&
				PathEx.Equals(FilePath,             other.FilePath)
			);
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
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "'" + obj.ToString() + "' does not specify a 'Command'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, nameof(obj)));
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
