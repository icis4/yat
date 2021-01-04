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
// YAT Version 2.2.0 Development
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

using YAT.Domain.Utilities;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure.
// This code is intentionally placed into the YAT.Domain namespace even though the file is
// located in YAT.Domain\BinaryTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary>
	/// <see cref="Terminal"/> implementation with binary semantics.
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
	public partial class BinaryTerminal : Terminal
	{
		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public BinaryTerminal(Settings.TerminalSettings settings)
			: base(settings)
		{
			AttachBinaryTerminalSettings();
			InitializeProcess();
		}

		/// <summary></summary>
		public BinaryTerminal(Settings.TerminalSettings settings, Terminal terminal)
			: base(settings, terminal)
		{
			AttachBinaryTerminalSettings();
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
		protected override void Dispose(bool disposing)
		{
			DetachBinaryTerminalSettings();

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
		public virtual Settings.BinaryTerminalSettings BinaryTerminalSettings
		{
			get
			{
				if (TerminalSettings != null)
					return (TerminalSettings.BinaryTerminal);
				else
					return (null);
			}
		}

		/// <summary>
		/// Gets the Tx sequence before.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] TxSequenceBefore
		{
			get
			{
				AssertUndisposed();

				if (this.binaryTxState != null)
					return (this.binaryTxState.SequenceBefore);
				else
					return (null);
			}
		}

		/// <summary>
		/// Gets the Rx sequence before.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] RxSequenceBefore
		{
			get
			{
				AssertUndisposed();

				if (this.binaryRxState != null)
					return (this.binaryRxState.SequenceBefore);
				else
					return (null);
			}
		}

		/// <summary>
		/// Gets the Tx sequence after.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] TxSequenceAfter
		{
			get
			{
				AssertUndisposed();

				if (this.binaryTxState != null)
					return (this.binaryTxState.SequenceAfter);
				else
					return (null);
			}
		}

		/// <summary>
		/// Gets the Rx sequence after.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Guidelines for Collections: Do use byte arrays instead of collections of bytes.")]
		public byte[] RxSequenceAfter
		{
			get
			{
				AssertUndisposed();

				if (this.binaryRxState != null)
					return (this.binaryRxState.SequenceAfter);
				else
					return (null);
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

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

			using (var p = new Parser.Parser((EncodingEx)BinaryTerminalSettings.EncodingFixed, TerminalSettings.IO.Endianness, parseMode))
				return (p.TryParse(s, out result, out textSuccessfullyParsed, defaultRadix));
		}

		/// <summary>
		/// Tries to parse <paramref name="s"/>, taking the current settings into account.
		/// </summary>
		public override bool TryParse(string s, Radix defaultRadix, out byte[] result)
		{
			AssertUndisposed();

			using (var p = new Parser.Parser((EncodingEx)BinaryTerminalSettings.EncodingFixed, TerminalSettings.IO.Endianness, TerminalSettings.Send.Text.ToParseMode()))
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
			return (ByteHelper.FormatHexString(data, false)); // Format is fixed to "without radix", see remarks above.
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
		/// The Tx and/or Rx <see cref="Settings.BinaryDisplaySettings.SequenceLineBreakBefore"/> and
		/// <see cref="Settings.BinaryDisplaySettings.SequenceLineBreakAfter"/> have different values.
		/// </exception>
		public override void RemoveFraming(byte[] data)
		{
			if (ArrayEx.ValuesEqual(TxSequenceBefore, RxSequenceBefore) && ArrayEx.ValuesEqual(TxSequenceAfter, RxSequenceAfter))
				RemoveFraming(data, BinaryTerminalSettings.TxDisplay.SequenceLineBreakBefore.Enabled, TxSequenceBefore, BinaryTerminalSettings.TxDisplay.SequenceLineBreakAfter.Enabled, TxSequenceAfter); // Sequence is the same for both directions, direction doesn't matter.

			throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "This method requires that 'Tx' and 'Rx' sequences before/after are set to the same value!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
				case IODirection.Tx:    RemoveFraming(data, BinaryTerminalSettings.TxDisplay.SequenceLineBreakBefore.Enabled, TxSequenceBefore, BinaryTerminalSettings.TxDisplay.SequenceLineBreakAfter.Enabled, TxSequenceAfter); break;
				case IODirection.Rx:    RemoveFraming(data, BinaryTerminalSettings.RxDisplay.SequenceLineBreakBefore.Enabled, RxSequenceBefore, BinaryTerminalSettings.RxDisplay.SequenceLineBreakAfter.Enabled, RxSequenceAfter); break;

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("d", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("d", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary>
		/// Removes the framing from the given data.
		/// </summary>
		/// <remarks>
		/// For text terminals, framing is typically defined by EOL.
		/// For binary terminals, framing is optionally defined by sequence before/after.
		/// </remarks>
		protected virtual void RemoveFraming(byte[] data, bool sequenceBeforeIsEnabled, byte[] sequenceBefore, bool sequenceAfterIsEnabled, byte[] sequenceAfter)
		{
			if (sequenceBeforeIsEnabled || sequenceAfterIsEnabled)
			{
				var l = new List<byte>(data); // Create list once.
				var hasChanged = false;

				if (sequenceBeforeIsEnabled)
				{
					if (l.Take(sequenceBefore.Length).SequenceEqual(sequenceBefore))
					{
						l.RemoveRange(0, sequenceBefore.Length); // 1st list modification.
						hasChanged = true;
					}
				}

				if (sequenceAfterIsEnabled)
				{
					var skipCount = (l.Count - sequenceAfter.Length);
					if (l.Skip(skipCount).SequenceEqual(sequenceAfter))
					{
						l.RemoveRange(skipCount, sequenceAfter.Length); // 2nd list modification.
						hasChanged = true;
					}
				}

				if (hasChanged)
					data = l.ToArray(); // Recreate array
			}
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

		private void AttachBinaryTerminalSettings()
		{
			if (BinaryTerminalSettings != null)
				BinaryTerminalSettings.Changed += BinaryTerminalSettings_Changed;
		}

		private void DetachBinaryTerminalSettings()
		{
			if (BinaryTerminalSettings != null)
				BinaryTerminalSettings.Changed -= BinaryTerminalSettings_Changed;
		}

		private void ApplyBinaryTerminalSettings()
		{
			RefreshRepositories();
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the quasi-private member
		/// <see cref="BinaryTerminalSettings"/>.
		/// </remarks>
		protected Settings.BinaryDisplaySettings GetBinaryDisplaySettings(IODirection dir)
		{
			switch (dir)
			{
				case IODirection.Tx:    return (BinaryTerminalSettings.TxDisplay);
				case IODirection.Rx:    return (BinaryTerminalSettings.RxDisplay);

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region Settings Events
		//------------------------------------------------------------------------------------------
		// Settings Events
		//------------------------------------------------------------------------------------------

		private void BinaryTerminalSettings_Changed(object sender, MKY.Settings.SettingsEventArgs e)
		{
			ApplyBinaryTerminalSettings();
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

			return (indent + "> Type: BinaryTerminal" + Environment.NewLine + base.ToExtendedDiagnosticsString(indent));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
