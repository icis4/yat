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
// YAT Version 2.4.0
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
using System.Linq;

using MKY;
using MKY.Text;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in YAT.Domain\TextTerminal for better separation of the implementation files.
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
	/// <item><description>Less simple-stupid-forwarder, e.g. directly raising events.</description></item>
	/// </list>
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
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
			InitializeProcess();
		}

		/// <summary></summary>
		public TextTerminal(Settings.TerminalSettings settings, Terminal terminal)
			: base(settings, terminal)
		{
			AttachTextTerminalSettings();
			InitializeProcess();
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "glueCharsOfLineTimeout", Justification = "DisposeProcess() disposes of this member.")]
		[SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "waitForResponseEvent",   Justification = "DisposeProcess() disposes of this member.")]
		protected override void Dispose(bool disposing)
		{
			DetachTextTerminalSettings();

			// Dispose of managed resources:
			if (disposing)
			{
				// Nothing else to do (yet).
			}

			base.Dispose(disposing);
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual Settings.TextTerminalSettings TextTerminalSettings
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
				AssertUndisposed();

				if (this.textTxState != null)
					return (this.textTxState.EolSequence);
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
				AssertUndisposed();

				if (this.textRxState != null)
					return (this.textRxState.EolSequence);
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
		protected override bool TryParse(string s, Radix defaultRadix, Parser.Mode parseMode, out Parser.Result[] result, out string textSuccessfullyParsed)
		{
			AssertUndisposed();

			using (var p = new Parser.SubstitutionParser(TextTerminalSettings.CharSubstitution, (EncodingEx)TextTerminalSettings.Encoding, TerminalSettings.IO.Endianness, parseMode))
				return (p.TryParse(s, out result, out textSuccessfullyParsed, defaultRadix));
		}

		/// <summary>
		/// Tries to parse <paramref name="s"/>, taking the current settings into account.
		/// </summary>
		public override bool TryParse(string s, Radix defaultRadix, out byte[] result)
		{
			AssertUndisposed();

			using (var p = new Parser.SubstitutionParser(TextTerminalSettings.CharSubstitution, (EncodingEx)TextTerminalSettings.Encoding, TerminalSettings.IO.Endianness, TerminalSettings.Send.Text.ToParseMode()))
				return (p.TryParse(s, out result, defaultRadix));
		}

		#endregion

		#region Format
		//------------------------------------------------------------------------------------------
		// Format
		//------------------------------------------------------------------------------------------

	#if (WITH_SCRIPTING)

		/// <summary>
		/// Formats the specified data sequence for scripting.
		/// </summary>
		/// <remarks>
		/// For text terminals, received messages are text lines, decoded with the configured encoding. Messages include control characters as configured. Messages do not include EOL.
		/// For binary terminals, received messages are hexadecimal values, separated by spaces, without radix.
		/// If ever needed differently, an [advanced configuration of scripting behavior] shall be added.
		/// </remarks>
		protected override string FormatMessageTextForScripting(DateTime timeStamp, byte[] data)
		{
			// Attention:
			// Similar code exists in RemoveFraming(byte[], byte[]).
			// Changes here likely have to be applied there too.
			// Code is duplicated rather than encapsulated for performance reasons (no need to create a list and recreate an array).

			var n = data.Length;

			var rxEolSequence = RxEolSequence;
			if (rxEolSequence.Length > 0) // "Messages do not include EOL."
			{
				var contentLength = (data.Length - rxEolSequence.Length);
				if (data.Skip(contentLength).SequenceEqual(rxEolSequence))
					n = contentLength;
			}

			// Attention:
			// Similar code exists in Terminal.Format(byte[], direction, radix).
			// Changes here likely have to be applied there too.

			var lp = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.

			var pendingMultiBytesToDecode = new List<byte>(4); // Preset the required capacity to improve memory management; 4 is the maximum value for multi-byte characters.
			foreach (byte b in data.Take(n))
			{
				var de = ByteToElement(b, timeStamp, IODirection.Rx, Radix.String, pendingMultiBytesToDecode);
			////AddContentSeparatorAsNeeded(IODirection.Rx, lp, de) is not needed as radix is fixed to 'String'.
				lp.Add(de);
			}

			return (lp.ElementsToString());
		}

	#endif // WITH_SCRIPTING

		#endregion

		#region Change
		//------------------------------------------------------------------------------------------
		// Change
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Removes the framing from the given data.
		/// </summary>
		/// <remarks>
		/// For text terminals, framing is typically defined by EOL.
		/// For binary terminals, framing is optionally defined by sequence before/after.
		/// </remarks>
		/// <exception cref="InvalidOperationException">
		/// <see cref="Settings.TextTerminalSettings.TxEol"/> and
		/// <see cref="Settings.TextTerminalSettings.RxEol"/> have different values.
		/// </exception>
		public override void RemoveFraming(byte[] data)
		{
			if (ArrayEx.ValuesEqual(TxEolSequence, RxEolSequence))
				RemoveFraming(data, IODirection.Tx); // Sequence is the same for both directions, direction doesn't matter.

			throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "This method requires that 'TxEOL' and 'RxEOL' are set to the same value!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary>
		/// Removes the framing from the given data.
		/// </summary>
		/// <remarks>
		/// For text terminals, framing is typically defined by EOL.
		/// For binary terminals, framing is optionally defined by sequence before/after.
		/// </remarks>
		public override void RemoveFraming(byte[] data, IODirection direction)
		{
			switch (direction)
			{
				case IODirection.Tx:    RemoveFraming(data, TxEolSequence); break;
				case IODirection.Rx:    RemoveFraming(data, RxEolSequence); break;

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is an invalid direction!"               + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Removes the framing from the given data.
		/// </summary>
		/// <remarks>
		/// For text terminals, framing is typically defined by EOL.
		/// For binary terminals, framing is optionally defined by sequence before/after.
		/// </remarks>
		protected virtual void RemoveFraming(byte[] data, byte[] eolSequence)
		{
			// Attention:
			// Similar code exists in FormatMessageTextForScripting(byte[]).
			// Changes here likely have to be applied there too.
			// Code is duplicated rather than encapsulated for performance reasons (no need to recreate array).

			if (eolSequence.Length > 0)
			{
				var n = (data.Length - eolSequence.Length);
				if (data.Skip(n).SequenceEqual(eolSequence))
					data.Take(n).ToArray();
			}

			// Note:
			// When adding BOL, i.e. an additional operation at the beginning of the data,
			// the implementation shall follow the one of the BinaryTerminal.
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

		/// <remarks>
		/// This method shall not be overridden as it accesses the quasi-private member
		/// <see cref="TextTerminalSettings"/>.
		/// </remarks>
		protected Settings.TextDisplaySettings GetTextDisplaySettings(IODirection dir)
		{
			switch (dir)
			{
				case IODirection.Tx:    return (TextTerminalSettings.TxDisplay);
				case IODirection.Rx:    return (TextTerminalSettings.RxDisplay);

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is an invalid direction!"               + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region Settings Events
		//------------------------------------------------------------------------------------------
		// Settings Events
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Used for e.g. showing/hiding EOL.
		/// </remarks>
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
			// AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

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
			// AssertUndisposed() shall not be called from such basic method! Its return value may be needed for debugging. All underlying fields are still valid after disposal.

			return (indent + "> Type: TextTerminal" + Environment.NewLine + base.ToExtendedDiagnosticsString(indent));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
