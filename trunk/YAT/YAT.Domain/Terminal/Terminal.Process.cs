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

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of line break:
////#define DEBUG_LINE_BREAK

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics;
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

		private ProcessState txProcessState;
		private ProcessState bidirProcessState;
		private ProcessState rxProcessState;

		/// <summary>
		/// Synchronize processing (raw chunk | timed line break).
		/// </summary>
		private object chunkVsTimeoutSyncObj = new object();

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
		protected virtual DisplayElement ByteToElement(byte b, DateTime ts, IODirection d)
		{
			switch (d)
			{
				case IODirection.Tx:   return (ByteToElement(b, ts, d, TerminalSettings.Display.TxRadix));
				case IODirection.Rx:   return (ByteToElement(b, ts, d, TerminalSettings.Display.RxRadix));

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement ByteToElement(byte b, DateTime ts, IODirection d, Radix r)
		{
			bool isControl;
			bool isByteToHide;
			bool isError;

			string text = ByteToText(b, ts, d, r, out isControl, out isByteToHide, out isError);

			if      (isError)
			{
				return (new DisplayElement.ErrorInfo(ts, (Direction)d, text));
			}
			else if (isByteToHide)
			{
				return (new DisplayElement.Nonentity()); // Return nothing, ignore the character, this results in hiding.
			}
			else if (isControl)
			{
				if (TerminalSettings.CharReplace.ReplaceControlChars)
					return (CreateControlElement(b, ts, d, text));
				else                         // !ReplaceControlChars => Use normal data element:
					return (CreateDataElement(b, ts, d, text));
			}
			else // Neither 'isError' nor 'isByteToHide' nor 'isError' => Use normal data element:
			{
				return (CreateDataElement(b, ts, d, text));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual string ByteToText(byte b, DateTime ts, IODirection d, Radix r, out bool isControl, out bool isByteToHide, out bool isError)
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
		protected virtual DisplayElement CreateDataElement(byte origin, DateTime ts, IODirection d, string text)
		{
			switch (d)
			{
				case IODirection.Tx:    return (new DisplayElement.TxData(ts, origin, text));
				case IODirection.Rx:    return (new DisplayElement.RxData(ts, origin, text));

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateDataElement(byte[] origin, DateTime ts, IODirection d, string text)
		{
			switch (d)
			{
				case IODirection.Tx:    return (new DisplayElement.TxData(ts, origin, text));
				case IODirection.Rx:    return (new DisplayElement.RxData(ts, origin, text));

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual DisplayElement CreateControlElement(byte origin, DateTime ts, IODirection d, string text)
		{
			switch (d)
			{
				case IODirection.Tx:   return (new DisplayElement.TxControl(ts, origin, text));
				case IODirection.Rx:   return (new DisplayElement.RxControl(ts, origin, text));

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual bool ElementsAreSeparate(IODirection d)
		{
			switch (d)
			{
				case IODirection.None:  return (false); // This can be the case e.g. when the line direction has not yet been determined.

				case IODirection.Tx:    return (ElementsAreSeparate(TerminalSettings.Display.TxRadix) /* Pragmatic best-effort approach. */                   );
				case IODirection.Bidir: return (ElementsAreSeparate(TerminalSettings.Display.TxRadix) || ElementsAreSeparate(TerminalSettings.Display.RxRadix));
				case IODirection.Rx:    return (                                                         ElementsAreSeparate(TerminalSettings.Display.RxRadix));

				default:                throw (new ArgumentOutOfRangeException("d", d, MessageHelper.InvalidExecutionPreamble + "'" + d + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

		/// <remarks>This default implementation is based on <see cref="DisplayElementCollection.ByteCount"/>.</remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual void AddSpaceIfNecessary(LineState lineState, IODirection d, DisplayElementCollection lp, DisplayElement de)
		{
			if (ElementsAreSeparate(d) && !string.IsNullOrEmpty(de.Text))
			{
				if ((lineState.Elements.ByteCount > 0) || (lp.ByteCount > 0))
					lp.Add(new DisplayElement.ContentSpace((Direction)d));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "d", Justification = "Short and compact for improved readability.")]
		protected virtual void RemoveSpaceIfNecessary(IODirection d, DisplayElementCollection lp)
		{
			if (ElementsAreSeparate(d))
			{
				int count = lp.Count;
				if ((count > 0) && (lp[count - 1] is DisplayElement.ContentSpace))
					lp.RemoveLast();
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

					lp.Add(new DisplayElement.DataLength(length, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache));
				}

				if (TerminalSettings.Display.ShowDuration)
				{
					if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
						lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));

					lp.Add(new DisplayElement.TimeDurationInfo(duration, TerminalSettings.Display.TimeDurationFormat, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache));
				}
			}
			else
			{
				lp = null;
			}
		}

		#endregion

		#region Process Elements
		//------------------------------------------------------------------------------------------
		// Process Elements
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Initializes the processing state.
		/// </summary>
		protected virtual void InitializeProcess()
		{
			this.txProcessState    = new ProcessState();
			this.bidirProcessState = new ProcessState();
			this.rxProcessState    = new ProcessState();

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

		/// <summary>
		/// Disposes the processing state.
		/// </summary>
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

		/// <summary>
		/// Resets the processing state for the given <paramref name="repositoryType"/>.
		/// </summary>
		protected virtual void ResetProcess(RepositoryType repositoryType)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    this.txProcessState   .Reset(); break;
				case RepositoryType.Bidir: this.bidirProcessState.Reset(); break;
				case RepositoryType.Rx:    this.rxProcessState   .Reset(); break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private members
		/// <see cref="txProcessState"/>, <see cref="bidirProcessState"/> and <see cref="rxProcessState"/>.
		/// </remarks>
		protected ProcessState GetProcessState(RepositoryType repositoryType)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    return (this.txProcessState);
				case RepositoryType.Bidir: return (this.bidirProcessState);
				case RepositoryType.Rx:    return (this.rxProcessState);

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// This method shall not be overridden, same as <see cref="GetProcessState"/>.
		/// </remarks>
		protected OverallState GetOverallState(RepositoryType repositoryType)
		{
			return (GetProcessState(repositoryType).Overall);
		}

		/// <remarks>
		/// This method shall not be overridden, same as <see cref="GetProcessState"/>.
		/// </remarks>
		protected LineState GetLineState(RepositoryType repositoryType)
		{
			return (GetProcessState(repositoryType).Line);
		}

		/// <remarks>
		/// This method shall not be overridden as it accesses the private members
		/// <see cref="txProcessState"/> and <see cref="rxProcessState"/>.
		/// </remarks>
		protected LineState GetUnidirLineState(IODirection dir)
		{
			switch (dir)
			{
				case IODirection.Tx:    return (txProcessState.Line);
				case IODirection.Rx:    return (rxProcessState.Line);

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// This method is private as it must synchronize against private <see cref="chunkVsTimeoutSyncObj"/>!
		/// </remarks>
		private void ProcessRawChunk(RawChunk chunk)
		{
			lock (this.chunkVsTimeoutSyncObj) // Synchronize processing (raw chunk | timed line break).
			{
				bool txIsAffected    =  (chunk.Direction == IODirection.Tx);
				bool bidirIsAffected = ((chunk.Direction == IODirection.Tx) ||(chunk.Direction == IODirection.Rx));
				bool rxIsAffected    =                                        (chunk.Direction == IODirection.Rx);

				ProcessAndSignalDeviceOrDirectionLineBreak(chunk, txIsAffected, bidirIsAffected, rxIsAffected);
				ProcessAndSignalRawChunk(                  chunk, txIsAffected, bidirIsAffected, rxIsAffected);
				ProcessAndSignalChunkLineBreak(            chunk, txIsAffected, bidirIsAffected, rxIsAffected);
			}
		}

		/// <summary>Check whether device or direction has changed.</summary>
		/// <remarks>A chunk is always tied to device and direction.</remarks>
		protected virtual void ProcessAndSignalDeviceOrDirectionLineBreak(RawChunk chunk,
		                                                                  bool txIsAffected, bool bidirIsAffected, bool rxIsAffected)
		{
			var isServerSocket = TerminalSettings.IO.IOTypeIsServerSocket;
			if (isServerSocket && TerminalSettings.Display.DeviceLineBreakEnabled) // Attention: This 'isServerSocket' restriction is also implemented at other locations!
			{
				if (txIsAffected)    { EvaluateAndSignalDeviceLineBreak(RepositoryType.Tx,    chunk.TimeStamp, chunk.Device); }
				if (bidirIsAffected) { EvaluateAndSignalDeviceLineBreak(RepositoryType.Bidir, chunk.TimeStamp, chunk.Device); }
				if (rxIsAffected)    { EvaluateAndSignalDeviceLineBreak(RepositoryType.Rx,    chunk.TimeStamp, chunk.Device); }
			}

			if (TerminalSettings.Display.DirectionLineBreakEnabled)
			{ // Must not be done for unidirectional repositories.
			////if (txIsAffected)    { EvaluateAndSignalDirectionLineBreak(RepositoryType.Tx,    chunk.TimeStamp, chunk.Direction); }
				if (bidirIsAffected) { EvaluateAndSignalDirectionLineBreak(RepositoryType.Bidir, chunk.TimeStamp, chunk.Direction); }
			////if (rxIsAffected)    { EvaluateAndSignalDirectionLineBreak(RepositoryType.Rx,    chunk.TimeStamp, chunk.Direction); }
			} // Must not be done for unidirectional repositories.
		}

		/// <summary>Enforce line break if requested.</summary>
		protected virtual void ProcessAndSignalChunkLineBreak(RawChunk chunk,
		                                                      bool txIsAffected, bool bidirIsAffected, bool rxIsAffected)
		{
			if (TerminalSettings.TxDisplayChunkLineBreakEnabled)
			{
				if (txIsAffected)                                           { EvaluateAndSignalChunkLineBreak(RepositoryType.Tx,    chunk.TimeStamp); }
				if (bidirIsAffected && (chunk.Direction == IODirection.Tx)) { EvaluateAndSignalChunkLineBreak(RepositoryType.Bidir, chunk.TimeStamp); }
			}

			if (TerminalSettings.RxDisplayChunkLineBreakEnabled)
			{
				if (bidirIsAffected && (chunk.Direction == IODirection.Rx)) { EvaluateAndSignalChunkLineBreak(RepositoryType.Bidir, chunk.TimeStamp); }
				if (rxIsAffected)                                           { EvaluateAndSignalChunkLineBreak(RepositoryType.Rx,    chunk.TimeStamp); }
			}
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalRawChunk(RawChunk chunk,
		                                                bool txIsAffected, bool bidirIsAffected, bool rxIsAffected)
		{
			TimeoutSettingTuple timedLineBreak;
			LineBreakTimeout lineBreakTimeout;
			switch (chunk.Direction)
			{
				case IODirection.Tx: timedLineBreak = TerminalSettings.TxDisplayTimedLineBreak; lineBreakTimeout = this.txLineBreakTimeout; break;
				case IODirection.Rx: timedLineBreak = TerminalSettings.RxDisplayTimedLineBreak; lineBreakTimeout = this.rxLineBreakTimeout; break;

				default: throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A raw chunk must always be tied to Tx or Rx!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			if (txIsAffected)    { ProcessDirection(RepositoryType.Tx,    chunk.Direction, TerminalSettings.Display.ShowDirection); }
			if (bidirIsAffected) { ProcessDirection(RepositoryType.Bidir, chunk.Direction, TerminalSettings.Display.ShowDirection); }
			if (rxIsAffected)    { ProcessDirection(RepositoryType.Rx,    chunk.Direction, TerminalSettings.Display.ShowDirection); }

			// Notes:
			//  > Processing is done sequentially for all repositories, in order to get synchronized
			//    content for Tx/Bidir and Bidir/Rx.
			//  > Signaling is only done once per chunk (unless flushing is involved), in order to
			//    improve performance (by reducing the number of events and repository updates).
			//  > Timed line breaks are processed asynchronously, except on reload.
			//    Alternatively, the chunk loop could check for timeout on each byte.
			//    However, this is considered too inefficient.

			DisplayElementCollection txElementsToAdd    = null;
			DisplayElementCollection bidirElementsToAdd = null;
			DisplayElementCollection rxElementsToAdd    = null;

			DisplayLineCollection txLinesToAdd    = null;
			DisplayLineCollection bidirLinesToAdd = null;
			DisplayLineCollection rxLinesToAdd    = null;

			if (txIsAffected)
			{
				txElementsToAdd = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.
				txLinesToAdd    = new DisplayLineCollection();    // No preset needed, the default initial capacity is good enough.
			}

			if (bidirIsAffected)
			{
				bidirElementsToAdd = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.
				bidirLinesToAdd    = new DisplayLineCollection();    // No preset needed, the default initial capacity is good enough.
			}

			if (rxIsAffected)
			{
				rxElementsToAdd = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.
				rxLinesToAdd    = new DisplayLineCollection();    // No preset needed, the default initial capacity is good enough.
			}

			foreach (byte b in chunk.Content)
			{
				DoRawBytePre(chunk.TimeStamp, chunk.Device, chunk.Direction, timedLineBreak, lineBreakTimeout);

				if (IsReloading) // In case of reloading, timed line breaks are synchronously evaluated here:
				{
					if (TerminalSettings.TxDisplayTimedLineBreak.Enabled)
					{
						if (txIsAffected)                                           { EvaluateTimedLineBreakOnReload(RepositoryType.Tx,    chunk.TimeStamp, timedLineBreak.Timeout, txElementsToAdd,    txLinesToAdd);    }
						if (bidirIsAffected && (chunk.Direction == IODirection.Tx)) { EvaluateTimedLineBreakOnReload(RepositoryType.Bidir, chunk.TimeStamp, timedLineBreak.Timeout, bidirElementsToAdd, bidirLinesToAdd); }
					}

					if (TerminalSettings.RxDisplayTimedLineBreak.Enabled)
					{
						if (bidirIsAffected && (chunk.Direction == IODirection.Rx)) { EvaluateTimedLineBreakOnReload(RepositoryType.Bidir, chunk.TimeStamp, timedLineBreak.Timeout, bidirElementsToAdd, bidirLinesToAdd); }
						if (rxIsAffected)                                           { EvaluateTimedLineBreakOnReload(RepositoryType.Rx,    chunk.TimeStamp, timedLineBreak.Timeout, rxElementsToAdd,    rxLinesToAdd);    }
					}
				}

				if (txIsAffected)    { DoRawByte(RepositoryType.Tx,    b, chunk.TimeStamp, chunk.Device, chunk.Direction, txElementsToAdd,    txLinesToAdd);    }
				if (bidirIsAffected) { DoRawByte(RepositoryType.Bidir, b, chunk.TimeStamp, chunk.Device, chunk.Direction, bidirElementsToAdd, bidirLinesToAdd); }
				if (rxIsAffected)    { DoRawByte(RepositoryType.Rx,    b, chunk.TimeStamp, chunk.Device, chunk.Direction, rxElementsToAdd,    rxLinesToAdd);    }

				DoRawBytePost(chunk.TimeStamp, chunk.Device, chunk.Direction, timedLineBreak, lineBreakTimeout);
			}

			if (txIsAffected)
			{
				if ((txElementsToAdd != null) && (txElementsToAdd.Count > 0))
					AddDisplayElements(RepositoryType.Tx, txElementsToAdd);

				if ((txLinesToAdd != null) && (txLinesToAdd.Count > 0))
					AddDisplayLines(RepositoryType.Tx, txLinesToAdd);
			}

			if (bidirIsAffected)
			{
				if ((bidirElementsToAdd != null) && (bidirElementsToAdd.Count > 0))
					AddDisplayElements(RepositoryType.Bidir, bidirElementsToAdd);

				if ((bidirLinesToAdd != null) && (bidirLinesToAdd.Count > 0))
					AddDisplayLines(RepositoryType.Bidir, bidirLinesToAdd);
			}

			if (rxIsAffected)
			{
				if ((rxElementsToAdd != null) && (rxElementsToAdd.Count > 0))
					AddDisplayElements(RepositoryType.Rx, rxElementsToAdd);

				if ((rxLinesToAdd != null) && (rxLinesToAdd.Count > 0))
					AddDisplayLines(RepositoryType.Rx, rxLinesToAdd);
			}
		}

		/// <summary></summary>
		protected virtual void ProcessDirection(RepositoryType repositoryType, IODirection dir, bool showDirection)
		{
			var processState = GetProcessState(repositoryType);
			if (processState.Line.Direction != IODirection.None) // IODirection.None means that line processing has not started yet.
			{
				if (processState.Line.Direction != dir)
				{
					processState.Line.Direction = IODirection.Bidir;

					if (showDirection) // Replace is only needed when containing a 'DisplayElement.DirectionInfo'.
					{
						foreach (var element in processState.Line.Elements)
						{
							var casted = (element as DisplayElement.DirectionInfo);
							if (casted != null)
								casted.ReplaceDirection(Direction.Bidir, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache);
						}

						FlushReplaceAlreadyStartedLine(repositoryType, processState);
					}
				}
			}
		}

		/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
		protected virtual void EvaluateAndSignalDeviceLineBreak(RepositoryType repositoryType, DateTime ts, string dev)
		{
			var elementsToAdd = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.
			var linesToAdd    = new DisplayLineCollection();    // No preset needed, the default initial capacity is good enough.

			EvaluateDeviceLineBreak(repositoryType, ts, dev, elementsToAdd, linesToAdd);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);
		}

		/// <summary></summary>
		protected virtual void EvaluateAndSignalDirectionLineBreak(RepositoryType repositoryType, DateTime ts, IODirection dir)
		{
			var elementsToAdd = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.
			var linesToAdd    = new DisplayLineCollection();    // No preset needed, the default initial capacity is good enough.

			EvaluateDirectionLineBreak(repositoryType, ts, dir, elementsToAdd, linesToAdd);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);
		}

		/// <summary></summary>
		protected virtual void EvaluateAndSignalChunkLineBreak(RepositoryType repositoryType, DateTime ts)
		{
			var elementsToAdd = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.
			var linesToAdd    = new DisplayLineCollection();    // No preset needed, the default initial capacity is good enough.

			EvaluateChunkLineBreak(repositoryType, ts, elementsToAdd, linesToAdd);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);
		}

		/// <summary></summary>
		protected virtual void EvaluateAndSignalTimedLineBreak(RepositoryType repositoryType, DateTime ts)
		{
			var elementsToAdd = new DisplayElementCollection(); // No preset needed, the default initial capacity is good enough.
			var linesToAdd    = new DisplayLineCollection();    // No preset needed, the default initial capacity is good enough.

			EvaluateTimedLineBreak(repositoryType, ts, elementsToAdd, linesToAdd);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);
		}

		/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
		[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1508:ClosingCurlyBracketsMustNotBePrecededByBlankLine", Justification = "Separating line for improved readability.")]
		protected virtual void EvaluateDeviceLineBreak(RepositoryType repositoryType, DateTime ts, string dev,
		                                               DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			var processState = GetProcessState(repositoryType);
			if (processState.Overall.DeviceLineBreak.IsFirstChunk)         // Not the ideal location to handle this flag and 'DeviceLineBreak.Device' further below.
			{                                                              // But good enough because not needed anywhere else and more performant if only done here.
				processState.Overall.DeviceLineBreak.IsFirstChunk = false; // Mitigated by using a dedicated 'DeviceLineBreak' sub-item making the scope obvious.
			}
			else // = 'IsSubsequentChunk'
			{                                                                   // See above!
				if (!StringEx.EqualsOrdinalIgnoreCase(dev, processState.Overall.DeviceLineBreak.Device))
				{
					if (processState.Line.Elements.Count > 0)
					{
						DebugLineBreak(repositoryType, "EvaluateDeviceLineBreak => DoLineEnd()");

						DoLineEnd(repositoryType, processState, ts, elementsToAdd, linesToAdd);
					}
				}
			}
			                     //// See above!
			processState.Overall.DeviceLineBreak.Device = dev;
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1508:ClosingCurlyBracketsMustNotBePrecededByBlankLine", Justification = "Separating line for improved readability.")]
		protected virtual void EvaluateDirectionLineBreak(RepositoryType repositoryType, DateTime ts, IODirection dir,
		                                                  DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			var processState = GetProcessState(repositoryType);
			if (processState.Overall.DirectionLineBreak.IsFirstChunk)         // Not the ideal location to handle this flag and 'DirectionLineBreak.Direction' further below.
			{                                                                 // But good enough because not needed anywhere else and more performant if only done here.
				processState.Overall.DirectionLineBreak.IsFirstChunk = false; // Mitigated by using a dedicated 'DirectionLineBreak' sub-item making the scope obvious.
			}
			else // = 'IsSubsequentChunk'
			{                                   // See above!
				if (dir != processState.Overall.DirectionLineBreak.Direction)
				{
					if (processState.Line.Elements.Count > 0)
					{
						DebugLineBreak(repositoryType, "EvaluateDirectionLineBreak => DoLineEnd()");

						DoLineEnd(repositoryType, processState, ts, elementsToAdd, linesToAdd);
					}
				}
			}
			                     //// See above!
			processState.Overall.DirectionLineBreak.Direction = dir;
		}

		/// <summary></summary>
		protected virtual void EvaluateChunkLineBreak(RepositoryType repositoryType, DateTime ts,
		                                              DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			var processState = GetProcessState(repositoryType);
			if (processState.Line.Elements.Count > 0)
			{
				DebugLineBreak(repositoryType, "EvaluateChunkLineBreak => DoLineEnd()");

				DoLineEnd(repositoryType, processState, ts, elementsToAdd, linesToAdd);
			}
		}

		/// <summary></summary>
		protected virtual void EvaluateTimedLineBreak(RepositoryType repositoryType, DateTime ts,
		                                              DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			var processState = GetProcessState(repositoryType);
			if (processState.Line.Elements.Count > 0)
			{
				DebugLineBreak(repositoryType, "EvaluateTimedLineBreak => DoLineEnd()");

				DoLineEnd(repositoryType, processState, ts, elementsToAdd, linesToAdd);
			}
		}

		/// <summary></summary>
		protected virtual void EvaluateTimedLineBreakOnReload(RepositoryType repositoryType, DateTime ts, int timeout,
		                                                      DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			var processState = GetProcessState(repositoryType);
			if (processState.Line.Elements.Count > 0)
			{
				var span = (ts - processState.Line.TimeStamp);
				if (span.TotalMilliseconds >= timeout)
				{
					DebugLineBreak(repositoryType, "EvaluateTimedLineBreakOnReload => DoLineEnd()");

					DoLineEnd(repositoryType, processState, ts, elementsToAdd, linesToAdd);
				}
			}
		}

		/// <remarks>
		/// Must be abstract/virtual because settings and behavior differ among text and binary.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected abstract void DoRawByte(RepositoryType repositoryType,
		                                  byte b, DateTime ts, string dev, IODirection dir,
		                                  DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd);

		/// <summary>
		/// Optional pre-processing before call of <see cref="DoRawByte"/>.
		/// </summary>
		/// <remarks>
		/// <paramref name="ts"/> and <paramref name="dev"/> are prepared for specialization
		/// by <see cref="BinaryTerminal"/> and <see cref="TextTerminal"/>.
		/// </remarks>
		protected virtual void DoRawBytePre(DateTime ts, string dev, IODirection dir,
		                                    TimeoutSettingTuple timedLineBreak, LineBreakTimeout lineBreakTimeout)
		{
			// Handle start/restart of timed line breaks:
			if (timedLineBreak.Enabled)
			{
				if (!IsReloading)
				{
					var lineState = GetUnidirLineState(dir); // Just checking for Tx or Rx is sufficient.
					if (lineState.Position == LinePosition.Begin)
						lineBreakTimeout.Start();
					else
						lineBreakTimeout.Restart(); // Restart as timeout refers to time after last received byte.
				}
			}
		}

		/// <summary>
		/// Optional pre-processing before call of <see cref="DoRawByte"/>.
		/// </summary>
		/// <remarks>
		/// <paramref name="ts"/> and <paramref name="dev"/> are prepared for specialization
		/// by <see cref="BinaryTerminal"/> and <see cref="TextTerminal"/>.
		/// </remarks>
		protected virtual void DoRawBytePost(DateTime ts, string dev, IODirection dir,
		                                     TimeoutSettingTuple timedLineBreak, LineBreakTimeout lineBreakTimeout)
		{
			// Handle stop of timed line breaks:
			if (timedLineBreak.Enabled)
			{
				if (!IsReloading)
				{
					var lineState = GetUnidirLineState(dir); // Just checking for Tx or Rx is sufficient.
					if (lineState.Position == LinePosition.End)
						lineBreakTimeout.Stop();
				}
			}
		}

		/// <remarks>
		/// <paramref name="repositoryType"/> and <paramref name="elementsToAdd"/> are required
		/// for specialization by <see cref="BinaryTerminal"/> and <see cref="TextTerminal"/>.
		/// </remarks>
		protected virtual void DoLineBegin(RepositoryType repositoryType, ProcessState processState,
		                                   DateTime ts, string dev, IODirection dir,
		                                   DisplayElementCollection elementsToAdd)
		{
			DebugLineBreak(repositoryType, string.Format("DoLineBegin() => NotifyLineBegin({0}, {1}, {2})", ts, dev, dir));

			processState.NotifyLineBegin(ts, dev, dir);
		}

		/// <remarks>
		/// <paramref name="repositoryType"/>, <paramref name="ts"/>,
		/// <paramref name="elementsToAdd"/> and <paramref name="linesToAdd"/> are required
		/// for specialization by <see cref="BinaryTerminal"/> and <see cref="TextTerminal"/>.
		/// </remarks>
		protected virtual void DoLineEnd(RepositoryType repositoryType, ProcessState processState,
		                                 DateTime ts,
		                                 DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			DebugLineBreak(repositoryType, "DoLineEnd() => NotifyLineEnd()");

			processState.NotifyLineEnd();
		}

		/// <remarks>Named 'Flush' to emphasize pending elements and lines are signaled and cleared.</remarks>
		protected virtual void Flush(RepositoryType repositoryType,
		                             DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			if ((elementsToAdd != null) && (elementsToAdd.Count > 0))
			{
				AddDisplayElements(repositoryType, elementsToAdd);
				elementsToAdd.Clear();
			}

			if ((linesToAdd != null) && (linesToAdd.Count > 0))
			{
				AddDisplayLines(repositoryType, linesToAdd);
				linesToAdd.Clear();
			}
		}

		/// <remarks>Named 'Flush' to emphasize pending elements and lines are signaled and cleared.</remarks>
		protected virtual void FlushReplaceAlreadyStartedLine(RepositoryType repositoryType, ProcessState processState)
		{
		////if ((elementsToAdd != null) && (elementsToAdd.Count > 0)) is not needed (yet).
		////{
		////	AddDisplayElements(repositoryType, elementsToAdd);
		////	elementsToAdd.Clear();
		////}

		////if ((linesToAdd != null) && (linesToAdd.Count > 0)) is not needed (yet).
		////{
		////	AddDisplayLines(repositoryType, linesToAdd);
		////	linesToAdd.Clear();
		////}

			ReplaceCurrentDisplayLine(repositoryType, processState.Line.Elements);
		}

		/// <remarks>Named 'Flush' to emphasize pending elements and lines are signaled and cleared.</remarks>
		protected virtual void FlushClearAlreadyStartedLine(RepositoryType repositoryType, ProcessState processState,
		                                                    DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			if ((elementsToAdd != null) && (elementsToAdd.Count > 0))
			{
				AddDisplayElements(repositoryType, elementsToAdd);
				elementsToAdd.Clear();
			}

			if ((linesToAdd != null) && (linesToAdd.Count > 0))
			{
				AddDisplayLines(repositoryType, linesToAdd);
				linesToAdd.Clear();
			}

			processState.Line.Elements.Clear();
			ClearCurrentDisplayLine(repositoryType);
		}

		#endregion

		#region Timer Events
		//------------------------------------------------------------------------------------------
		// Timer Events
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// This event handler must synchronize against <see cref="chunkVsTimeoutSyncObj"/>!
		/// </remarks>
		private void txLineBreakTimeout_Elapsed(object sender, EventArgs e)
		{
			DebugLineBreak("txLineBreakTimeout_Elapsed");

			lock (this.chunkVsTimeoutSyncObj) // Synchronize processing (raw chunk | timed line break).
			{
				if (IsDisposed)
					return; // Ensure not to handle async timer callbacks during closing anymore.

			////if (TerminalSettings.TxDisplayTimedLineBreak.Enabled) is implicitly given.
				{
					EvaluateAndSignalTimedLineBreak(RepositoryType.Tx,    DateTime.Now);
					EvaluateAndSignalTimedLineBreak(RepositoryType.Bidir, DateTime.Now);
				}
			}
		}

		/// <remarks>
		/// This event handler must synchronize against <see cref="chunkVsTimeoutSyncObj"/>!
		/// </remarks>
		private void rxLineBreakTimeout_Elapsed(object sender, EventArgs e)
		{
			DebugLineBreak("rxLineBreakTimeout_Elapsed");

			lock (this.chunkVsTimeoutSyncObj) // Synchronize processing (raw chunk | timed line break).
			{
				if (IsDisposed)
					return; // Ensure not to handle async timer callbacks during closing anymore.

			////if (TerminalSettings.RxDisplayTimedLineBreak.Enabled) is implicitly given.
				{
					EvaluateAndSignalTimedLineBreak(RepositoryType.Bidir, DateTime.Now);
					EvaluateAndSignalTimedLineBreak(RepositoryType.Rx,    DateTime.Now);
				}
			}
		}

		#endregion

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <summary></summary>
		[Conditional("DEBUG_LINE_BREAK")]
		protected virtual void DebugLineBreak(string message)
		{
			DebugMessage(message);
		}

		/// <summary></summary>
		[Conditional("DEBUG_LINE_BREAK")]
		protected virtual void DebugLineBreak(RepositoryType repositoryType, string message)
		{
			if (repositoryType == RepositoryType.Bidir) // Limited to tricky case.
				DebugLineBreak(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
