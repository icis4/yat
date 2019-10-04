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
// MKY Version 1.0.27
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2019 Matthias Kläy.
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
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
	/// In addition, this implementation has added so-called array-options. Array-options can be
	/// used to deal with a variable number of additional argument, i.e. ellipsis. This feature is
	/// optional.
	/// 
	/// An alternative command line infrastructure can be found at http://commandline.codeplex.com/.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
	public abstract class ArgsHandler
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

	#if (DEBUG)
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Emphasize that this exception type shall only be used by the ArgsHandler class. Anyway, it is only used for DEBUG.")]
		[Serializable]
		public class DevelopmentValidationException : Exception
		{
			/// <summary></summary>
			public DevelopmentValidationException()
			{
			}

			/// <summary></summary>
			public DevelopmentValidationException(string message)
				: base(message)
			{
			}

			/// <summary></summary>
			public DevelopmentValidationException(string message, Exception innerException)
				: base(message, innerException)
			{
			}

			/// <summary></summary>
			protected DevelopmentValidationException(SerializationInfo info, StreamingContext context)
				: base(info, context)
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
		public static readonly bool AllowForwardSlash = (Array.IndexOf(System.IO.Path.GetInvalidPathChars(), '/') < 0);

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool supportValueArgs;       // = false;
		private bool supportOptionArgs;      // = false;
		private bool supportArrayOptionArgs; // = false;

		private string[] args; // = null;

		private Dictionary<string, object> argsOverride; // = null;

		private List<string>    valueArgs;       // = null;
		private List<string>    optionArgs;      // = null;
		private List<string[]>  arrayOptionArgs; // = null;
		private List<string>    invalidArgs;     // = null;

		private List<FieldInfo> optionFields; // = null;

		private bool hasBeenProcessed; // = false;
		private bool hasBeenValidated; // = false;

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
			this.args = args;

			this.supportValueArgs = supportValueArgs;
			this.supportOptionArgs = supportOptionArgs;
			this.supportArrayOptionArgs = supportArrayOptionArgs;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Gets a value indicating whether this instance has been processed.
		/// </summary>
		public bool HasBeenProcessed
		{
			get { return (this.hasBeenProcessed); }
		}

		/// <summary>
		/// Gets a value indicating whether this instance has been validated.
		/// </summary>
		public bool HasBeenValidated
		{
			get { return (this.hasBeenValidated); }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is valid.
		/// </summary>
		/// <remarks>
		/// Until the args have been validated using <see cref="ProcessAndValidate"/>,
		/// this property returns <c>false</c>.
		/// </remarks>
		public bool IsValid
		{
			get { return (HasBeenValidated && InvalidArgsCount <= 0); }
		}

		/// <summary>
		/// Gets the value arguments.
		/// </summary>
		public ReadOnlyCollection<string> ValueArgs
		{
			get
			{
				if (this.valueArgs != null)
					return (this.valueArgs.AsReadOnly());

				return (null);
			}
		}

		/// <summary>
		/// Gets the value argument count.
		/// </summary>
		public int ValueArgsCount
		{
			get
			{
				if (this.valueArgs != null)
					return (this.valueArgs.Count);

				return (0);
			}
		}

		/// <summary>
		/// Gets the option arguments.
		/// </summary>
		public ReadOnlyCollection<string> OptionArgs
		{
			get
			{
				if (this.optionArgs != null)
					return (this.optionArgs.AsReadOnly());

				return (null);
			}
		}

		/// <summary>
		/// Gets the option argument count.
		/// </summary>
		public int OptionArgsCount
		{
			get
			{
				if (this.optionArgs != null)
					return (this.optionArgs.Count);

				return (0);
			}
		}

		/// <summary>
		/// Gets the array option arguments.
		/// </summary>
		public ReadOnlyCollection<string[]> ArrayOptionArgs
		{
			get
			{
				if (this.arrayOptionArgs != null)
					return (this.arrayOptionArgs.AsReadOnly());

				return (null);
			}
		}

		/// <summary>
		/// Gets the option argument count.
		/// </summary>
		public int ArrayOptionArgsCount
		{
			get
			{
				if (this.arrayOptionArgs != null)
					return (this.arrayOptionArgs.Count);

				return (0);
			}
		}

		/// <summary>
		/// Gets the total valid argument count.
		/// </summary>
		public int ValidArgsCount
		{
			get { return (ValueArgsCount + OptionArgsCount + ArrayOptionArgsCount); }
		}

		/// <summary>
		/// Gets the invalid arguments.
		/// </summary>
		public ReadOnlyCollection<string> InvalidArgs
		{
			get
			{
				if (this.invalidArgs != null)
					return (this.invalidArgs.AsReadOnly());

				return (null);
			}
		}

		/// <summary>
		/// Gets the total invalid argument count.
		/// </summary>
		public int InvalidArgsCount
		{
			get
			{
				if (this.invalidArgs != null)
					return (this.invalidArgs.Count);

				return (0);
			}
		}

		/// <summary>
		/// Gets the total argument count.
		/// </summary>
		public int ArgsCount
		{
			get { return (ValidArgsCount + InvalidArgsCount); }
		}

		/// <summary>
		/// Indicates that there are no args at all.
		/// </summary>
		public bool HasNoArgs
		{
			get { return (ArgsCount <= 0); }
		}

		/// <summary>
		/// Gets the <see cref="string"/> at the specified index.
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
			foreach (var att in GetOptionArgAttributes(field))
			{
				foreach (var attName in att.Names)
				{
					if (StringEx.EqualsOrdinalIgnoreCase(attName, name))
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
			foreach (var att in GetOptionArgAttributes(field))
			{
				foreach (var attShortName in att.ShortNames)
				{
					if (StringEx.EqualsOrdinalIgnoreCase(attShortName, name))
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
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
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
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
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
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
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

						if (string.IsNullOrEmpty(valueStr))
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

				DebugEx.WriteException(GetType(), ex, "Exception while parsing option!");

				return (false);
			}
		}

		/// <summary>
		/// Initializes an option argument.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
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
				DebugEx.WriteException(GetType(), ex, "Exception while initializing option!");
				return (false);
			}
		}

		/// <summary>
		/// Initializes an option argument.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
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
				DebugEx.WriteException(GetType(), ex, "Exception while initializing array option!");
				return (false);
			}
		}

		/// <summary>
		/// Splits the option and value.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual bool SplitOptionAndValue(string optionArgWithoutIndicator, out string optionStr, out string valueStr)
		{
			// Look for ":" or "=" separator in the option:
			int pos = optionArgWithoutIndicator.IndexOfAny(new char[] { ':', '=' });
			if (pos >= 1)
			{
				optionStr = optionArgWithoutIndicator.Substring(0, pos);
				optionStr = optionStr.Trim();

				valueStr = optionArgWithoutIndicator.Substring(pos + 1);
				valueStr = valueStr.Trim();

				return (true);
			}
			else
			{
				optionStr = null;
				valueStr = null;

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
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
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
				DebugEx.WriteException(GetType(), ex, "Exception while initializing value!");
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
			var messageLead = "Validation failed for " + GetType() + "!";
			var fields = GetMemberFields();
			var optionStrings = new List<string>(fields.Length); // Preset the initial capacity to improve memory management.
			foreach (var field in fields)
			{
				foreach (var att in GetOptionArgAttributes(field))
				{
					if (!SupportOptionArgs)
					{
						string message = " Option argument defined, but support for option arguments has not been not enabled!";
						Debug.WriteLine(messageLead + message);
						throw (new DevelopmentValidationException(messageLead + message));

						// When executing this validation from the NUnit based test with debugger attached,
						// the debugger will stop below stating "...Exception was unhandled by user code"
						// since it apparently doesn't detect that NUnit expects it at Assert.Throws<>().
						// Simply continue execution [F5] to continue test execution. Optionally disable
						// the [Break when this exception type is user-unhandled] setting.
					}

					if (field.FieldType.IsArray && !SupportArrayOptionArgs)
					{
						string message = " Array option argument defined, but support for array option arguments has not been not enabled!";
						Debug.WriteLine(messageLead + message);
						throw (new DevelopmentValidationException(messageLead + message));

						// When executing this validation from the NUnit based test with debugger attached,
						// the debugger will stop below stating "...Exception was unhandled by user code"
						// since it apparently doesn't detect that NUnit expects it at Assert.Throws<>().
						// Simply continue execution [F5] to continue test execution. Optionally disable
						// the [Break when this exception type is user-unhandled] setting.
					}

					foreach (var attShortName in att.ShortNames)
					{
						if (optionStrings.Contains(attShortName))
						{
							string message = " Duplicate command line argument " + attShortName + "!";
							Debug.WriteLine(messageLead + message);
							throw (new DevelopmentValidationException(messageLead + message));

							// When executing this validation from the NUnit based test with debugger attached,
							// the debugger will stop below stating "...Exception was unhandled by user code"
							// since it apparently doesn't detect that NUnit expects it at Assert.Throws<>().
							// Simply continue execution [F5] to continue test execution. Optionally disable
							// the [Break when this exception type is user-unhandled] setting.
						}
						else
						{
							optionStrings.Add(attShortName);
						}
					}

					foreach (var attName in att.Names)
					{
						if (optionStrings.Contains(attName))
						{
							string message = " Duplicate command line argument " + attName + "!";
							Debug.WriteLine(messageLead + message);
							throw (new DevelopmentValidationException(messageLead + message));

							// When executing this validation from the NUnit based test with debugger attached,
							// the debugger will stop below stating "...Exception was unhandled by user code"
							// since it apparently doesn't detect that NUnit expects it at Assert.Throws<>().
							// Simply continue execution [F5] to continue test execution. Optionally disable
							// the [Break when this exception type is user-unhandled] setting.
						}
						else
						{
							optionStrings.Add(attName);
						}
					}
				}

				foreach (ValueArgAttribute att in GetValueArgAttributes(field))
				{
					if (!SupportValueArgs)
					{
						string message = ". Value argument defined, but support for value arguments has not been not enabled!";
						Debug.WriteLine(messageLead + message);
						throw (new DevelopmentValidationException(messageLead + message));

						// When executing this validation from the NUnit based test with debugger attached,
						// the debugger will stop below stating "...Exception was unhandled by user code"
						// since it apparently doesn't detect that NUnit expects it at Assert.Throws<>().
						// Simply continue execution [F5] to continue test execution. Optionally disable
						// the [Break when this exception type is user-unhandled] setting.
					}
				}
			}
		#endif

			this.hasBeenValidated = true;
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

		/// <summary>
		/// Overrides an argument. This can be useful to default arguments at runtime, e.g. after
		/// the command line has been received but before it is processed, no matter what the
		/// command line is set to.
		/// </summary>
		/// <remarks>
		/// This mechanism allows to override an argument WITHOUT changing the original command line.
		/// 
		/// Unfortunately, there is no practicable way to use a delegate to access a field, nor an
		/// easy way to get a <see cref="FieldInfo"/> of a given class' field. So, the field name
		/// is used to reflect the field.
		/// </remarks>
		/// <param name="fieldName">The name of the argument to.</param>
		/// <param name="value">The value of the argument.</param>
		public virtual void Override(string fieldName, object value)
		{
			// Only create override once needed:
			if (this.argsOverride == null)
				this.argsOverride = new Dictionary<string, object>(1); // Preset the required capacity to improve memory management.

			// Remove key if it already exists, i.e. replace the value:
			if (this.argsOverride.ContainsKey(fieldName))
				this.argsOverride.Remove(fieldName);

			this.argsOverride.Add(fieldName, value);
		}

		/// <summary>
		/// Processes and validates the specified args.
		/// </summary>
		/// <returns>
		/// Returns <c>true</c> if the specified args could successfully be processed and validated,
		/// <c>false</c> otherwise.
		/// </returns>
		/// <remarks>
		/// This method intentionally is public, and not called by the constructor, because it
		/// calls virtual methods. If a derived class has overridden such a method, the derived
		/// class version will be called before the derived class constructor is called. Finding
		/// of FxCop.
		/// This method may be called multiple times. Upon each call, the state of the object is
		/// reset and all arguments are processed and validated.
		/// </remarks>
		public virtual bool ProcessAndValidate()
		{
			// Prepare the argument containers:
			this.valueArgs       = new List<string>();
			this.optionArgs      = new List<string>();
			this.arrayOptionArgs = new List<string[]>();
			this.invalidArgs     = new List<string>();

			this.optionFields    = new List<FieldInfo>();

			// Process the arguments:
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

			ProcessOverride();
			this.hasBeenProcessed = true;
			return (Validate());
		}

		private FieldInfo    arrayOptionField;
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

		/// <summary></summary>
		public virtual bool OptionIsGiven(string name)
		{
			if (this.args == null)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'ProcessAndValidate()' must be called first!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

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
			var lines = new StringBuilder();

			int width = (maxWidth - indent);
			foreach (string line in StringEx.SplitLexically(text, width))
				lines.AppendLine(IndentSpace(indent) + line);

			return (lines.ToString());
		}

		private void ProcessOverride()
		{
			if (this.argsOverride != null)
			{
				foreach (KeyValuePair<string, object> item in this.argsOverride)
				{
					FieldInfo field = this.GetMemberField(item.Key);
					if (field != null)
						field.SetValue(this, item.Value);
				}
			}
		}

		/// <summary>
		/// Gets the help text.
		/// </summary>
		[SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Well, if FxCop knows better about how the help text should look like...")]
		public virtual string GetHelpText(int maxWidth)
		{
			// Must be reduced to ensure that lines that exactly match the number of characters
			// do not lead to an empty line (due to the NewLine which is added).
			maxWidth--;

			var helpText = new StringBuilder();
			var fields = GetMemberFields();

			if (this.supportValueArgs)
			{
				helpText.AppendLine();
				helpText.AppendLine("Value arguments:");
				helpText.AppendLine();
				foreach (var field in fields)
				{
					foreach (var att in GetValueArgAttributes(field))
					{
						if (!string.IsNullOrEmpty(att.Description))
						{
							helpText.Append(SplitIntoLines(maxWidth, MinorIndent, att.Description));
							helpText.AppendLine();
						}
					}
				}
			}

			if (this.supportOptionArgs || this.supportArrayOptionArgs)
			{
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

					foreach (var att in GetOptionArgAttributes(field))
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

						var names = new StringBuilder();

						// Short name(s):
						if (att.ShortNames != null)
						{
							foreach (var attShortName in att.ShortNames)
							{
								string option = attShortName.ToLowerInvariant(); // Short names shall be lower case.

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
								string option = name; // 'Normal' names shall be case-sensitive.

								if (names.Length > 0)
									names.Append(", ");

								names.Append("--" + option);
								names.Append(valueTypeString);
							}
						}

						// Long name, only if there is no short nor 'normal' name:
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
			}

			if (this.supportValueArgs || this.supportOptionArgs || this.supportArrayOptionArgs)
			{
				helpText.AppendLine();
				helpText.AppendLine("Notes:");
			}

			if (this.supportOptionArgs || this.supportArrayOptionArgs)
			{
				string continuousText =
					"Options that take values may use an equal sign '=', a colon ':' or a space" +
					" to separate the name from its value. Option names are case-insensitive. " +
					"Option values are also case-insensitive unless stated otherwise.";

				helpText.AppendLine();
				helpText.Append(SplitIntoLines(maxWidth, MinorIndent, continuousText));
			}

			if (this.supportValueArgs || this.supportOptionArgs || this.supportArrayOptionArgs)
			{
				string continuousText =
					@"Use quotes to pass string values ""including spaces"". " +
					@"Use \"" to pass a quote.";

				helpText.AppendLine();
				helpText.Append(SplitIntoLines(maxWidth, MinorIndent, continuousText));
			}

			if (this.supportArrayOptionArgs)
			{
				string continuousText =
					"Array options start with the option name and continue until the next option. " +
					"Typically an array option is used at the end of the command line to take" +
					" a variable number of additional arguments, i.e. ellipsis.";

				helpText.AppendLine();
				helpText.Append(SplitIntoLines(maxWidth, MinorIndent, continuousText));
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
