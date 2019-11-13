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
	public class RtfPrinter : IDisposable, IDisposableEx
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

		/// <summary></summary>
		public bool IsDisposed { get; protected set; }

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				// Dispose of managed resources if requested:
				if (disposing)
				{
					if (this.foreColorBrush != null)
						this.foreColorBrush.Dispose();

					if (this.richTextProvider != null)
						this.richTextProvider.Dispose();

					if (this.reader != null)
						this.reader.Dispose();
				}

				// Set state to disposed:
				this.richTextProvider = null;
				this.reader = null;
				IsDisposed = true;
			}
		}

	#if (DEBUG)
		/// <remarks>
		/// Microsoft.Design rule CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable requests
		/// "Types that declare disposable members should also implement IDisposable. If the type
		///  does not own any unmanaged resources, do not implement a finalizer on it."
		///
		/// Well, true for best performance on finalizing. However, it's not easy to find missing
		/// calls to <see cref="Dispose()"/>. In order to detect such missing calls, the finalizer
		/// is kept for DEBUG, indicating missing calls.
		///
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		~RtfPrinter()
		{
			Dispose(false);

			MKY.Diagnostics.DebugDisposal.DebugNotifyFinalizerInsteadOfDispose(this);
		}
	#endif // DEBUG

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (IsDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed!"));
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
			if (this.foreColorBrushColor != formatSettings.IODeviceFormat.Color)
			{
				this.foreColorBrushColor = formatSettings.IODeviceFormat.Color;

				if (this.foreColorBrush != null)
					this.foreColorBrush.Dispose();

				this.foreColorBrush = new SolidBrush(formatSettings.IODeviceFormat.Color);
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
