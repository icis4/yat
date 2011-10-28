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
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2011 Matthias Kläy.
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
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using MKY.Diagnostics;

#endregion

namespace MKY.CommandLine
{
	/// <summary>
	/// This command line infrastructure is based on the NUnit command line infrastructure:
	/// > NUnit version 2.5.10 (2011-03-14)
	/// > NUnit file "/src/ClientUtilities/util/CommandLineOptions.cs" (nunit.util.dll)
	/// 
	/// See below for original file header content:
	/// -------------------------------------------------------------------------------------------
	/// This is a re-usable component to be used when you need to parse command-line options and
	/// parameters. It separates command line parameters from command line options. It uses
	/// reflection to populate member variables the derived class with the values of the options.
	/// 
	/// An option can start with "-" or "--". On Windows systems, it can start with "/" as well.
	/// 
	/// I define 3 types of "options":
	///   1. Boolean options (yes/no values), e.g. /r to recurse
	///   2. Value options, e.g. /loglevel=3
	///   3. Parameters, i.e. standalone strings like file names
	/// 
	/// An example to explain:
	///   csc /nologo /t:exe myfile.cs
	///       |       |      |
	///       |       |      + parameter
	///       |       |
	///       |       + value option
	///       |
	///       + boolean option
	/// 
	/// Please see a short description of the CommandLineOptions class
	/// at http://codeblast.com/~gert/dotnet/sells.html
	/// 
	/// Gert Lombard (gert@codeblast.com)
	/// James Newkirk (jim@nunit.org)
	/// -------------------------------------------------------------------------------------------
	/// </summary>
	/// <remarks>
	/// The implementation has been copied and improved to be more .NET-ish and edited to comply
	/// with the YAT/MKY naming and coding conventions such as passing FxCop and StyleCop analysis.
	/// </remarks>
	public abstract class ArgsHandler
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const int MinorIndent =  2;
		/// <summary></summary>
		public const int MajorIndent = 10;

		/// <summary></summary>
		public readonly string MinorIndentSpace = new string(' ', MinorIndent);
		/// <summary></summary>
		public readonly string MajorIndentSpace = new string(' ', MajorIndent);

		/// <summary>
		/// Gets a value indicating whether forward slashes are allowed as option indicator.
		/// This applies to Windows systems.
		/// </summary>
		public readonly bool AllowForwardSlash = (Array.IndexOf(System.IO.Path.GetInvalidPathChars(), '/') < 0);

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private string[] args;
		private List<string> valueArgs = new List<string>();
		private List<string> optionArgs = new List<string>();
		private List<string> invalidArgs = new List<string>();

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="ArgsHandler"/> class.
		/// </summary>
		public ArgsHandler(string[] args)
		{
			Initialize(args);
			Validate();
		}

