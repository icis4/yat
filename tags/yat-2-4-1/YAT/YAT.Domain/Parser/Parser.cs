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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

using MKY;
using MKY.Diagnostics;
using MKY.Text;

#endregion

namespace YAT.Domain.Parser
{
	/// <remarks>
	/// This parser provides all functionality to parse any parseable text to send (commands, files).
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
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Semantic of readonly fields is constant.")]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Parsable' is a correct English term.")]
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public class Parser : DisposableBase
	{
		#region Constant Help Text
		//==========================================================================================
		// Constant Help Text
		//==========================================================================================

		/// <summary></summary>
		public static readonly string FormatHelp =
			@"The following formats are supported (quoted content):" + Environment.NewLine +
			Environment.NewLine +
			@"Default ""OK""" + Environment.NewLine +
			@"Binary ""\b(01001111 01001011)""" + Environment.NewLine +
			@"Octal ""\o(117 113)""" + Environment.NewLine +
			@"Decimal ""\d(79 75)""" + Environment.NewLine +
			@"Hexadecimal ""\h(4F 4B)""" + Environment.NewLine +
			@"String ""\s(OK)""" + Environment.NewLine +
			@"Character ""\c(O K)""" + Environment.NewLine +
			@"Unicode notation ""\U+20AC"" or ""\U(20AC)""" + Environment.NewLine +
			@"ASCII controls (0x00..0x1F and 0x7F) ""<CR><LF>""" + Environment.NewLine +
			Environment.NewLine +
			@"Format specifiers are case insensitive, e.g. ""\H"" = ""\h"", ""4f"" = ""4F"", ""<lf>"" = ""<LF>""" + Environment.NewLine +
			@"Formats can also be applied on each value, e.g. ""\d(79)\d(75)""" + Environment.NewLine +
			@"Formats can be nested, e.g. ""\d(79 \h(4B) 79)""" + Environment.NewLine +
			@"Three letter radix identifiers are also allowed, e.g. ""\hex"" alternative to ""\h""" + Environment.NewLine +
			@"+/- signs are not allowed, neither are decimal points nor separators such as the apostroph." + Environment.NewLine +
			@"Enable [Explicit Default Radix] to switch default radix to a different setting than string." + Environment.NewLine +
			Environment.NewLine +
			@"In addition, C-style escape sequences are supported:" + Environment.NewLine +
			@"""\r\n"" alternatives to ""<CR><LF>""" + Environment.NewLine +
			@"""\a\b\e\t"" alternatives to ""<BEL><BS><ESC><TAB>""" + Environment.NewLine +
			@"""\0"" alternative to ""<NUL>"" or ""\d(0)"" or ""\h(0)""" + Environment.NewLine +
			@"""\01"" alternative to ""\o(1)""" + Environment.NewLine +
			@"""\12"" alternative to ""\d(12)""" + Environment.NewLine +
			@"""\0x1A"" or ""\x1A"" alternative to ""\h(1A)""" + Environment.NewLine +
			@"""\0b01001111"" alternative to ""\b(01001111)""" + Environment.NewLine +
			@"""\u20AC"" alternative to ""\U+20AC"" or ""\U(20AC)""" + Environment.NewLine +
			Environment.NewLine +
			@"Use \\ to send a backslash '\'." + Environment.NewLine +
			@"Use \< to send an opening angle bracket '<'." + Environment.NewLine +
			@"Use \) to send a closing parenthesis ')' within an escape.";

		/// <summary></summary>
		public static readonly string KeywordHelp =
			@"In addition, the following keywords are supported:" + Environment.NewLine +
			Environment.NewLine +
			@"Clear the monitors ""Send something\!(" + (KeywordEx)Keyword.Clear + @")""." + Environment.NewLine +
			Environment.NewLine +
			@"Delay ""Send something\!(" + (KeywordEx)Keyword.Delay + @"(1000))Send delayed"" or without argument according to the advanced settings." + Environment.NewLine +
			@"Delay after ""Send something and then delay\!(" + (KeywordEx)Keyword.LineDelay + @"(1000))"" or without argument according to the advanced settings." + Environment.NewLine +
			@"Interval ""Send something in interval\!(" + (KeywordEx)Keyword.LineInterval + @"(1000))"" or without argument according to the advanced settings." + Environment.NewLine +
		////@"Repeat ""Send something multiple times\!(" + (KeywordEx)Keyword.Repeat + @"(OK, 10))"" or ""\!(" + (KeywordEx)Keyword.Repeat + @"(OK, 10, 10))"" with additional delay inbetween repetitions." + Environment.NewLine +
			@"Repeat ""Send something multiple times\!(" + (KeywordEx)Keyword.LineRepeat + @"(10))"" or without argument according to the advanced settings." + Environment.NewLine +
			@"Repeat ""Send something infinitely\!(" + (KeywordEx)Keyword.LineRepeat + @"(-1))"" or without argument according to the advanced settings." + Environment.NewLine +
			@"Note that ""\!(" + (KeywordEx)Keyword.LineRepeat + @")"" will repeat as fast as possible, thus resulting in 100% load of the CPU core in use." + Environment.NewLine +
			@"To prevent, use the keyword combined with a delay or interval keyword, e.g. ""\!(" + (KeywordEx)Keyword.LineRepeat + @")\!(" + (KeywordEx)Keyword.LineInterval + @")""." + Environment.NewLine +
			@"Use [Ctrl+B] to break an ongoing ""\!(" + (KeywordEx)Keyword.LineRepeat + @")"" operation." + Environment.NewLine +
			Environment.NewLine +
			@"""Send the current time stamp ""\!(" + (KeywordEx)Keyword.TimeStamp + @"())"" formatted according to [View > Format Settings... > Options > Time Stamp]." + Environment.NewLine +
			Environment.NewLine +
			@"Change serial COM port to COM10 ""\!(" + (KeywordEx)Keyword.Port + @"(10))""." + Environment.NewLine +          // \remind (2018-06-13 / MKY) flow control as integer since yet limited to parsing integer values (FR #404).
			@"Change serial COM port settings to the specified values ""\!(" + (KeywordEx)Keyword.PortSettings + @"(19200, 7, 1))"" (all values must be specified as integer values)." + Environment.NewLine +
			@"Change baud rate to the specified value ""\!(" + (KeywordEx)Keyword.Baud + @"(19200))""." + Environment.NewLine +
			@"Change data bits to the specified value ""\!(" + (KeywordEx)Keyword.DataBits + @"(7))""." + Environment.NewLine +
			@"Change parity to the specified value ""\!(" + (KeywordEx)Keyword.Parity + @"(2))"" (value must be specified as corresponding integer value)." + Environment.NewLine +
			@"Change stop bits to the specified value ""\!(" + (KeywordEx)Keyword.StopBits + @"(2))"" (value must be specified as integer value, i.e. 1.5 is not supported by this keyword (yet))." + Environment.NewLine +
			@"Change flow control to the specified value ""\!(" + (KeywordEx)Keyword.FlowControl + @"(1))"" (value must be specified as corresponding integer value)." + Environment.NewLine +
			@"These keywords only apply to serial COM ports. Port will be closed and reopened to apply keywords." + Environment.NewLine +
			Environment.NewLine +                                                                   // \remind (2018-06-13 / MKY) flow control as integer since yet limited to parsing integer values (FR #404).
			@"RTS (aka RFR and RTR) signal on ""\!(" + (KeywordEx)Keyword.RtsOn + @")""." + Environment.NewLine +
			@"RTS (aka RFR and RTR) signal off ""\!(" + (KeywordEx)Keyword.RtsOff + @")""." + Environment.NewLine +
			@"RTS (aka RFR and RTR) signal toggle ""\!(" + (KeywordEx)Keyword.RtsToggle + @")""." + Environment.NewLine +
			@"These keywords only apply to serial COM ports. Port will be flushed before keywords get applied. Keywords cannot be applied when automatic hardware or RS-485 flow control is active." + Environment.NewLine +
			Environment.NewLine +
			@"DTR signal on ""\!(" + (KeywordEx)Keyword.DtrOn + @")""." + Environment.NewLine +
			@"DTR signal off ""\!(" + (KeywordEx)Keyword.DtrOff + @")""." + Environment.NewLine +
			@"DTR signal toggle ""\!(" + (KeywordEx)Keyword.DtrToggle + @")""." + Environment.NewLine +
			@"These keywords only apply to serial COM ports. Port will be flushed before keywords get applied." + Environment.NewLine +
			Environment.NewLine +
			@"Output break state on ""\!(" + (KeywordEx)Keyword.OutputBreakOn + @")""." + Environment.NewLine +
			@"Output break state off ""\!(" + (KeywordEx)Keyword.OutputBreakOff + @")""." + Environment.NewLine +
			@"Output break state toggle ""\!(" + (KeywordEx)Keyword.OutputBreakToggle + @")""." + Environment.NewLine +
			@"These keywords only apply to serial COM ports. Port will be flushed before keywords get applied." + Environment.NewLine +
			Environment.NewLine +
			@"Framing errors on ""\!(" + (KeywordEx)Keyword.FramingErrorsOn + @")""." + Environment.NewLine +
			@"Framing errors off ""\!(" + (KeywordEx)Keyword.FramingErrorsOff + @")""." + Environment.NewLine +
			@"Restore framing error setting ""\!(" + (KeywordEx)Keyword.FramingErrorsRestore + @")""." + Environment.NewLine +
			@"These keywords only apply to serial COM ports." + Environment.NewLine +
			Environment.NewLine +
			@"Change USB Ser/HID report ID to 1 ""\!(" + (KeywordEx)Keyword.ReportId + @"(1))""." + Environment.NewLine +
			@"This keyword only applies to USB Ser/HID.";

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const Radix DefaultRadixDefault = Radix.String;

		/// <summary>
		/// Default is <see cref="EncodingEx.Default"/> which is <see cref="SupportedEncoding.UTF8"/>
		/// which corresponds to <see cref="Encoding.UTF8"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="Encoding.Default"/> must not be used because that is limited to an ANSI code page.
		/// </remarks>
		/// <remarks>
		/// Must be implemented as property (instead of a readonly) since <see cref="Encoding"/>
		/// is a mutable reference type. Defining a readonly would correctly result in FxCop
		/// message CA2104 "DoNotDeclareReadOnlyMutableReferenceTypes" (Microsoft.Security).
		/// </remarks>
		public static Encoding EncodingDefault
		{
			get { return (Encoding.GetEncoding((int)(EncodingEx.Default))); }
		}

		/// <summary>
		/// Default is <see cref="EndiannessEx.Default"/>.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Endianness", Justification = "'Endianness' is a correct English term.")]
		public const Endianness EndiannessDefault = EndiannessEx.Default;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Parser parentParser;

		private Encoding encoding;        // = null;
		private Endianness endianness;    // = BigEndian;
		private Mode mode;                // = None;

		private Radix defaultRadix;       // = None;

		private StringReader charReader;  // = null;
		private MemoryStream bytesWriter; // = null;
		private List<Result> result;      // = null;
		private ParserState state;        // = null;

		private bool isKeywordParser;     // = false;
		private bool doProbe;             // = false;

		private bool hasFinished;         // = false;

		private Parser nestedParser;      // = null;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <remarks>
		/// <paramref name="mode"/> intentionally not defaulted, client must actively select the
		/// desired mode in order to prevent obscure behavior (default = <see cref="Mode.None"/> or
		/// rather <see cref="Mode.Default"/> which is <see cref="Mode.AllEscapes"/>?).
		/// </remarks>
		public Parser(Mode mode)
			: this(EncodingDefault, mode)
		{
		}

		/// <remarks>
		/// <paramref name="mode"/> intentionally not defaulted, client must actively select the
		/// desired mode in order to prevent obscure behavior (default = <see cref="Mode.None"/> or
		/// rather <see cref="Mode.Default"/> which is <see cref="Mode.AllEscapes"/>?).
		/// </remarks>
		public Parser(Encoding encoding, Mode mode)
			: this(encoding, EndiannessDefault, mode)
		{
		}

		/// <remarks>
		/// <paramref name="mode"/> intentionally not defaulted, client must actively select the
		/// desired mode in order to prevent obscure behavior (default = <see cref="Mode.None"/> or
		/// rather <see cref="Mode.Default"/> which is <see cref="Mode.AllEscapes"/>?).
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public Parser(Endianness endianness, Mode mode)
			: this(EncodingDefault, endianness, mode)
		{
		}

		/// <remarks>
		/// <paramref name="mode"/> intentionally not defaulted, client must actively select the
		/// desired mode in order to prevent obscure behavior (default = <see cref="Mode.None"/> or
		/// rather <see cref="Mode.Default"/> which is <see cref="Mode.AllEscapes"/>?).
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "endianness", Justification = "'Endianness' is a correct English term.")]
		public Parser(Encoding encoding, Endianness endianness, Mode mode)
		{
			this.encoding   = encoding;
			this.endianness = endianness;
			this.mode       = mode;
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

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			// Dispose of managed resources:
			if (disposing)
			{
				DisposeAndReset();
			}
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
			AssertUndisposed();

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

		internal virtual Mode Mode
		{
			get { return (this.mode); }
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual byte[] Parse(string s, Radix defaultRadix = DefaultRadixDefault)
		{
		////AssertUndisposed() is called by 'TryParse()' below.

			byte[] result;
			FormatException formatException = new FormatException("");
			if (!TryParse(s, out result, ref formatException, defaultRadix))
				throw (formatException);

			return (result);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual bool TryParse(string s, out byte[] result, Radix defaultRadix = DefaultRadixDefault)
		{
		////AssertUndisposed() is called by 'TryParse()' below.

			FormatException formatException = new FormatException("");
			return (TryParse(s, out result, ref formatException, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual bool TryParse(string s, out byte[] result, out string successfullyParsed, Radix defaultRadix = DefaultRadixDefault)
		{
		////AssertUndisposed() is called by 'TryParse()' below.

			FormatException formatException = new FormatException("");
			return (TryParse(s, out result, out successfullyParsed, ref formatException, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Required for recursion.")]
		public virtual bool TryParse(string s, out byte[] result, ref FormatException formatException, Radix defaultRadix = DefaultRadixDefault)
		{
		////AssertUndisposed() is called by 'TryParse()' below.

			string successfullyParsed;
			return (TryParse(s, out result, out successfullyParsed, ref formatException, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for recursion.")]
		public virtual bool TryParse(string s, out byte[] result, out string successfullyParsed, ref FormatException formatException, Radix defaultRadix = DefaultRadixDefault)
		{
		////AssertUndisposed() is called by 'TryParse()' below.

			Result[] typedResult;
			if (TryParse(s, out typedResult, out successfullyParsed, ref formatException, defaultRadix))
			{
				using (var bytes = new MemoryStream())
				{
					foreach (Result r in typedResult)
					{
						var bar = (r as BytesResult);
						if (bar != null)
						{
							byte[] a = bar.Bytes;
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual bool TryParse(string s, Radix defaultRadix = DefaultRadixDefault)
		{
		////AssertUndisposed() is called by 'TryParse()' below.

			Result[] result;
			return (TryParse(s, out result, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual bool TryParse(string s, out string successfullyParsed, Radix defaultRadix = DefaultRadixDefault)
		{
		////AssertUndisposed() is called by 'TryParse()' below.

			Result[] result;
			return (TryParse(s, out result, out successfullyParsed, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Required for recursion.")]
		public virtual bool TryParse(string s, out string successfullyParsed, ref FormatException formatException, Radix defaultRadix = DefaultRadixDefault)
		{
		////AssertUndisposed() is called by 'TryParse()' below.

			Result[] result;
			return (TryParse(s, out result, out successfullyParsed, ref formatException, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual bool TryParse(string s, out Result[] result, Radix defaultRadix = DefaultRadixDefault)
		{
		////AssertUndisposed() is called by 'TryParse()' below.

			string successfullyParsed;
			return (TryParse(s, out result, out successfullyParsed, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual bool TryParse(string s, out Result[] result, out string successfullyParsed, Radix defaultRadix = DefaultRadixDefault)
		{
		////AssertUndisposed() is called by 'TryParse()' below.

			FormatException formatException = new FormatException("");
			return (TryParse(s, out result, out successfullyParsed, ref formatException, defaultRadix));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for recursion.")]
		public virtual bool TryParse(string s, out Result[] result, out string successfullyParsed, ref FormatException formatException, Radix defaultRadix = DefaultRadixDefault)
		{
			AssertUndisposed();

			bool doProbe = (s.Length <= 256); // Inhibit probing in order to keep speed at a decent level...
			InitializeTopLevel(s, defaultRadix, doProbe);

			while (!HasFinished)
			{
				int c = this.charReader.Read();
				if (!this.state.TryParse(this, c, ref formatException))
				{
					CommitPendingBytes();

					string remaining = this.charReader.ReadToEnd();
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
						int successfullyParsedLength = Math.Max(0, (s.Length - remaining.Length - 1));
						successfullyParsed = StringEx.Left(s, successfullyParsedLength);
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

		/// <summary>
		/// Determines whether the specified character is an escape character.
		/// </summary>
		public static bool IsEscape(char c)
		{
			if (c == '\\')
				return (true);

			if (c == '<')
				return (true);

			if (c == '(')
				return (true);

			return (false);
		}

		#endregion

		#region Internal Methods
		//==========================================================================================
		// Internal Methods
		//==========================================================================================

		/// <remarks>
		/// Spelled "WhiteSpace" instead of "Whitespace" for consistency with the
		/// <see cref="char.IsWhiteSpace(char)"/> method.
		/// </remarks>
		/// <remarks>
		/// Using <c>int</c> instead of <c>char</c> for ease of use after calling
		/// <see cref="StringReader.Peek()"/> and <see cref="StringReader.Read()"/>.
		/// </remarks>
		/// <remarks>
		/// Virtual instance member instead of static member to be prepared for potential overload.
		/// </remarks>
		internal virtual bool IsWhiteSpace(int parseChar)
		{
			if (Int32Ex.IsWithin(parseChar, char.MinValue, char.MaxValue) && char.IsWhiteSpace((char)parseChar)) // "official" whitespace.
				return (true);

			if (Int32Ex.IsWithin(parseChar, byte.MinValue, byte.MaxValue) && Ascii.IsControl((byte)parseChar)) // ASCII control.
				return (true);

			return (false);
		}

		/// <summary></summary>
		internal virtual void CommitPendingBytes()
		{
			AssertUndisposed();

			if (this.bytesWriter.Length > 0)
			{
				// Commit:
				this.result.Add(new BytesResult(this.bytesWriter.ToArray()));

				// Restart:
				this.bytesWriter.Dispose();
				this.bytesWriter = new MemoryStream(); // Former stream has just been disposed of above.
			}
		}

		/// <summary></summary>
		internal virtual void CommitResult(Result result)
		{
			AssertUndisposed();

			this.result.Add(result);
		}

		/// <summary>
		/// Parses <paramref name="s"/> for one or more items in the specified base <paramref name="radix"/>, separated by spaces.
		/// </summary>
		/// <param name="s">String to be parsed.</param>
		/// <param name="radix">Numeric radix.</param>
		/// <param name="result">Array containing the resulting bytes.</param>
		/// <param name="formatException">Returned if invalid string format.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		/// <exception cref="OverflowException">Thrown if a value cannot be converted into bytes.</exception>
		internal virtual bool TryParseContiguousRadix(string s, Radix radix, out byte[] result, ref FormatException formatException)
		{
			using (var bytes = new MemoryStream())
			{
				if (radix == Radix.String)
				{
					byte[] a;
					if (TryParseContiguousRadixItem(s, radix, out a, ref formatException))
					{
						bytes.Write(a, 0, a.Length);
					}
					else
					{
						result = null;
						return (false);
					}
				}
				else
				{
					foreach (string item in s.Split())
					{
						if (item.Length > 0)
						{
							byte[] a;
							if (TryParseContiguousRadixItem(item, radix, out a, ref formatException))
							{
								bytes.Write(a, 0, a.Length);
							}
							else
							{
								result = null;
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
			AssertUndisposed();

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
				case Radix.Unicode:
					return (TryParseAndConvertContiguousNumericItem(s, radix, out result, ref formatException));

				default:
					throw (new ArgumentOutOfRangeException("radix", radix, MessageHelper.InvalidExecutionPreamble + "'" + radix + "' radix is missing here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Encodes <paramref name="s"/> containing a plain string into bytes.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Symmetricity with TryEncodeCharItem() below.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Symmetricity with TryEncodeCharItem() below.")]
		internal virtual bool TryEncodeStringItem(string s, out byte[] result, ref FormatException formatException)
		{
			result = GetBytes(s);
			return (true);
		}

		/// <summary>
		/// Encodes the given string, taking encoding into account.
		/// </summary>
		internal byte[] GetBytes(string s)
		{
			byte[] a = this.encoding.GetBytes(s);
			return (a);

			// \remind (2017-12-09 / MKY / bug #400)
			// YAT versions 1.99.70 and 1.99.80 used to take the endianness into account when encoding
			// and decoding multi-byte encoded characters. However, it was always done, but of course e.g.
			// UTF-8 is independent on endianness. The endianness would only have to be applied to single
			// multi-byte values, not multi-byte values split into multiple fragments. However, a .NET
			// "Encoding" object does not tell whether the encoding is potentially endianness capable or
			// not. Thus, it was decided to again remove the character encoding endianness awareness.
		}

		/// <summary>
		/// Encodes <paramref name="s"/> containing a single char item into bytes.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Required for recursion.")]
		internal virtual bool TryEncodeCharItem(string s, out byte[] result, ref FormatException formatException)
		{
			if (s.Length == 1) // Must be a single character, otherwise something went wrong...
			{
				result = GetBytes(s[0]);
				return (true);
			}
			else
			{
				formatException = new FormatException(@"""" + s + @""" does not consist of a single character.");
				result = null;
				return (false);
			}
		}

		/// <summary>
		/// Encodes the given character, taking encoding into account.
		/// </summary>
		internal byte[] GetBytes(char c)
		{
			byte[] a = this.encoding.GetBytes(new char[] { c });
			return (a);

			// \remind (2017-12-09 / MKY / bug #400)
			// YAT versions 1.99.70 and 1.99.80 used to take the endianness into account when encoding
			// and decoding multi-byte encoded characters. However, it was always done, but of course e.g.
			// UTF-8 is independent on endianness. The endianness would only have to be applied to single
			// multi-byte values, not multi-byte values split into multiple fragments. However, a .NET
			// "Encoding" object does not tell whether the encoding is potentially endianness capable or
			// not. Thus, it was decided to again remove the character encoding endianness awareness.
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
		/// <remarks>
		/// Implementation allows e.g. \h(01020A0F) to easily send data from e.g. a .hex file.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Required for recursion.")]
		internal virtual bool TryParseAndConvertContiguousNumericItem(string s, Radix radix, out byte[] result, ref FormatException formatException)
		{
			using (var bytes = new MemoryStream())
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
								ulong value;
								if (UInt64Ex.TryParseBinary(StringEx.Left(remaining, i), out value))
								{
									if (value <= 0xFF) // i left-most characters are a valid binary byte!
									{
										bytes.WriteByte((byte)value);

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
								ulong value;
								if (UInt64Ex.TryParseOctal(StringEx.Left(remaining, i), out value))
								{
									if (value <= 0xFF) // i left-most characters are a valid octal byte!
									{
										bytes.WriteByte((byte)value);

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
								byte value;
								if (byte.TryParse(StringEx.Left(remaining, i), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
								{
									bytes.WriteByte(value);

									remaining = remaining.Remove(0, i);
									found = true;
									break; // Quit for-loop and continue within remaining string.
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
								byte value;
								if (byte.TryParse(StringEx.Left(remaining, i), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value))
								{
									bytes.WriteByte(value);

									remaining = remaining.Remove(0, i);
									found = true;
									break; // Quit for-loop and continue within remaining string.
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

					case Radix.Unicode:
					{
						while (remaining.Length > 0)
						{
							bool found = false;

							int from = Math.Min(4, remaining.Length); // Note limitation FR #329: Unicode is limited to the basic multilingual plane (U+0000..U+FFFF).
							for (int i = from; i >= 1; i--) // Probe the 4-3-2-1 left-most characters for a valid hexadecimal Unicode value.
							{
								ushort value;
								if (ushort.TryParse(StringEx.Left(remaining, i), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value))
								{
									var c = char.ConvertFromUtf32(value);
									var a = this.encoding.GetBytes(c);
									bytes.Write(a, 0, a.Length);

									// \remind (2017-12-09 / MKY / bug #400)
									// YAT versions 1.99.70 and 1.99.80 used to take the endianness into account when encoding
									// and decoding multi-byte encoded characters. However, it was always done, but of course e.g.
									// UTF-8 is independent on endianness. The endianness would only have to be applied to single
									// multi-byte values, not multi-byte values split into multiple fragments. However, a .NET
									// "Encoding" object does not tell whether the encoding is potentially endianness capable or
									// not. Thus, it was decided to again remove the character encoding endianness awareness.

									remaining = remaining.Remove(0, i);
									found = true;
									break; // Quit for-loop and continue within remaining string.
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
						throw (new ArgumentOutOfRangeException("radix", radix, MessageHelper.InvalidExecutionPreamble + "'" + radix + "' radix is not supported for numeric values!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}

				if (success)
				{
					result = bytes.ToArray();
					return (true);
				}
				else
				{
					var sb = new StringBuilder();

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
						case Radix.Bin:     sb.Append("binary");      break;
						case Radix.Oct:     sb.Append("octal");       break;
						case Radix.Dec:     sb.Append("decimal");     break;
						case Radix.Hex:     sb.Append("hexadecimal"); break;
						case Radix.Unicode: sb.Append("Unicode");     break;

						default: throw (new ArgumentOutOfRangeException("radix", radix, MessageHelper.InvalidExecutionPreamble + "'" + radix + "' radix is not supported for numeric values!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}

					sb.Append(" value.");

					formatException = new FormatException(sb.ToString());
					result = null;
					return (false);
				}
			} // using (MemoryStream)
		}

		/// <summary>
		/// Parses <paramref name="s"/> for a keyword.
		/// </summary>
		/// <param name="s">String to be parsed.</param>
		/// <param name="result">Array containing the results.</param>
		/// <param name="formatException">Returned if invalid string format.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Non-static for orthogonality with other TryParse() methods.")]
		internal virtual bool TryParseKeyword(string s, out KeywordResult result, ref FormatException formatException)
		{
			Keyword keyword;
			if (KeywordEx.TryParse(s, out keyword))
			{
				result = new KeywordResult(keyword);
				return (true);
			}
			else
			{
				formatException = new FormatException(@"""" + s + @""" is an invalid keyword.");
				result = null;
				return (false);
			}
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
			this.mode            = parent.mode;

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

			this.charReader   = null;
			this.bytesWriter  = null;
			this.state        = null;
			this.nestedParser = null;

			this.parentParser = null; // Last because needed further above.
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
