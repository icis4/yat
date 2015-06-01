//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 1 Version 1.99.32
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;

#endregion

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Static utility class providing RTF reader functionality for YAT.
	/// </summary>
	public static class RtfReader
	{
		/// <summary></summary>
		public static string[] LinesFromRtfFile(string rtfFilePath)
		{
			RichTextBox richTextProvider = new RichTextBox();
			using (FileStream fs = File.OpenRead(rtfFilePath))
			{
				richTextProvider.LoadFile(fs, RichTextBoxStreamType.RichText);
			}
			return (richTextProvider.Lines);
		}
	}

	/// <summary>
	/// Static utility class providing RTF writer functionality for YAT.
	/// </summary>
	public static class RtfWriter
	{
		/// <summary></summary>
		public static void LinesToRtfFile(List<Domain.DisplayLine> lines, string rtfFilePath, Settings.FormatSettings formatSettings, RichTextBoxStreamType rtfType)
		{
			RichTextBox richTextProvider = LinesToRichTextBox(lines, formatSettings);
			richTextProvider.SaveFile(rtfFilePath, rtfType);
		}

		/// <summary></summary>
		public static void LinesToClipboard(List<Domain.DisplayLine> lines, Settings.FormatSettings formatSettings)
		{
			RichTextBox richTextProvider = LinesToRichTextBox(lines, formatSettings);
			richTextProvider.SelectAll();
			richTextProvider.Copy();
		}

		/// <summary></summary>
		public static RichTextBox LinesToRichTextBox(List<Domain.DisplayLine> lines, Settings.FormatSettings formatSettings)
		{
			RichTextBox richTextProvider = new RichTextBox();
			foreach (Domain.DisplayLine line in lines)
				AppendDisplayLine(richTextProvider, line, formatSettings);

			return (richTextProvider);
		}

		private static void AppendDisplayLine(RichTextBox richTextProvider, Domain.DisplayLine line, Settings.FormatSettings formatSettings)
		{
			AppendDisplayElements(richTextProvider, line, formatSettings);
		}

		private static void AppendDisplayElements(RichTextBox richTextProvider, List<Domain.DisplayElement> elements, Settings.FormatSettings formatSettings)
		{
			foreach (Domain.DisplayElement de in elements)
				AppendDisplayElement(richTextProvider, de, formatSettings);
		}

		[SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Element is intentionally casted to LineBreak twice for implementation readability.")]
		private static void AppendDisplayElement(RichTextBox richTextProvider, Domain.DisplayElement element, Settings.FormatSettings formatSettings)
		{
			string fontName = formatSettings.Font.Name;
			float fontSize = formatSettings.Font.Size;

			if (element is Domain.DisplayElement.TxData)
			{
				richTextProvider.SelectionFont = new Font(fontName, fontSize, formatSettings.TxDataFormat.FontStyle);
				richTextProvider.SelectionColor = formatSettings.TxDataFormat.Color;
			}
			else if (element is Domain.DisplayElement.TxControl)
			{
				richTextProvider.SelectionFont = new Font(fontName, fontSize, formatSettings.TxControlFormat.FontStyle);
				richTextProvider.SelectionColor = formatSettings.TxControlFormat.Color;
			}
			else if (element is Domain.DisplayElement.RxData)
			{
				richTextProvider.SelectionFont = new Font(fontName, fontSize, formatSettings.RxDataFormat.FontStyle);
				richTextProvider.SelectionColor = formatSettings.RxDataFormat.Color;
			}
			else if (element is Domain.DisplayElement.RxControl)
			{
				richTextProvider.SelectionFont = new Font(fontName, fontSize, formatSettings.RxControlFormat.FontStyle);
				richTextProvider.SelectionColor = formatSettings.RxControlFormat.Color;
			}
			else if (element is Domain.DisplayElement.TimeStamp)
			{
				richTextProvider.SelectionFont = new Font(fontName, fontSize, formatSettings.TimeStampFormat.FontStyle);
				richTextProvider.SelectionColor = formatSettings.TimeStampFormat.Color;
			}
			else if (element is Domain.DisplayElement.LineLength)
			{
				richTextProvider.SelectionFont = new Font(fontName, fontSize, formatSettings.LengthFormat.FontStyle);
				richTextProvider.SelectionColor = formatSettings.LengthFormat.Color;
			}
			else if ((element is Domain.DisplayElement.LeftMargin) ||
					 (element is Domain.DisplayElement.Space) ||
					 (element is Domain.DisplayElement.RightMargin) ||
					 (element is Domain.DisplayElement.LineBreak))
			{
				richTextProvider.SelectionFont = new Font(fontName, fontSize, formatSettings.WhiteSpacesFormat.FontStyle);
				richTextProvider.SelectionColor = formatSettings.WhiteSpacesFormat.Color;
			}
			else if (element is Domain.DisplayElement.IOError)
			{
				richTextProvider.SelectionFont = new Font(fontName, fontSize, formatSettings.ErrorFormat.FontStyle);
				richTextProvider.SelectionColor = formatSettings.ErrorFormat.Color;
			}
			else
			{
				throw (new NotImplementedException("Unknown DisplayElement!"));
			}

			// Handle line break according to current system.
			if (element is Domain.DisplayElement.LineBreak)
				richTextProvider.AppendText(Environment.NewLine);
			else
				richTextProvider.AppendText(element.Text);
		}
	}

	/// <summary>
	/// Static utility class providing RTF printer functionality for YAT.
	/// </summary>
	public class RtfPrinter : IDisposable
	{
		private bool isDisposed;

		private PrintDocument document;
		private RichTextBox richTextProvider;
		private StringReader reader;

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public RtfPrinter(PrinterSettings settings)
		{
			this.document = new PrintDocument();
			this.document.PrintPage += new PrintPageEventHandler(document_PrintPage);
			this.document.PrinterSettings = settings;
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
				// In any case, dispose of the as they were created in the constructor:
				if (this.document != null)
					this.document.Dispose();

				if (this.richTextProvider != null)
					this.richTextProvider.Dispose();

				if (this.reader != null)
					this.reader.Dispose();

				// Dispose of managed resources if requested:
				if (disposing)
				{
				}

				// Set state to disposed:
				this.document = null;
				this.richTextProvider = null;
				this.reader = null;
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

		/// <summary></summary>
		/// <exception cref="System.Drawing.Printing.InvalidPrinterException">
		/// The printer named in the System.Drawing.Printing.PrinterSettings.PrinterName property does not exist.
		/// </exception>
		public virtual void Print(RichTextBox richTextProvider)
		{
			this.richTextProvider = richTextProvider;
			this.reader = new StringReader(this.richTextProvider.Text);
			try
			{
				this.document.Print();
			}
			finally
			{
				this.reader.Close(); // \fixme: Does this really work? Stream is closed even though it is accessed by pd_PrintPage()...
			}
		}

		private void document_PrintPage(object sender, PrintPageEventArgs e)
		{
			// Calculate the number of lines per page.
			int linesPerPage = (int)(e.MarginBounds.Height / this.richTextProvider.Font.GetHeight(e.Graphics));
			int lineCount = 0;

			// Print each line of the file.
			string line = null;
			while ((lineCount < linesPerPage) && ((line = this.reader.ReadLine()) != null))
			{
				float y = e.MarginBounds.Top + (lineCount * this.richTextProvider.Font.GetHeight(e.Graphics));
				e.Graphics.DrawString(line, this.richTextProvider.Font, Brushes.Black, e.MarginBounds.Left, y, new StringFormat());
				lineCount++;
			}

			// If more lines exist, print another page.
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