		/// <summary>
		/// Initializes the specified args.
		/// </summary>
		private void Initialize(string[] args)
		{
			this.args = args;

			for (int i = 0; i < args.Length; i++)
			{
				int pos;
				if (IsOption(args[i], out pos))
				{
					// Prepare next argument:
					string nextArg = null;
					if ((i + 1) < args.Length)
						nextArg = args[i + 1];

					// Store option argument:
					bool nextArgHasBeenConsumedToo = false;
					if (InitializeOption(args[i].Substring(pos), nextArg, ref nextArgHasBeenConsumedToo))
					{
						if (!nextArgHasBeenConsumedToo)
							this.optionArgs.Add(args[i]);
						else
							this.optionArgs.Add(args[i] + " " + nextArg);
					}
					else
					{
						this.invalidArgs.Add(args[i]);
					}

					if (nextArgHasBeenConsumedToo)
						i++; // Advance index.
				}
				else
				{
					if (InitializeValue(args[i], this.valueArgs.Count))
						this.valueArgs.Add(args[i]);
					else
						this.invalidArgs.Add(args[i]);
				}
			}
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Gets the total argument count.
		/// </summary>
		public int ArgsCount
		{
			get { return (ValueArgsCount + OptionArgsCount); }
		}

		/// <summary>
		/// Gets the value arguments.
		/// </summary>
		public string[] ValueArgs
		{
			get { return (this.valueArgs.ToArray()); }
		}

		/// <summary>
		/// Gets the value argument count.
		/// </summary>
		public int ValueArgsCount
		{
			get { return (this.valueArgs.Count); }
		}

		/// <summary>
		/// Gets the option arguments.
		/// </summary>
		public string[] OptionArgs
		{
			get { return (this.optionArgs.ToArray()); }
		}

		/// <summary>
		/// Gets the option argument count.
		/// </summary>
		public int OptionArgsCount
		{
			get { return (this.optionArgs.Count); }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is invalid.
		/// </summary>
		public bool IsInvalid
		{
			get { return (this.invalidArgs.Count > 0); }
		}

		/// <summary>
		/// Gets the invalid arguments.
		/// </summary>
		public string[] InvalidArgs
		{
			get { return (this.invalidArgs.ToArray()); }
		}

		/// <summary>
		/// Indicates that there are no args.
		/// </summary>
		public bool NoArgs
		{
			get { return (ArgsCount <= 0); }
		}

		/// <summary>
		/// Gets the <see cref="System.String"/> at the specified index.
		/// </summary>
		public string this[int index]
		{
			get
			{
				if (this.args != null)
					return (this.args[index]);

				return (null);

			}
		}

		#endregion

		#region Protected Methods
		//==========================================================================================
		// Protected Methods
		//==========================================================================================

		/// <summary>
		/// Gets the member field.
		/// </summary>
		protected virtual FieldInfo GetMemberField(string name)
		{
			Type t = this.GetType();
			FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo field in fields)
			{
				if (MatchesName(field, name))
					return (field);

				if (MatchesShortName(field, name))
					return (field);

				if (StringEx.EqualsOrdinalIgnoreCase(field.Name, name))
					return (field);

			}
			return (null);
		}

		/// <summary>
		/// Matches the name.
		/// </summary>
		protected virtual bool MatchesName(FieldInfo field, string name)
		{
			object[] atts = field.GetCustomAttributes(typeof(OptionArgAttribute), true);
			foreach (OptionArgAttribute att in atts)
			{
				if (StringEx.EqualsOrdinalIgnoreCase(att.Name, name))
					return (true);
			}
			return (false);
		}

		/// <summary>
		/// Matches the short name.
		/// </summary>
		protected virtual bool MatchesShortName(FieldInfo field, string name)
		{
			object[] atts = field.GetCustomAttributes(typeof(OptionArgAttribute), true);
			foreach (OptionArgAttribute att in atts)
			{
				foreach (string shortName in att.ShortNames)
				{
					if (StringEx.EqualsOrdinalIgnoreCase(shortName, name))
						return (true);
				}
			}
			return (false);
		}

		/// <summary>
		/// Determines whether the specified character is a valid option name character.
		/// </summary>
		protected virtual bool IsOptionNameChar(char c)
		{
			return (char.IsLetterOrDigit(c) || c == '?');
		}

		/// <summary>
		/// Determines whether the specified argument is an option and returns the position of
		/// the first character of the option name. An option starts with "/", "-" or "--".
		/// </summary>
		protected virtual bool IsOption(string arg, out int pos)
		{
			if (arg.Length < 2)
			{
				pos = 0;
				return (false);
			}
			else if (arg.Length == 2)
			{
				char[] c = arg.ToCharArray();
				if (((c[0] == '-') || ((c[0] == '/') && AllowForwardSlash)) && IsOptionNameChar(c[1]))
				{
					pos = 1;
					return (true);
				}
				else
				{
					pos = 0;
					return (false);
				}
			}
			else // arg.Length > 2
			{
				char[] c = arg.ToCharArray(0, 3);
				if ((c[0] == '-') && (c[1] == '-') && IsOptionNameChar(c[2]))
				{
					pos = 2;
					return (true);
				}
				else
				{
					pos = 0;
					return (false);
				}
			}
		}

		/// <summary>
		/// Initializes an option argument.
		/// </summary>
		protected virtual bool InitializeOption(string optionArgWithoutIndicator, string nextArg, ref bool nextArgHasBeenConsumedToo)
		{
			try
			{
				// The option can be written as "-o=value" or "-o:value" or "-o value".
				// If given arg is written "-o value", the first argument only contains
				// the option name, and the next argument contains the value.

				string optionStr;
				string valueStr;
				bool isSingleArgOption = SplitOptionAndValue(optionArgWithoutIndicator, out optionStr, out valueStr);

				if (!isSingleArgOption)
					optionStr = optionArgWithoutIndicator;

				FieldInfo field = GetMemberField(optionStr);
				if (field != null)
				{
					if (!isSingleArgOption)
					{
						if (string.IsNullOrEmpty(nextArg))
							return (false);

						valueStr = nextArg;
						nextArgHasBeenConsumedToo = true;
					}

					object value;
					if (field.FieldType == typeof(bool))
					{
						// Boolean options are set to true when given as argument.
						value = true;
					}
					else
					{
						if (string.IsNullOrEmpty(valueStr))
							return (false);

						if (field.FieldType.IsEnum)
							value = Enum.Parse(field.FieldType, valueStr, true);
						else // string, integral types, floating point types or decimal.
							value = valueStr;
					}

					field.SetValue(this, Convert.ChangeType(value, field.FieldType));
					return (true);
				}
				else
				{
					return (false);
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(this.GetType(), ex);
				return (false);
			}
		}

		/// <summary>
		/// Splits the option and value.
		/// </summary>
		protected virtual bool SplitOptionAndValue(string optionArgWithoutIndicator, out string option, out string value)
		{
			// Look for ":" or "=" separator in the option:
			int pos = optionArgWithoutIndicator.IndexOfAny(new char[] { ':', '=' });
			if (pos >= 1)
			{
				option = optionArgWithoutIndicator.Substring(0, pos);
				option.Trim();

				value = optionArgWithoutIndicator.Substring(pos + 1);
				value.Trim();
				value.Trim('"');

				return (true);
			}
			else
			{
				option = null;
				value = null;
				return (false);
			}
		}

		/// <summary>
		/// Initializes a value argument.
		/// </summary>
		protected virtual bool InitializeValue(string value, int index)
		{
			try
			{
				// Trim value argument:
				value.Trim();
				value.Trim('"');

				// Optionally validate the value argument:
				if (IsValidValueArg(value))
				{
					int valueArgIndex = 0;
					Type t = this.GetType();
					FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public);
					for (int fieldIndex = 0; fieldIndex < fields.Length; fieldIndex++)
					{
						FieldInfo field = fields[fieldIndex];
						if (field.GetCustomAttributes(typeof(ValueArgAttribute), true).Length > 0)
						{
							if (index == valueArgIndex)
							{
								field.SetValue(this, Convert.ChangeType(value, field.FieldType));
								return (true);
							}
							else
							{
								valueArgIndex++;
							}
						}
					}
					return (false);
				}
				else
				{
					return (false);
				}
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(this.GetType(), ex);
				return (false);
			}
		}

		/// <summary>
		/// Determines whether the specified argument is a valid value argument.
		/// Override this method to implement application specific validation logic.
		/// </summary>
		protected virtual bool IsValidValueArg(string arg)
		{
			return (true);
		}

		/// <summary>
		/// Processes the command line options and validates them.
		/// Override this method to implement application specific validation logic.
		/// </summary>
		protected virtual bool Validate()
		{
			return (true);
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary>
		/// Gets the help text.
		/// </summary>
		public virtual string GetHelpText()
		{
			// 80 is the default console width. Cannot retrieve the effective console width here
			// because Console.WindowWidth throws an exception.
			return (GetHelpText(80));
		}

		/// <summary>
		/// Gets the help text.
		/// </summary>
		public virtual string GetHelpText(int maxWidth)
		{
			StringBuilder helpText = new StringBuilder();

			Type t = this.GetType();
			FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public);

			helpText.AppendLine("Arguments:");
			helpText.AppendLine();
			foreach (FieldInfo field in fields)
			{
				foreach (ValueArgAttribute att in field.GetCustomAttributes(typeof(ValueArgAttribute), true))
				{
					if (!string.IsNullOrEmpty(att.Description))
					{
						int max = (maxWidth - MajorIndent);
						foreach (string line in StringEx.SplitLexically(att.Description, max))
							helpText.AppendLine(MinorIndentSpace + line);

						helpText.AppendLine();
					}
				}
			}

			helpText.AppendLine("Options:");
			helpText.AppendLine();
			foreach (FieldInfo field in fields)
			{
				string valueTypeString;
				if      (field.FieldType == typeof(string)) valueTypeString = "=STR";
				else if (field.FieldType != typeof(bool))   valueTypeString = "=VAL";
				else                                        valueTypeString = "";

				foreach (OptionArgAttribute att in field.GetCustomAttributes(typeof(OptionArgAttribute), true))
				{
					//
					// Example:
					//
					//   /r, -r, --recursive, --processrecursive
					//           Processes the command recursive, blablablablablablablablablablablblabl
					//           Multiple lines if description is longer than 70 (80-10) characters
					//   /i=VAL, -i=VAL, --index=VAL, --requestedindex=VAL
					//           Processes the item with the given index, blablablablablablablablablabl
					//           Multiple lines if description is longer than 70 (80-10) characters
					//
					// 0 3       10                                                                    80
					//

					StringBuilder names = new StringBuilder();

					// Short name(s):
					if (att.ShortNames != null)
					{
						foreach (string shortName in att.ShortNames)
						{
							string option = shortName.ToLowerInvariant();

							if (names.Length > 0)
								names.Append(", ");

							if (AllowForwardSlash)
							{
								names.Append("/" + option);
								names.Append(valueTypeString);
								names.Append(", ");
							}
							names.Append("-" + option);
							names.Append(valueTypeString);
						}
					}

					// Name:
					if (!string.IsNullOrEmpty(att.Name))
					{
						if (names.Length > 0)
							names.Append(", ");

						names.Append("--" + att.Name.ToLowerInvariant());
						names.Append(valueTypeString);
					}

					// Long name, only if there is no short nor standard name:
					if (names.Length <= 0)
					{
						names.Append("--" + field.Name.ToLowerInvariant());
						names.Append(valueTypeString);
					}

					// 1st line = Names:
					helpText.AppendLine(MinorIndentSpace + names.ToString());

					// Next line(s) = Description:
					if (!string.IsNullOrEmpty(att.Description))
					{
						int max = (maxWidth - MajorIndent);
						foreach (string line in StringEx.SplitLexically(att.Description, max))
							helpText.AppendLine(MajorIndentSpace + line);
					}

					helpText.AppendLine();
				}
			}

			string note = "Options that take values may use an equal sign, a colon or a space to separate the option from its value.";
			foreach (string line in StringEx.SplitLexically(note, maxWidth))
				helpText.AppendLine(MinorIndentSpace + line);
			
			return (helpText.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
