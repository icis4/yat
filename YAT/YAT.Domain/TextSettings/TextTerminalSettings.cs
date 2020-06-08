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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Text;
using System.Xml.Serialization;

using MKY;
using MKY.Text;
using MKY.Text.RegularExpressions;

#endregion

// The YAT.Domain.Settings namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain.Settings namespace even though the file is
// located in YAT.Domain\TextSettings for better separation of the implementation files.
namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
	public class TextTerminalSettings : MKY.Settings.SettingsItem, IEquatable<TextTerminalSettings>
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const bool SeparateTxRxEolDefault = false;

		/// <summary></summary>
		public static readonly string EolDefault = EolEx.Parse(Environment.NewLine);

		/// <summary>
		/// Default is <see cref="EncodingEx.Default"/> which is <see cref="Encoding.UTF8"/>.
		/// </summary>
		/// <remarks>
		/// <see cref="Encoding.Default"/> must not be used because that is limited to an ANSI code page.
		/// </remarks>
		public static readonly int EncodingDefault = (EncodingEx)EncodingEx.Default;

		/// <summary></summary>
		public const bool ShowEolDefault = false;

		/// <summary></summary>
		public const bool SeparateTxRxDisplayDefault = false;

		/// <remarks>
		/// At e.g. 9600 baud, the frame time is ~1 ms, thus initially tried 50 ms. But tests on
		/// serial loopback port at 9600 baud revealed chunks of a 26 byte long line at e.g.:
		/// <list type="bullet">
		/// <item><description>x.024 and x.067 seconds =>  ~50 ms</description></item>
		/// <item><description>x.055 and x.131 seconds =>  ~80 ms</description></item>
		/// <item><description>x.102 and x.143 seconds =>  ~40 ms</description></item>
		/// <item><description>x.131 and x.212 seconds =>  ~80 ms</description></item>
		/// <item><description>...</description></item>
		/// <item><description>x.835 and x.984 seconds => ~150 ms</description></item>
		/// <item><description>x.907 and x.996 seconds => ~100 ms</description></item>
		/// </list>
		/// Initially concluded to use 250 ms. However, that's too short even for automated testing
		/// on two interconnected TCP/IP AutoSockets. Therefore using 500 ms, same as other features
		/// <see cref="LineSendDelay"/> and <see cref="WaitForResponse"/> further below.
		/// </remarks>
		public const int GlueCharsOfLineTimeoutDefault = 500;

		/// <remarks>
		/// Default is <see cref="TimeoutSettingTuple.Enabled"/> = <c>true</c> for two reasons:
		/// <list type="bullet">
		/// <item><description>Most users don't like unnecessarily broken additional lines.</description></item>
		/// <item><description>Dealing with partial lines is still possible with the very short timeout default.</description></item>
		/// </list>
		/// </remarks>
		public static readonly TimeoutSettingTuple GlueCharsOfLineDefault = new TimeoutSettingTuple(true, GlueCharsOfLineTimeoutDefault);

		/// <summary></summary>
		public const int LineSendDelayTimeoutDefault = 500;

		/// <summary></summary>
		public static readonly TextLineSendDelaySettingTuple LineSendDelayDefault = new TextLineSendDelaySettingTuple(false, LineSendDelayTimeoutDefault, 1);

		/// <summary></summary>
		public const int WaitForResponseTimeoutDefault = 500;

		/// <summary></summary>
		public static readonly TextWaitForResponseSettingTuple WaitForResponseDefault = new TextWaitForResponseSettingTuple(false, 1, 1, WaitForResponseTimeoutDefault);

		/// <summary></summary>
		public const CharSubstitution CharSubstitutionDefault = CharSubstitution.None;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool   separateTxRxEol;
		private string txEol;
		private string rxEol;
		private int    encoding;
		private bool   showEol;

		private bool separateTxRxDisplay;
		private TextDisplaySettings txDisplay;
		private TextDisplaySettings rxDisplay;
		private TimeoutSettingTuple glueCharsOfLine;

		private TextLineSendDelaySettingTuple   lineSendDelay;
		private TextWaitForResponseSettingTuple waitForResponse;
		private CharSubstitution                charSubstitution;
		private TextExclusionSettings           textExclusion;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TextTerminalSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public TextTerminalSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();

			TxDisplay     = new TextDisplaySettings(SettingsType);
			RxDisplay     = new TextDisplaySettings(SettingsType);
			TextExclusion = new TextExclusionSettings(SettingsType);

			SetNodeDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TextTerminalSettings(TextTerminalSettings rhs)
			: base(rhs)
		{
			SeparateTxRxEol  = rhs.SeparateTxRxEol;
			TxEol            = rhs.TxEol;
			RxEol            = rhs.RxEol;
			Encoding         = rhs.Encoding;
			ShowEol          = rhs.ShowEol;

			SeparateTxRxDisplay =               rhs.SeparateTxRxDisplay;
			TxDisplay = new TextDisplaySettings(rhs.TxDisplay);
			RxDisplay = new TextDisplaySettings(rhs.RxDisplay);
			GlueCharsOfLine =                   rhs.GlueCharsOfLine;

			LineSendDelay    = rhs.LineSendDelay;
			WaitForResponse  = rhs.WaitForResponse;
			CharSubstitution = rhs.CharSubstitution;

			TextExclusion = new TextExclusionSettings(rhs.TextExclusion);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			SeparateTxRxEol  = SeparateTxRxEolDefault;
			TxEol            = EolDefault;
			RxEol            = EolDefault;
			Encoding         = EncodingDefault;
			ShowEol          = ShowEolDefault;

			SeparateTxRxDisplay = SeparateTxRxDisplayDefault;
			GlueCharsOfLine     = GlueCharsOfLineDefault;

			LineSendDelay    = LineSendDelayDefault;
			WaitForResponse  = WaitForResponseDefault;
			CharSubstitution = CharSubstitutionDefault;
		}

		/// <remarks>
		/// No other way has yet been found to properly set defaults of a list of settings. Before
		/// this solution, bug #3581368>#244 "EOL comment indicators always contain the defaults"
		/// existed. With this solution, the underlying node itself does no longer know any default,
		/// and default deserialization properly deserializes the settings into the underlying node.
		/// </remarks>
		protected override void SetNodeDefaults()
		{
			base.SetNodeDefaults();

			TextExclusion.Patterns.Clear();
			TextExclusion.Patterns.Add(          @"\s*//" + CommonPatterns.PositiveLookaheadOutsideQuotes +  ".*$");       // C/C++/C#/Java/...-style comments: From // until the end of the line, including leading whitespace.
			TextExclusion.Patterns.Add(            @"/\*" + CommonPatterns.PositiveLookaheadOutsideQuotes + @".*\*/");     // C/C++/C#/Java/...-style comments: From /* until */, not including surrounding whitespace.
			TextExclusion.Patterns.Add(        @"^\s*/\*" + CommonPatterns.PositiveLookaheadOutsideQuotes + @".*\*/\s*$"); // C/C++/C#/Java/...-style comments: Lines only consisting of /**/, including surrounding whitespace.
		////TextExclusion.Patterns.Add(            "<!--" + CommonPatterns.PositiveLookaheadOutsideQuotes +  ".*-->");     // XML style comments <!--...--> has been excluded as pattern conflicts with YAT <> tags.
		////TextExclusion.Patterns.Add(       @"^\s*<!--" + CommonPatterns.PositiveLookaheadOutsideQuotes + @".*-->\s*$");
			TextExclusion.Patterns.Add(           @"\s*#" + CommonPatterns.PositiveLookaheadOutsideQuotes +  ".*$");       // Shell-style comments: From # until the end of the line, including leading whitespace.
		////TextExclusion.Patterns.Add(              "<#" + CommonPatterns.PositiveLookaheadOutsideQuotes +  ".*#>");      // Shell-style comments <#...#> has been excluded as pattern conflicts with YAT <> tags.
		////TextExclusion.Patterns.Add(         @"^\s*<#" + CommonPatterns.PositiveLookaheadOutsideQuotes + @".*#>\s*$");
			TextExclusion.Patterns.Add(   @"^\s*(REM|::)" + CommonPatterns.PositiveLookaheadOutsideQuotes + @"\s*.*$");    // DOS-style comments: Lines starting with REM or ::, including leading whitespace.
			TextExclusion.Patterns.Add(@"\s*&\s*(REM|::)" + CommonPatterns.PositiveLookaheadOutsideQuotes + @"\s*.*$");    // DOS-style comments: From & REM or :: until the end of the line, including leading whitespace.
			TextExclusion.Patterns.Add(             @"%=" + CommonPatterns.PositiveLookaheadOutsideQuotes + @".*=%");      // DOS-style comments: From %= until =%, not including surrounding whitespace.
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("SeparateTxRxEol")]
		public virtual bool SeparateTxRxEol
		{
			get { return (this.separateTxRxEol); }
			set
			{
				if (this.separateTxRxEol != value)
				{
					this.separateTxRxEol = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxEol")]
		public virtual string TxEol
		{
			get { return (this.txEol); }
			set
			{
				if (this.txEol != value)
				{
					this.txEol = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxEol")]
		public virtual string RxEol
		{
			get
			{
				if (this.separateTxRxEol)
					return (this.rxEol);
				else // Rx redirects to Tx:
					return (this.txEol);
			}
			set
			{
				if (this.rxEol != value)
				{
					this.rxEol = value;
					SetMyChanged();
				}

				// Do not redirect on 'set'. this would not be an understandable behavior.
				// It could even confuse the user, e.g. when temporarily separating the settings,
				// and then load them again from XML => temporary settings get lost.
			}
		}

		/// <summary></summary>
		[XmlElement("Encoding")]
		public virtual int Encoding
		{
			get { return (this.encoding); }
			set
			{
				if (this.encoding != value)
				{
					this.encoding = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowEol")]
		public virtual bool ShowEol
		{
			get { return (this.showEol); }
			set
			{
				if (this.showEol != value)
				{
					this.showEol = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SeparateTxRxDisplay")]
		public virtual bool SeparateTxRxDisplay
		{
			get { return (this.separateTxRxDisplay); }
			set
			{
				if (this.separateTxRxDisplay != value)
				{
					this.separateTxRxDisplay = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TxDisplay")]
		public virtual TextDisplaySettings TxDisplay
		{
			get { return (this.txDisplay); }
			set
			{
				if (this.txDisplay != value)
				{
					var oldNode = this.txDisplay;
					this.txDisplay = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxDisplay")]
		public virtual TextDisplaySettings RxDisplay
		{
			get
			{
				if (this.separateTxRxDisplay)
					return (this.rxDisplay);
				else // Rx redirects to Tx:
					return (this.txDisplay);
			}
			set
			{
				if (this.rxDisplay != value)
				{
					var oldNode = this.rxDisplay;
					this.rxDisplay = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}

				// Do not redirect on 'set'. this would not be an understandable behavior.
				// It could even confuse the user, e.g. when temporarily separating the settings,
				// and then load them again from XML => temporary settings get lost.
			}
		}

		/// <remarks>
		/// Instead of glueing characters of a line and waiting with displaying the next line in the
		/// opposite direction (or other I/O device), characters could also be appended to two lines
		/// simultanously. Technically this would be possible by adding a ReplacePreviousLine() in
		/// next to <see cref="DisplayRepository.ReplaceCurrentLine(DisplayElementCollection)"/>.
		/// However:
		/// <list type="bullet">
		/// <item><description>Resulting behavior is very uncommon = incomprehensible.</description></item>
		/// <item><description>Implementation becomes a nightmare (filter, suppress,...).</description></item>
		/// </list>
		/// </remarks>
		[XmlElement("GlueCharsOfLine")]
		public virtual TimeoutSettingTuple GlueCharsOfLine
		{
			get { return (this.glueCharsOfLine); }
			set
			{
				if (this.glueCharsOfLine != value)
				{
					this.glueCharsOfLine = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// This option is one of two ways to delay while sending.
		/// It supports adding a fixed delay each N lines.
		///
		/// Alternatively, the \!(LineDelay()) or \!(LineInterval()) keyword can be used.
		/// The first is equal to setting "each 1 line".
		/// The latter allows a more precise timing.
		///
		/// The above text is copy-pasted to/from the tool tip text of the text settings dialog.
		/// </remarks>
		[XmlElement("LineSendDelay")]
		public virtual TextLineSendDelaySettingTuple LineSendDelay
		{
			get { return (this.lineSendDelay); }
			set
			{
				if (this.lineSendDelay != value)
				{
					this.lineSendDelay = value;
					SetMyChanged();
				}
			}
		}

		/// <remarks>
		/// Technically, a "wait for N characters or bytes before" option could also be implemented.
		/// Such option could also be provided by binary terminals. However, usefulness of such
		/// option is questionable, thus not (yet) implemented until somebody requests this.
		/// </remarks>
		[XmlElement("WaitForResponse")]
		public virtual TextWaitForResponseSettingTuple WaitForResponse
		{
			get { return (this.waitForResponse); }
			set
			{
				if (this.waitForResponse != value)
				{
					this.waitForResponse = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CharSubstitution")]
		public virtual CharSubstitution CharSubstitution
		{
			get { return (this.charSubstitution); }
			set
			{
				if (this.charSubstitution != value)
				{
					this.charSubstitution = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TextExclusion")]
		public virtual TextExclusionSettings TextExclusion
		{
			get { return (this.textExclusion); }
			set
			{
				if (this.textExclusion != value)
				{
					var oldNode = this.textExclusion;
					this.textExclusion = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^  SeparateTxRxEol      .GetHashCode();
				hashCode = (hashCode * 397) ^ (TxEol != null ? TxEol.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RxEol != null ? RxEol.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  Encoding             .GetHashCode();
				hashCode = (hashCode * 397) ^  ShowEol              .GetHashCode();

				hashCode = (hashCode * 397) ^  SeparateTxRxDisplay.GetHashCode();
				hashCode = (hashCode * 397) ^  GlueCharsOfLine    .GetHashCode();

				hashCode = (hashCode * 397) ^  LineSendDelay   .GetHashCode();
				hashCode = (hashCode * 397) ^  WaitForResponse .GetHashCode();
				hashCode = (hashCode * 397) ^  CharSubstitution.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as TextTerminalSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(TextTerminalSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				SeparateTxRxEol       .Equals(other.SeparateTxRxEol) &&
				StringEx.EqualsOrdinal(TxEol, other.TxEol)           &&
				StringEx.EqualsOrdinal(RxEol, other.RxEol)           &&
				Encoding              .Equals(other.Encoding)        &&
				ShowEol               .Equals(other.ShowEol)         &&

				SeparateTxRxDisplay.Equals(other.SeparateTxRxDisplay) &&
				GlueCharsOfLine    .Equals(other.GlueCharsOfLine)     &&

				LineSendDelay   .Equals(other.LineSendDelay)   &&
				WaitForResponse .Equals(other.WaitForResponse) &&
				CharSubstitution.Equals(other.CharSubstitution)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TextTerminalSettings lhs, TextTerminalSettings rhs)
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
		public static bool operator !=(TextTerminalSettings lhs, TextTerminalSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
