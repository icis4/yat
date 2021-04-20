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

using DW.RtfWriter;

using MKY;
using MKY.Collections.Generic;

using YAT.Domain;
using YAT.Format.Settings;

#endregion

namespace YAT.Log.Utilities
{
	/// <summary>
	/// Utility class providing RTF writer functionality for YAT.
	/// </summary>
	/// <remarks>
	/// Wraps an 'netrtfwriter' based <see cref="RtfDocument"/> into an object.
	/// </remarks>
	public class RtfWriter : DisposableBase
	{
		private struct FormatDescriptor
		{
			public System.Drawing.FontStyle FontStyle;
			public ColorDescriptor FontColor;
			public ColorDescriptor BackColor;

			public FormatDescriptor(System.Drawing.FontStyle fontStyle, ColorDescriptor fontColor, ColorDescriptor backColor)
			{
				this.FontStyle = fontStyle;
				this.FontColor = fontColor;
				this.BackColor = backColor;
			}
		}

		private RtfDocument document;
		private Align alignment;

		private FormatDescriptor txDataFormat;
		private FormatDescriptor txControlFormat;
		private FormatDescriptor rxDataFormat;
		private FormatDescriptor rxControlFormat;
		private FormatDescriptor timeStampFormat;
		private FormatDescriptor timeSpanFormat;
		private FormatDescriptor timeDeltaFormat;
		private FormatDescriptor timeDurationFormat;
		private FormatDescriptor deviceFormat;
		private FormatDescriptor directionFormat;
		private FormatDescriptor lengthFormat;
		private FormatDescriptor ioControlFormat;
		private FormatDescriptor warningFormat;
		private FormatDescriptor errorFormat;
		private FormatDescriptor separatorFormat;

		private StreamWriter writer;
		private object writerSyncObj = new object();

