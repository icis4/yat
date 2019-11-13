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
// Copyright © 2003-2019 Matthias Kläy.
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
using System.Media;
using System.Text;

using MKY;
using MKY.Diagnostics;
using MKY.Text;

using YAT.Domain.Utilities;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in the YAT.Domain\TextTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// <see cref="Terminal"/> implementation with text semantics.
	/// </summary>
	/// <remarks>
	/// This class is implemented using partial classes separating sending/processing functionality.
	/// Using partial classes rather than aggregated sender, processor,... so far for these reasons:
	/// <list type="bullet">
	/// <item><description>Simpler for implementing text/binary specialization.</description></item>
	/// <item><description>Simpler for implementing synchronization among Tx and Rx.</description></item>
	/// <item><description>Less "Durchlauferhitzer", e.g. directly raising events.</description></item>
	/// </list>
	/// </remarks>
	public partial class TextTerminal : Terminal
	{
		#region Constant Help Text
		//==========================================================================================
		// Constant Help Text
		//==========================================================================================

		/// <summary></summary>
		public static readonly string KeywordHelp =
			@"For text terminals, the following additional keywords are supported :" + Environment.NewLine +
			Environment.NewLine +
			@"Send EOL sequence ""OK\!(" + (Parser.KeywordEx)Parser.Keyword.Eol + @")""" + Environment.NewLine +
			@"Do not send EOL ""OK\!(" + (Parser.KeywordEx)Parser.Keyword.NoEol + @")""" + Environment.NewLine +
			@"""\!(" + (Parser.KeywordEx)Parser.Keyword.NoEol + @")"" is useful if your text protocol does have an EOL sequence except for a few special commands (e.g. synchronization commands).";

		#endregion

		#region Types
		//------------------------------------------------------------------------------------------
		// Types
		//------------------------------------------------------------------------------------------

		private class LineSendDelayState
		{
			public int LineCount { get; set; }

			public LineSendDelayState()
			{
				LineCount = 0;
			}

			public LineSendDelayState(LineSendDelayState rhs)
			{
				LineCount = rhs.LineCount;
			}

			public virtual void Reset()
			{
				LineCount = 0;
			}
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private LineSendDelayState lineSendDelayState;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public TextTerminal(Settings.TerminalSettings settings)
			: base(settings)
		{
			AttachTextTerminalSettings();

			this.lineSendDelayState = new LineSendDelayState();
		}

		/// <summary></summary>
		public TextTerminal(Settings.TerminalSettings settings, Terminal terminal)
			: base(settings, terminal)
		{
			AttachTextTerminalSettings();

			var casted = (terminal as TextTerminal);
			if (casted != null)
			{
				// Tx:
				{
					this.txUnidirTextLineState = new TextLineState(casted.txUnidirTextLineState);
					this.txBidirTextLineState  = new TextLineState(casted.txBidirTextLineState);
				}

				// Rx:
				{
					this.rxUnidirTextLineState = new TextLineState(casted.rxUnidirTextLineState);
					this.rxBidirTextLineState  = new TextLineState(casted.rxBidirTextLineState);
				}

				// Bidir:
				{
					this.lineSendDelayState = new LineSendDelayState(casted.lineSendDelayState);
				}
			}
			else
			{
				this.lineSendDelayState = new LineSendDelayState();
			}
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected override void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
					DetachTextTerminalSettings();
				}
			}

			base.Dispose(disposing);
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		private Settings.TextTerminalSettings TextTerminalSettings
		{
			get
			{
				if (TerminalSettings != null)
					return (TerminalSettings.TextTerminal);
				else
					return (null);
			}
		}

		/// <summary>
		/// Gets the Tx EOL sequence.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] TxEolSequence
		{
			get
			{
				AssertNotDisposed();

				if (this.txUnidirTextLineState != null)
					return (this.txUnidirTextLineState.EolSequence);
				else
					return (null);
			}
		}

		/// <summary>
		/// Gets the Rx EOL sequence.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] RxEolSequence
		{
			get
			{
				AssertNotDisposed();

				if (this.rxUnidirTextLineState != null)
					return (this.rxUnidirTextLineState.EolSequence);
				else
					return (null);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region Open
		//------------------------------------------------------------------------------------------
		// Open
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public override bool Start()
		{
			bool success = base.Start();

			if (success)
				this.lineSendDelayState.Reset();

			return (success);
		}

		#endregion

		#region Parse
		//------------------------------------------------------------------------------------------
		// Parse
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Tries to parse <paramref name="s"/>, taking the current settings into account.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public override bool TryParseText(string s, out byte[] result, Radix defaultRadix = Radix.String)
		{
			using (var p = new Parser.SubstitutionParser(TextTerminalSettings.CharSubstitution, (EncodingEx)TextTerminalSettings.Encoding, TerminalSettings.IO.Endianness, TerminalSettings.Send.Text.ToParseMode()))
				return (p.TryParse(s, out result, defaultRadix));
		}

		#endregion

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region Settings
		//------------------------------------------------------------------------------------------
		// Settings
		//------------------------------------------------------------------------------------------

		private void AttachTextTerminalSettings()
		{
			if (TextTerminalSettings != null)
				TextTerminalSettings.Changed += TextTerminalSettings_Changed;
		}

		private void DetachTextTerminalSettings()
		{
			if (TextTerminalSettings != null)
				TextTerminalSettings.Changed -= TextTerminalSettings_Changed;
		}

		private void ApplyTextTerminalSettings()
		{
			RefreshRepositories();
		}

		#endregion

		#region Settings Events
		//------------------------------------------------------------------------------------------
		// Settings Events
		//------------------------------------------------------------------------------------------

		private void TextTerminalSettings_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			ApplyTextTerminalSettings();
		}

		#endregion

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging.

			return (ToExtendedDiagnosticsString()); // No 'real' ToString() method required yet.
		}

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Extended <see cref="ToString()"/> method which can be used for trace/debug.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public override string ToExtendedDiagnosticsString(string indent = "")
		{
			// Do not call AssertNotDisposed() on such basic method! Its return value may be needed for debugging.

			return (indent + "> Type: TextTerminal" + Environment.NewLine + base.ToExtendedDiagnosticsString(indent));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
