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
// YAT Version 2.0.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

using MKY;
using MKY.Text;
using MKY.Text.RegularExpressions;

// The YAT.Domain.Settings namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain.Settings namespace even though the file is
// located in the YAT.Domain\TextSettings for better separation of the implementation files.
namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	[Serializable]
	public class TextTerminalSettings : MKY.Settings.SettingsItem, IEquatable<TextTerminalSettings>
	{
		/// <summary></summary>
		public const bool SeparateTxRxEolDefault = false;

		/// <summary></summary>
		public static readonly string EolDefault = EolEx.Parse(Environment.NewLine);

		/// <summary></summary>
		public static readonly int EncodingDefault = (EncodingEx)System.Text.Encoding.Default;

		/// <summary></summary>
		public const bool ShowEolDefault = false;

		/// <summary></summary>
		public static readonly TextLineSendDelay LineSendDelayDefault = new TextLineSendDelay(false, 500, 1);

		/// <summary></summary>
		public static readonly WaitForResponse WaitForResponseDefault = new WaitForResponse(false, 500);

		/// <summary></summary>
		public const CharSubstitution CharSubstitutionDefault = CharSubstitution.None;

		private bool   separateTxRxEol;
		private string txEol;
		private string rxEol;
		private int    encoding;
		private bool   showEol;

		private TextLineSendDelay     lineSendDelay;
		private WaitForResponse       waitForResponse;
		private CharSubstitution      charSubstitution;
		private TextExclusionSettings textExclusion;

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

			TextExclusion = new TextExclusionSettings(SettingsType);

			SetNodeDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
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
			LineSendDelay    = rhs.LineSendDelay;
			WaitForResponse  = rhs.WaitForResponse;
			CharSubstitution = rhs.CharSubstitution;

			TextExclusion = new TextExclusionSettings(rhs.TextExclusion);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			SeparateTxRxEol  = SeparateTxRxEolDefault;
			TxEol            = EolDefault;
			RxEol            = EolDefault;
			Encoding         = EncodingDefault;
			ShowEol          = ShowEolDefault;
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

		/// <remarks>
		/// This option is one of two ways to delay while sending.
		/// It supports adding a fixed delay each N lines.
		/// 
		/// Alternatively, the \!(LineDelay) or \!(LineInterval) keyword can be used.
		/// The first is equal to setting "each 1 line".
		/// The latter allows a more precise timing.
		/// 
		/// The text above is copy-pasted into the tool tip text of the text settings dialog.
		/// </remarks>
		[XmlElement("LineSendDelay")]
		public virtual TextLineSendDelay LineSendDelay
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
		/// \remind (2017-12-11 / MKY)
		/// This feature is not yet implemented.
		/// It is tracked as feature request #19 and bug #176.
		/// </remarks>
		[XmlElement("WaitForResponse")]
		public virtual WaitForResponse WaitForResponse
		{
			get { return (this.waitForResponse); }
			set
			{
				if (this.waitForResponse != WaitForResponseDefault) // value)
				{
					this.waitForResponse = WaitForResponseDefault; // value;
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
				hashCode = (hashCode * 397) ^  LineSendDelay        .GetHashCode();
				hashCode = (hashCode * 397) ^  WaitForResponse      .GetHashCode();
				hashCode = (hashCode * 397) ^  CharSubstitution     .GetHashCode();

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
				LineSendDelay         .Equals(other.LineSendDelay)   &&
				WaitForResponse       .Equals(other.WaitForResponse) &&
				CharSubstitution      .Equals(other.CharSubstitution)
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