		/// <summary></summary>
		public RtfWriter(FileStream stream, FormatSettings settings)
		{
			var ci = CultureInfo.CurrentCulture; // Do not use the UICulture as people are likely to use English on non-US computers too.

			PaperSize paper;
			if      (StringEx.EndsWithOrdinalIgnoreCase  (ci.Name, "-US"))      paper = PaperSize.Letter;
			else                                                                paper = PaperSize.A4;

			Lcid lcid;
			if      (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "de"     ))  lcid = Lcid.German;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "fr"     ))  lcid = Lcid.French;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "it"     ))  lcid = Lcid.Italian;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "es"     ))  lcid = Lcid.Spanish;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "ja"     ))  lcid = Lcid.Japanese;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "ko"     ))  lcid = Lcid.Korean;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "zh-Hans"))  lcid = Lcid.SimplifiedChinese;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "zh-Hant"))  lcid = Lcid.TraditionalChinese;
			else                                                                lcid = Lcid.English;

			// Document base:
			this.document = new RtfDocument(paper, PaperOrientation.Landscape, lcid); // Same orientation as maximized terminal monitor.
			this.document.DefaultCharFormat.Font = this.document.createFont(settings.Font.Name);
			this.document.DefaultCharFormat.FontSize = settings.Font.Size;
			this.alignment = Align.Left;

			// Formats:
			this.txDataFormat       = new FormatDescriptor(settings.TxDataFormat.FontStyle,       this.document.createColor(settings.TxDataFormat.Color),       this.document.createColor(settings.BackColor));
			this.txControlFormat    = new FormatDescriptor(settings.TxControlFormat.FontStyle,    this.document.createColor(settings.TxControlFormat.Color),    this.document.createColor(settings.BackColor));
			this.rxDataFormat       = new FormatDescriptor(settings.RxDataFormat.FontStyle,       this.document.createColor(settings.RxDataFormat.Color),       this.document.createColor(settings.BackColor));
			this.rxControlFormat    = new FormatDescriptor(settings.RxControlFormat.FontStyle,    this.document.createColor(settings.RxControlFormat.Color),    this.document.createColor(settings.BackColor));
			this.timeStampFormat    = new FormatDescriptor(settings.TimeStampFormat.FontStyle,    this.document.createColor(settings.TimeStampFormat.Color),    this.document.createColor(settings.BackColor));
			this.timeSpanFormat     = new FormatDescriptor(settings.TimeSpanFormat.FontStyle,     this.document.createColor(settings.TimeSpanFormat.Color),     this.document.createColor(settings.BackColor));
			this.timeDeltaFormat    = new FormatDescriptor(settings.TimeDeltaFormat.FontStyle,    this.document.createColor(settings.TimeDeltaFormat.Color),    this.document.createColor(settings.BackColor));
			this.timeDurationFormat = new FormatDescriptor(settings.TimeDurationFormat.FontStyle, this.document.createColor(settings.TimeDurationFormat.Color), this.document.createColor(settings.BackColor));
			this.deviceFormat       = new FormatDescriptor(settings.DeviceFormat.FontStyle,       this.document.createColor(settings.DeviceFormat.Color),       this.document.createColor(settings.BackColor));
			this.directionFormat    = new FormatDescriptor(settings.DirectionFormat.FontStyle,    this.document.createColor(settings.DirectionFormat.Color),    this.document.createColor(settings.BackColor));
			this.lengthFormat       = new FormatDescriptor(settings.LengthFormat.FontStyle,       this.document.createColor(settings.LengthFormat.Color),       this.document.createColor(settings.BackColor));
			this.ioControlFormat    = new FormatDescriptor(settings.IOControlFormat.FontStyle,    this.document.createColor(settings.IOControlFormat.Color),    this.document.createColor(settings.BackColor));
			this.warningFormat      = new FormatDescriptor(settings.WarningFormat.FontStyle,      this.document.createColor(settings.WarningFormat.Color),      this.document.createColor(settings.BackColor));
			this.errorFormat        = new FormatDescriptor(settings.ErrorFormat.FontStyle,        this.document.createColor(settings.ErrorFormat.Color),        this.document.createColor(settings.BackColor));
			this.separatorFormat    = new FormatDescriptor(settings.SeparatorFormat.FontStyle,    this.document.createColor(settings.SeparatorFormat.Color),    this.document.createColor(settings.BackColor));

			// Header:
			var header = this.document.Header.addParagraph();
			header.Alignment = Align.Center;
			header.setText(ApplicationEx.ProductName + " Log"); // File header shall differ for "YAT" and "YATConsole".

			// Footer:
			var footer = this.document.Footer.addParagraph();
			footer.Alignment = Align.Center;
			footer.setText(" / "); // First space is required! 0 inserts AFTER the character at index 0!
			footer.addControlWord(0, RtfFieldControlWord.FieldType.Page);
			footer.addControlWord(1, RtfFieldControlWord.FieldType.NumPages);

			// Render the RTF beginning:
			var rtf = this.document.renderBeginning();

			// Write the RTF beginning into the file:
			this.writer = new StreamWriter(stream, Encoding.ASCII);
			this.writer.Write(rtf);
		}

		private void SetFormat(DisplayElement element, out FormatDescriptor format)
		{
			if      ( element is DisplayElement.TxData)           { format = this.txDataFormat;       }
			else if ( element is DisplayElement.TxControl)        { format = this.txControlFormat;    }
			else if ( element is DisplayElement.RxData)           { format = this.rxDataFormat;       }
			else if ( element is DisplayElement.RxControl)        { format = this.rxControlFormat;    }
			else if ( element is DisplayElement.TimeStampInfo)    { format = this.timeStampFormat;    }
			else if ( element is DisplayElement.TimeSpanInfo)     { format = this.timeSpanFormat;     }
			else if ( element is DisplayElement.TimeDeltaInfo)    { format = this.timeDeltaFormat;    }
			else if ( element is DisplayElement.TimeDurationInfo) { format = this.timeDurationFormat; }
			else if ( element is DisplayElement.DeviceInfo)       { format = this.deviceFormat;       }
			else if ( element is DisplayElement.DirectionInfo)    { format = this.directionFormat;    }
			else if ( element is DisplayElement.ContentLength)    { format = this.lengthFormat;       }
			else if ((element is DisplayElement.Nonentity)        ||
			         (element is DisplayElement.ContentSeparator) ||
			         (element is DisplayElement.InfoSeparator)    ||
			         (element is DisplayElement.LineStart)        ||
			         (element is DisplayElement.LineBreak))       { format = this.separatorFormat;    }
			else if ( element is DisplayElement.IOControlInfo)    { format = this.ioControlFormat;    }
			else if ( element is DisplayElement.WarningInfo)      { format = this.warningFormat;      }
			else if ( element is DisplayElement.ErrorInfo)        { format = this.errorFormat;        }
			else
			{
				throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + element.GetType() + "' is a display element that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
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
				if (this.writer != null) {
					this.writer.Dispose();
					this.writer = null;
				}
			}
		}

		#endregion

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public virtual void WriteLine(DisplayLine line)
		{
			AssertUndisposed();

			lock (writerSyncObj)
			{
				// Analyze the line and split it into segments:
				int position = 0;
				var text = new StringBuilder();
				var segments = new List<Pair<DisplayElement, Pair<int, int>>>(line.Count); // Preset the required capacity to improve memory management.
				foreach (var element in line)
				{
					if (!string.IsNullOrEmpty(element.Text))
					{
						string segment = element.Text;
						text.Append(segment);                                                        // segment begin            segment end
						segments.Add(new Pair<DisplayElement, Pair<int, int>>(element, new Pair<int, int>(position, (position + segment.Length - 1))));
						position += segment.Length;
					}
				}

				try
				{
					// Set the paragraph text:
					var par = this.document.addParagraph();
					par.Alignment = this.alignment;
					par.setText(text.ToString());

					// Set the text format segment-by-segment:
					foreach (Pair<DisplayElement, Pair<int, int>> segment in segments)
					{
						RtfCharFormat fmt = par.addCharFormat(segment.Value2.Value1, segment.Value2.Value2);

						FormatDescriptor format;
						SetFormat(segment.Value1, out format);

						if ((format.FontStyle & System.Drawing.FontStyle.Bold)      != 0) fmt.FontStyle.addStyle(FontStyleFlag.Bold);
						if ((format.FontStyle & System.Drawing.FontStyle.Italic)    != 0) fmt.FontStyle.addStyle(FontStyleFlag.Italic);
						if ((format.FontStyle & System.Drawing.FontStyle.Underline) != 0) fmt.FontStyle.addStyle(FontStyleFlag.Underline);
						if ((format.FontStyle & System.Drawing.FontStyle.Strikeout) != 0) fmt.FontStyle.addStyle(FontStyleFlag.Strike);

						fmt.FgColor = format.FontColor;
						fmt.BgColor = format.BackColor;
					}

					// Render and write the RTF line into the file:
					var rtf = this.document.renderContent();
					this.writer.Write(rtf);

					// Remove the content from the RTF document:
					this.document.clearBlocks();
				}
				catch (Exception ex)
				{
					MKY.Diagnostics.DebugEx.WriteException(GetType(), ex, "Exception while creating RTF!");
				}
			}
		}

		/// <summary>
		/// Clears all buffers for the current writer and causes any buffered data to be written
		/// to the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		public void Flush()
		{
			AssertUndisposed();

			lock (writerSyncObj)
				this.writer.Flush();
		}

		/// <summary>
		/// Closes the current object and the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		public void Close()
		{
			AssertUndisposed();

			lock (writerSyncObj)
			{
				// Render and write the RTF ending into the file:
				var rtf = this.document.renderEnding();
				this.writer.Write(rtf);

				// Close the file:
				this.writer.Close();
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
