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

		private DeviceAndDirectionLineState txDeviceAndDirectionLineState;
		private DeviceAndDirectionLineState bidirDeviceAndDirectionLineState;
		private DeviceAndDirectionLineState rxDeviceAndDirectionLineState;

		/// <summary>
		/// Synchronize processing (raw chunk => device|direction / raw chunk => bytes / raw chunk => chunk / timeout => line break)!
		/// </summary>
		protected object ProcessSyncObj { get; }  = new object();

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		#region ByteTo.../UnicodeValueTo.../...Element
		//------------------------------------------------------------------------------------------
		// ByteTo.../UnicodeValueTo.../...Element
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
		protected virtual void InitializeProcessStates()
		{
			this.txLineState    = new LineState();
			this.bidirLineState = new LineState();
			this.rxLineState    = new LineState();

			this.txDeviceAndDirectionLineState    = new DeviceAndDirectionLineState();
			this.bidirDeviceAndDirectionLineState = new DeviceAndDirectionLineState();
			this.rxDeviceAndDirectionLineState    = new DeviceAndDirectionLineState();
		}

		/// <summary></summary>
		protected virtual void ResetProcessStates(RepositoryType repositoryType)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    this.txDeviceAndDirectionLineState   .Reset(); break;
				case RepositoryType.Bidir: this.bidirDeviceAndDirectionLineState.Reset(); break;
				case RepositoryType.Rx:    this.rxDeviceAndDirectionLineState   .Reset(); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

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

		/// <summary></summary>
		protected abstract void ProcessRawByte(byte b, LineChunkAttribute attribute);

		/// <summary></summary>
		protected virtual void ProcessAndSignalRawChunk(RawChunk chunk, LineChunkAttribute attribute)
		{
			// Check whether device or direction has changed, a chunk is always tied to device and direction:
			ProcessAndSignalDeviceOrDirectionLineBreak(RepositoryType.Tx,    chunk.TimeStamp, chunk.Device, chunk.Direction);
			ProcessAndSignalDeviceOrDirectionLineBreak(RepositoryType.Bidir, chunk.TimeStamp, chunk.Device, chunk.Direction);
			ProcessAndSignalDeviceOrDirectionLineBreak(RepositoryType.Rx,    chunk.TimeStamp, chunk.Device, chunk.Direction);

			// Process chunk:
			foreach (byte b in chunk.Content)
			{
				ProcessRawByte(b, attribute);
			}

			// Enforce line break if requested:
			if (TerminalSettings.Display.ChunkLineBreakEnabled)
			{
				ProcessAndSignalChunkOrTimedLineBreak(RepositoryType.Tx,    chunk.TimeStamp, chunk.Device, chunk.Direction);
				ProcessAndSignalChunkOrTimedLineBreak(RepositoryType.Bidir, chunk.TimeStamp, chunk.Device, chunk.Direction);
				ProcessAndSignalChunkOrTimedLineBreak(RepositoryType.Rx,    chunk.TimeStamp, chunk.Device, chunk.Direction);
			}

			// Note that timed line breaks are processed asynchronously, always. Alternatively, the chunk
			// loop above could check for timeout on each byte. However, this is considered too inefficient.
		}

	#if (WITH_SCRIPTING)

		/// <remarks>
		/// Processing for scripting differs from "normal" processing for displaying because...
		/// ...received messages must not be impacted by 'DirectionLineBreak'.
		/// ...received data must not be processed individually, only as packets/messages.
		/// ...received data must not be reprocessed on <see cref="RefreshRepositories"/>.
		/// </remarks>
		protected virtual void ProcessAndSignalRawChunkForScripting(RawChunk chunk)
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
		protected virtual void ProcessAndSignalDeviceOrDirectionLineBreak(RepositoryType repositoryType, DateTime ts, string dev, IODirection dir)
		{
			DisplayElementCollection elementsToAdd;
			DisplayLineCollection linesToAdd;
			bool clearAlreadyStartedLine;

			DoDeviceOrDirectionLineBreak(repositoryType, ts, dev, dir, out elementsToAdd, out linesToAdd, out clearAlreadyStartedLine);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);

			if (clearAlreadyStartedLine)
				ClearCurrentDisplayLine(repositoryType);
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalChunkOrTimedLineBreak(RepositoryType repositoryType, DateTime ts, string dev, IODirection dir)
		{
			DisplayElementCollection elementsToAdd;
			DisplayLineCollection linesToAdd;
			bool clearAlreadyStartedLine;

			DoChunkOrTimedLineBreak(repositoryType, ts, dev, dir, out elementsToAdd, out linesToAdd, out clearAlreadyStartedLine);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);

			if (clearAlreadyStartedLine)
				ClearCurrentDisplayLine(repositoryType);
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1508:ClosingCurlyBracketsMustNotBePrecededByBlankLine", Justification = "Separating line for improved readability.")]
		protected virtual void DoDeviceOrDirectionLineBreak(RepositoryType repositoryType, DateTime ts, string dev, IODirection dir, out DisplayElementCollection elementsToAdd, out DisplayLineCollection linesToAdd, out bool clearAlreadyStartedLine)
		{
			lock (ProcessSyncObj) // Synchronize processing (raw chunk => device|direction / raw chunk => bytes / raw chunk => chunk / timeout => line break)!
			{
				DeviceAndDirectionLineState deviceOrDirectionlineState;

				switch (repositoryType)
				{
					case RepositoryType.Tx:    deviceOrDirectionlineState = this.txDeviceAndDirectionLineState;    break;
					case RepositoryType.Bidir: deviceOrDirectionlineState = this.bidirDeviceAndDirectionLineState; break;
					case RepositoryType.Rx:    deviceOrDirectionlineState = this.rxDeviceAndDirectionLineState;    break;

					case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				if (deviceOrDirectionlineState.IsFirstChunk)
				{
					deviceOrDirectionlineState.IsFirstChunk = false;
				}
				else // = 'IsSubsequentChunk'.
				{
					if (TerminalSettings.Display.DeviceLineBreakEnabled ||
					    TerminalSettings.Display.DirectionLineBreakEnabled)
					{
						if (!StringEx.EqualsOrdinalIgnoreCase(dev, deviceOrDirectionlineState.Device) || (dir != deviceOrDirectionlineState.Direction))
						{
							LineState lineState;

							if (dir == deviceOrDirectionlineState.Direction)
							{
								switch (dir)
								{
									case IODirection.Tx:   lineState = this.txLineState; break;
									case IODirection.Rx:   lineState = this.rxLineState; break;

									case IODirection.None: throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
									default:               throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
								}
							}
							else // Attention: Direction changed => Use other state.
							{
								switch (dir)
								{
									case IODirection.Tx: lineState = this.rxLineState; break; // Reversed!
									case IODirection.Rx: lineState = this.txLineState; break; // Reversed!

									case IODirection.None: throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
									default:               throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
								}
							}

							if ((lineState.Elements != null) && (lineState.Elements.Count > 0))
							{
								ExecuteLineEnd(lineState, ts, dev, out elementsToAdd, out linesToAdd, out clearAlreadyStartedLine);
							}
						} // a line break has been detected
					} // a line break is active
				} // is subsequent chunk

				deviceOrDirectionlineState.Device = dev;
				deviceOrDirectionlineState.Direction = dir;

			} // lock (processSyncObj)
		}

		/// <summary></summary>
		protected virtual void DoChunkOrTimedLineBreak(RepositoryType repositoryType, DateTime ts, string dev, IODirection dir, out DisplayElementCollection elementsToAdd, out DisplayLineCollection linesToAdd, out bool clearAlreadyStartedLine)
		{
			lock (ProcessSyncObj) // Synchronize processing (raw chunk => device|direction / raw chunk => bytes / raw chunk => chunk / timeout => line break)!
			{
				if (lineState.Elements.Count > 0)
					ExecuteLineEnd(lineState, ts, dev, elementsToAdd, linesToAdd, ref clearAlreadyStartedLine);
			}
		}

		#endregion

		#region Timer Events
		//------------------------------------------------------------------------------------------
		// Timer Events
		//------------------------------------------------------------------------------------------

		private void txTimedLineBreakTimeout_Elapsed(object sender, EventArgs e)
		{
			ProcessAndSignalChunkOrTimedLineBreak(RepositoryType.Tx,    DateTime.Now, IODirection.Tx);
			ProcessAndSignalChunkOrTimedLineBreak(RepositoryType.Bidir, DateTime.Now, IODirection.Tx);
		}   // Underlying DoChunkOrTimedLineBreak() will synchronize among this asyc callback and sync processing.

		private void rxTimedLineBreakTimeout_Elapsed(object sender, EventArgs e)
		{
			ProcessAndSignalChunkOrTimedLineBreak(RepositoryType.Bidir, DateTime.Now, IODirection.Rx);
			ProcessAndSignalChunkOrTimedLineBreak(RepositoryType.Rx,    DateTime.Now, IODirection.Rx);
		}   // Underlying DoChunkOrTimedLineBreak() will synchronize among this asyc callback and sync processing.

		#endregion

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
