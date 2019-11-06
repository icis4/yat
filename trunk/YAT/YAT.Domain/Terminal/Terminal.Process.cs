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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using MKY;
using MKY.Text;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\Terminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <remarks>
	/// This partial class implements the process part of <see cref="Terminal"/>.
	/// </remarks>
	public abstract partial class Terminal
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private LineState txLineState;
		private LineState bidirLineState;
		private LineState rxLineState;

		/// <summary>
		/// Synchronize processing (raw chunk | timed line break).
		/// </summary>
		protected object ChunkVsTimeoutSyncObj { get; } = new object();

		private LineBreakTimeout txLineBreakTimeout;
		private LineBreakTimeout rxLineBreakTimeout;

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region ByteTo.../...Element
		//------------------------------------------------------------------------------------------
		// ByteTo.../...Element
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement ByteToElement(byte b, IODirection d)
		{
			switch (d)
			{
				case IODirection.Tx: return (ByteToElement(b, d, TerminalSettings.Display.TxRadix));
				case IODirection.Rx: return (ByteToElement(b, d, TerminalSettings.Display.RxRadix));

				default: throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement ByteToElement(byte b, IODirection d, Radix r)
		{
			bool isControl;
			bool isByteToHide;
			bool isError;

			string text = ByteToText(b, d, r, out isControl, out isByteToHide, out isError);

			if      (isError)
			{
				return (new DisplayElement.ErrorInfo((Direction)d, text));
			}
			else if (isByteToHide)
			{
				return (new DisplayElement.Nonentity()); // Return nothing, ignore the character, this results in hiding.
			}
			else if (isControl)
			{
				if (TerminalSettings.CharReplace.ReplaceControlChars)
					return (CreateControlElement(b, d, text));
				else                         // !ReplaceControlChars => Use normal data element:
					return (CreateDataElement(b, d, text));
			}
			else // Neither 'isError' nor 'isByteToHide' nor 'isError' => Use normal data element:
			{
				return (CreateDataElement(b, d, text));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual string ByteToText(byte b, IODirection d, Radix r, out bool isControl, out bool isByteToHide, out bool isError)
		{
			isByteToHide = false;
			if      (b == 0x00)
			{
				if (TerminalSettings.CharHide.Hide0x00)
					isByteToHide = true;
			}
			else if (b == 0xFF)
			{
				if (TerminalSettings.SupportsHide0xFF && TerminalSettings.CharHide.Hide0xFF)
					isByteToHide = true;
			}
			else if (MKY.IO.Serial.XOnXOff.IsXOnOrXOffByte(b))
			{
				if (TerminalSettings.IO.FlowControlUsesXOnXOff && TerminalSettings.CharHide.HideXOnXOff)
					isByteToHide = true;
			}

			isControl = Ascii.IsControl(b);
			isError = false;

			switch (r)
			{
				case Radix.String:
				case Radix.Char:
				{
					if (isByteToHide)
					{
						return (null); // Return nothing, ignore the character, this results in hiding.
					}
					else if (isControl)
					{
						if (TerminalSettings.CharReplace.ReplaceControlChars)
							return (ByteToControlCharReplacementString(b, TerminalSettings.CharReplace.ControlCharRadix));
						else
							return (ByteToCharacterString(b));
					}
					else if (b == ' ') // Space.
					{
						if (TerminalSettings.CharReplace.ReplaceSpace)
							return (Settings.CharReplaceSettings.SpaceReplaceChar);
						else
							return (" ");
					}
					else
					{
						return (ByteToCharacterString(b));
					}
				}

				case Radix.Bin:
				case Radix.Oct:
				case Radix.Dec:
				case Radix.Hex:
				case Radix.Unicode:
				{
					if (isByteToHide)
					{
						return (null); // Return nothing, ignore the character, this results in hiding.
					}
					else if (isControl)
					{
						if (TerminalSettings.CharReplace.ReplaceControlChars)
							return (ByteToControlCharReplacementString(b, TerminalSettings.CharReplace.ControlCharRadix));
						else
							return (ByteToNumericRadixString(b, r)); // Current display radix.
					}
					else
					{
						return (ByteToNumericRadixString(b, r)); // Current display radix.
					}
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is a radix that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected virtual string ByteToCharacterString(byte b)
		{
			return (((char)b).ToString(CultureInfo.InvariantCulture));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected virtual string ByteToAsciiString(byte b)
		{
		////if      ((b == '\a') && !TerminalSettings.CharReplace.ReplaceBell) does not exist; CharAction.BeepOnBell exists but is handled elsewhere.
		////	return ("\a");
			if      ((b == '\b') && !TerminalSettings.CharReplace.ReplaceBackspace)
				return ("\b");
			else if ((b == '\t') && !TerminalSettings.CharReplace.ReplaceTab)
				return ("\t");
			else
				return ("<" + Ascii.ConvertToMnemonic(b) + ">");
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual string ByteToNumericRadixString(byte b, Radix r)
		{
			switch (r)
			{
				case Radix.Bin:
				{
					if (TerminalSettings.Display.ShowRadix)
						return (ByteEx.ConvertToBinaryString(b) + "b");
					else
						return (ByteEx.ConvertToBinaryString(b));
				}

				case Radix.Oct:
				{
					if (TerminalSettings.Display.ShowRadix)
						return (ByteEx.ConvertToOctalString(b) + "o");
					else
						return (ByteEx.ConvertToOctalString(b));
				}

				case Radix.Dec:
				{
					if (TerminalSettings.Display.ShowRadix)
						return (b.ToString("D3", CultureInfo.InvariantCulture) + "d");
					else
						return (b.ToString("D3", CultureInfo.InvariantCulture));
				}

				case Radix.Hex:
				{
					if (TerminalSettings.Display.ShowRadix)
						return (b.ToString("X2", CultureInfo.InvariantCulture) + "h");
					else
						return (b.ToString("X2", CultureInfo.InvariantCulture));
				}

				case Radix.Unicode:
				{
					return (UnicodeValueToNumericString(b));
				}

				default:
				{
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is a numeric radix that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <remarks>
		/// Note the limitation FR #329:
		/// Unicode is limited to the basic multilingual plane (U+0000..U+FFFF).
		/// </remarks>
		[CLSCompliant(false)]
		protected virtual string UnicodeValueToNumericString(ushort value)
		{
			if (TerminalSettings.Display.ShowRadix)
				return ("U+" + value.ToString("X4", CultureInfo.InvariantCulture));
			else
				return (       value.ToString("X4", CultureInfo.InvariantCulture));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual string ByteToControlCharReplacementString(byte b, ControlCharRadix r)
		{
			switch (r)
			{
				case ControlCharRadix.Char:
					return (ByteToCharacterString(b));

				case ControlCharRadix.Bin:
				case ControlCharRadix.Oct:
				case ControlCharRadix.Dec:
				case ControlCharRadix.Hex:
					return (ByteToNumericRadixString(b, (Radix)TerminalSettings.CharReplace.ControlCharRadix));

				case ControlCharRadix.AsciiMnemonic:
					return (ByteToAsciiString(b));

				default: // Includes 'String' and 'Unicode' which are not supported for control character replacement.
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is an ASCII control character radix that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte origin, IODirection d, string text)
		{
			switch (d)
			{
				case IODirection.Tx: return (new DisplayElement.TxData(origin, text));
				case IODirection.Rx: return (new DisplayElement.RxData(origin, text));

				default: throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte[] origin, IODirection d, string text)
		{
			switch (d)
			{
				case IODirection.Tx: return (new DisplayElement.TxData(origin, text));
				case IODirection.Rx: return (new DisplayElement.RxData(origin, text));

				default: throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateControlElement(byte origin, IODirection d, string text)
		{
			switch (d)
			{
				case IODirection.Tx: return (new DisplayElement.TxControl(origin, text));
				case IODirection.Rx: return (new DisplayElement.RxControl(origin, text));

				default: throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual bool ElementsAreSeparate(IODirection d)
		{
			switch (d)
			{
				case IODirection.Tx: return (ElementsAreSeparate(TerminalSettings.Display.TxRadix));
				case IODirection.Rx: return (ElementsAreSeparate(TerminalSettings.Display.RxRadix));

				default: throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual bool ElementsAreSeparate(Radix r)
		{
			switch (r)
			{
				case Radix.String:  return (false);
				case Radix.Char:    return (true);

				case Radix.Bin:     return (true);
				case Radix.Oct:     return (true);
				case Radix.Dec:     return (true);
				case Radix.Hex:     return (true);

				case Radix.Unicode: return (true);

				default: throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is a radix that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region Process Elements
		//------------------------------------------------------------------------------------------
		// Process Elements
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected virtual void InitializeProcess()
		{
			this.txLineState    = new LineState();
			this.bidirLineState = new LineState();
			this.rxLineState    = new LineState();

			if (this.txLineBreakTimeout != null)
			{	// Ensure to free referenced resources such as the 'Elapsed' event handler.
				this.txLineBreakTimeout.Elapsed -= txLineBreakTimeout_Elapsed;
				this.txLineBreakTimeout.Dispose();
			}

			this.txLineBreakTimeout = new LineBreakTimeout(TerminalSettings.TxDisplayTimedLineBreak.Timeout);
			this.txLineBreakTimeout.Elapsed += txLineBreakTimeout_Elapsed;

			if (this.rxLineBreakTimeout != null)
			{	// Ensure to free referenced resources such as the 'Elapsed' event handler.
				this.rxLineBreakTimeout.Elapsed -= rxLineBreakTimeout_Elapsed;
				this.rxLineBreakTimeout.Dispose();
			}

			this.rxLineBreakTimeout = new LineBreakTimeout(TerminalSettings.RxDisplayTimedLineBreak.Timeout);
			this.rxLineBreakTimeout.Elapsed += rxLineBreakTimeout_Elapsed;
		}

		/// <summary></summary>
		protected virtual void DisposeProcess()
		{
			// In the 'normal' case, timers are stopped in ExecuteLineEnd().

			if (this.txLineBreakTimeout != null)
			{	// Ensure to free referenced resources such as event handlers.
				EventHandlerHelper.RemoveAllEventHandlers(this.txLineBreakTimeout);
				this.txLineBreakTimeout.Dispose();
			}

			this.txLineBreakTimeout = null;

			if (this.rxLineBreakTimeout != null)
			{	// Ensure to free referenced resources such as event handlers.
				EventHandlerHelper.RemoveAllEventHandlers(this.rxLineBreakTimeout);
				this.rxLineBreakTimeout.Dispose();
			}

			this.rxLineBreakTimeout = null;
		}

		/// <summary></summary>
		protected virtual void ResetProcess(RepositoryType repositoryType)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    this.txLineState   .Reset(); break;
				case RepositoryType.Bidir: this.bidirLineState.Reset(); break;
				case RepositoryType.Rx:    this.rxLineState   .Reset(); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private members
		/// <see cref="txLineState"/>, <see cref="bidirLineState"/> and <see cref="rxLineState"/>.
		/// </remarks>
		protected LineState GetLineState(RepositoryType repositoryType)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    return (this.txLineState);
				case RepositoryType.Bidir: return (this.bidirLineState);
				case RepositoryType.Rx:    return (this.rxLineState);

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// This method must synchronize against <see cref="ChunkVsTimeoutSyncObj"/>!
		/// </remarks>
		protected virtual void ProcessRawChunk(RawChunk chunk, LineChunkAttribute attribute)
		{
			lock (ChunkVsTimeoutSyncObj) // Synchronize processing (raw chunk | timed line break).
			{
				LineBreakTimeout lbt = null;
				switch (chunk.Direction)
				{
					case IODirection.Tx: lbt = txLineBreakTimeout; break;
					case IODirection.Rx: lbt = rxLineBreakTimeout; break;
					default: throw ( // A chunk must ever be tied to Tx or Rx.
				}

				// Check whether device or direction has changed, a chunk is always tied to device and direction:
				{
					if (chunk.Direction == IODirection.Tx)
						EvaluateAndSignalLineBreak(RepositoryType.Tx,    chunk.TimeStamp, chunk.Device, chunk.Direction);

					if ((chunk.Direction == IODirection.Tx) || (chunk.Direction == IODirection.Rx))
						EvaluateAndSignalLineBreak(RepositoryType.Bidir, chunk.TimeStamp, chunk.Device, chunk.Direction);

					if (chunk.Direction == IODirection.Rx)
						EvaluateAndSignalLineBreak(RepositoryType.Rx,    chunk.TimeStamp, chunk.Device, chunk.Direction);
				}

				// Process chunk:
				foreach (byte b in chunk.Content)
				{
					// Handle start/restart of timed line breaks:
					if (displaySettings.TimedLineBreak.Enabled)
					{
						if (!IsReloading)
						{
							if (lineState.Position == LinePosition.Begin)
								lbt.Start();
							else
								lbt.Restart(); // Restart as timeout refers to time after last received byte.
						}
						else // In case of reloading, timed line breaks are synchronously evaluated here:
						{
							EvaluateTimedLineBreakOnReload(displaySettings, lineState, chunk.TimeStamp, chunk.Device, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);
						}
					}

					if (chunk.Direction == IODirection.Tx)
						ProcessRawByte(RepositoryType.Tx,    b, attribute);

					if ((chunk.Direction == IODirection.Tx) || (chunk.Direction == IODirection.Rx))
						ProcessRawByte(RepositoryType.Bidir, b, attribute);

					if (chunk.Direction == IODirection.Rx)
						ProcessRawByte(RepositoryType.Rx,    b, attribute);

					// Handle stop of timed line breaks:
					if (displaySettings.TimedLineBreak.Enabled)
					{
						if (!IsReloading)
						{
							if (lineState.Position == LinePosition.End)
								lbt.Stop();
						}
					}
				}

				// Enforce line break if requested:
				if (TerminalSettings.Display.ChunkLineBreakEnabled)
				{
					if (chunk.Direction == IODirection.Tx)
						EvaluateAndSignalChunkLineBreak(RepositoryType.Tx,    chunk.TimeStamp, chunk.Device, chunk.Direction);

					if ((chunk.Direction == IODirection.Tx) || (chunk.Direction == IODirection.Rx))
						EvaluateAndSignalChunkLineBreak(RepositoryType.Bidir, chunk.TimeStamp, chunk.Device, chunk.Direction);

					if (chunk.Direction == IODirection.Rx)
						EvaluateAndSignalChunkLineBreak(RepositoryType.Rx,    chunk.TimeStamp, chunk.Device, chunk.Direction);
				}

				// Note that timed line breaks are processed asynchronously, always. Alternatively, the chunk
				// loop above could check for timeout on each byte. However, this is considered too inefficient.
			}
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private members
		/// <see cref="txLineBreakTimeout"/> and <see cref="rxLineBreakTimeout"/>.
		/// </remarks>
		protected void ProcessRawByte(RepositoryType repositoryType, byte b, LineChunkAttribute attribute)
		{
			// Handle start/restart of timed line breaks:
			if (displaySettings.TimedLineBreak.Enabled)
			{
				if (!IsReloading)
				{
					if (lineState.Position == LinePosition.Begin)
						lineState.BreakTimeout.Start();
					else
						lineState.BreakTimeout.Restart(); // Restart as timeout refers to time after last received byte.
				}
				else // In case of reloading, timed line breaks are synchronously evaluated here:
				{
					EvaluateTimedLineBreakOnReload(displaySettings, lineState, chunk.TimeStamp, chunk.Device, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);
				}
			}

			ProcessRawByte(b, attribute);

			// Handle stop of timed line breaks:
			if (displaySettings.TimedLineBreak.Enabled)
			{
				if (!IsReloading)
				{
					if (lineState.Position == LinePosition.End)
						lineState.BreakTimeout.Stop();
				}
			}
		}

		/// <remarks>
		/// Must be abstract/virtual because settings differ among text and binary.
		/// </remarks>
		protected abstract void ProcessRawByte(byte b, LineChunkAttribute attribute);

	#if (WITH_SCRIPTING)

		/// <remarks>
		/// Processing for scripting differs from "normal" processing for displaying because...
		/// ...received messages must not be impacted by 'DirectionLineBreak'.
		/// ...received data must not be processed individually, only as packets/messages.
		/// ...received data must not be reprocessed on <see cref="RefreshRepositories"/>.
		/// </remarks>
		protected virtual void ProcessRawChunkForScripting(RawChunk chunk)
		{
			if (chunk.Direction == IODirection.Rx)
			{
				var data = new byte[chunk.Content.Count];
				chunk.Content.CopyTo(data, 0);

				var message = Format(data, IODirection.Rx);

				EnqueueReceivedMessageForScripting(message.ToString()); // Enqueue before invoking event to
				                                                        // have message ready for event.
				OnScriptPacketReceived(new PacketEventArgs(data));
				OnScriptMessageReceived(new MessageEventArgs(message.ToString()));
			}
		}

	#endif // WITH_SCRIPTING

		/// <summary></summary>
		protected virtual void EvaluateAndSignalLineBreak(RepositoryType repositoryType, DateTime ts, string dev, IODirection dir)
		{
			DisplayElementCollection elementsToAdd;
			DisplayLineCollection linesToAdd;
			bool clearAlreadyStartedLine;

			EvaluateDeviceOrDirectionLineBreak(repositoryType, ts, dev, dir, out elementsToAdd, out linesToAdd, out clearAlreadyStartedLine);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);

			if (clearAlreadyStartedLine)
				ClearCurrentDisplayLine(repositoryType);
		}

		/// <summary></summary>
		protected virtual void EvaluateAndSignalChunkLineBreak(RepositoryType repositoryType, DateTime ts, string dev, IODirection dir)
		{
			DisplayElementCollection elementsToAdd;
			DisplayLineCollection linesToAdd;
			bool clearAlreadyStartedLine;

			EvaluateChunkLineBreak(repositoryType, ts, dev, dir, out elementsToAdd, out linesToAdd, out clearAlreadyStartedLine);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);

			if (clearAlreadyStartedLine)
				ClearCurrentDisplayLine(repositoryType);
		}

		/// <summary></summary>
		protected virtual void EvaluateAndSignalTimedLineBreak(RepositoryType repositoryType, DateTime ts, IODirection dir)
		{
			DisplayElementCollection elementsToAdd;
			DisplayLineCollection linesToAdd;
			bool clearAlreadyStartedLine;

			EvaluateTimedLineBreak(repositoryType, ts, dir, out elementsToAdd, out linesToAdd, out clearAlreadyStartedLine);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);

			if (clearAlreadyStartedLine)
				ClearCurrentDisplayLine(repositoryType);
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1508:ClosingCurlyBracketsMustNotBePrecededByBlankLine", Justification = "Separating line for improved readability.")]
		protected virtual void EvaluateDeviceOrDirectionLineBreak(RepositoryType repositoryType, DateTime ts, string dev, IODirection dir, out DisplayElementCollection elementsToAdd, out DisplayLineCollection linesToAdd, out bool clearAlreadyStartedLine)
		{
			elementsToAdd = null;
			linesToAdd = null;
			clearAlreadyStartedLine = false;

			var lineState = GetLineState(repositoryType);
			if (lineState.IsFirstChunk)
			{
				lineState.IsFirstChunk = false;
			}
			else // = 'IsSubsequentChunk'.
			{
				if (TerminalSettings.Display.DeviceLineBreakEnabled ||
				    TerminalSettings.Display.DirectionLineBreakEnabled)
				{
					if (!StringEx.EqualsOrdinalIgnoreCase(dev, lineState.Device) || (dir != lineState.Direction))
					{
						if (lineState.Elements.Count > 0)
							DoLineEnd(repositoryType, ts, dev, out elementsToAdd, out linesToAdd, out clearAlreadyStartedLine);
					}
				}
			}

			lineState.Device = dev;
			lineState.Direction = dir;
		}

		/// <summary></summary>
		protected virtual void EvaluateChunkLineBreak(RepositoryType repositoryType, DateTime ts, string dev, IODirection dir, out DisplayElementCollection elementsToAdd, out DisplayLineCollection linesToAdd, out bool clearAlreadyStartedLine)
		{
			elementsToAdd = null;
			linesToAdd = null;
			clearAlreadyStartedLine = false;

			var lineState = GetLineState(repositoryType);
			if (lineState.Elements.Count > 0)
				DoLineEnd(repositoryType, ts, dev, out elementsToAdd, out linesToAdd, out clearAlreadyStartedLine);
		}

		/// <summary></summary>
		protected virtual void EvaluateTimedLineBreak(RepositoryType repositoryType, DateTime ts, IODirection dir, out DisplayElementCollection elementsToAdd, out DisplayLineCollection linesToAdd, out bool clearAlreadyStartedLine)
		{
			elementsToAdd = null;
			linesToAdd = null;
			clearAlreadyStartedLine = false;

			var lineState = GetLineState(repositoryType);
			if (lineState.Elements.Count > 0)
				DoLineEnd(repositoryType, ts, lineState.Device, out elementsToAdd, out linesToAdd, out clearAlreadyStartedLine);
		}

		/// <remarks>Must be abstract/virtual because settings differ among text and binary.</remarks>
		protected abstract void DoLineEnd(RepositoryType repositoryType, DateTime ts, string dev, out DisplayElementCollection elementsToAdd, out DisplayLineCollection linesToAdd, out bool clearAlreadyStartedLine);

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual void PrepareLineBeginInfo(DateTime ts, TimeSpan diff, TimeSpan delta, string dev, IODirection dir, out DisplayElementCollection lp)
		{
			if (TerminalSettings.Display.ShowTimeStamp || TerminalSettings.Display.ShowTimeSpan || TerminalSettings.Display.ShowTimeDelta ||
			    TerminalSettings.Display.ShowDevice    ||
			    TerminalSettings.Display.ShowDirection)
			{
				lp = new DisplayElementCollection(10); // Preset the required capacity to improve memory management.

				if (TerminalSettings.Display.ShowTimeStamp)
				{
					lp.Add(new DisplayElement.TimeStampInfo(ts, TerminalSettings.Display.TimeStampFormat, TerminalSettings.Display.TimeStampUseUtc, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!

					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
				}

				if (TerminalSettings.Display.ShowTimeSpan)
				{
					lp.Add(new DisplayElement.TimeSpanInfo(diff, TerminalSettings.Display.TimeSpanFormat, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!

					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
				}

				if (TerminalSettings.Display.ShowTimeDelta)
				{
					lp.Add(new DisplayElement.TimeDeltaInfo(delta, TerminalSettings.Display.TimeDeltaFormat, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!

					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
				}

				if (TerminalSettings.Display.ShowDevice)
				{
					lp.Add(new DisplayElement.DeviceInfo(dev, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!

					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
				}

				if (TerminalSettings.Display.ShowDirection)
				{
					lp.Add(new DisplayElement.DirectionInfo((Direction)dir, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache));

					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
				}
			}
			else
			{
				lp = null;
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "Why not? 'Byte' not only is a type, but also emphasizes a purpose.")]
		protected virtual void PrepareLineEndInfo(int length, TimeSpan duration, out DisplayElementCollection lp)
		{
			if (TerminalSettings.Display.ShowLength || TerminalSettings.Display.ShowDuration) // Meaning: "byte count"/"char count" and "line duration".
			{
				lp = new DisplayElementCollection(4); // Preset the required capacity to improve memory management.

				if (TerminalSettings.Display.ShowLength)
				{
					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));

					lp.Add(new DisplayElement.DataLength(length, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may be both!
				}

				if (TerminalSettings.Display.ShowDuration)
				{
					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));

					lp.Add(new DisplayElement.TimeDurationInfo(duration, TerminalSettings.Display.TimeDurationFormat, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may be both!
				}
			}
			else
			{
				lp = null;
			}
		}

		#endregion

		#region Timer Events
		//------------------------------------------------------------------------------------------
		// Timer Events
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// This event handler must synchronize against <see cref="ChunkVsTimeoutSyncObj"/>!
		/// </remarks>
		private void txLineBreakTimeout_Elapsed(object sender, EventArgs e)
		{
			lock (ChunkVsTimeoutSyncObj) // Synchronize processing (raw chunk | timed line break).
			{
				EvaluateAndSignalTimedLineBreak(RepositoryType.Tx,    DateTime.Now, IODirection.Tx);
				EvaluateAndSignalTimedLineBreak(RepositoryType.Bidir, DateTime.Now, IODirection.Tx);
			}
		}

		/// <remarks>
		/// This event handler must synchronize against <see cref="ChunkVsTimeoutSyncObj"/>!
		/// </remarks>
		private void rxLineBreakTimeout_Elapsed(object sender, EventArgs e)
		{
			lock (ChunkVsTimeoutSyncObj) // Synchronize processing (raw chunk | timed line break).
			{
				EvaluateAndSignalTimedLineBreak(RepositoryType.Bidir, DateTime.Now, IODirection.Rx);
				EvaluateAndSignalTimedLineBreak(RepositoryType.Rx,    DateTime.Now, IODirection.Rx);
			}
		}

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
