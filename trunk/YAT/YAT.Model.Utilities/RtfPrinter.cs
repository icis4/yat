//==================================================================================================
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
// Copyright © 2003-2015 Matthias Kläy.
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
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

using YAT.Domain;

#endregion

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Utility class providing RTF printer functionality for YAT.
	/// </summary>
	public class RtfPrinter : IDisposable
	{
		private bool isDisposed;

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
				if (this.richTextProvider != null) {
					this.richTextProvider.Dispose();
					this.richTextProvider = null;
				}

				if (this.reader != null) {
					this.reader.Dispose();
					this.reader = null;
				}

				// Dispose of managed resources if requested:
				if (disposing)
				{
				}

				// Set state to disposed:
				this.isDisposed = true;
			}
		}

		/// <summary></summary>
		~RtfPrinter()
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

		#endregion

		/// <remarks>
		/// Pragmatic implementation of printing RTF. 'netrtfwriter' is only used for stream-based logging.
		/// </remarks>
		/// <exception cref="InvalidPrinterException">
		/// The printer named in the <see cref="PrinterSettings.PrinterName"/> property does not exist.
		/// </exception>
		public virtual void Print(List<DisplayLine> lines, Settings.FormatSettings formatSettings)
		{
			Print(RtfWriterHelper.LinesToRichTextBox(lines, formatSettings));
		}

		/// <remarks>
		/// Pragmatic implementation of printing RTF. 'netrtfwriter' is only used for stream-based logging.
		/// </remarks>
		/// <exception cref="System.Drawing.Printing.InvalidPrinterException">
		/// The printer named in the <see cref="PrinterSettings.PrinterName"/> property does not exist.
		/// </exception>
		public virtual void Print(RichTextBox richTextProvider)
		{
			this.richTextProvider = richTextProvider;
			this.reader = new StringReader(this.richTextProvider.Text);
			try
			{
				using (PrintDocument document = new PrintDocument())
				{
					document.PrintPage += new PrintPageEventHandler(document_PrintPage);
					document.PrinterSettings = this.settings;
					document.Print();
				}
			}
			finally
			{
				this.reader.Close();
			}
		}

		private void document_PrintPage(object sender, PrintPageEventArgs e)
		{
			// Calculate the number of lines per page:
			int linesPerPage = (int)(e.MarginBounds.Height / this.richTextProvider.Font.GetHeight(e.Graphics));
			int lineCount = 0;

			// Print each line of the file:
			string line = null;
			while ((lineCount < linesPerPage) && ((line = this.reader.ReadLine()) != null))
			{
				float y = e.MarginBounds.Top + (lineCount * this.richTextProvider.Font.GetHeight(e.Graphics));
				e.Graphics.DrawString(line, this.richTextProvider.Font, Brushes.Black, e.MarginBounds.Left, y, new StringFormat());
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
