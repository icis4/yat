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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

using MKY;

using YAT.Domain;
using YAT.Format.Settings;

#endregion

namespace YAT.View.Utilities
{
	/// <summary>
	/// Utility class providing RTF printer functionality for YAT.
	/// </summary>
	public class RtfPrinter : DisposableBase
	{
		private Color foreColorBrushColor;
		private Brush foreColorBrush;

		private PrinterSettings settings;
		private RichTextBox richTextProvider;
		private StringReader reader;

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public RtfPrinter(PrinterSettings settings)
		{
			this.settings = settings;
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
				if (this.foreColorBrush != null) {
					this.foreColorBrush.Dispose();
					this.foreColorBrush = null;
				}

				if (this.richTextProvider != null) {
					this.richTextProvider.Dispose();
					this.richTextProvider = null;
				}

				if (this.reader != null) {
					this.reader.Dispose();
					this.reader = null;
				}
			}
		}

		#endregion

		#endregion

		/// <remarks>
		/// Pragmatic implementation of printing RTF. Yet limited to printing in 'Device' format.
		/// </remarks>
		/// <exception cref="InvalidPrinterException">
		/// The printer named in the <see cref="PrinterSettings.PrinterName"/> property does not exist.
		/// </exception>
		public virtual void Print(DisplayLineCollection lines, FormatSettings formatSettings)
		{
			if (this.foreColorBrushColor != formatSettings.DeviceFormat.Color)
			{
				this.foreColorBrushColor = formatSettings.DeviceFormat.Color;

				if (this.foreColorBrush != null)
					this.foreColorBrush.Dispose();

				this.foreColorBrush = new SolidBrush(formatSettings.DeviceFormat.Color);
			}

			Print(RtfWriterHelper.CopyLinesToRichTextBox(lines, formatSettings));
		}

		/// <remarks>
		/// Pragmatic implementation of printing RTF. Yet limited to printing in 'Device' format.
		/// </remarks>
		/// <exception cref="InvalidPrinterException">
		/// The printer named in the <see cref="PrinterSettings.PrinterName"/> property does not exist.
		/// </exception>
		public virtual void Print(RichTextBox richTextProvider)
		{
			this.richTextProvider = richTextProvider;

			this.reader = new StringReader(this.richTextProvider.Text);
			try // This try-finally is an explicit "using (this.reader)", including resetting the field to null.
			{   // Field is required because document_PrintPage must be able to access the reader object.
				using (PrintDocument document = new PrintDocument())
				{
					document.PrintPage += new PrintPageEventHandler(document_PrintPage);
					document.PrinterSettings = this.settings;
					document.Print();
				}
			}
			finally
			{
				this.reader.Close(); // MSDN: "Close() calls the Dispose() method passing a true value."
				this.reader = null;
			}
		}

		private void document_PrintPage(object sender, PrintPageEventArgs e)
		{
			float fontHeight = this.richTextProvider.Font.GetHeight(e.Graphics);

			// Calculate the number of lines per page:
			int linesPerPage = (int)(e.MarginBounds.Height / fontHeight);
			int lineCount = 0;

			// Print each line of the file:
			string line = null;
			while ((lineCount < linesPerPage) && ((line = this.reader.ReadLine()) != null))
			{
				float y = e.MarginBounds.Top + (lineCount * fontHeight);
				e.Graphics.DrawString(line, this.richTextProvider.Font, this.foreColorBrush, e.MarginBounds.Left, y, new StringFormat());
				lineCount++;
			}

			// If more lines exist, print another page:
			if (line != null)
				e.HasMorePages = true;
			else
				e.HasMorePages = false;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
