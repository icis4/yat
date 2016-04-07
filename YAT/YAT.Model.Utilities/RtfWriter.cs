﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using DW.RtfWriter;

using MKY;
using MKY.Collections.Generic;

using YAT.Domain;
using YAT.Model.Settings;

#endregion

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Utility class providing RTF writer functionality for YAT.
	/// </summary>
	/// <remarks>
	/// Wraps an 'netrtfwriter' based <see cref="RtfDocument"/> into an object.
	/// </remarks>
	public class RtfWriter : IDisposable
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

		private bool isDisposed;

		private RtfDocument document;
		private Align alignement;

		private FormatDescriptor txDataFormat;
		private FormatDescriptor txControlFormat;
		private FormatDescriptor rxDataFormat;
		private FormatDescriptor rxControlFormat;
		private FormatDescriptor dateFormat;
		private FormatDescriptor timeFormat;
		private FormatDescriptor portFormat;
		private FormatDescriptor directionFormat;
		private FormatDescriptor lengthFormat;
		private FormatDescriptor whiteSpacesFormat;
		private FormatDescriptor errorFormat;

		private StreamWriter writer;
		private object writerSyncObj = new object();

		/// <summary></summary>
		public RtfWriter(FileStream stream, Encoding encoding, FormatSettings settings)
		{
			CultureInfo ci = CultureInfo.CurrentCulture;
			// Do not use the UICulture as people are likely to use English on non-US computers too.

			PaperSize paper;
			if      (StringEx.EndsWithOrdinalIgnoreCase  (ci.Name, "-US"))		paper = PaperSize.Letter;
			else																paper = PaperSize.A4;

			Lcid lcid;
			if      (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "de"		))	lcid = Lcid.German;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "fr"		))	lcid = Lcid.French;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "it"		))	lcid = Lcid.Italian;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "es"		))	lcid = Lcid.Spanish;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "ja"		))	lcid = Lcid.Japanese;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "ko"		))	lcid = Lcid.Korean;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "zh-Hans"))	lcid = Lcid.SimplifiedChinese;
			else if (StringEx.StartsWithOrdinalIgnoreCase(ci.Name, "zh-Hant"))	lcid = Lcid.TraditionalChinese;
			else																lcid = Lcid.English;

			// Document base:
			this.document = new RtfDocument(paper, PaperOrientation.Landscape, lcid); // Same orientation as maximized terminal monitor.
			this.document.DefaultCharFormat.Font = this.document.createFont(settings.Font.Name);
			this.document.DefaultCharFormat.FontSize = settings.Font.Size;
			this.alignement = Align.Left;

			// Formats:
			this.txDataFormat      = new FormatDescriptor(settings.TxDataFormat.FontStyle,      this.document.createColor(settings.TxDataFormat.Color),      this.document.createColor(settings.BackColor));
			this.txControlFormat   = new FormatDescriptor(settings.TxControlFormat.FontStyle,   this.document.createColor(settings.TxControlFormat.Color),   this.document.createColor(settings.BackColor));
			this.rxDataFormat      = new FormatDescriptor(settings.RxDataFormat.FontStyle,      this.document.createColor(settings.RxDataFormat.Color),      this.document.createColor(settings.BackColor));
			this.rxControlFormat   = new FormatDescriptor(settings.RxControlFormat.FontStyle,   this.document.createColor(settings.RxControlFormat.Color),   this.document.createColor(settings.BackColor));
			this.dateFormat        = new FormatDescriptor(settings.DateFormat.FontStyle,        this.document.createColor(settings.DateFormat.Color),        this.document.createColor(settings.BackColor));
			this.timeFormat        = new FormatDescriptor(settings.TimeFormat.FontStyle,        this.document.createColor(settings.TimeFormat.Color),        this.document.createColor(settings.BackColor));
			this.portFormat        = new FormatDescriptor(settings.PortFormat.FontStyle,        this.document.createColor(settings.PortFormat.Color),        this.document.createColor(settings.BackColor));
			this.directionFormat   = new FormatDescriptor(settings.DirectionFormat.FontStyle,   this.document.createColor(settings.DirectionFormat.Color),   this.document.createColor(settings.BackColor));
			this.lengthFormat      = new FormatDescriptor(settings.LengthFormat.FontStyle,      this.document.createColor(settings.LengthFormat.Color),      this.document.createColor(settings.BackColor));
			this.whiteSpacesFormat = new FormatDescriptor(settings.WhiteSpacesFormat.FontStyle, this.document.createColor(settings.WhiteSpacesFormat.Color), this.document.createColor(settings.BackColor));
			this.errorFormat       = new FormatDescriptor(settings.ErrorFormat.FontStyle,       this.document.createColor(settings.ErrorFormat.Color),       this.document.createColor(settings.BackColor));

			// Header:
			RtfParagraph header = this.document.Header.addParagraph();
			header.Alignment = Align.Center;
			header.setText("YAT Log");

			// Footer:
			RtfParagraph footer = this.document.Footer.addParagraph();
			footer.Alignment = Align.Center;
			footer.setText(" / "); // First space is required! 0 inserts AFTER the character at index 0!
			footer.addControlWord(0, RtfFieldControlWord.FieldType.Page);
			footer.addControlWord(1, RtfFieldControlWord.FieldType.NumPages);

			// Render the RTF beginning:
			string rtf = this.document.renderBeginning();

			// Write the RTF beginning into the file:
			this.writer = new StreamWriter(stream, Encoding.ASCII);
			this.writer.Write(rtf);
		}

		private void SetFormat(DisplayElement element, out FormatDescriptor format)
		{
			if      ( element is DisplayElement.TxData)			{ format = this.txDataFormat; }
			else if ( element is DisplayElement.TxControl)		{ format = this.txControlFormat; }
			else if ( element is DisplayElement.RxData)			{ format = this.rxDataFormat; }
			else if ( element is DisplayElement.RxControl)		{ format = this.rxControlFormat; }
			else if ( element is DisplayElement.DateInfo)		{ format = this.dateFormat; }
			else if ( element is DisplayElement.TimeInfo)		{ format = this.timeFormat; }
			else if ( element is DisplayElement.PortInfo)		{ format = this.portFormat; }
			else if ( element is DisplayElement.DirectionInfo)	{ format = this.directionFormat; }
			else if ( element is DisplayElement.Length)			{ format = this.lengthFormat; }
			else if ((element is DisplayElement.NoData) ||
			         (element is DisplayElement.LeftMargin) ||
			         (element is DisplayElement.Space) ||
			         (element is DisplayElement.RightMargin) ||
			         (element is DisplayElement.LineBreak))		{ format = this.whiteSpacesFormat; }
			else if (element is DisplayElement.ErrorInfo)		{ format = this.errorFormat; }
			else { throw (new NotImplementedException("Unknown DisplayElement!")); }
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (this.writer != null) {
					this.writer.Close();
					this.writer.Dispose();
				}

				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~RtfWriter()
		{
			Dispose(false);

			System.Diagnostics.Debug.WriteLine("The finalizer of '" + GetType().FullName + "' should have never been called! Ensure to call Dispose()!");
		}

		/// <summary></summary>
		public bool IsDisposed
		{
			get { return (this.isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (this.isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
		}

		#endregion

		/// <summary></summary>
		public virtual void WriteLine(DisplayLine line)
		{
			AssertNotDisposed();

			lock (writerSyncObj)
			{
				// Analyze the line and split it into segments:
				int position = 0;
				StringBuilder text = new StringBuilder();
				var segments = new List<Pair<DisplayElement, Pair<int, int>>>();
				foreach (DisplayElement element in line)
				{
					if (element.Text.Length > 0)
					{
						string segment = element.Text;
						text.Append(segment);     // segment begin            segment end
						segments.Add(new Pair<DisplayElement, Pair<int, int>>(element, new Pair<int, int>(position, (position + segment.Length - 1))));
						position += segment.Length;
					}
				}

				try
				{
					// Set the paragraph text:
					RtfParagraph par = this.document.addParagraph();
					par.Alignment = this.alignement;
					par.setText(text.ToString());

					// Set the text format segment-by-segment:
					foreach (Pair<DisplayElement, Pair<int, int>> segment in segments)
					{
						RtfCharFormat fmt = par.addCharFormat(segment.Value2.Value1, segment.Value2.Value2);

						FormatDescriptor format;
						SetFormat(segment.Value1, out format);

						if ((format.FontStyle & System.Drawing.FontStyle.Bold)      == System.Drawing.FontStyle.Bold)      fmt.FontStyle.addStyle(FontStyleFlag.Bold);
						if ((format.FontStyle & System.Drawing.FontStyle.Italic)    == System.Drawing.FontStyle.Italic)    fmt.FontStyle.addStyle(FontStyleFlag.Italic);
						if ((format.FontStyle & System.Drawing.FontStyle.Underline) == System.Drawing.FontStyle.Underline) fmt.FontStyle.addStyle(FontStyleFlag.Underline);
						if ((format.FontStyle & System.Drawing.FontStyle.Strikeout) == System.Drawing.FontStyle.Strikeout) fmt.FontStyle.addStyle(FontStyleFlag.Strike);

						fmt.FgColor = format.FontColor;
						fmt.BgColor = format.BackColor;
					}

					// Render and write the RTF line into the file:
					string rtf = this.document.renderContent();
					this.writer.Write(rtf);

					// Remove the content from the RTF document:
					this.document.clearBlocks();
				}
				catch (Exception ex)
				{
					MKY.Diagnostics.DebugEx.WriteException(GetType(), ex);
				}
			}
		}

		/// <summary>
		/// Clears all buffers for the current writer and causes any buffered data to be written
		/// to the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		public void Flush()
		{
			AssertNotDisposed();

			lock (writerSyncObj)
				this.writer.Flush();
		}

		/// <summary>
		/// Closes the current object and the underlying <see cref="StreamWriter"/> stream.
		/// </summary>
		public void Close()
		{
			AssertNotDisposed();

			lock (writerSyncObj)
			{
				// Render and write the RTF ending into the file:
				string rtf = this.document.renderEnding();
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
