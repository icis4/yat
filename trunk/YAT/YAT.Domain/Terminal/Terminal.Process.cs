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
// Copyright © 2003-2020 Matthias Kläy.
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

	// Enable postponing of whole chunks or parts of it (e.g. for glueing chars of a line):
////#define DEBUG_CHUNKS

	// Enable debugging of line break:
////#define DEBUG_LINE_BREAK

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

using MKY;
using MKY.Text;

using YAT.Domain.Settings;

#endregion

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in
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
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Vs", Justification = "'vs.' is a correct English abbreviation.")]
		protected object ChunkVsTimedSyncObj { get; private set; } = new object();

		private ProcessTimeout txTimedLineBreak;
		private ProcessTimeout rxTimedLineBreak;

		#endregion

		#region Non-Public Methods
		//==========================================================================================
		// Non-Public Methods
		//==========================================================================================

		#region ByteTo.../...Element
		//------------------------------------------------------------------------------------------
		// ByteTo.../...Element
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Terminals supporting other than <see cref="Encoding.IsSingleByte"/> require a state to accumulate multi-byte sequences.
		/// This is provided by <paramref name="pendingMultiBytesToDecode"/>. Knowing that it seems a bit weird to find something
		/// called "multi-byte" in the signature of a method called "byte", this is the most appropriate approach found so far.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiBytes'?")]
		protected virtual DisplayElement ByteToElement(byte b, DateTime ts, IODirection dir, List<byte> pendingMultiBytesToDecode)
		{
			switch (dir)
			{
				case IODirection.Tx:    return (ByteToElement(b, ts, dir, TerminalSettings.Display.TxRadix, pendingMultiBytesToDecode));
				case IODirection.Rx:    return (ByteToElement(b, ts, dir, TerminalSettings.Display.RxRadix, pendingMultiBytesToDecode));

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("d", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// Terminals supporting other than <see cref="Encoding.IsSingleByte"/> require a state to accumulate multi-byte sequences.
		/// This is provided by <paramref name="pendingMultiBytesToDecode"/>. Knowing that it seems a bit weird to find something
		/// called "multi-byte" in the signature of a method called "byte", this is the most appropriate approach found so far.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "What's wrong with 'MultiBytes'?")]
		protected virtual DisplayElement ByteToElement(byte b, DateTime ts, IODirection dir, Radix r, List<byte> pendingMultiBytesToDecode)
		{
			bool isControl;
			bool isByteToHide;
			bool isError;

			string text = ByteToText(b, ts, r, out isControl, out isByteToHide, out isError);

			if      (isError)
			{
				return (new DisplayElement.ErrorInfo(ts, (Direction)dir, text));
			}
			else if (isByteToHide)
			{
				return (new DisplayElement.Nonentity()); // Return nothing, ignore the character, this results in hiding.
			}
			else if (isControl)
			{
				if (RadixIsStringOrChar(r) && TerminalSettings.CharReplace.ReplaceControlChars)
					return (CreateControlElement(b, ts, dir, text));
				else
					return (CreateDataElement(b, ts, dir, text));
			}
			else // Neither 'isError' nor 'isByteToHide' nor 'isError' => Use normal data element:
			{
				return (CreateDataElement(b, ts, dir, text));
			}
		}

		/// <summary></summary>
	////[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1120:CommentsMustContainText", Justification = "Separation of section header.")] does not prevent separator issue below, fixing it by ////.
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected virtual string ByteToText(byte b, DateTime ts, Radix r, out bool isControl, out bool isByteToHide, out bool isError)
		{
			isByteToHide = false;                                         // Notes on hiding:
			if      (b == 0x00)                                         ////
			{                                                             // Implementing hiding here has pros and cons:
				if (TerminalSettings.CharHide.Hide0x00)                   //  + Obvious location
					isByteToHide = true;                                  //  + Simple straight-forward
			}                                                             //  - Lines only containing a hidden element yet, e.g. an initial <XOn>,
			else if (b == 0xFF)                                           //    will a) show line start information when receiving the hidden element
			{                                                             //    and b) initially use the time stamp of the hidden element. These facts
				if (TerminalSettings.SupportsHide0xFF && TerminalSettings.CharHide.Hide0xFF)
					isByteToHide = true;                                  //    have to be accepted or handled elsewhere. However, this is consistent
			}                                                             //    with other behavior, e.g. the "Receiving..." notification in the status
			else if (MKY.IO.Serial.XOnXOff.IsXOnOrXOffByte(b))            //    bar. Also note that most users won't notice or care.
			{
				if (TerminalSettings.SupportsHideXOnXOff && TerminalSettings.CharHide.HideXOnXOff)
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
						if (RadixIsStringOrChar(r) && TerminalSettings.CharReplace.ReplaceControlChars)
							return (ByteToControlCharReplacementString(b, TerminalSettings.CharReplace.ControlCharRadix));
						else
							return (ByteToCharacterString(b));
					}
					else if (b == ' ') // Space.
					{
						if (TerminalSettings.CharReplace.ReplaceSpace)
							return (CharReplaceSettings.SpaceReplaceChar);
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
				case ControlCharRadix.Bin:
				case ControlCharRadix.Oct:
				case ControlCharRadix.Dec:
				case ControlCharRadix.Hex:
					return (ByteToNumericRadixString(b, (Radix)TerminalSettings.CharReplace.ControlCharRadix));

				case ControlCharRadix.AsciiMnemonic:
					return (ByteToAsciiString(b));

				default: // Includes 'String', 'Char' and 'Unicode' which are not supported for control character replacement.
					throw (new ArgumentOutOfRangeException("r", r, MessageHelper.InvalidExecutionPreamble + "'" + r + "' is an ASCII control character radix that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual DisplayElement CreateDataElement(byte origin, DateTime ts, IODirection dir, string text)
		{
			switch (dir)
			{
				case IODirection.Tx:    return (new DisplayElement.TxData(ts, origin, text));
				case IODirection.Rx:    return (new DisplayElement.RxData(ts, origin, text));

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual DisplayElement CreateDataElement(byte[] origin, DateTime ts, IODirection dir, string text)
		{
			switch (dir)
			{
				case IODirection.Tx:    return (new DisplayElement.TxData(ts, origin, text));
				case IODirection.Rx:    return (new DisplayElement.RxData(ts, origin, text));

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual DisplayElement CreateControlElement(byte origin, DateTime ts, IODirection dir, string text)
		{
			switch (dir)
			{
				case IODirection.Tx:    return (new DisplayElement.TxControl(ts, origin, text));
				case IODirection.Rx:    return (new DisplayElement.RxControl(ts, origin, text));

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "r", Justification = "Short and compact for improved readability.")]
		protected static bool RadixIsStringOrChar(Radix r)
		{
			return ((r == Radix.String) || (r == Radix.Char));
		}

		/// <summary></summary>
		protected virtual bool RadixIsStringOrChar(IODirection dir)
		{
			switch (dir)
			{
				case IODirection.Tx:    return (TerminalSettings.TxRadixIsStringOrChar);
				case IODirection.Rx:    return (TerminalSettings.RxRadixIsStringOrChar);

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual bool ElementsAreSeparate(IODirection dir)
		{
			switch (dir)
			{                                                                                         // Pragmatic best-effort approach.
				case IODirection.None:  return (ElementsAreSeparate(TerminalSettings.Display.TxRadix) || ElementsAreSeparate(TerminalSettings.Display.RxRadix));
				case IODirection.Bidir: return (ElementsAreSeparate(TerminalSettings.Display.TxRadix) || ElementsAreSeparate(TerminalSettings.Display.RxRadix));

				case IODirection.Tx:    return (ElementsAreSeparate(TerminalSettings.Display.TxRadix)                                                         );
				case IODirection.Rx:    return (                                                         ElementsAreSeparate(TerminalSettings.Display.RxRadix));

				default:                throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

		/// <summary>
		/// Add a separator to the given collection, depending on the given state.
		/// </summary>
		/// <remarks>
		/// Non-line-state dependent implementation for e.g. <see cref="Format(byte[], IODirection, Radix)"/>.
		/// </remarks>
		protected virtual void AddContentSeparatorIfNecessary(IODirection dir, DisplayElementCollection lp, DisplayElement de)
		{
			if (ElementsAreSeparate(dir) && !string.IsNullOrEmpty(de.Text))
			{
				if (lp.ByteCount > 0)
				{
					if (!string.IsNullOrEmpty(TerminalSettings.Display.ContentSeparatorCache))
						lp.Add(new DisplayElement.ContentSeparator((Direction)dir, TerminalSettings.Display.ContentSeparatorCache));
				}
			}
		}

		/// <summary>
		/// Add a separator to the given collection, depending on the given state.
		/// </summary>
		/// <remarks>
		/// This default implementation is based on <see cref="DisplayElementCollection.ByteCount"/>.
		/// </remarks>
		protected virtual void AddContentSeparatorIfNecessary(LineState lineState, IODirection dir, DisplayElementCollection lp, DisplayElement de)
		{
			if (ElementsAreSeparate(dir) && !string.IsNullOrEmpty(de.Text))
			{
				if ((lineState.Elements.ByteCount > 0) || (lp.ByteCount > 0))
				{
					if (!string.IsNullOrEmpty(TerminalSettings.Display.ContentSeparatorCache))
						lp.Add(new DisplayElement.ContentSeparator((Direction)dir, TerminalSettings.Display.ContentSeparatorCache));
				}
			}
		}

		/// <summary></summary>
		protected virtual void RemoveContentSeparatorIfNecessary(IODirection dir, DisplayElementCollection lp)
		{
			if (ElementsAreSeparate(dir))
			{
				int count = lp.Count;
				if ((count > 0) && (lp[count - 1] is DisplayElement.ContentSeparator))
					lp.RemoveLast();
			}
		}

		/// <summary>
		/// Add a separator to the given collection, depending on the given state.
		/// </summary>
		protected virtual void AddInfoSeparatorIfNecessary(DisplayElementCollection lp)
		{
			if (!string.IsNullOrEmpty(TerminalSettings.Display.InfoSeparatorCache))
				lp.Add(new DisplayElement.InfoSeparator(TerminalSettings.Display.InfoSeparatorCache));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		protected virtual void PrepareLineBeginInfo(DateTime stamp, TimeSpan span, TimeSpan delta, string dev, IODirection dir, out DisplayElementCollection lp)
		{
			if (TerminalSettings.Display.ShowTimeStamp || TerminalSettings.Display.ShowTimeSpan || TerminalSettings.Display.ShowTimeDelta ||
			    TerminalSettings.Display.ShowDevice    ||
			    TerminalSettings.Display.ShowDirection)
			{
				lp = new DisplayElementCollection(10); // Preset the required capacity to improve memory management.

				if (TerminalSettings.Display.ShowTimeStamp)
				{
					lp.Add(new DisplayElement.TimeStampInfo(stamp, TerminalSettings.Display.TimeStampFormat, TerminalSettings.Display.TimeStampUseUtc, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!
					AddInfoSeparatorIfNecessary(lp);
				}

				if (TerminalSettings.Display.ShowTimeSpan)
				{
					lp.Add(new DisplayElement.TimeSpanInfo(span, TerminalSettings.Display.TimeSpanFormat, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!
					AddInfoSeparatorIfNecessary(lp);
				}

				if (TerminalSettings.Display.ShowTimeDelta)
				{
					lp.Add(new DisplayElement.TimeDeltaInfo(delta, TerminalSettings.Display.TimeDeltaFormat, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!
					AddInfoSeparatorIfNecessary(lp);
				}

				if (TerminalSettings.Display.ShowDevice)
				{
					lp.Add(new DisplayElement.DeviceInfo(dev, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache)); // Direction may become both!
					AddInfoSeparatorIfNecessary(lp);
				}

				if (TerminalSettings.Display.ShowDirection)
				{
					lp.Add(new DisplayElement.DirectionInfo((Direction)dir, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache));
					AddInfoSeparatorIfNecessary(lp);
				}

				// Last separator will separate info(s) from content, i.e. be the former 'LeftMargin'.
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

				// First separator will separate content from info(s), i.e. be the former 'RightMargin'.

				if (TerminalSettings.Display.ShowLength)
				{
					AddInfoSeparatorIfNecessary(lp);
					lp.Add(new DisplayElement.ContentLength(length, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache));
				}

				if (TerminalSettings.Display.ShowDuration)
				{
					AddInfoSeparatorIfNecessary(lp);
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

		/// <remarks>
		/// <c>private</c> rather than <c>protected virtual</c> because derived method(s) depend(s)
		/// on code sequence in constructor(s).
		/// </remarks>
		private void InitializeProcess()
		{
			this.txProcessState    = new ProcessState();
			this.bidirProcessState = new ProcessState();
			this.rxProcessState    = new ProcessState();

			InitializeTimedLineBreaksIfNeeded();
		}

		/// <summary>
		/// Disposes the processing state.
		/// </summary>
		protected virtual void DisposeProcess()
		{
			// In the 'normal' case, timers are stopped in ExecuteLineEnd().
			DisposeTimedLineBreaksIfNeeded();
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

			ResetTimedLineBreaksIfNeeded(repositoryType);
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
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unidir", Justification = "Orthogonality with 'Bidir'.")]
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

		/// <summary></summary>
		protected static IODirection GetOtherDirection(IODirection dir)
		{
			return ((dir != IODirection.Tx) ? (IODirection.Tx) : (IODirection.Rx));
		}

		/// <summary>
		/// This processing method is called by the <see cref="RawTerminal.ChunkSent"/> and
		/// <see cref="RawTerminal.ChunkReceived"/> event handlers. It sequentially updates the
		/// two affected repositories.
		/// </summary>
		/// <remarks>
		/// Before introduction of <see cref="TextTerminalSettings.GlueCharsOfLine"/>, processing
		/// happened simultaneously byte-by-byte for both affected repositories, rather than whole
		/// chunk for first and then second affected repository. However, glueing requires that the
		/// part of a chunk after a line break may by postponed, and glueing only applies to bidir.
		/// Thus, with YAT 2.2.0, processing changed to the current strategy.
		/// </remarks>
		/// <remarks>
		/// This method must synchronize against <see cref="ChunkVsTimedSyncObj"/>!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'glueing' is a correct English term.")]
		protected virtual void ProcessChunk(RawChunk chunk)
		{
			lock (ChunkVsTimedSyncObj) // Synchronize processing (raw chunk | timed line break).
			{
				DebugChunks(string.Format(CultureInfo.InvariantCulture, "Processing {0} chunk of {1} byte(s) stamped {2}.", chunk.Direction, chunk.Content.Count, chunk.TimeStamp));

				switch (chunk.Direction)
				{
					case IODirection.Tx: ProcessChunk(RepositoryType.Tx,    chunk);
					                     ProcessChunk(RepositoryType.Bidir, chunk);
					                     break;

					case IODirection.Rx: ProcessChunk(RepositoryType.Bidir, chunk);
					                     ProcessChunk(RepositoryType.Rx,    chunk);
					                     break;

					default: throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A chunk must always be tied to Tx or Rx!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <summary>
		/// This processing method is called by the <see cref="ProcessChunk(RawChunk)"/> method
		/// as well as the <see cref="RefreshRepository"/> and <see cref="RefreshRepositories"/>
		/// methods on reloading. It only affects one of the repositories.
		/// </summary>
		/// <remarks>
		/// This method must synchronize against <see cref="ChunkVsTimedSyncObj"/>!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		protected virtual void ProcessChunk(RepositoryType repositoryType, RawChunk chunk)
		{
			lock (ChunkVsTimedSyncObj) // Synchronize processing (raw chunk | timed line break).
			{
				// Process chunk(s) of given direction:
				{
					SuspendChunkTimeouts(repositoryType, chunk.Direction);
					{
						var overallState = GetOverallState(repositoryType);
						var postponedChunks = overallState.RemovePostponedChunks(chunk.Direction);
						if (postponedChunks.Length > 0)
						{
							DebugChunks(string.Format(CultureInfo.InvariantCulture, "Processing {0} postponed {1} chunk(s) before processing {2} chunk stamped {3}.", postponedChunks.Length, chunk.Direction, chunk.Direction, chunk.TimeStamp));

							var chunksToProcess = new List<RawChunk>(postponedChunks.Length + 1); // Preset the required capacity to improve memory management.
							chunksToProcess.AddRange(postponedChunks);
							chunksToProcess.Add(chunk);

							PostponeResult postponeResult; // Ignore, postponed chunks will be processed anyway.
							ProcessChunksOfSameDirection(repositoryType, chunksToProcess.ToArray(), chunk.Direction, out postponeResult);
						}
						else
						{
							PostponeResult postponeResult; // Ignore, postponed chunks will be processed anyway.
							ProcessChunk(repositoryType, chunk, out postponeResult);
						}
					}
					ResumeChunkTimeouts(repositoryType, chunk.Direction);
				}

				// Then process postponed chunk(s) starting with other direction:
				{
					var initialDir = GetOtherDirection(chunk.Direction);
					ProcessPostponedChunks(repositoryType, initialDir);
				}
			}
		}

		/// <remarks>
		/// The caller of this method must synchronize against <see cref="ChunkVsTimedSyncObj"/>!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "'out' is preferred over return value in this particular case.")]
		protected virtual void ProcessChunksOfSameDirection(RepositoryType repositoryType, RawChunk[] chunks, IODirection dir, out PostponeResult postponeResult)
		{
			postponeResult = PostponeResult.Nothing;

			for (int i = 0; i < chunks.Length; i++)
			{
				var chunk = chunks[i];

				if (chunk.Direction != dir)
					throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "This method requires that chunks all share the same 'Direction'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				ProcessChunk(repositoryType, chunk, out postponeResult);

				if (postponeResult != PostponeResult.Nothing)
				{
					int chunkCountTotal = chunks.Length;
					int chunkCountProcessed = (i + 1); // Current chunk has been processed and/or already postponed in any case.
					PostponeRemainingChunks(repositoryType, chunks, chunkCountTotal, chunkCountProcessed);
					break;
				}
			}
		}

		/// <summary></summary>
		protected virtual void PostponeRemainingChunks(RepositoryType repositoryType, RawChunk[] chunks, int chunkCountTotal, int chunkCountProcessed)
		{
			var overallState = GetOverallState(repositoryType);
			for (int i = chunkCountProcessed; i < chunkCountTotal; i++)
				overallState.AddPostponedChunk(chunks[i]);
		}

		/// <summary></summary>
		protected virtual void ProcessPostponedChunks(RepositoryType repositoryType, IODirection initialDir)
		{
			var overallState = GetOverallState(repositoryType);
			var otherDir = GetOtherDirection(initialDir);

			// Process chunk(s) starting with given initial direction then toggle direction and continue as long as needed:
			var previousByteCount = overallState.GetPostponedByteCount();
			while (overallState.GetPostponedChunkCount() > 0)
			{
				// Initially implemented as single loop toggling direction, but then switched to
				// two direction dedicated sections for more comprehensive implemenation.

				// Initial direction:
				var dir = initialDir;
				var postponedChunks = overallState.RemovePostponedChunks(dir);
				if (postponedChunks.Length > 0)
				{
					DebugChunks(string.Format(CultureInfo.InvariantCulture, "Processing {0} postponed {1} chunk(s).", postponedChunks.Length, dir));

					SuspendChunkTimeouts(repositoryType, dir);
					{
						PostponeResult postponeResult; // Ignore, other direction shall be processed at least once.
						ProcessChunksOfSameDirection(repositoryType, postponedChunks, dir, out postponeResult);
					}
					ResumeChunkTimeouts(repositoryType, dir);
				}

				// Other direction:
				dir = otherDir;
				postponedChunks = overallState.RemovePostponedChunks(dir);
				if (postponedChunks.Length > 0)
				{
					DebugChunks(string.Format(CultureInfo.InvariantCulture, "Processing {0} postponed {1} chunk(s).", postponedChunks.Length, dir));

					SuspendChunkTimeouts(repositoryType, dir);
					{
						PostponeResult postponeResult; // Ignore, break condition is based on processed byte count.
						ProcessChunksOfSameDirection(repositoryType, postponedChunks, dir, out postponeResult);
					}
					ResumeChunkTimeouts(repositoryType, dir);
				}

				// Break condition:
				var currentByteCount = overallState.GetPostponedByteCount();
				var nothingProcessed = (currentByteCount == previousByteCount);
				if (nothingProcessed)
					break; // Nothing to process or postpone, e.g. due to infinite timeout.

				previousByteCount = overallState.GetPostponedByteCount();
			}
		}

		/// <summary></summary>
		protected virtual void SuspendChunkTimeouts(RepositoryType repositoryType, IODirection dir)
		{
			if (!IsReloading) // See comments further below.
				SuspendTimedLineBreakIfNeeded(dir);
		}

		/// <summary></summary>
		protected virtual void ResumeChunkTimeouts(RepositoryType repositoryType, IODirection dir)
		{
			if (!IsReloading) // See comments further below.
				ResumeTimedLineBreakIfNeeded(repositoryType, dir);
		}

		/// <summary></summary>
		protected virtual void ProcessChunkPre(RepositoryType repositoryType, RawChunk chunk)
		{
			// Timed line breaks are processed asynchronously, as they are only triggered
			// after a timeout. Except on reload, then timed line breaks are calculated.
			// Note that all bytes of a chunk have the same time stamp, thus no need to
			// check for timeout on each byte.

			if (IsReloading)
				ProcessAndSignalTimedLineBreakOnReloadIfNeeded(repositoryType, chunk);
		}

		/// <summary></summary>
		protected virtual void ProcessChunkPost(RepositoryType repositoryType, RawChunk chunk)
		{
			// Nothing to do yet.
		}

		/// <remarks>
		/// The caller of this method must synchronize against <see cref="ChunkVsTimedSyncObj"/>!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "'out' is preferred over return value in this particular case.")]
		protected virtual void ProcessChunk(RepositoryType repositoryType, RawChunk chunk, out PostponeResult postponeResult)
		{
			ProcessChunkPre(                                   repositoryType, chunk);

			ProcessAndSignalDeviceOrDirectionLineBreakIfNeeded(repositoryType, chunk);
			ProcessAndSignalChunkAttributes(                   repositoryType, chunk); // Needed e.g. in case direction changes within a line.

			int byteCountProcessed;
			ProcessAndSignalChunkContent(                      repositoryType, chunk, out byteCountProcessed);

			int byteCountTotal = chunk.Content.Count;
			if (byteCountProcessed == 0)
				postponeResult = PostponeResult.CompleteChunk;
			else if (byteCountProcessed < byteCountTotal)
				postponeResult = PostponeResult.PartOfChunk;
			else
				postponeResult = PostponeResult.Nothing;

			if (postponeResult != PostponeResult.Nothing)
				PostponeRemainingBytes(                        repositoryType, chunk, byteCountTotal, byteCountProcessed);
			else
				ProcessAndSignalChunkLineBreakIfNeeded(        repositoryType, chunk);

			ProcessChunkPost(                                  repositoryType, chunk);
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte", Justification = "Why not? 'Byte' not only is a type, but also emphasizes a purpose.")]
		protected virtual void PostponeRemainingBytes(RepositoryType repositoryType, RawChunk chunk, int byteCountTotal, int byteCountProcessed)
		{
			var contentPostponed = new List<byte>(byteCountTotal - byteCountProcessed);
			for (int i = byteCountProcessed; i < byteCountTotal; i++)
				contentPostponed.Add(chunk.Content[i]);

			var remainingPostponed = new RawChunk
			                         (
			                             contentPostponed.ToArray(),
			                             chunk.TimeStamp,
			                             chunk.Device,
			                             chunk.Direction
			                         );

			PostponeChunk(repositoryType, remainingPostponed);
		}

		/// <summary></summary>
		protected virtual void PostponeChunk(RepositoryType repositoryType, RawChunk chunk)
		{
			DebugChunks(string.Format(CultureInfo.InvariantCulture, "Postponing whole or partial {0} chunk of {1} byte(s) stamped {2}.", chunk.Direction, chunk.Content.Count, chunk.TimeStamp));

			var overallState = GetOverallState(repositoryType);
			overallState.AddPostponedChunk(chunk);
		}

		/// <summary></summary>
		protected virtual bool RepositoryIsAffected(RepositoryType repositoryType, IODirection dir)
		{
			switch (repositoryType)
			{
				case RepositoryType.Tx:    return (dir == IODirection.Tx);
				case RepositoryType.Bidir: return (true);
				case RepositoryType.Rx:    return (dir == IODirection.Rx);

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalTimedLineBreakOnReloadIfNeeded(RepositoryType repositoryType, RawChunk chunk)
		{
			ProcessAndSignalTimedLineBreakOnReloadIfNeeded(repositoryType, chunk.TimeStamp, chunk.Direction);
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalTimedLineBreakOnReloadIfNeeded(RepositoryType repositoryType, DateTime ts, IODirection dir)
		{
			if (RepositoryIsAffected(repositoryType, dir))
			{
				TimeoutSettingTuple settings;
				switch (dir)
				{
					case IODirection.Tx: settings = TerminalSettings.TxDisplayTimedLineBreak; break;
					case IODirection.Rx: settings = TerminalSettings.RxDisplayTimedLineBreak; break;

					default: throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A chunk must always be tied to Tx or Rx!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				if (settings.Enabled)
					ProcessAndSignalTimedLineBreakOnReload(repositoryType, ts, dir, settings.Timeout);
			}
		}

		/// <summary>Check whether device or direction has changed.</summary>
		/// <remarks>A chunk is always tied to device and direction.</remarks>
		protected virtual void ProcessAndSignalDeviceOrDirectionLineBreakIfNeeded(RepositoryType repositoryType, RawChunk chunk)
		{
			var isServerSocket = TerminalSettings.IO.IOTypeIsServerSocket;
			if (isServerSocket && TerminalSettings.Display.DeviceLineBreakEnabled) // Attention: This 'isServerSocket' restriction is also implemented at other locations!
				ProcessAndSignalDeviceLineBreak(repositoryType, chunk.TimeStamp, chunk.Device, chunk.Direction);

			if ((TerminalSettings.Display.DirectionLineBreakEnabled) && (repositoryType == RepositoryType.Bidir)) // Not needed for unidirectional repositories.
				ProcessAndSignalDirectionLineBreak(repositoryType, chunk.TimeStamp, chunk.Direction);
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalChunkAttributes(RepositoryType repositoryType, RawChunk chunk)
		{
			var overallState = GetOverallState(repositoryType);
			overallState.NotifyChunk(chunk);

			ProcessAndSignalDirection(repositoryType, chunk.Direction);
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalChunkLineBreakIfNeeded(RepositoryType repositoryType, RawChunk chunk)
		{
			if (RepositoryIsAffected(repositoryType, chunk.Direction))
			{
				bool enabled;
				switch (chunk.Direction)
				{
					case IODirection.Tx: enabled = TerminalSettings.TxDisplayChunkLineBreakEnabled; break;
					case IODirection.Rx: enabled = TerminalSettings.RxDisplayChunkLineBreakEnabled; break;

					default: throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "A chunk must always be tied to Tx or Rx!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				if (enabled)
					ProcessAndSignalChunkLineBreak(repositoryType, chunk.TimeStamp, chunk.Direction);
			}
		}

		/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
		protected virtual void ProcessAndSignalDeviceLineBreak(RepositoryType repositoryType, DateTime ts, string dev, IODirection dir)
		{
			var elementsToAdd = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.
			var linesToAdd    = new DisplayLineCollection();    // No preset needed, the default behavior is good enough.

			ProcessDeviceLineBreak(repositoryType, ts, dev, dir, elementsToAdd, linesToAdd);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalDirectionLineBreak(RepositoryType repositoryType, DateTime ts, IODirection dir)
		{
			var elementsToAdd = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.
			var linesToAdd    = new DisplayLineCollection();    // No preset needed, the default behavior is good enough.

			ProcessDirectionLineBreak(repositoryType, ts, dir, elementsToAdd, linesToAdd);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalDirection(RepositoryType repositoryType, IODirection dir)
		{
			ProcessDirection(repositoryType, dir); // Nothing to signal (yet).
		}

		/// <remarks>
		/// Before introduction of <see cref="TextTerminalSettings.GlueCharsOfLine"/>, processing
		/// happened simultaneously byte-by-byte for both affected repositories, rather than whole
		/// chunk for first and then second affected repository. However, glueing requires that the
		/// part of a chunk after a line break may by postponed, and glueing only applies to bidir.
		/// Thus, with YAT 2.2.0, processing changed to the current strategy.
		/// </remarks>
		/// <remarks>
		/// Signaling is only done once per chunk (unless flushing is involved), in order to improve
		/// performance (by reducing the number of events and repository updates).
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'glueing' is a correct English term.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "'out' is preferred over return value in this particular case.")]
		protected virtual void ProcessAndSignalChunkContent(RepositoryType repositoryType, RawChunk chunk, out int byteCountProcessed)
		{
			var elementsToAdd = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.
			var linesToAdd    = new DisplayLineCollection();    // No preset needed, the default behavior is good enough.

			byteCountProcessed = chunk.Content.Count;
			for (int i = 0; i < chunk.Content.Count; i++)
			{
				bool isFirstByteOfChunk = (i == 0);
				bool isLastByteOfChunk = (i == (chunk.Content.Count - 1));

				bool breakChunk;
				ProcessByteOfChunk(repositoryType, chunk.Content[i], chunk.TimeStamp, chunk.Device, chunk.Direction, isFirstByteOfChunk, isLastByteOfChunk, elementsToAdd, linesToAdd, out breakChunk);
				if (breakChunk)
				{
					byteCountProcessed = (i + 1); // Current byte has been processed and/or already postponed in any case.
					break;
				}
			}

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalChunkLineBreak(RepositoryType repositoryType, DateTime ts, IODirection dir)
		{
			var elementsToAdd = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.
			var linesToAdd    = new DisplayLineCollection();    // No preset needed, the default behavior is good enough.

			ProcessChunkLineBreak(repositoryType, ts, dir, elementsToAdd, linesToAdd);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalTimedLineBreak(RepositoryType repositoryType, DateTime ts, IODirection dir)
		{
			var elementsToAdd = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.
			var linesToAdd    = new DisplayLineCollection();    // No preset needed, the default behavior is good enough.

			ProcessTimedLineBreak(repositoryType, ts, dir, elementsToAdd, linesToAdd);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);
		}

		/// <summary></summary>
		protected virtual void ProcessAndSignalTimedLineBreakOnReload(RepositoryType repositoryType, DateTime ts, IODirection dir, int timeout)
		{
			var elementsToAdd = new DisplayElementCollection(); // No preset needed, the default behavior is good enough.
			var linesToAdd    = new DisplayLineCollection();    // No preset needed, the default behavior is good enough.

			ProcessTimedLineBreakOnReload(repositoryType, ts, dir, timeout, elementsToAdd, linesToAdd);

			if (elementsToAdd.Count > 0)
				AddDisplayElements(repositoryType, elementsToAdd);

			if (linesToAdd.Count > 0)
				AddDisplayLines(repositoryType, linesToAdd);
		}

		/// <remarks>Nothing to signal (yet).</remarks>
		protected virtual void ProcessDirection(RepositoryType repositoryType, IODirection dir)
		{
			var lineState = GetLineState(repositoryType);
			if (lineState.Direction != IODirection.None) // IODirection.None means that line processing has not started yet.
			{
				if (lineState.Direction != dir)
				{
					lineState.Direction = IODirection.Bidir;

					if (TerminalSettings.Display.ShowDirection) // Replace is only needed when containing a 'DisplayElement.DirectionInfo'.
					{
						lineState.Elements.ReplaceDirection(Direction.Bidir, TerminalSettings.Display.InfoEnclosureLeftCache, TerminalSettings.Display.InfoEnclosureRightCache);
					////elementsToAdd.Clear() is not needed as only replace happens above.
						FlushReplaceAlreadyBeganLine(repositoryType, lineState);
					}
				}
			}
		}

		/// <remarks>Named 'Device' for simplicity even though using 'I/O Device' for user.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1508:ClosingCurlyBracketsMustNotBePrecededByBlankLine", Justification = "Separating line for improved readability.")]
		protected virtual void ProcessDeviceLineBreak(RepositoryType repositoryType, DateTime ts, string dev, IODirection dir,
		                                              DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			var processState = GetProcessState(repositoryType);
			if (processState.Overall.DeviceLineBreak.IsFirstChunk)         // Not the ideal location to handle this flag and 'DeviceLineBreak.Device' further below.
			{                                                              // But good enough because not needed anywhere else and more performant if only done here.
				processState.Overall.DeviceLineBreak.IsFirstChunk = false; // Mitigated by using a dedicated 'DeviceLineBreak' sub-item making the scope obvious.
			}
			else // = 'IsSubsequentChunk'
			{                                                  // See above!
				if (DeviceHasChanged(dev, processState.Overall.DeviceLineBreak.Device))
				{
					if (processState.Line.Position != LinePosition.Begin) // 'Begin' also applies if the next line has not been started yet, i.e. 'LinePosition.None'.
					{
						DebugLineBreak(repositoryType, "ProcessDeviceLineBreak => DoLineEnd()");

						DoLineEnd(repositoryType, processState, ts, dir, elementsToAdd, linesToAdd); // Line end = line break is directly invoked,
					}                                                                                // not indirectly by setting 'Position' to 'End'.
				}
			}
			                     //// See above!
			processState.Overall.DeviceLineBreak.Device = dev;
		}

		/// <summary></summary>
		protected virtual bool DeviceHasChanged(string currentDevice, string previousDevice)
		{
			return (!StringEx.EqualsOrdinalIgnoreCase(currentDevice, previousDevice));
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1508:ClosingCurlyBracketsMustNotBePrecededByBlankLine", Justification = "Separating line for improved readability.")]
		protected virtual void ProcessDirectionLineBreak(RepositoryType repositoryType, DateTime ts, IODirection dir,
		                                                 DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			var processState = GetProcessState(repositoryType);
			if (processState.Overall.DirectionLineBreak.IsFirstChunk)         // Not the ideal location to handle this flag and 'DirectionLineBreak.Direction' further below.
			{                                                                 // But good enough because not needed anywhere else and more performant if only done here.
				processState.Overall.DirectionLineBreak.IsFirstChunk = false; // Mitigated by using a dedicated 'DirectionLineBreak' sub-item making the scope obvious.
			}
			else // = 'IsSubsequentChunk'
			{                                                     // See above!
				if (DirectionHasChanged(dir, processState.Overall.DirectionLineBreak.Direction))
				{
					if (processState.Line.Position != LinePosition.Begin) // 'Begin' also applies if the next line has not been started yet, i.e. 'LinePosition.None'.
					{
						DebugLineBreak(repositoryType, "ProcessDirectionLineBreak => DoLineEnd()");

						DoLineEnd(repositoryType, processState, ts, dir, elementsToAdd, linesToAdd); // Line end = line break is directly invoked,
					}                                                                                // not indirectly by setting 'Position' to 'End'.
				}
			}
			                     //// See above!
			processState.Overall.DirectionLineBreak.Direction = dir;
		}

		/// <summary></summary>
		protected virtual bool DirectionHasChanged(IODirection currentDirection, IODirection previousDirection)
		{
			return (currentDirection != previousDirection);
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		protected virtual void ProcessChunkLineBreak(RepositoryType repositoryType, DateTime ts, IODirection dir,
		                                             DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			var processState = GetProcessState(repositoryType);
			if (processState.Line.Position != LinePosition.Begin) // 'Begin' also applies if the next line has not been started yet, i.e. 'LinePosition.None'.
			{
				DebugLineBreak(repositoryType, "ProcessChunkLineBreak => DoLineEnd()");

				DoLineEnd(repositoryType, processState, ts, dir, elementsToAdd, linesToAdd); // Line end = line break is directly invoked,
			}                                                                                // not indirectly by setting 'Position' to 'End'.
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		protected virtual void ProcessTimedLineBreak(RepositoryType repositoryType, DateTime ts, IODirection dir,
		                                             DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			var processState = GetProcessState(repositoryType);
			if (processState.Line.Position != LinePosition.Begin) // 'Begin' also applies if the next line has not been started yet, i.e. 'LinePosition.None'.
			{
				DebugLineBreak(repositoryType, "ProcessTimedLineBreak => DoLineEnd()");

				DoLineEnd(repositoryType, processState, ts, dir, elementsToAdd, linesToAdd); // Line end = line break is directly invoked,
			}                                                                                // not indirectly by setting 'Position' to 'End'.
		}

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		protected virtual void ProcessTimedLineBreakOnReload(RepositoryType repositoryType, DateTime ts, IODirection dir, int timeout,
		                                                     DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			var processState = GetProcessState(repositoryType);
			if (processState.Line.Position != LinePosition.Begin) // 'Begin' also applies if the next line has not been started yet, i.e. 'LinePosition.None'.
			{
				var span = (ts - processState.Line.TimeStamp);
				if (span.TotalMilliseconds >= timeout)
				{
					DebugLineBreak(repositoryType, "ProcessTimedLineBreakOnReload => DoLineEnd()");

					DoLineEnd(repositoryType, processState, ts, dir, elementsToAdd, linesToAdd); // Line end = line break is directly invoked,
				}                                                                                // not indirectly by setting 'Position' to 'End'.
			}
		}

		/// <remarks>
		/// Must be abstract/virtual because settings and behavior differ among text and binary.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "9#", Justification = "'out' is preferred over return value in this particular case.")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "b", Justification = "Short and compact for improved readability.")]
		protected abstract void ProcessByteOfChunk(RepositoryType repositoryType,
		                                           byte b, DateTime ts, string dev, IODirection dir,
		                                           bool isFirstByteOfChunk, bool isLastByteOfChunk,
		                                           DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd,
		                                           out bool breakChunk);

		/// <remarks>
		/// <paramref name="repositoryType"/> and <paramref name="elementsToAdd"/> are required
		/// for specialization by <see cref="BinaryTerminal"/> and <see cref="TextTerminal"/>.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
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
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		protected virtual void DoLineEnd(RepositoryType repositoryType, ProcessState processState,
		                                 DateTime ts, IODirection dir,
		                                 DisplayElementCollection elementsToAdd, DisplayLineCollection linesToAdd)
		{
			DebugLineBreak(repositoryType, "DoLineEnd() => NotifyLineEnd()");

			processState.NotifyLineEnd();
		}

		/// <remarks>Named 'Flush' to emphasize pending elements and lines are signaled and cleared.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
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
		/// <remarks>Named 'Began' for consistency with <see cref="LinePosition.Begin"/>.</remarks>
		protected virtual void FlushReplaceAlreadyBeganLine(RepositoryType repositoryType, LineState lineState)
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

			ReplaceCurrentDisplayLine(repositoryType, lineState.Elements.Clone()); // Clone to ensure decoupling!
		}                                                                          // Elements will be used again!

		/// <remarks>Named 'Flush' to emphasize pending elements and lines are signaled and cleared.</remarks>
		/// <remarks>Named 'Began' for consistency with <see cref="LinePosition.Begin"/>.</remarks>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		protected virtual void FlushClearAlreadyBeganLine(RepositoryType repositoryType, ProcessState processState,
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

		#region TimedLineBreak
		//------------------------------------------------------------------------------------------
		// TimedLineBreak
		//------------------------------------------------------------------------------------------

		private void InitializeTimedLineBreaksIfNeeded()
		{
			if (TerminalSettings.TxDisplayTimedLineBreak.Enabled)
			{
				if (this.txTimedLineBreak != null) // Must be given by this terminal.
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'DisposeTimeoutsIfNeeded()' must be called first!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				this.txTimedLineBreak = new ProcessTimeout(TerminalSettings.TxDisplayTimedLineBreak.Timeout);
				this.txTimedLineBreak.Elapsed += txTimedLineBreak_Elapsed;
			}

			if (TerminalSettings.RxDisplayTimedLineBreak.Enabled)
			{
				if (this.rxTimedLineBreak != null) // Must be given by this terminal.
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'DisposeTimeoutsIfNeeded()' must be called first!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

				this.rxTimedLineBreak = new ProcessTimeout(TerminalSettings.RxDisplayTimedLineBreak.Timeout);
				this.rxTimedLineBreak.Elapsed += rxTimedLineBreak_Elapsed;
			}
		}

		private void DisposeTimedLineBreaksIfNeeded()
		{
			if (this.txTimedLineBreak != null)
			{
				this.txTimedLineBreak.Elapsed -= txTimedLineBreak_Elapsed;
				this.txTimedLineBreak.Dispose();
			}

			this.txTimedLineBreak = null;

			if (this.rxTimedLineBreak != null)
			{
				this.rxTimedLineBreak.Elapsed -= rxTimedLineBreak_Elapsed;
				this.rxTimedLineBreak.Dispose();
			}

			this.rxTimedLineBreak = null;
		}

		private void ResetTimedLineBreaksIfNeeded(RepositoryType repositoryType)
		{
			bool txIsAffected;
			bool rxIsAffected;

			switch (repositoryType)
			{
				case RepositoryType.Tx:    txIsAffected = true;  rxIsAffected = false; break;
				case RepositoryType.Bidir: txIsAffected = true;  rxIsAffected = true;  break;
				case RepositoryType.Rx:    txIsAffected = false; rxIsAffected = false; break;

				case RepositoryType.None:  throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                   throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			if (txIsAffected && TerminalSettings.TxDisplayTimedLineBreak.Enabled)
				this.txTimedLineBreak.Stop();

			if (rxIsAffected && TerminalSettings.RxDisplayTimedLineBreak.Enabled)
				this.rxTimedLineBreak.Stop();
		}

		private void GetTimedLineBreak(IODirection dir, out TimeoutSettingTuple settings, out ProcessTimeout timeout)
		{
			switch (dir)
			{
				case IODirection.Tx: settings = TerminalSettings.TxDisplayTimedLineBreak; timeout = this.txTimedLineBreak; break;
				case IODirection.Rx: settings = TerminalSettings.RxDisplayTimedLineBreak; timeout = this.rxTimedLineBreak; break;

				case IODirection.Bidir:
				case IODirection.None:  throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is a direction that is not valid here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				default:                throw (new ArgumentOutOfRangeException("dir", dir, MessageHelper.InvalidExecutionPreamble + "'" + dir + "' is an invalid direction!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>
		/// Chunk and timed processing is synchronized against <see cref="ChunkVsTimedSyncObj"/>.
		/// Thus, time line breaks can be suspended during chunk processing.
		/// </remarks>
		protected virtual void SuspendTimedLineBreakIfNeeded(IODirection dir)
		{
			TimeoutSettingTuple settings;
			ProcessTimeout timeout;
			GetTimedLineBreak(dir, out settings, out timeout);

			if (settings.Enabled)
			{
				timeout.Stop();
			}
		}

		/// <remarks>
		/// Chunk and timed processing is synchronized against <see cref="ChunkVsTimedSyncObj"/>.
		/// Thus, time line breaks can be suspended during chunk processing.
		/// </remarks>
		protected virtual void ResumeTimedLineBreakIfNeeded(RepositoryType repositoryType, IODirection dir)
		{
			TimeoutSettingTuple settings;
			ProcessTimeout timeout;
			GetTimedLineBreak(dir, out settings, out timeout);

			if (settings.Enabled)
			{
				var lineState = GetUnidirLineState(dir);
				switch (lineState.Position)
				{
					case LinePosition.Begin:
					case LinePosition.End:
						// Nothing to do, keep timeout stopped.
						break;

					case LinePosition.Content:
					case LinePosition.ContentExceeded:
						var overallState = GetOverallState(repositoryType);
						var lastChunkTimeStampOfSameDir = overallState.GetLastChunkTimeStamp(dir);

						if (lastChunkTimeStampOfSameDir == DateTime.MinValue) // This condition may apply at the very beginning when no chunk
							lastChunkTimeStampOfSameDir = DateTime.Now;       // of the given direction has been processed yet, fallback to now.

						timeout.Start(lastChunkTimeStampOfSameDir); // Timed line break is direction dependent.
						break;

					default:
						throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + lineState.Position + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		/// <remarks>
		/// This event handler must synchronize against <see cref="ChunkVsTimedSyncObj"/>!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void txTimedLineBreak_Elapsed(object sender, EventArgs<DateTime> e)
		{
			// No need to synchronize this event, the 'ProcessTimeout' always is single-shot.

			if (IsReloading) // See remarks at Terminal.RefreshRepositories() for reason.
			{
				DebugLineBreak("txTimedLineBreak_Elapsed is being ignored while reloading");
				return;
			}

			DebugLineBreak("txTimedLineBreak_Elapsed");

			lock (ChunkVsTimedSyncObj) // Synchronize processing (raw chunk | timed line break).
			{
				if (IsInDisposal) // Ensure to not handle async timer callback during closing anymore.
					return;

			////if (TerminalSettings.TxDisplayTimedLineBreak.Enabled) is implicitly given.
				{
					ProcessAndSignalTimedLineBreak(RepositoryType.Tx,    e.Value, IODirection.Tx);
					ProcessAndSignalTimedLineBreak(RepositoryType.Bidir, e.Value, IODirection.Tx);
				}
			}
		}

		/// <remarks>
		/// This event handler must synchronize against <see cref="ChunkVsTimedSyncObj"/>!
		///
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void rxTimedLineBreak_Elapsed(object sender, EventArgs<DateTime> e)
		{
			// No need to synchronize this event, the 'ProcessTimeout' always is single-shot.

			if (IsReloading) // See remarks at Terminal.RefreshRepositories() for reason.
			{
				DebugLineBreak("rxTimedLineBreak_Elapsed is being ignored while reloading");
				return;
			}

			DebugLineBreak("rxTimedLineBreak_Elapsed");

			lock (ChunkVsTimedSyncObj) // Synchronize processing (raw chunk | timed line break).
			{
				if (IsInDisposal) // Ensure to not handle async timer callback during closing anymore.
					return;

			////if (TerminalSettings.RxDisplayTimedLineBreak.Enabled) is implicitly given.
				{
					ProcessAndSignalTimedLineBreak(RepositoryType.Bidir, e.Value, IODirection.Rx);
					ProcessAndSignalTimedLineBreak(RepositoryType.Rx,    e.Value, IODirection.Rx);
				}
			}
		}

		#endregion

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_CHUNKS")]
		private void DebugChunks(string message)
		{
			DebugMessage(message);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_LINE_BREAK")]
		private void DebugLineBreak(string message)
		{
			DebugMessage(message);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is an FxCop false-positive, the called DebugLineBreak() cannot be static.")]
		[Conditional("DEBUG_LINE_BREAK")]
		private void DebugLineBreak(RepositoryType repositoryType, string message)
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
