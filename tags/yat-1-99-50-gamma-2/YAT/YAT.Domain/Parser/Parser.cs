﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Version 1.99.50
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

using MKY;
using MKY.Diagnostics;

#endregion

namespace YAT.Domain.Parser
{
	/// <remarks>
	/// This parser provides all functionality to parse any parsable text to send (commands, files).
	/// The parser is implemented using the state pattern. The states are implemented in a separate
	/// file 'States.cs'.
	/// 
	/// The abstract base class <see cref="ParserState"/> defines the state's interface and provides
	/// some common methods. The concrete state classes implement the required states:
	///  - <see cref="DefaultState"/>       : Default parser, handles contiguous sequences.
	///  - <see cref="NestedState"/>        : Handles a nested context.
	///  - <see cref="EscapeState"/>        : Handles an escaping '\'.
	///  - <see cref="OpeningState"/>       : Handles an opening '('.
	///  - <see cref="AsciiMnemonicState"/> : Sequence of ASCII mnemonics.
	///  - <see cref="NumericValueState"/>  : Sequence of numeric values.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parsable' is a correct English term.")]
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parsable", Justification = "'Parsable' is a correct English term.")]
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public class Parser : IDisposable
	{
		#region Constant Help Text
		//==========================================================================================
		// Constant Help Text
		//==========================================================================================

		/// <summary></summary>
		public static readonly string FormatHelp =
			@"The following formats are supported (type without quotes):" + Environment.NewLine +
			Environment.NewLine +
			@"Default ""OK""" + Environment.NewLine +
			@"Binary ""\b(01001111 01001011)""" + Environment.NewLine +
			@"Octal ""\o(117 113)""" + Environment.NewLine +
			@"Decimal ""\d(79 75)""" + Environment.NewLine +
			@"Hexadecimal ""\h(4F 4B)""" + Environment.NewLine +
			@"Character ""\c(O K)""" + Environment.NewLine +
			@"String ""\s(OK)""" + Environment.NewLine +
			@"ASCII controls (0x00..0x1F and 0x7F) ""<CR><LF>""" + Environment.NewLine +
			Environment.NewLine +
			@"Format specifiers are case insensitive, e.g. ""\H"" = ""\h"", ""4f"" = ""4F"", ""<lf>"" = ""<LF>""" + Environment.NewLine +
			@"Formats can also be applied on each value, e.g. ""\d(79)\d(75)""" + Environment.NewLine +
			@"Formats can be nested, e.g. ""\d(79 \h(4B) 79)""" + Environment.NewLine +
			@"Three letter radix identifiers are also allowed, e.g. ""\hex"" alternative to ""\h""" + Environment.NewLine +
			@"+/- signs are not allowed, neither are decimal points nor separators such as the apostroph" + Environment.NewLine +
			@"Enable ""Explicit Default Radix"" to switch default radix to a different setting than string" + Environment.NewLine +
			Environment.NewLine +
			@"In addition, C-style escape sequences are supported:" + Environment.NewLine +
			@"""\r\n"" alternative to ""<CR><LF>""" + Environment.NewLine +
			@"""\0"" alternative to ""<NUL>"" or \d(0) or \h(0)" + Environment.NewLine +
			@"""\01"" alternative to \o(1)" + Environment.NewLine +
			@"""\12"" alternative to \d(12)" + Environment.NewLine +
			@"""\0x1A"" or ""\x1A"" alternative to \h(1A)" + Environment.NewLine +
			Environment.NewLine +
			@"Type \\ to send a backspace" + Environment.NewLine +
			@"Type \< to send an opening angle bracket" + Environment.NewLine +
			@"Type \) to send a closing parenthesis";

		/// <summary></summary>
		public static readonly string KeywordHelp =
			@"In addition, the following keywords are supported:" + Environment.NewLine +
			Environment.NewLine +
			@"Clear the monitors ""Send something\!(" + (KeywordEx)Keyword.Clear + @")""" + Environment.NewLine +
			Environment.NewLine +
			@"Delay ""Send something\!(" + (KeywordEx)Keyword.Delay + @")Send delayed"" according to advanced settings" + Environment.NewLine +
			@"Delay after ""Send something and then delay\!(" + (KeywordEx)Keyword.LineDelay + @")"" according to advanced settings" + Environment.NewLine +
			@"Interval ""Send something in interval\!(" + (KeywordEx)Keyword.LineInterval + @")"" according to advanced settings" + Environment.NewLine +
			@"Repeat ""Send something multiple times\!(" + (KeywordEx)Keyword.LineRepeat + @")"" according to advanced settings" + Environment.NewLine +
			Environment.NewLine +
			@"Output break state on ""\!(" + (KeywordEx)Keyword.OutputBreakOn + @")""" + Environment.NewLine +
			@"Output break state off ""\!(" + (KeywordEx)Keyword.OutputBreakOff + @")""" + Environment.NewLine +
			@"Output break state toggle ""\!(" + (KeywordEx)Keyword.OutputBreakToggle + @")""" + Environment.NewLine +
			@"Output break state only applies to serial COM ports";

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const Radix DefaultRadixDefault = Radix.String;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool isDisposed;

		private Parser parentParser;

		private Encoding encoding = Encoding.Default;
		private Endianness endianness = Endianness.BigEndian;
		private Radix defaultRadix = DefaultRadixDefault;
		private Modes modes = Modes.All;

		private StringReader charReader;
		private MemoryStream bytesWriter;
		private List<Result> result;
		private ParserState state;

		private bool isKeywordParser;
		private bool doProbe;

		private bool hasFinished;

		private Parser nestedParser;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		public Parser(Modes modes = Modes.All)
		{
			this.modes = modes;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public Parser(Endianness endianness, Modes modes = Modes.All)
		{
			this.endianness = endianness;
			this.modes      = modes;
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public Parser(Encoding encoding, Endianness endianness = Endianness.BigEndian, Modes modes = Modes.All)
		{
			this.encoding   = encoding;
			this.endianness = endianness;
			this.modes      = modes;
		}

		/// <summary></summary>
		internal Parser(Parser parent, ParserState parserState)
		{
			InitializeNestedLevelFromParent(parent, parserState);
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				DisposeAndReset();

				// Dispose of managed resources if requested:
				if (disposing)
				{
				}

				// Set state to disposed:
				this.isDisposed = true;
			}
		}

#if (DEBUG)

		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		/// 
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		/// 
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~Parser()
		{
			Dispose(false);

			DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}

#endif // DEBUG

		/// <summary></summary>
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		#endregion

		#region Factory
		//==========================================================================================
		// Factory
		//==========================================================================================

		/// <summary></summary>
		internal virtual Parser GetNestedParser(ParserState parserState)
		{
			AssertNotDisposed();

			return (new Parser(this, parserState));
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		internal virtual StringReader CharReader
		{
			get { return (this.charReader); }
		}

		internal virtual MemoryStream BytesWriter
		{
			get { return (this.bytesWriter); }
		}

		internal virtual ParserState State
		{
			get { return (this.state); }
			set
			{
				if (this.state != null)
					this.state.Dispose();

				this.state = value;
			}
		}

		/// <summary></summary>
		public virtual Encoding Encoding
		{
			get { return (this.encoding); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Endianness", Justification = "'Endianness' is a correct English term.")]
		public virtual Endianness Endianness
		{
			get { return (this.endianness); }
		}

		internal virtual Modes Modes
		{
			get { return (this.modes); }
		}

		/// <remarks>Radix: public get, private set.</remarks>
		public virtual Radix Radix
		{
			get { return (this.defaultRadix); }
		}

		/// <remarks>Radix: public get, private set.</remarks>
		internal virtual void SetDefaultRadix(Radix defaultRadix)
		{
			this.defaultRadix = defaultRadix;
		}

		/// <summary></summary>
		public virtual bool IsTopLevel
		{
			get { return (this.parentParser == null); }
		}

		internal virtual bool IsKeywordParser
		{
			get { return (this.isKeywordParser); }
			set { this.isKeywordParser = value; }
		}

		internal virtual bool DoProbe
		{
			get { return (this.doProbe); }
			set { this.doProbe = value;  }
		}

		internal virtual bool HasFinished
		{
			get { return (this.hasFinished); }
			set { this.hasFinished = value; }
		}

		internal virtual Parser NestedParser
		{
			get { return (this.nestedParser); }
			set
			{
				if (this.nestedParser != null)
					this.nestedParser.Dispose();

				this.nestedParser = value;
			}
		}

		#endregion

		#region Public Methods
		//==========================================================================================
		// Public Methods
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		public virtual byte[] Parse(string s, Radix defaultRadix = DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			byte[] result;
			FormatException formatException = new FormatException("");
			if (!TryParse(s, out result, ref formatException, defaultRadix))
				throw (formatException);

			return (result);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		public virtual bool TryParse(string s, out byte[] result, Radix defaultRadix = DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			FormatException formatException = new FormatException("");
			return (TryParse(s, out result, ref formatException, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		public virtual bool TryParse(string s, out byte[] result, out string successfullyParsed, Radix defaultRadix = DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			FormatException formatException = new FormatException("");
			return (TryParse(s, out result, out successfullyParsed, ref formatException, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Required for recursion.")]
		public virtual bool TryParse(string s, out byte[] result, ref FormatException formatException, Radix defaultRadix = DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			string successfullyParsed;
			return (TryParse(s, out result, out successfullyParsed, ref formatException, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for recursion.")]
		public virtual bool TryParse(string s, out byte[] result, out string successfullyParsed, ref FormatException formatException, Radix defaultRadix = DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			Result[] typedResult;
			if (TryParse(s, out typedResult, out successfullyParsed, ref formatException, defaultRadix))
			{
				using (MemoryStream bytes = new MemoryStream())
				{
					foreach (Result r in typedResult)
					{
						var bar = (r as ByteArrayResult);
						if (bar != null)
						{
							byte[] a = bar.ByteArray;
							bytes.Write(a, 0, a.Length);
						}
					}

					result = bytes.ToArray();
					return (true);
				}
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		public virtual bool TryParse(string s, Radix defaultRadix = DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			Result[] result;
			return (TryParse(s, out result, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		public virtual bool TryParse(string s, out string successfullyParsed, Radix defaultRadix = DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			Result[] result;
			return (TryParse(s, out result, out successfullyParsed, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Required for recursion.")]
		public virtual bool TryParse(string s, out string successfullyParsed, ref FormatException formatException, Radix defaultRadix = DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			Result[] result;
			return (TryParse(s, out result, out successfullyParsed, ref formatException, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		public virtual bool TryParse(string s, out Result[] result, Radix defaultRadix = DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			string successfullyParsed;
			return (TryParse(s, out result, out successfullyParsed, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		public virtual bool TryParse(string s, out Result[] result, out string successfullyParsed, Radix defaultRadix = DefaultRadixDefault)
		{
			// AssertNotDisposed() is called by 'TryParse()' below.

			FormatException formatException = new FormatException("");
			return (TryParse(s, out result, out successfullyParsed, ref formatException, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for recursion.")]
		public virtual bool TryParse(string s, out Result[] result, out string successfullyParsed, ref FormatException formatException, Radix defaultRadix = DefaultRadixDefault)
		{
			AssertNotDisposed();

			InitializeTopLevel(s, defaultRadix, (s.Length <= 1000)); // Inhibit probing in order to keep speed at a decent level...

			while (!HasFinished)
			{
				int c = CharEx.InvalidChar; // 'int' is given by Read() below.
				try
				{
					c = this.charReader.Read();
				}
				catch (ObjectDisposedException ex)
				{
					DebugEx.WriteException(GetType(), ex); // Debug use only.
				}

				if (!this.state.TryParse(this, c, ref formatException))
				{
					CommitPendingBytes();

					string remaining = null;
					try
					{
						remaining = this.charReader.ReadToEnd();
					}
					catch (ObjectDisposedException ex)
					{
						DebugEx.WriteException(GetType(), ex); // Debug use only.
					}

					if (remaining == null)
					{
						// Signal that parsing resulted in a severe stream error:
						successfullyParsed = null;
						result = null;
						return (false);
					}
					else
					{
						// Signal that parsing resulted in a parse error and
						//   return the part of the string that could be parsed:
						successfullyParsed = StringEx.Left(s, s.Length - remaining.Length - 1);
						result = this.result.ToArray();
						return (false);
					}
				}
			}

			CommitPendingBytes();

			successfullyParsed = s;
			result = this.result.ToArray();
			return (true);
		}

		#endregion

		#region Protected Methods
		//==========================================================================================
		// Protected Methods
		//==========================================================================================

		/// <summary></summary>
		internal virtual void CommitPendingBytes()
		{
			AssertNotDisposed();

			if (this.bytesWriter.Length > 0)
			{
				// Commit:
				this.result.Add(new ByteArrayResult(this.bytesWriter.ToArray()));

				// Restart:
				this.bytesWriter.Dispose();
				this.bytesWriter = new MemoryStream(); // Former stream has just been disposed of above.
			}
		}

		/// <summary></summary>
		internal virtual void CommitResult(IEnumerable<Result> result)
		{
			AssertNotDisposed();

			this.result.AddRange(result);
		}

		/// <summary>
		/// Parses <paramref name="s"/> for one or more items in the specified base <paramref name="radix"/>, separated with spaces.
		/// </summary>
		/// <param name="s">String to be parsed.</param>
		/// <param name="radix">Numeric radix.</param>
		/// <param name="result">Array containing the resulting bytes.</param>
		/// <param name="formatException">Returned if invalid string format.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		/// <exception cref="OverflowException">Thrown if a value cannot be converted into bytes.</exception>
		internal virtual bool TryParseContiguousRadix(string s, Radix radix, out byte[] result, ref FormatException formatException)
		{
			using (MemoryStream bytes = new MemoryStream())
			{
				if (radix == Radix.String)
				{
					byte[] b;
					if (TryParseContiguousRadixItem(s, radix, out b, ref formatException))
					{
						bytes.Write(b, 0, b.Length);
					}
					else
					{
						result = new byte[] { };
						return (false);
					}
				}
				else
				{
					string[] items = s.Split(' ');
					foreach (string item in items)
					{
						if (item.Length > 0)
						{
							byte[] b;
							if (TryParseContiguousRadixItem(item, radix, out b, ref formatException))
							{
								bytes.Write(b, 0, b.Length);
							}
							else
							{
								result = new byte[] { };
								return (false);
							}
						}
					}
				}

				result = bytes.ToArray();
				return (true);
			} // using (MemoryStream)
		}

		/// <summary>
		/// Parses <paramref name="s"/> for a single item in the specified <paramref name="radix"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Required for recursion.")]
		internal virtual bool TryParseContiguousRadixItem(string s, Radix radix, out byte[] result, ref FormatException formatException)
		{
			AssertNotDisposed();

			switch (radix)
			{
				case Radix.String:
					return (TryEncodeStringItem(s, out result, ref formatException));

				case Radix.Char:
					return (TryEncodeCharItem(s, out result, ref formatException));

				case Radix.Bin:
				case Radix.Oct:
				case Radix.Dec:
				case Radix.Hex:
					return (TryParseAndConvertContiguousNumericItem(s, radix, out result, ref formatException));

				default:
					throw (new ArgumentOutOfRangeException("radix", radix, @"Unknown radix """ + radix + @"""!"));
			}
		}

		/// <summary>
		/// Encodes <paramref name="s"/> containing a plain string into bytes.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for recursion.")]
		internal virtual bool TryEncodeStringItem(string s, out byte[] result, ref FormatException formatException)
		{
			result = this.encoding.GetBytes(s);
			return (true);
		}

		/// <summary>
		/// Encodes <paramref name="s"/> containing a single char item into bytes.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for recursion.")]
		internal virtual bool TryEncodeCharItem(string s, out byte[] result, ref FormatException formatException)
		{
			char c;
			if (char.TryParse(s, out c))
			{
				result = this.encoding.GetBytes(new char[] { c });
				return (true);
			}
			else
			{
				formatException = new FormatException(@"""" + s + @""" does not contain a valid single character.");
				result = new byte[] { };
				return (false);
			}
		}

		/// <summary>
		/// Parses <paramref name="s"/> for a sequence of contiguous numeric digits in the current
		/// radix and converts them into bytes. The digits will sequentially be parsed and converted
		/// byte-by-byte.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for recursion.")]
		internal virtual bool TryParseAndConvertContiguousNumericItem(string s, out byte[] result, ref FormatException formatException)
		{
			return (TryParseAndConvertContiguousNumericItem(s, this.Radix, out result, ref formatException));
		}

		/// <summary>
		/// Parses <paramref name="s"/> for a sequence of contiguous numeric digits in the specified
		/// <paramref name="radix"/> and converts them into bytes. The digits will sequentially be
		/// parsed and converted byte-by-byte.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Required for recursion.")]
		internal virtual bool TryParseAndConvertContiguousNumericItem(string s, Radix radix, out byte[] result, ref FormatException formatException)
		{
			using (MemoryStream bytes = new MemoryStream())
			{
				string remaining = s;
				bool success = true;

				switch (radix)
				{
					case Radix.Bin:
					{
						while (remaining.Length > 0)
						{
							bool found = false;

							int from = Math.Min(8, remaining.Length);
							for (int i = from; i >= 1; i--) // Probe the 8-7-...-2-1 left-most characters for a valid binary byte.
							{
								ulong tempResult;
								if (UInt64Ex.TryParseBinary(StringEx.Left(remaining, i), out tempResult))
								{
									if (tempResult <= 0xFF) // i left-most characters are a valid binary byte!
									{
										bytes.WriteByte((byte)tempResult);

										remaining = remaining.Remove(0, i);
										found = true;
										break; // Quit for-loop and continue within remaining string.
									}
								}
							}

							if (!found)
							{
								success = false;
								break; // Quit while-loop.
							}
						}

						break; // Break switch-case.
					}

					case Radix.Oct:
					{
						while (remaining.Length > 0)
						{
							bool found = false;

							int from = Math.Min(3, remaining.Length);
							for (int i = from; i >= 1; i--) // Probe the 3-2-1 left-most characters for a valid octal byte.
							{
								ulong tempResult;
								if (UInt64Ex.TryParseOctal(StringEx.Left(remaining, i), out tempResult))
								{
									if (tempResult <= 0xFF) // i left-most characters are a valid octal byte!
									{
										bytes.WriteByte((byte)tempResult);

										remaining = remaining.Remove(0, i);
										found = true;
										break; // Quit for-loop and continue within remaining string.
									}
								}
							}

							if (!found)
							{
								success = false;
								break; // Quit while-loop.
							}
						}

						break; // Break switch-case.
					}

					case Radix.Dec:
					{
						while (remaining.Length > 0)
						{
							bool found = false;

							int from = Math.Min(3, remaining.Length);
							for (int i = from; i >= 1; i--) // Probe the 3-2-1 left-most characters for a valid decimal byte.
							{
								byte tempResult;
								if (byte.TryParse(StringEx.Left(remaining, i), NumberStyles.Integer, CultureInfo.InvariantCulture, out tempResult))
								{
									if (tempResult <= 0xFF) // i left-most characters are a valid decimal byte!
									{
										bytes.WriteByte(tempResult);

										remaining = remaining.Remove(0, i);
										found = true;
										break; // Quit for-loop and continue within remaining string.
									}
								}
							}

							if (!found)
							{
								success = false;
								break; // Quit while-loop.
							}
						}

						break; // Break switch-case.
					}

					case Radix.Hex:
					{
						while (remaining.Length > 0)
						{
							bool found = false;

							int from = Math.Min(2, remaining.Length);
							for (int i = from; i >= 1; i--) // Probe the 2-1 left-most characters for a valid hexadecimal byte.
							{
								byte tempResult;
								if (byte.TryParse(StringEx.Left(remaining, i), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out tempResult))
								{
									if (tempResult <= 0xFF) // i left-most characters are a valid hexadecimal byte!
									{
										bytes.WriteByte(tempResult);

										remaining = remaining.Remove(0, i);
										found = true;
										break; // Quit for-loop and continue within remaining string.
									}
								}
							}

							if (!found)
							{
								success = false;
								break; // Quit while-loop.
							}
						}

						break; // Break switch-case.
					}

					default:
					{
						throw (new ArgumentOutOfRangeException("radix", radix, @"Unknown radix """ + radix + @"""!"));
					}
				}

				if (success)
				{
					result = bytes.ToArray();
					return (true);
				}
				else
				{
					StringBuilder sb = new StringBuilder();

					if (remaining.Length != s.Length)
					{
						sb.Append(@"""");
						sb.Append(remaining);
						sb.Append(@""" of ");
					}

					sb.Append(@"""");
					sb.Append(s);
					sb.Append(@""" is an invalid ");

					switch (radix)
					{
						case Radix.Bin: sb.Append("binary");      break;
						case Radix.Oct: sb.Append("octal");       break;
						case Radix.Dec: sb.Append("decimal");     break;
						case Radix.Hex: sb.Append("hexadecimal"); break;
						default: throw (new ArgumentOutOfRangeException("radix", radix, @"Unknown radix """ + radix + @"""!"));
					}

					sb.Append(" value.");

					formatException = new FormatException(sb.ToString());
					result = new byte[] { };
					return (false);
				}
			} // using (MemoryStream)
		}

		/// <summary>
		/// Parses <paramref name="s"/> for keywords.
		/// </summary>
		/// <param name="s">String to be parsed.</param>
		/// <param name="result">Array containing the results.</param>
		/// <param name="formatException">Returned if invalid string format.</param>
		/// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Non-static for orthogonality with other TryParse() methods.")]
		internal virtual bool TryParseContiguousKeywords(string s, out Result[] result, ref FormatException formatException)
		{
			List<Result> l = new List<Result>();
			string[] tokens = s.Split(' ');
			foreach (string token in tokens)
			{
				if (token.Length == 0)
					continue;

				Keyword keyword;
				if (KeywordEx.TryParse(token, out keyword))
				{
					l.Add(new KeywordResult(keyword));
				}
				else
				{
					result = new Result[] { };
					formatException = new FormatException(@"""" + token + @""" is an invalid keyword.");
					return (false);
				}
			}
			result = l.ToArray();
			return (true);
		}

		#endregion

		#region Private Methods
		//==========================================================================================
		// Private Methods
		//==========================================================================================

		/// <summary>
		/// Initialize or re-initialize the top-level of a parser.
		/// </summary>
		private void InitializeTopLevel(string s, Radix defaultRadix, bool doProbe)
		{
			DisposeAndReset();

		////this.parentParser has just been reset to 'null' by DisposeAndReset() above.

		////this.encoding   is set by the constructor.
		////this.endianness is set by the constructor.
		////this.modes      is set by the constructor.

			this.defaultRadix    = defaultRadix;

			this.charReader      = new StringReader(s); // Former stream has just been disposed of by DisposeAndReset() above.
			this.bytesWriter     = new MemoryStream();  // Former stream has just been disposed of by DisposeAndReset() above.
			this.result          = new List<Result>();
			this.state           = new DefaultState();

			this.parentParser    = null;
			this.isKeywordParser = false;
			this.doProbe         = doProbe;

			this.hasFinished     = false;

		////this.nestedParser has just been reset to 'null' by DisposeAndReset() above.
		}

		/// <summary>
		/// Initialize a nested-level of a parser.
		/// </summary>
		private void InitializeNestedLevelFromParent(Parser parent, ParserState parserState)
		{
			DisposeAndReset();

			this.parentParser    = parent;

			this.encoding        = parent.encoding;
			this.endianness      = parent.endianness;
			this.modes           = parent.modes;

			this.defaultRadix    = parent.defaultRadix;

			this.charReader      = parent.charReader;  // Former reader has just been reset to 'null' by DisposeAndReset() above.
			this.bytesWriter     = new MemoryStream(); // Former stream has just been disposed of by DisposeAndReset() above.
			this.result          = parent.result;
			this.state           = parserState;

			this.isKeywordParser = false; // Keywords cannot be nested (yet).
			this.doProbe         = parent.doProbe;

			this.hasFinished     = false;

		////this.nestedParser has just been reset to 'null' by DisposeAndReset() above.
		}

		/// <summary></summary>
		public void Close()
		{
			this.parentParser = null;
			this.charReader   = null; // The reader belongs to the parent!
		}

		private void DisposeAndReset()
		{
			// Do not dispose of the parent!

			// Only dispose of the reader if this is the top-level,
			// i.e. the reader does not below to a overlying parent!
			if (this.parentParser == null)
			{
				if (this.charReader != null)
					this.charReader.Dispose();
			}

			if (this.bytesWriter != null)
				this.bytesWriter.Dispose();

			if (this.state != null)
				this.state.Dispose();

			if (this.nestedParser != null)
				this.nestedParser.Dispose();

			this.parentParser = null;
			this.charReader   = null;
			this.bytesWriter  = null;
			this.state        = null;
			this.nestedParser = null;
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================