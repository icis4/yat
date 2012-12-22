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
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2012 Matthias Kläy.
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
	/// 
	/// The implementation has been copied and improved to be more .NET-ish and edited to comply
	/// with the YAT/MKY naming and coding conventions such as passing FxCop and StyleCop analysis.
	/// 
	/// In addition, this implementation has added so-called multi-options. Multi-options can be
	/// used to deal with a variable number of additional argument, i.e. ellipsis. This feature is
	/// optional.
	/// </summary>
	public abstract class ArgsHandler
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

	#if (DEBUG)
		/// <summary></summary>
		[Serializable]
		public class RuntimeValidationException : Exception
		{
			/// <summary></summary>
			public RuntimeValidationException(string message)
				: base(message)
			{
			}
		}
	#endif

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const int MinorIndent =  2;

		/// <summary></summary>
		public const int MajorIndent = 10;

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

		private bool supportValueArgs;
		private bool supportOptionArgs;
		private bool supportArrayOptionArgs;

		private string[] args;

		private List<string> valueArgs = new List<string>();
		private List<string> optionArgs = new List<string>();
		private List<string[]> arrayOptionArgs = new List<string[]>();
		private List<string> invalidArgs = new List<string>();

		private List<FieldInfo> optionFields = new List<FieldInfo>();

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="ArgsHandler"/> class.
		/// </summary>
		protected ArgsHandler(string[] args, bool supportValueArgs, bool supportOptionArgs, bool supportArrayOptionArgs)
		{
			this.supportValueArgs = supportValueArgs;
			this.supportOptionArgs = supportOptionArgs;
			this.supportArrayOptionArgs = supportArrayOptionArgs;

			Initialize(args);
			Validate();
		}

		/// <summary>
		/// Initializes the specified args.
		/// </summary>
		private void Initialize(string[] args)
		{
			this.args = args;
			if (this.args != null)
			{
				for (int i = 0; i < this.args.Length; i++)
				{
					string thisArg = this.args[i];

					int pos;
					if (string.IsNullOrEmpty(thisArg))
					{
						// Ignore empty strings within the array.
					}
					else if (IsOption(thisArg, out pos))
					{
						EndArrayOption();

						if (SupportOptionArgs)
						{
							// Prepare next argument:
							string nextArg = null;
							if ((i + 1) < this.args.Length)
								nextArg = this.args[i + 1];

							// Store option argument:
							bool nextArgHasBeenConsumedToo;
							object value;
							FieldInfo field;
							if (ParseOption(thisArg.Substring(pos), nextArg, out nextArgHasBeenConsumedToo, out value, out field))
							{
								string arg;
								if (!nextArgHasBeenConsumedToo)
									arg = thisArg;
								else
									arg = thisArg + " " + nextArg;

								if (field.FieldType.IsArray)
								{
									BeginArrayOption(field, value, arg);
								}
								else
								{
									if (InitializeOption(field, value))
										this.optionArgs.Add(arg);
									else
										this.invalidArgs.Add(arg);
								}
							}
							else
							{
								this.invalidArgs.Add(thisArg);
							}

							if (nextArgHasBeenConsumedToo)
								i++; // Advance index.
						}
						else // !SupportOptionArgs
						{
							Debug.WriteLine(@"Option arg """ + thisArg + @""" detected but " + GetType() + " doesn't support option args");
							this.invalidArgs.Add(thisArg);
						}
					}
					else if (IsValue(thisArg))
					{
						if (ArrayOptionIsBeingComposed)   // No need to check 'SupportOptionArgs',
						{                                 // array option would never be began with
							ContinueArrayOption(thisArg); // if 'SupportOptionArgs' was false.
						}
						else
						{
							EndArrayOption();

							if (SupportValueArgs)
							{
								if (InitializeValue(thisArg, this.valueArgs.Count))
									this.valueArgs.Add(thisArg);
								else
									this.invalidArgs.Add(thisArg);
							}
							else
							{
								Debug.WriteLine(@"Value arg """ + thisArg + @""" detected but " + GetType() + " doesn't support value args");
								this.invalidArgs.Add(thisArg);
							}
						}
					}
					else
					{
						EndArrayOption();

						Debug.WriteLine(@"Invalid arg """ + thisArg + @""" detected in " + GetType());
						this.invalidArgs.Add(thisArg);
					}
				}

				EndArrayOption();
			}
		}

		private FieldInfo arrayOptionField;
		private List<object> arrayOptionValues;
		private List<string> arrayOptionStrings;

		private bool ArrayOptionIsBeingComposed
		{
			get { return (this.arrayOptionField != null); }
		}

		private void BeginArrayOption(FieldInfo field, object value, string arg)
		{
			this.arrayOptionField = field;

			this.arrayOptionValues = new List<object>();
			this.arrayOptionValues.Add(value);

			this.arrayOptionStrings = new List<string>();
			this.arrayOptionStrings.Add(arg);
		}

		private void ContinueArrayOption(string arg)
		{
			this.arrayOptionValues.Add(arg);
			this.arrayOptionStrings.Add(arg);
		}

		private void EndArrayOption()
		{
			if ((this.arrayOptionField != null) && (this.arrayOptionValues != null) &&
				(this.arrayOptionStrings != null) && (this.arrayOptionStrings.Count > 0))
			{
				if (this.arrayOptionValues.Count > 0)
				{
					InitializeArrayOption(this.arrayOptionField, this.arrayOptionValues.ToArray());
					this.arrayOptionArgs.Add(this.arrayOptionStrings.ToArray());
				}
				else
				{
					foreach (string arg in this.arrayOptionStrings)
						this.invalidArgs.Add(arg);
				}
			}

			this.arrayOptionField = null;
			this.arrayOptionValues = null;
			this.arrayOptionStrings = null;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Gets a value indicating whether this instance is invalid.
		/// </summary>
		public bool IsValid
		{
			get { return (this.invalidArgs.Count <= 0); }
		}

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
		/// Gets the array option arguments.
		/// </summary>
		public string[][] ArrayOptionArgs
		{
			get { return (this.arrayOptionArgs.ToArray()); }
		}

		/// <summary>
		/// Gets the option argument count.
		/// </summary>
		public int ArrayOptionArgsCount
		{
			get { return (this.arrayOptionArgs.Count); }
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

		/// <summary></summary>
		protected virtual FieldInfo[] GetMemberFields()
		{
			Type t = GetType();
			FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public);
			return (fields);
		}

		/// <summary></summary>
		protected virtual ValueArgAttribute[] GetValueArgAttributes(FieldInfo field)
		{
			ValueArgAttribute[] atts = (ValueArgAttribute[])field.GetCustomAttributes(typeof(ValueArgAttribute), true);
			return (atts);
		}

		/// <summary></summary>
		protected virtual OptionArgAttribute[] GetOptionArgAttributes(FieldInfo field)
		{
			OptionArgAttribute[] atts = (OptionArgAttribute[])field.GetCustomAttributes(typeof(OptionArgAttribute), true);
			return (atts);
		}

		/// <summary>
		/// Gets the member field.
		/// </summary>
		protected virtual FieldInfo GetMemberField(string name)
		{
			foreach (FieldInfo field in GetMemberFields())
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
			foreach (OptionArgAttribute att in GetOptionArgAttributes(field))
			{
				foreach (string s in att.Names)
				{
					if (StringEx.EqualsOrdinalIgnoreCase(s, name))
						return (true);
				}
			}
			return (false);
		}

		/// <summary>
		/// Matches the short name.
		/// </summary>
		protected virtual bool MatchesShortName(FieldInfo field, string name)
		{
			foreach (OptionArgAttribute att in GetOptionArgAttributes(field))
			{
				foreach (string s in att.ShortNames)
				{
					if (StringEx.EqualsOrdinalIgnoreCase(s, name))
						return (true);
				}
			}
			return (false);
		}

		/// <summary>
		/// Determines whether value args are supported.
		/// </summary>
		protected virtual bool SupportValueArgs
		{
			get { return (this.supportValueArgs); }
		}

		/// <summary>
		/// Determines whether option args are supported.
		/// </summary>
		protected virtual bool SupportOptionArgs
		{
			get { return (this.supportOptionArgs || this.supportArrayOptionArgs); }
		}

		/// <summary>
		/// Determines whether array option args are supported.
		/// </summary>
		protected virtual bool SupportArrayOptionArgs
		{
			get { return (this.supportArrayOptionArgs); }
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
			int totalLength = arg.Length;
			arg = arg.TrimStart();
			int offset = totalLength - arg.Length;

			int startPos;
			if (IsOptionStart(arg, out startPos))
			{
				int checkPos = offset + startPos;

				char[] chars = arg.ToCharArray();
				if (checkPos <= (chars.Length - 1))
				{
					if (IsOptionNameChar(chars[checkPos]))
					{
						pos = checkPos;
						return (true);
					}
				}
			}

			pos = 0;
			return (false);
		}

		/// <summary>
		/// Determines whether the specified argument string starts with an option start sequence.
		/// An option starts with "/", "-" or "--".
		/// </summary>
		protected virtual bool IsOptionStart(string arg)
		{
			int pos;
			return (IsOptionStart(arg, out pos));
		}

		/// <summary>
		/// Determines whether the specified argument starts with an option start sequence and
		/// returns the position of the first character of the option name. An option starts
		/// with "/", "-" or "--".
		/// </summary>
		protected virtual bool IsOptionStart(string arg, out int pos)
		{
			if      (arg.StartsWith("--", StringComparison.OrdinalIgnoreCase)) // Attention: "--" must be tested before "-" because
				pos = 2;                   //            "--" also is "-"!
			else if (arg.StartsWith("-",  StringComparison.OrdinalIgnoreCase))
				pos = 1;
			else if (arg.StartsWith("/",  StringComparison.OrdinalIgnoreCase) && AllowForwardSlash)
				pos = 1;
			else
				pos = 0;

			return (pos > 0);
		}

		/// <summary>
		/// Initializes an option argument.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "The args handler implementation intentionally doesn't use generics.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		protected virtual bool ParseOption(string optionArgWithoutIndicator, string nextArg, out bool nextArgHasBeenConsumedToo, out object value, out FieldInfo field)
		{
			try
			{
				nextArgHasBeenConsumedToo = false;
				value = null;
				field = null;

				// The option can be written as "-o=value" or "-o:value" or "-o value".
				// If given arg is written "-o value", the first argument only contains
				// the option name, and the next argument contains the value.

				string optionStr;
				string valueStr;
				bool hasSuccessfullyBeenSplit = SplitOptionAndValue(optionArgWithoutIndicator, out optionStr, out valueStr);

				// Successful split means
				//   "-o=value" or
				//   "-o:value".
				// Not successful split means
				//   "-o value" or
				//   "-o" without value, i.e. boolean option.
				if (!hasSuccessfullyBeenSplit)
					optionStr = optionArgWithoutIndicator.Trim();

				field = GetMemberField(optionStr);
				if (field != null)
				{
					if (field.FieldType == typeof(bool))
					{
						if (hasSuccessfullyBeenSplit && (!string.IsNullOrEmpty(valueStr)))
							return (false);

						// Boolean options are set to true when given as argument:
						value = true;
					}
					else
					{
						// Get value from next argument if needed:
						if (!hasSuccessfullyBeenSplit)
						{
							if (string.IsNullOrEmpty(nextArg))
								return (false);

							valueStr = nextArg;
							nextArgHasBeenConsumedToo = true;
						}

						// Trim and process value:
						valueStr = valueStr.Trim();
						valueStr = StringEx.TrimSymmetrical(valueStr, '"');

						if (valueStr.Length <= 0)
							return (false);

						if (field.FieldType.IsEnum)
							value = Enum.Parse(field.FieldType, valueStr, true);
						else // string, integral types, floating point types or decimal.
							value = valueStr;
					}

					return (true);
				}
				else
				{
					return (false);
				}
			}
			catch (Exception ex)
			{
				nextArgHasBeenConsumedToo = false;
				value = null;
				field = null;

				DebugEx.WriteException(GetType(), ex);

				return (false);
			}
		}

		/// <summary>
		/// Initializes an option argument.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		protected virtual bool InitializeOption(FieldInfo field, object value)
		{
			try
			{
				object convertedValue = Convert.ChangeType(value, field.FieldType, CultureInfo.InvariantCulture);
				field.SetValue(this, convertedValue);
				this.optionFields.Add(field);
				return (true);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex);
				return (false);
			}
		}

		/// <summary>
		/// Initializes an option argument.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		protected virtual bool InitializeArrayOption(FieldInfo field, object[] values)
		{
			try
			{
				Type t = field.FieldType.GetElementType();
				int n = values.Length;
				Array convertedValues = Array.CreateInstance(t, n);
				Array.Copy(Array.ConvertAll(values, value => Convert.ChangeType(value, t, CultureInfo.InvariantCulture)), convertedValues, n);
				field.SetValue(this, convertedValues);
				this.optionFields.Add(field);
				return (true);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(GetType(), ex);
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
				option = option.Trim();

				value = optionArgWithoutIndicator.Substring(pos + 1);
				value = value.Trim();

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
		/// Determines whether the specified argument is a value argument. A value argument must
		/// not start with "/", "-" or "--". Additional rules can be defined by overriding this
		/// method.
		/// </summary>
		protected virtual bool IsValue(string arg)
		{
			return (!IsOptionStart(arg));
		}

		/// <summary>
		/// Initializes a value argument.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		protected virtual bool InitializeValue(string value, int index)
		{
			try
			{
				// Trim value argument:
				value = value.Trim();
				value = value.Trim('"');

				// Optionally validate the value argument:
				if (IsValidValueArg(value))
				{
					int valueArgIndex = 0;
					FieldInfo[] fields = GetMemberFields();
					for (int fieldIndex = 0; fieldIndex < fields.Length; fieldIndex++)
					{
						FieldInfo field = fields[fieldIndex];
						if (field.GetCustomAttributes(typeof(ValueArgAttribute), true).Length > 0)
						{
							if (index == valueArgIndex)
							{
								field.SetValue(this, Convert.ChangeType(value, field.FieldType, CultureInfo.InvariantCulture));
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
				DebugEx.WriteException(GetType(), ex);
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
		[SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "att", Justification = "Local variable 'att' is required for 'foreach'.")]
		protected virtual bool Validate()
		{
		#if (DEBUG)
			List<string> optionStrings = new List<string>();
			foreach (FieldInfo field in GetMemberFields())
			{
				foreach (OptionArgAttribute att in GetOptionArgAttributes(field))
				{
					if (!SupportOptionArgs)
					{
						string message = "Option argument defined, but support for option arguments in not enabled in constructor call";
						Debug.WriteLine("Runtime validation failed for " + GetType() + ": " + message);
						throw (new RuntimeValidationException(message));
					}

					if (field.FieldType.IsArray && !SupportArrayOptionArgs)
					{
						string message = "Array option argument defined, but support for array option arguments in not enabled in constructor call";
						Debug.WriteLine("Runtime validation failed for " + GetType() + ": " + message);
						throw (new RuntimeValidationException(message));
					}

					foreach (string s in att.ShortNames)
					{
						if (optionStrings.Contains(s))
						{
							string message = "Duplicate command line argument " + s;
							Debug.WriteLine("Runtime validation failed for " + GetType() + ": " + message);
							throw (new RuntimeValidationException(message));
						}
						else
						{
							optionStrings.Add(s);
						}
					}
					foreach (string s in att.Names)
					{
						if (optionStrings.Contains(s))
						{
							string message = "Duplicate command line argument " + s;
							Debug.WriteLine("Runtime validation failed for " + GetType() + ": " + message);
							throw (new RuntimeValidationException(message));
						}
						else
						{
							optionStrings.Add(s);
						}
					}
				}
				foreach (ValueArgAttribute att in GetValueArgAttributes(field))
				{
					if (!SupportValueArgs)
					{
						string message = "Value argument defined, but support for value arguments in not enabled in constructor call";
						Debug.WriteLine("Runtime validation failed for " + GetType() + ": " + message);
						throw (new RuntimeValidationException(message));
					}
				}
			}
		#endif
			return (IsValid);
		}

		/// <summary>
		/// Invalidates the command line argument object by adding a dedicated string to the invalid
		/// args list.
		/// </summary>
		protected virtual void Invalidate(string invalidationMessage)
		{
			this.invalidArgs.Add(@"!""" + invalidationMessage + @"""");
		}

		/// <summary>
		/// Determines whether the specified argument starts with "!".
		/// </summary>
		protected virtual bool IsInvalidationStart(string arg)
		{
			return (arg.StartsWith("!", StringComparison.OrdinalIgnoreCase));
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual bool OptionIsGiven(string name)
		{
			if (this.supportOptionArgs || this.supportArrayOptionArgs)
			{
				FieldInfo fi = GetMemberField(name);
				return (this.optionFields.Contains(fi));
			}
			else
			{
				return (false);
			}
		}

		/// <summary></summary>
		public virtual string IndentSpace(int indent)
		{
			return (new string(' ', indent));
		}

		/// <summary></summary>
		public virtual string SplitIntoLines(int maxWidth, int indent, string text)
		{
			StringBuilder lines = new StringBuilder();

			int width = (maxWidth - indent);
			foreach (string line in StringEx.SplitLexically(text, width))
				lines.AppendLine(IndentSpace(indent) + line);

			return (lines.ToString());
		}

		/// <summary>
		/// Gets the help text.
		/// </summary>
		public virtual string GetHelpText(int maxWidth)
		{
			// Must be reduced to ensure that lines that exactly match the number of characters
			// do not lead to an empty line (due to the NewLine which is added).
			maxWidth--;

			StringBuilder helpText = new StringBuilder();
			FieldInfo[] fields = GetMemberFields();

			helpText.AppendLine();
			helpText.AppendLine("Value arguments:");
			helpText.AppendLine();
			foreach (FieldInfo field in fields)
			{
				foreach (ValueArgAttribute att in GetValueArgAttributes(field))
				{
					if (!string.IsNullOrEmpty(att.Description))
					{
						helpText.Append(SplitIntoLines(maxWidth, MinorIndent, att.Description));
						helpText.AppendLine();
					}
				}
			}

			helpText.AppendLine();
			helpText.AppendLine("Option arguments:");
			helpText.AppendLine();
			foreach (FieldInfo field in fields)
			{
				string valueTypeString;
				if (field.FieldType == typeof(bool))
				{
					valueTypeString = "";
				}
				else if (field.FieldType.IsArray)
				{
					if (field.FieldType.GetElementType().IsPrimitive)
						valueTypeString = "=VAL[]";
					else
						valueTypeString = "=STR[]";
				}
				else
				{
					if (field.FieldType.IsPrimitive)
						valueTypeString = "=VAL";
					else
						valueTypeString = "=STR";
				}

				foreach (OptionArgAttribute att in GetOptionArgAttributes(field))
				{
					//-----------------------------------------------------------------------------------
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
					//-----------------------------------------------------------------------------------

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

					// Name(s):
					if (att.Names != null)
					{
						foreach (string name in att.Names)
						{
							string option = name;

							if (names.Length > 0)
								names.Append(", ");

							names.Append("--" + option);
							names.Append(valueTypeString);
						}
					}

					// Long name, only if there is no short nor standard name:
					if (names.Length <= 0)
					{
						names.Append("--" + field.Name.ToLowerInvariant());
						names.Append(valueTypeString);
					}

					// 1st line = Names:
					helpText.Append(SplitIntoLines(maxWidth, MinorIndent, names.ToString()));

					// Next line(s) = Description:
					if (!string.IsNullOrEmpty(att.Description))
						helpText.Append(SplitIntoLines(maxWidth, MajorIndent, att.Description));

					helpText.AppendLine();
				}
			}

			helpText.AppendLine();
			helpText.AppendLine("Notes:");

			if (this.supportOptionArgs || this.supportArrayOptionArgs)
			{
				helpText.AppendLine();
				helpText.Append(SplitIntoLines(maxWidth, MinorIndent,
					"Options that take values may use an equal sign '=', a colon ':' or a space" +
					" to separate the name from its value. Option names are case-insensitive. " +
					"Option values are also case-insensitive unless stated otherwise."));
			}

			// Applies to any kind of args.
			{
				helpText.AppendLine();
				helpText.Append(SplitIntoLines(maxWidth, MinorIndent,
					@"Use quotes to pass string values ""including spaces"". " +
					@"Use \"" to pass a quote."));
			}

			if (this.supportArrayOptionArgs)
			{
				helpText.AppendLine();
				helpText.Append(SplitIntoLines(maxWidth, MinorIndent,
					"Array options start with the option name and continue until the next option. " +
					"Typically an array option is used at the end of the command line to take" +
					" a variable number of additional arguments, i.e. ellipsis."));
			}

			return (helpText.ToString());
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
